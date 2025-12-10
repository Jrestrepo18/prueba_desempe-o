# TalentoPlus - Employee Management System

![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-336791)
![License](https://img.shields.io/badge/License-MIT-green)

Comprehensive Human Resources Management System for the fictional company **TalentoPlus S.A.S**, developed with ASP.NET Core MVC, REST API and PostgreSQL.

## 📋 Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [REST API](#rest-api)
- [Docker](#docker)
- [Test Credentials](#test-credentials)

---

## ✨ Features

### 🌐 Web Application (Administrator)
- **Authentication** with ASP.NET Core Identity
- **Employee CRUD** (create, edit, list, delete)
- **Excel Import** (bulk update/register employees)
- **PDF Curriculum Vitae Generation**
- **Dashboard with AI** (Gemini) for natural language queries
- **Real-time Statistics** (total employees, active, on vacation, etc.)

### 🔌 REST API
- **Public Endpoints:**
  - List departments
  - Employee registration (self-service)
  - Employee login
  - Welcome email sending

- **Protected Endpoints (JWT):**
  - Get authenticated employee profile
  - Download Curriculum Vitae PDF

### 🧠 Artificial Intelligence
- **Google Gemini** integration
- Natural language queries about company data
- Responses based on real database information

### 📧 Services
- **EmailService**: SMTP email sending (Gmail compatible)
- **PdfService**: Dynamic curriculum vitae generation
- **GeminiService**: AI integration for queries

---

## 🛠️ Requirements

- **.NET SDK 8.0** or higher
- **PostgreSQL 12+**
- **Docker and Docker Compose** (optional, for container execution)
- **Git**
- **Visual Studio Code** (recommended) or Visual Studio

---

## 📦 Installation

### 1. Clone Repository

```bash
git clone <your-repository>
cd TalentoPlus.Web
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Environment Variables

Copy the example file:

```bash
cp .env.example .env
```

Edit `.env` and configure:

```env
# Database
DB_HOST=localhost
DB_PORT=5432
DB_NAME=prueba_jero
DB_USER=envyguard_user
DB_PASSWORD=jE15QhCwINzUNUw1FdclOB8YqZOE89

# JWT (keep as is for development)
JWT_SECRET=TalentoPlus_Secret_Key_2025_VeryLongAndSecure123456789
JWT_ISSUER=TalentoPlus
JWT_AUDIENCE=TalentoPlus

# Gemini API (Google) - IMPORTANT: Get API key
GEMINI_API_KEY=your-gemini-api-key-here

# SMTP for emails (Gmail)
SMTP_SERVER=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
```

**To get Gemini API Key:**
1. Go to [Google AI Studio](https://makersuite.google.com/app/apikey)
2. Create a free API key
3. Copy and paste it in `.env`

### 4. Update Database

```bash
# Apply migrations
dotnet ef database update

# This will create the tables automatically
```

---

## ⚙️ Configuration

### appsettings.json

The `appsettings.json` file contains the main configuration:

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=prueba_jero;Username=envyguard_user;Password=jE15QhCwINzUNUw1FdclOB8YqZOE89"
  },
  "Jwt": {
    "Secret": "TalentoPlus_Secret_Key_2025_VeryLongAndSecure123456789",
    "Issuer": "TalentoPlus",
    "Audience": "TalentoPlus",
    "ExpirationMinutes": 1440
  },
  "Gemini": {
    "ApiKey": "your-api-key",
    "Model": "gemini-pro"
  },
  "Email": {
    "Smtp": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

---

## 🚀 Usage

### Run Application in Development

```bash
dotnet run
```

The application will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`

### Access Admin Panel

1. Go to `http://localhost:5000/Account/Register`
2. Register an administrator user
3. Log in
4. Access `/Employees/Index` to see the listing

---

## 🗂️ Project Structure

```
TalentoPlus.Web/
│
├── Controllers/              # MVC and API Controllers
│   ├── HomeController.cs         # Dashboard
│   ├── EmployeesController.cs    # CRUD + Excel + PDF
│   ├── AccountController.cs      # Admin Authentication
│   └── Api/
│       ├── AuthController.cs     # Register/Login API
│       ├── MiPerfilController.cs # My Profile + PDF
│       └── DepartmentsController.cs
│
├── Models/                   # Entities and DTOs
│   ├── Employee.cs
│   ├── Department.cs
│   ├── ErrorViewModel.cs
│   └── Dtos.cs
│
├── Data/                     # Entity Framework Core
│   ├── ApplicationDbContext.cs
│   └── Migrations/           # Database change history
│
├── Repositories/             # Repository Pattern
│   ├── IRepositoryEmployee.cs
│   └── RepositoryEmployee.cs
│
├── Services/                 # Business Services
│   ├── EmailService.cs      # SMTP email sending
│   ├── PdfService.cs        # PDF generation
│   └── GeminiService.cs     # AI integration
│
├── Views/                    # Razor Views
│   ├── Home/
│   │   ├── Index.cshtml         # Dashboard
│   │   └── Privacy.cshtml
│   ├── Employees/
│   │   ├── Index.cshtml         # Listing
│   │   ├── Create.cshtml        # Create
│   │   ├── Edit.cshtml          # Edit
│   │   └── Delete.cshtml        # Delete
│   ├── Account/
│   │   ├── Login.cshtml
│   │   └── Register.cshtml
│   └── Shared/
│       ├── _Layout.cshtml       # Master layout
│       └── _ValidationScriptsPartial.cshtml
│
├── wwwroot/                  # Static files
│   ├── css/
│   └── js/
│
├── Program.cs               # DI Configuration
├── appsettings.json        # Configuration
├── .env.example            # Environment variables
├── Dockerfile              # Containerization
├── docker-compose.yml      # Orchestration
└── README.md               # This file
```

---

## 🔌 REST API

### Authentication

The API uses **JWT (JSON Web Tokens)** to protect private endpoints.

#### 1. Employee Registration (Public)

```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Smith",
  "document": "123456789",
  "email": "john@example.com",
  "phone": "+57 300 123 4567",
  "departmentId": 1
}
```

**Response:**
```json
{
  "message": "Successful registration. A confirmation email has been sent."
}
```

#### 2. Login (Public)

```http
POST /api/auth/login
Content-Type: application/json

{
  "document": "123456789",
  "email": "john@example.com"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "message": "Successful authentication"
}
```

#### 3. Get Profile (Protected)

```http
GET /api/miperfil
Authorization: Bearer <token>
```

**Response:**
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Smith",
  "email": "john@example.com",
  "position": "Analyst",
  "salary": 2500000.00,
  "departmentId": 1
}
```

#### 4. Download Curriculum Vitae (Protected)

```http
GET /api/miperfil/download-pdf
Authorization: Bearer <token>
```

Downloads a PDF with employee data.

#### 5. List Departments (Public)

```http
GET /api/departments
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Technology",
    "description": "IT and software development"
  },
  {
    "id": 2,
    "name": "Human Resources",
    "description": "Personnel management"
  }
]
```

---

## 🐳 Docker

### Run with Docker Compose

```bash
# Build and run
docker-compose up -d

# Check containers
docker-compose ps

# View logs
docker-compose logs -f app
```

### Access after Docker

- **Application**: `http://localhost:5000`
- **Database**: `localhost:5432`

### Stop containers

```bash
docker-compose down
```

---

## 🔐 Test Credentials

### Administrator (to create - first time)

No predefined user. First user is registered at:
- URL: `http://localhost:5000/Account/Register`

### Example Employees (after Excel import)

Can be created manually or imported from an Excel file with structure:

| FirstName | LastName | Document | Email | Position | Salary | HireDate | Status | EducationLevel | Department |
|-----------|----------|----------|-------|----------|--------|----------|--------|----------------|------------|
| John | Smith | 123456789 | john@example.com | Analyst | 2500000 | 2023-01-15 | Active | Professional | Technology |
| Jane | Doe | 987654321 | jane@example.com | Manager | 4000000 | 2022-06-10 | Active | Postgraduate | Sales |

---

## 📝 Import Employees from Excel

1. Go to `/Employees/ImportExcel`
2. Select an `.xlsx` file
3. The system:
   - Validates the structure
   - Creates or updates employees
   - Sends confirmation

---

## 🤖 Query AI (Gemini)

### In the Dashboard

1. Access `/Home/Index`
2. Write a question like:
   - "How many analysts are on the platform?"
   - "How many employees are on vacation?"
   - "What is the average salary?"

3. Gemini will respond based on real database data

---

## 📊 Database Migration

### Create new migration (after Model changes)

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Revert to previous migrations

```bash
dotnet ef database update PreviousMigrationName
```

---

## 🧪 Tests

Run unit and integration tests:

```bash
dotnet test
```

---

## 📄 License

MIT License - Academic project for TalentoPlus S.A.S

---

## 👨‍💻 Author

Developed as performance evaluation for **.NET** module

---

## 📞 Support

For questions or to report issues:
- Email: contacto@talentoplus.com
- Phone: +57 (1) 2345-6789

---

## 🔄 Recent Changes

- ✅ Base project structure
- ✅ Models and DbContext
- ✅ Authentication with Identity
- ✅ REST API with JWT
- ✅ Employee CRUD
- ✅ Excel import
- ✅ PDF generation
- ✅ Gemini AI integration
- ✅ Docker Compose
- ✅ Model internationalization (English)
- ⏳ Razor views (pending refinement)
- ⏳ Unit/integration tests (pending)

---

**Last update**: January 9, 2025
