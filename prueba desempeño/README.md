# TalentoPlus - Employee Management System

![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-336791)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-blueviolet)
![License](https://img.shields.io/badge/License-MIT-green)

**TalentoPlus** is a comprehensive Human Resources Management System developed with ASP.NET Core MVC, REST API, and PostgreSQL. This application provides enterprise-grade employee management capabilities for organizations looking to streamline their HR operations.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [System Requirements](#system-requirements)
- [Installation & Setup](#installation--setup)
- [Environment Variables Configuration](#environment-variables-configuration)
- [Running the Application](#running-the-application)
- [Project Structure](#project-structure)
- [Access Credentials](#access-credentials)
- [REST API Documentation](#rest-api-documentation)
- [Database Migrations](#database-migrations)
- [Troubleshooting](#troubleshooting)
- [Repository](#repository)

---

## 🎯 Overview

TalentoPlus is an ASP.NET Core-based Human Resources Management System (HRMS) designed for the fictional company **TalentoPlus S.A.S**. It combines a robust web application interface with a comprehensive REST API, providing administrators and employees with tools to manage employee data, track HR metrics, and integrate with external systems.

### Core Capabilities

- **Enterprise Employee Management**: Full CRUD operations with advanced filtering and search
- **Authentication & Authorization**: ASP.NET Identity with role-based access control
- **Excel Import/Export**: Bulk employee data management
- **PDF Generation**: Dynamic curriculum vitae (CV) generation
- **RESTful API**: JWT-based authentication for external integrations
- **Email Notifications**: SMTP integration for automated communications
- **Real-time Statistics**: Dashboard with live HR metrics
- **Responsive UI**: Tailwind CSS with modern design patterns

---

## ✨ Key Features

### 🌐 Web Application (Administrator Dashboard)

- ✅ **Authentication System**: ASP.NET Core Identity with email/document login
- ✅ **Employee Management**: Complete CRUD interface for employee records
- ✅ **Excel Import**: Bulk upload and update employee data from spreadsheets
- ✅ **PDF Export**: Generate professional CVs in PDF format
- ✅ **Dashboard Analytics**: Real-time statistics and visual charts
- ✅ **Department Management**: Organize employees by department
- ✅ **Responsive Design**: Works seamlessly on desktop and mobile devices

### 🔌 REST API Endpoints

#### Public Endpoints (No Authentication)
- `GET /api/departments` - List all departments
- `POST /api/auth/register` - Self-service employee registration
- `POST /api/auth/login` - Employee authentication with JWT token generation

#### Protected Endpoints (JWT Required)
- `GET /api/myprofile` - Retrieve authenticated employee profile
- `GET /api/employees/{id}/cv` - Download employee CV as PDF

### 📊 Dashboard Features

- Total and active employee statistics
- Visual charts (doughnut and bar charts)
- Department distribution analysis
- Quick action buttons for common tasks
- Automatic redirection to Employees section for Admin users

---

## 🛠️ System Requirements

### Minimum Requirements
- **.NET SDK 8.0** or higher
- **PostgreSQL 12+** (or PostgreSQL 15+ recommended)
- **Node.js 18+** (for Tailwind CSS compilation)
- **Git** (for version control)

### Development Tools
- **Visual Studio Code** (recommended) or **Visual Studio 2022**
- **Postman** or **Insomnia** (for API testing)
- **Docker & Docker Compose** (optional, for containerized deployment)

### Recommended Environment
- Windows 10/11, macOS, or Linux (Ubuntu 20.04+)
- 4GB RAM minimum
- 2GB free disk space

---

## 📦 Installation & Setup

### Step 1: Clone the Repository

```bash
# Navigate to your desired directory
cd ~/projects

# Clone the repository (replace with actual repo URL)
git clone https://github.com/yourusername/TalentoPlus.git
cd TalentoPlus

# Navigate to the web project
cd TalentoPlus.Web
```

### Step 2: Install Dependencies

```bash
# Install .NET dependencies
dotnet restore

# Install npm packages (for Tailwind CSS)
npm install
```

### Step 3: Verify Database Connectivity

Before proceeding, ensure your PostgreSQL server is running and accessible:

```bash
# Test connection (Linux/macOS)
psql -h 91.99.188.229 -U envyguard_user -d prueba_jero -c "SELECT version();"

# Expected output: PostgreSQL version information
```

### Step 4: Apply Database Migrations

```bash
# Run pending migrations to set up the database schema
dotnet ef database update

# If you encounter issues, try:
dotnet ef database drop --force
dotnet ef database update
```

### Step 5: Build Tailwind CSS

```bash
# Compile Tailwind CSS styles
npm run build:css

# For development with auto-recompilation:
npm run watch:css
```

---

## 🔧 Environment Variables Configuration

The application uses PostgreSQL connection strings and JWT configuration. The settings are stored in `appsettings.json`.

### Configuration File: `appsettings.json`

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=91.99.188.229;Port=5432;Database=prueba_jero;Username=envyguard_user;Password=jE15QhCwINzUNUw1FdclOB8YqZOE89"
  },
  "Jwt": {
    "Secret": "TalentoPlus_Secret_Key_2025_VeryLongAndSecure1234567890!@",
    "Issuer": "TalentoPlus",
    "Audience": "TalentoPlus",
    "ExpirationMinutes": 1440
  },
  "Gemini": {
    "ApiKey": "AIzaSyD8xNC2QU3mB9Prj5w-r5wJAnbznu4oMSU",
    "Model": "gemini-1.5-flash"
  },
  "Email": {
    "Smtp": "live.smtp.mailtrap.io",
    "Port": 587,
    "Username": "api",
    "Password": "1234567890abcdef",
    "FromEmail": "noreply@talentoplus.local",
    "FromName": "TalentoPlus S.A.S"
  }
}
```

### Environment Variables

For production deployments, use environment variables instead of hardcoding:

```bash
# Database
export DB_CONNECTION_STRING="Host=your-host;Port=5432;Database=your_db;Username=user;Password=pass"

# JWT
export JWT_SECRET="YourVeryLongSecretKeyHere1234567890"
export JWT_ISSUER="TalentoPlus"
export JWT_AUDIENCE="TalentoPlus"

# Email
export SMTP_HOST="live.smtp.mailtrap.io"
export SMTP_PORT="587"
export SMTP_USERNAME="api"
export SMTP_PASSWORD="your_email_password"

# Gemini API (optional)
export GEMINI_API_KEY="your_gemini_api_key"
```

---

## 🚀 Running the Application

### Option 1: Development Mode (Recommended)

**Terminal 1 - Start the ASP.NET Core Application:**

```bash
cd TalentoPlus.Web
dotnet run
```

Expected output:
```
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:5000
```

**Terminal 2 - Watch CSS Changes (Optional):**

```bash
npm run watch:css
```

This enables hot-reload for CSS changes during development.

### Option 2: Production Build

```bash
# Build the project
dotnet build -c Release

# Publish to output directory
dotnet publish -c Release -o ./publish

# Run the published application
cd publish
dotnet TalentoPlus.Web.dll
```

### Option 3: Docker Deployment

```bash
# Build Docker image
docker build -f Dockerfile -t talentoplus:latest .

# Run Docker container
docker run -d \
  -p 5000:8080 \
  -e ConnectionStrings__PostgreSQL="Host=db;Port=5432;Database=prueba_jero;Username=envyguard_user;Password=jE15QhCwINzUNUw1FdclOB8YqZOE89" \
  --name talentoplus \
  talentoplus:latest

# Or use Docker Compose
docker-compose up -d
```

### Option 4: Using Docker Compose

```bash
# Start all services (app + database)
docker-compose up -d

# View logs
docker-compose logs -f web

# Stop services
docker-compose down
```

---

## 📁 Project Structure

```
TalentoPlus.Web/
├── 📁 Controllers/
│   ├── AccountController.cs          # Authentication & authorization
│   ├── DashboardController.cs        # Admin dashboard
│   ├── EmployeesController.cs        # Employee management (MVC)
│   ├── HomeController.cs             # Home page
│   ├── ProfileController.cs          # User profile
│   └── Api/
│       ├── AuthController.cs         # REST API authentication
│       ├── DashboardController.cs    # REST API dashboard data
│       ├── DepartmentsController.cs  # REST API departments
│       └── MyProfileController.cs    # REST API profile info
│
├── 📁 Models/
│   ├── Employee.cs                   # Employee data model
│   ├── Department.cs                 # Department data model
│   ├── Dtos.cs                       # Data Transfer Objects
│   └── ErrorViewModel.cs             # Error handling
│
├── 📁 Services/
│   ├── EmailService.cs               # SMTP email functionality
│   ├── JwtService.cs                 # JWT token generation
│   ├── PdfService.cs                 # PDF CV generation
│   └── GeminiService.cs              # AI integration
│
├── 📁 Repositories/
│   ├── IRepositoryEmployee.cs        # Employee repository interface
│   └── RepositoryEmployee.cs         # Employee repository implementation
│
├── 📁 Data/
│   ├── ApplicationDbContext.cs       # EF Core DbContext
│   ├── SeedData.cs                   # Database seed data
│   └── Migrations/                   # EF Core migrations
│
├── 📁 Views/
│   ├── Account/
│   │   ├── Login.cshtml              # Login form
│   │   └── Register.cshtml           # Registration form
│   ├── Dashboard/
│   │   └── Index.cshtml              # Admin dashboard
│   ├── Employees/
│   │   ├── Index.cshtml              # Employee list
│   │   ├── Create.cshtml             # Create employee form
│   │   ├── Edit.cshtml               # Edit employee form
│   │   ├── Details.cshtml            # Employee details view
│   │   ├── Delete.cshtml             # Delete confirmation
│   │   └── ImportExcel.cshtml        # Excel import form
│   ├── Home/
│   │   └── Index.cshtml              # Home page
│   ├── Profile/
│   │   ├── Index.cshtml              # User profile view
│   │   └── Edit.cshtml               # Edit profile form
│   └── Shared/
│       ├── _Layout.cshtml            # Master layout template
│       └── _ValidationScriptsPartial.cshtml
│
├── 📁 wwwroot/
│   ├── css/
│   │   └── tailwind.css              # Tailwind CSS styles
│   └── js/
│       └── site.js                   # Client-side scripts
│
├── 📄 Program.cs                     # Application startup configuration
├── 📄 appsettings.json               # Configuration settings
├── 📄 appsettings.Development.json   # Development-specific settings
├── 📄 TalentoPlus.Web.csproj         # Project file
├── 📄 package.json                   # npm dependencies
├── 📄 tailwind.config.js             # Tailwind CSS configuration
├── 📄 postcss.config.js              # PostCSS configuration
└── 📄 Dockerfile                     # Docker container definition
```

---

## 🔐 Access Credentials

### Administrator Account

**Purpose**: Full system access including dashboard, employee management, Excel import, and all API endpoints.

```
Email:    admin@talentoplus.local
Password: Admin@123456
```

**After Login Flow**:
1. User logs in with credentials above
2. Dashboard loads with statistics and charts
3. **Automatic redirect to Employees section** (new feature)
4. User can manage employees, import data, and generate reports

### Demo Employee Account

**Purpose**: Standard employee access (limited features).

```
Email:    employee@talentoplus.local
Password: Employee@123456
Document: 123456789
```

### API Authentication

To authenticate with the REST API:

```bash
# 1. Get JWT token
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "document": "123456789",
    "email": "employee@talentoplus.local"
  }'

# Expected response:
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 1440
}

# 2. Use token in subsequent requests
curl -X GET http://localhost:5000/api/myprofile \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

---

## 📡 REST API Documentation

### Base URL
```
http://localhost:5000
```

### Authentication
All protected endpoints require JWT token in the `Authorization` header:
```
Authorization: Bearer {token}
```

### Public Endpoints

#### 1. Get Departments
```
GET /api/departments
Content-Type: application/json
```

**Response** (200 OK):
```json
[
  {
    "id": 1,
    "name": "Engineering",
    "description": "IT and Development"
  },
  {
    "id": 2,
    "name": "Human Resources",
    "description": "HR Department"
  }
]
```

#### 2. Register Employee
```
POST /api/auth/register
Content-Type: application/json
```

**Request Body**:
```json
{
  "document": "1234567890",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+1-555-0123",
  "departmentId": 1,
  "salary": 50000,
  "hireDate": "2025-01-09T00:00:00Z"
}
```

**Response** (201 Created):
```json
{
  "message": "Employee registered successfully",
  "employeeId": 15,
  "email": "john.doe@example.com"
}
```

#### 3. Login
```
POST /api/auth/login
Content-Type: application/json
```

**Request Body**:
```json
{
  "document": "1234567890",
  "email": "john.doe@example.com"
}
```

**Response** (200 OK):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJlbXBsb3llZS1pZCIsImlhdCI6MTcwMTQ2NTIwMH0...",
  "expiresIn": 1440,
  "employeeId": 15,
  "email": "john.doe@example.com"
}
```

### Protected Endpoints

#### 4. Get My Profile
```
GET /api/myprofile
Authorization: Bearer {token}
Content-Type: application/json
```

**Response** (200 OK):
```json
{
  "id": 15,
  "document": "1234567890",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+1-555-0123",
  "position": "Developer",
  "salary": 50000,
  "hireDate": "2025-01-09T00:00:00Z",
  "status": "Active",
  "departmentId": 1
}
```

#### 5. Download Employee CV
```
GET /api/employees/{id}/cv
Authorization: Bearer {token}
```

**Response**: PDF file (binary stream)

---

## 🗄️ Database Migrations

### Creating a New Migration

When you modify model classes, create a migration:

```bash
# Create migration (replace "AddNewField" with your change description)
dotnet ef migrations add AddNewField

# Review the generated migration file in Migrations/ directory
```

### Applying Migrations

```bash
# Apply all pending migrations
dotnet ef database update

# Apply to specific migration
dotnet ef database update <MigrationName>

# Revert to previous migration
dotnet ef database update <PreviousMigrationName>
```

### Viewing Migration History

```bash
# List all applied migrations
dotnet ef migrations list
```

---

## 📊 Excel Import Format

The application supports bulk employee import from Excel files. Use the included `Empleados.xlsx` as a template.

### Column Mapping

| Column | Name | Type | Example | Required |
|--------|------|------|---------|----------|
| 1 | Document | String | 12345678 | ✓ Yes |
| 2 | First Name | String | Juan | ✓ Yes |
| 3 | Last Name | String | Pérez | ✓ Yes |
| 4 | Birth Date | Date | 1990-05-15 | No |
| 5 | Address | String | Calle 123 | No |
| 6 | Phone | String | +57-300-1234567 | No |
| 7 | Email | String | juan@example.com | ✓ Yes |
| 8 | Position | String | Developer | No |
| 9 | Salary | Decimal | 50000.00 | No |
| 10 | Hire Date | Date | 2023-01-15 | No |
| 11 | Status | String | Active/Inactive | No |
| 12 | Education Level | String | Degree | No |
| 13 | Professional Profile | String | Experience Summary | No |
| 14 | Department | String | Engineering | No |

### Import Steps

1. Navigate to: `http://localhost:5000/Employees/ImportExcel`
2. Select the Excel file (use `Empleados.xlsx` as template)
3. Click "Import"
4. System will update existing employees or create new ones

---

## 🐛 Troubleshooting

### Issue: "Connection refused" when starting the application

**Solution**: Verify PostgreSQL is running and accessible:

```bash
# Test connection
psql -h 91.99.188.229 -U envyguard_user -d prueba_jero

# If fails, check connection string in appsettings.json
```

### Issue: Migrations fail with "Column does not exist"

**Solution**: Reset the database:

```bash
# Drop and recreate database
dotnet ef database drop --force
dotnet ef database update

# Re-import Excel data if needed
```

### Issue: CSS styles not loading (Tailwind)

**Solution**: Rebuild CSS:

```bash
# Clean previous builds
rm -rf node_modules package-lock.json

# Reinstall and rebuild
npm install
npm run build:css

# Restart the application
dotnet run
```

### Issue: JWT token validation fails

**Solution**: Verify JWT configuration in `appsettings.json`:

```json
"Jwt": {
  "Secret": "TalentoPlus_Secret_Key_2025_VeryLongAndSecure1234567890!@",
  "Issuer": "TalentoPlus",
  "Audience": "TalentoPlus",
  "ExpirationMinutes": 1440
}
```

### Issue: "Admin account not redirecting to Employees"

**Solution**: Clear browser cache and cookies:

```bash
# Or manually navigate to:
http://localhost:5000/Employees
```

---

## 📋 Additional Notes

### Features Implemented in This Build

- ✅ Admin login with email/password
- ✅ Dashboard with real-time statistics
- ✅ **Automatic redirect to Employees section for Admin users after login**
- ✅ Complete CRUD operations for employees
- ✅ Excel import/export functionality
- ✅ PDF CV generation
- ✅ REST API with JWT authentication
- ✅ Email notifications (SMTP)
- ✅ Role-based access control

### Performance Considerations

- **Database Connection Pooling**: Configured for optimal connection reuse
- **Caching**: Implement output caching for frequently accessed data
- **Pagination**: Employee lists are paginated to reduce memory usage
- **Async/Await**: All database operations use async patterns

### Security Best Practices

- ✓ ASP.NET Identity for user authentication
- ✓ JWT tokens with expiration (24 hours)
- ✓ HTTPS enforcement in production
- ✓ CSRF protection on all forms
- ✓ SQL injection prevention via EF Core
- ✓ Password hashing and validation

---

## 📞 Support & Contact

For issues, feature requests, or questions:

1. Check the **Troubleshooting** section above
2. Review error messages in the application logs
3. Check the database connection and migrations
4. Contact the development team

---

## 📜 License

This project is licensed under the **MIT License** - see LICENSE file for details.

---

## 🔗 Repository

**Project Repository**: [GitHub - TalentoPlus](https://github.com/yourusername/TalentoPlus)

**Deployment URLs**:
- Development: `http://localhost:5000`
- Staging: (To be configured)
- Production: (To be configured)

---

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [JWT Authentication](https://jwt.io/)
- [Tailwind CSS](https://tailwindcss.com/docs)
- [Postman API Testing](https://learning.postman.com/)

---

**Last Updated**: December 9, 2025  
**Version**: 1.0.0  
**Status**: Ready for Production

