# DCIT 318 Semester Project

## Group Project

1. Kimathi Sedegah – 22237205
2. Edmund Nii Laryea Boye – 22121983
3. Mends-Brew Jason Nana Sam – 22044742
4. Nana Ohenewaa Owusu-Ansah – 22074304
5. Iyad Fuseini – 22069364
6. Rabiatu Abdul Salam – 22176448
7. Barbara Elizabeth Korlekie Sackey – 22012722
8. Chris Awaitey Larbi – 22033787
9. Opoku Chris Nana – 10957271
10. Rebecca Anuoluwapo Ogunnubi – 11365448
11. Maame Esi Armah-Mensah – 22033196
12. Asiamah Emmanuel Donkor – 22244211

---

## 1. Project Selection & Category

### Project Title
Queue Management and Appointment Scheduling System

### Category
Web Applications & Services

### Frameworks & Technologies
- ASP.NET Core Web API
- Blazor WebAssembly / Blazor Server
- SignalR (real-time communication)
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity (authentication and role management)

---

## 2. Problem Statement

Many service centers such as hospitals, banks, salons, government offices, and customer service centers experience long queues, overcrowded waiting areas, and poor communication between customers and staff. In most cases, customers arrive physically without knowing the expected wait time, which causes frustration, time loss, and inefficiency.

The lack of an automated queue system also makes it difficult for staff to organize appointments, manage demand, and monitor service performance. In high-traffic environments, this leads to missed appointments, slow processing, inefficient resource allocation, and low customer satisfaction.

This project addresses these issues by creating a digital platform that allows users to book appointments, receive queue updates in real time, and enables staff to manage customer flow more effectively.

---

## 3. Project Brief & Functionality

This project is a web-based appointment booking and queue management system designed to improve service delivery efficiency in high-traffic environments. The system digitalizes the queue process and makes service flow more transparent for both customers and staff.

The target users include:
- Customers booking services and tracking queue status
- Staff members managing queue progression and service sessions
- Administrators overseeing services, branch operations, and system reporting

### Core User Interface
The application will include a role-based web interface consisting of:
- A customer portal for booking appointments and tracking queue position
- A staff dashboard for managing service queues and handling customers
- An administrative dashboard for managing services, branches, and analytics

### Key Functionalities
1. Online Appointment Booking
   - Users can book appointments by selecting a service, branch, date, and available time slot.
2. Automated Queue Generation
   - The system generates queue numbers automatically and organizes customers into service-specific queues.
3. Real-Time Queue Tracking
   - Using SignalR, queue updates are pushed instantly to users and staff without page refresh.
4. Staff Queue Management Dashboard
   - Staff can call the next customer, mark service completion, skip, or recall queue entries.
5. Reporting and Analytics
   - Administrators can view system usage statistics including peak hours, average waiting times, and service demand trends.

---

## 4. Objectives

The key objectives of the project are to:
- Reduce waiting time and improve service efficiency in busy service environments.
- Provide users with an online appointment booking system that is simple and user-friendly.
- Automate queue generation and service allocation based on branch and service type.
- Enable real-time updates so customers and staff can track progress instantly.
- Support role-based access to ensure secure system use for customers, staff, and administrators.
- Generate reports and analytics that help management make informed operational decisions.

---

## 5. Scope of the System

### In Scope
- User registration and authentication
- Appointment booking and scheduling
- Queue generation for active services
- Real-time queue updates using SignalR
- Staff dashboard for queue handling
- Administrative management of services, branches, and users
- Reporting and analytics dashboard
- Database integration and API communication

### Out of Scope (Initial Version)
- Mobile application support
- Payment integration
- SMS or email notifications beyond basic notification features
- Advanced predictive analytics and AI-based wait-time estimation
- QR code check-in at the initial stage

---

## 6. Target Users

The proposed system is intended for:
- Customers who need to book appointments and monitor service progress
- Front-desk or service staff who manage queues and services
- Administrators responsible for operations, analytics, and system configuration
- Service organizations such as hospitals, banks, salons, and government offices

---

## 7. Functional Requirements

### Customer Requirements
- Register and log in to the system
- Book appointments for specific services and branches
- Select a preferred date and available time slot
- View appointment confirmation and queue status
- Receive real-time updates regarding queue position and service progress

### Staff Requirements
- View assigned service queue and current customer status
- Call the next customer for service
- Mark appointments as served, skipped, or rescheduled
- Maintain a smooth queue flow and update queue records
- Access reports related to service activity

### Admin Requirements
- Create and manage service categories, branches, and departments
- Manage user roles and permissions
- Monitor overall queue performance and service workload
- View analytics on demand, peak times, and waiting trends
- Configure system-wide operational settings

---

## 8. Non-Functional Requirements

- Security: Role-based access control using ASP.NET Core Identity
- Performance: Fast response times for booking and queue updates
- Reliability: Stable queue processing and accurate status updates
- Usability: Simple interfaces for customers, staff, and administrators
- Scalability: Ability to support multiple branches and concurrent users
- Maintainability: Clear separation of modules and reusable application logic

---

## 9. System Architecture Overview

The system will follow a client-server architecture with separation between frontend and backend services. The frontend will be developed using Blazor, while the backend will use ASP.NET Core Web API. SQL Server will serve as the primary database, and Entity Framework Core will handle data access and persistence.

SignalR will be used for real-time communication between the client and server to push queue status updates instantly. Authentication and authorization will be managed through ASP.NET Core Identity to support user roles such as customer, staff, and administrator.

---

## 10. Team Roster & Role Distribution

The project team consists of 12 members assigned to the following roles:

- 2 Members – Authentication & User Management (login, registration, role-based access)
- 2 Members – Appointment Booking Module (scheduling logic and UI)
- 2 Members – Queue Management System (queue generation and processing logic)
- 1 Member – Real-Time Communication (SignalR integration)
- 1 Member – Staff Dashboard Development
- 1 Member – Customer Portal Development
- 1 Member – Admin Dashboard & Service Configuration
- 1 Member – Reporting & Analytics Module
- 1 Member – Database Design & API Integration

This structure ensures that major system modules are distributed across the team while maintaining collaboration between functional areas.

---

## 11. Development Methodology

The project will be implemented using an incremental and collaborative development approach. The team will divide the system into modules and work in parallel where possible, followed by integration and testing. Each module will be validated before moving to the next stage.

The expected phases include:
1. Requirements review and system analysis
2. Database and backend design
3. UI and role-based dashboard development
4. Queue logic and SignalR integration
5. Reporting and analytics
6. Testing, debugging, and final deployment review

---

## 12. Timeline and Deliverables

### Proposed Timeline
- Week 1–2: Requirement gathering, planning, and system design
- Week 3–4: Database schema and API structure
- Week 5–6: Authentication and user management
- Week 7–8: Appointment booking and queue logic
- Week 9–10: Real-time communication and dashboards
- Week 11–12: Reporting, testing, and refinement
- Week 13: Final demonstration and project documentation

### Deliverables
- Functional web application
- SQL database schema
- Role-based user interfaces
- Queue processing logic
- Analytics dashboard
- Final project report and presentation

---

## 13. Expected Benefits

The system is expected to:
- Improve customer experience by reducing uncertainty and waiting time
- Increase operational efficiency in service centers
- Improve transparency between customers and staff
- Support decision-making through performance analytics
- Reduce manual workload and errors in queue handling

---

## 14. Risk and Challenges

Some challenges that may arise include:
- Incomplete or changing requirements during implementation
- Integration issues between frontend, API, and database
- Real-time queue synchronization complexity
- Need for secure role-based access across users
- Testing the system under various queue scenarios

These challenges will be addressed through regular team meetings, modular development, and continuous testing.

---

## 15. Project Scope Note

This proposal represents the initial scope of the project and is subject to refinement during development. Additional enhancements such as notifications (email/SMS), QR code check-ins, and predictive wait-time estimation may be introduced in later stages of the project.

This project will serve as a practical solution to real-world queue management problems and will demonstrate the application of modern web technologies, software design, and team-based software engineering.
