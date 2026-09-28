using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QueueManagement.Api;
using QueueManagement.Api.Data;
using QueueManagement.Api.Hubs;
using QueueManagement.Shared;

static string GenerateQueueCode(int queueNumber, string serviceCode)
{
    var prefix = string.IsNullOrWhiteSpace(serviceCode)
        ? "GE"
        : serviceCode.Trim().ToUpperInvariant().Replace("-", string.Empty);

    if (prefix.Length < 2)
    {
        prefix = prefix.PadRight(2, 'X');
    }

    return $"{prefix.Substring(0, 2)}-{queueNumber:D4}";
}

var builder = WebApplication.CreateBuilder(args);

var dbPath = DatabasePathProvider.ResolveDatabasePath(builder.Environment.ContentRootPath);

builder.Services.AddDbContext<QueueDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddSignalR();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5267",
                "https://localhost:7042",
                "https://localhost:7041",
                "http://localhost:5187")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QueueDbContext>();
    db.Database.EnsureCreated();
    SeedData.Initialize(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowClient");
app.UseHttpsRedirection();

app.MapGet("/api/health", () => new { Status = "ok", Timestamp = DateTime.UtcNow });

var clientApi = app.MapGroup("/client");
var staffApi = app.MapGroup("/staff");

clientApi.MapGet("/branches", async (QueueDbContext db) =>
    await db.Branches.OrderBy(b => b.Name).Take(1).ToListAsync());

clientApi.MapGet("/services", async (QueueDbContext db) =>
    await db.Services.OrderBy(s => s.Name).ToListAsync());

clientApi.MapPost("/tickets", async (CreateAppointmentRequest request, QueueDbContext db, IHubContext<QueueHub> hub) =>
{
    if (string.IsNullOrWhiteSpace(request.CustomerName))
    {
        return Results.BadRequest("Customer name is required.");
    }

    if (string.IsNullOrWhiteSpace(request.CustomerPhone))
    {
        return Results.BadRequest("Customer phone number is required.");
    }

    if (request.ServiceId <= 0)
    {
        return Results.BadRequest("A valid service is required.");
    }

    if (!AppointmentScheduling.IsBusinessDay(request.AppointmentDate))
    {
        return Results.BadRequest("Appointments are available Monday to Friday only.");
    }

    if (string.IsNullOrWhiteSpace(request.TimeSlot) || !AppointmentScheduling.IsWithinBusinessHours(request.TimeSlot))
    {
        return Results.BadRequest("Please select a preferred time between 09:00 and 17:00 during office hours.");
    }

    var service = await db.Services.FirstOrDefaultAsync(s => s.Id == request.ServiceId);
    if (service is null)
    {
        return Results.BadRequest("Selected service does not exist.");
    }

    var branch = await db.Branches.OrderBy(b => b.Id).FirstOrDefaultAsync();
    if (branch is null)
    {
        return Results.BadRequest("Branch configuration is missing.");
    }

    var lastQueue = await db.Appointments.MaxAsync(a => (int?)a.QueueNumber) ?? 0;
    var queueNumber = lastQueue + 1;
    var queueCode = GenerateQueueCode(queueNumber, service.ServiceCode);
    var issuedAt = DateTime.UtcNow;
    var expectedTime = AppointmentScheduling.GetExpectedAppointmentTime(request.AppointmentDate, request.TimeSlot) ?? request.AppointmentDate.Date.AddHours(9);

    var appointment = new Appointment
    {
        CustomerName = request.CustomerName,
        CustomerEmail = request.CustomerEmail,
        CustomerPhone = request.CustomerPhone,
        ServiceId = request.ServiceId,
        BranchId = branch.Id,
        AppointmentDate = request.AppointmentDate,
        TimeSlot = string.IsNullOrWhiteSpace(request.TimeSlot) ? "Walk-in" : request.TimeSlot,
        QueueNumber = queueNumber,
        QueueCode = queueCode,
        Status = AppointmentStatus.Waiting,
        CreatedAt = issuedAt
    };

    db.Appointments.Add(appointment);
    await db.SaveChangesAsync();
    await hub.Clients.All.SendAsync("QueueUpdated");

    return Results.Ok(new
    {
        appointment.Id,
        appointment.QueueNumber,
        appointment.QueueCode,
        appointment.Status,
        BranchName = branch.Name,
        ServiceName = service.Name,
        ServiceCode = service.ServiceCode,
        TimeSlot = appointment.TimeSlot,
        ExpectedTime = expectedTime,
        IssuedAt = issuedAt
    });
});

staffApi.MapGet("/queue", async (QueueDbContext db) =>
    await db.Appointments
        .Include(a => a.Service)
        .Include(a => a.Branch)
        .Where(a => a.Status != AppointmentStatus.Served && a.Status != AppointmentStatus.Missed)
        .OrderBy(a => a.QueueNumber)
        .Select(a => new QueueViewItem
        {
            Id = a.Id,
            QueueNumber = a.QueueNumber,
            QueueCode = a.QueueCode,
            CustomerName = a.CustomerName,
            CustomerEmail = a.CustomerEmail,
            CustomerPhone = a.CustomerPhone,
            ServiceName = a.Service!.Name,
            BranchName = a.Branch!.Name,
            TimeSlot = a.TimeSlot,
            AppointmentDate = a.AppointmentDate,
            Status = a.Status,
            CreatedAt = a.CreatedAt,
            CalledAt = a.CalledAt
        })
        .ToListAsync());

staffApi.MapGet("/dashboard", async (QueueDbContext db) =>
{
    var appointments = await db.Appointments.ToListAsync();
    var activeQueue = appointments.Count(a => a.Status != AppointmentStatus.Served && a.Status != AppointmentStatus.Missed);

    return new DashboardSummary
    {
        TotalAppointments = appointments.Count,
        ActiveQueue = activeQueue,
        StaffCallsToday = appointments.Count(a => a.Status == AppointmentStatus.Called || a.Status == AppointmentStatus.Serving),
        ServicesAvailable = await db.Services.CountAsync(),
        AverageWaitMinutes = activeQueue == 0 ? 0 : Math.Max(5, activeQueue * 7)
    };
});

staffApi.MapGet("/history", async (QueueDbContext db) =>
{
    var history = await db.Appointments
        .Include(a => a.Service)
        .Include(a => a.Branch)
        .Where(a => a.Status == AppointmentStatus.Served || a.Status == AppointmentStatus.Missed)
        .OrderByDescending(a => a.ServedAt ?? a.CreatedAt)
        .ToListAsync();

    return history.Select(a => new AppointmentHistoryItem
    {
        Id = a.Id,
        CustomerName = a.CustomerName,
        CustomerEmail = a.CustomerEmail,
        CustomerPhone = a.CustomerPhone,
        ServiceName = a.Service?.Name ?? "Unknown service",
        ServiceCode = a.Service?.ServiceCode ?? "—",
        BranchName = a.Branch?.Name ?? "Main Branch",
        QueueCode = a.QueueCode,
        AppointmentDate = a.AppointmentDate,
        TimeSlot = a.TimeSlot,
        CreatedAt = a.CreatedAt,
        ServedAt = a.ServedAt,
        ServiceDurationMinutes = a.CalledAt.HasValue && a.ServedAt.HasValue
            ? (int)Math.Max(0, (a.ServedAt.Value - a.CalledAt.Value).TotalMinutes)
            : null,
        Status = a.Status
    }).ToList();
});

staffApi.MapPost("/queue/{appointmentId:int}/call", async (int appointmentId, QueueDbContext db, IHubContext<QueueHub> hub) =>
{
    var appointment = await db.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
    if (appointment is null)
    {
        return Results.NotFound();
    }

    appointment.Status = AppointmentStatus.Called;
    appointment.CalledAt ??= DateTime.UtcNow;
    await db.SaveChangesAsync();
    await hub.Clients.All.SendAsync("QueueUpdated");
    return Results.Ok(appointment);
});

staffApi.MapPost("/queue/{appointmentId:int}/serve", async (int appointmentId, QueueDbContext db, IHubContext<QueueHub> hub) =>
{
    var appointment = await db.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
    if (appointment is null)
    {
        return Results.NotFound();
    }

    appointment.Status = AppointmentStatus.Served;
    appointment.ServedAt ??= DateTime.UtcNow;
    await db.SaveChangesAsync();
    await hub.Clients.All.SendAsync("QueueUpdated");
    return Results.Ok(appointment);
});

staffApi.MapPost("/queue/{appointmentId:int}/skip", async (int appointmentId, QueueDbContext db, IHubContext<QueueHub> hub) =>
{
    var appointment = await db.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
    if (appointment is null)
    {
        return Results.NotFound();
    }

    appointment.Status = AppointmentStatus.Missed;
    await db.SaveChangesAsync();
    await hub.Clients.All.SendAsync("QueueUpdated");
    return Results.Ok(appointment);
});

app.MapHub<QueueHub>("/queueHub");

app.Run();
