# CompanyERP API

A RESTful Web API for managing company operations — employees, branches, projects, and automated payroll processing — built with **ASP.NET Core 9** and **Oracle Database**.

---

## Tech Stack

| | |
|---|---|
| **Framework** | ASP.NET Core 9 Web API |
| **Database** | Oracle DB via Entity Framework Core 8 |
| **Authentication** | JWT Bearer Tokens |
| **Authorization** | Role-Based Access Control (RBAC) |
| **Mapping** | AutoMapper 12 |
| **Documentation** | Swagger / OpenAPI (Swashbuckle) |
| **Password Security** | ASP.NET Identity PasswordHasher |
| **Background Jobs** | IHostedService / BackgroundService |

---

## Features

- **JWT Authentication** — secure login with token-based auth and claim extraction
- **Role-Based Authorization** — three roles with fine-grained endpoint permissions: `Admin`, `Manager`, `Employee`
- **Employee Management** — full CRUD with branch assignment and secure password hashing
- **Branch Management** — create and manage company branches
- **Project Management** — track projects with status, budget, and team assignments
- **Employee-Project Assignment** — many-to-many relationship with report generation
- **Payroll Processing** — monthly salary calculation with overtime (1.5×) and deduction logic
- **Salary Preview** — preview net salary before committing to the database
- **Automated Payroll** — background worker runs on the 30th of each month, auto-processes all employees and skips those already processed manually
- **Audit Logging** — every Create / Update / Delete operation across all tables is automatically captured via `SaveChangesAsync` override, storing old/new values as JSON with the acting user's identity
- **Global Exception Handling** — custom middleware for consistent error responses

---

## Project Structure

```
CompanyERP/
├── Controllers/          # API endpoints & routing
├── Services/             # Business logic implementation
├── IServices/            # Service interfaces
├── Entities/             # Database models
├── DTOs/                 # Data transfer objects
├── Profiles/             # AutoMapper mapping profiles
├── Security/             # JWT token generation
├── Exceptions/           # Custom exceptions & middleware
├── Data/                 # AppDbContext (with audit hook)
├── BackgroundServices/   # Automated payroll worker
├── Migrations/           # EF Core migrations (seeded admin user)
└── Enums/                # UserRole, ProjectStatus
```

---

## API Endpoints

### Auth
| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/api/auth/login` | Login and receive JWT token | Public |

### Employees
| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/employees` | Get all employees | All roles |
| GET | `/api/employees/{id}` | Get employee by ID | All roles |
| POST | `/api/employees` | Create employee | Admin |
| PUT | `/api/employees` | Update employee | Admin |
| DELETE | `/api/employees/{id}` | Delete employee | Admin |
| GET | `/api/employees/{id}/preview-salary?actualHours=` | Preview monthly net salary | Admin, Manager |
| POST | `/api/employees/{id}/calculate-salary?actualHours=&month=&year=` | Save monthly payroll record | Admin, Manager |

### Branches
| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/branches` | Get all branches | All roles |
| GET | `/api/branches/{id}` | Get branch by ID | All roles |
| POST | `/api/branches` | Create branch | Admin |
| PUT | `/api/branches` | Update branch | Admin |
| DELETE | `/api/branches/{id}` | Delete branch | Admin |

### Projects
| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/projects` | Get all projects | All roles |
| GET | `/api/projects/{id}` | Get project by ID | All roles |
| POST | `/api/projects` | Create project | Admin, Manager |
| PUT | `/api/projects` | Update project | Admin, Manager |
| DELETE | `/api/projects/{id}` | Delete project | Admin, Manager |
| GET | `/api/projects/{id}/budget-report` | Get project budget report | Admin, Manager |

### Employee-Project Assignments
| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/api/employeeprojects` | Assign employee to project | Admin, Manager |
| DELETE | `/api/employeeprojects/remove?employeeId=&projectId=` | Remove employee from project | Admin, Manager |
| GET | `/api/employeeprojects/employee/{employeeId}` | Get all projects for an employee | All roles |
| GET | `/api/employeeprojects/project/{projectId}` | Get all employees on a project | All roles |

### Audit Logs
| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/auditlogs?pageNumber=&pageSize=` | Get paginated audit logs | Admin |
| GET | `/api/auditlogs/{id}` | Get audit log by ID | Admin |

---

## Roles & Permissions

| Action | Admin | Manager | Employee |
|--------|:-----:|:-------:|:--------:|
| View employees, branches, projects | ✅ | ✅ | ✅ |
| View employee/project reports | ✅ | ✅ | ✅ |
| Create / Update / Delete employees & branches | ✅ | ❌ | ❌ |
| Create / Update / Delete projects | ✅ | ✅ | ❌ |
| Assign / remove employees from projects | ✅ | ✅ | ❌ |
| Preview & save payroll | ✅ | ✅ | ❌ |
| View audit logs | ✅ | ❌ | ❌ |

---

## Payroll Calculation Logic

Monthly payroll is based on **160 standard hours/month**:

| Scenario | Formula |
|----------|---------|
| Hours > 160 (Overtime) | Extra hours × hourly rate × **1.5** |
| Hours < 160 (Deduction) | Missing hours × hourly rate |
| Net Salary | Base Salary + Overtime − Deductions |

Duplicate payroll entries for the same employee/month/year are automatically rejected.
The background worker runs hourly, processes payroll on day 30, and skips employees already processed manually.

---

## Audit Logging

All data changes (Added / Modified / Deleted) across every entity are automatically captured by overriding `SaveChangesAsync` in `AppDbContext`. Each log entry records:

- **Who** — the authenticated user's email (or `System/Worker` for background jobs)
- **What** — the affected table and action type
- **When** — UTC timestamp
- **Delta** — old values and new values serialized as JSON

---

## Getting Started

### Prerequisites

- .NET 9 SDK
- Oracle Database instance

### Configuration

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=YOUR_USER;Password=YOUR_PASSWORD;Data Source=YOUR_HOST:1521/YOUR_SERVICE;"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "CompanyERP",
    "Audience": "CompanyERP"
  }
}
```

### Run

```bash
git clone https://github.com/AmjadQattawi/CompanyERP.git
cd CompanyERP
dotnet restore
dotnet ef database update
dotnet run
```

Swagger UI: `https://localhost:{port}/swagger`

### Default Admin Credentials (seeded via migration)

| Field | Value |
|-------|-------|
| Email | `admin@company.com` |
| Password | `Admin123!` |

### Authenticate in Swagger

1. `POST /api/auth/login` → copy the token
2. Click **Authorize** → enter `Bearer YOUR_TOKEN`

---

## Author

**Amjad Qattawi** — Backend Developer  
[LinkedIn](https://www.linkedin.com/in/amjad-qattawi) · [GitHub](https://github.com/AmjadQattawi)
