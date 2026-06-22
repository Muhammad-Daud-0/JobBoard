<!-- @format -->

# 💼 JobBoard

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927)](https://www.microsoft.com/sql-server)

A comprehensive, production-ready Job Board application built with **ASP.NET Core 8.0 MVC**. This platform streamlines the hiring process by connecting job seekers with employers through an intuitive interface, real-time communication, intelligent job matching, and seamless application tracking.

> **Project Status:** Active Development

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Database](#database)
- [API Documentation](#api-documentation)
- [Usage Guide](#usage-guide)
- [User Roles](#user-roles)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

## Overview

**JobBoard** is designed to streamline the hiring process with a focus on user experience and scalability. The platform empowers employers to post and manage job opportunities while enabling job seekers to discover roles, submit applications with CVs, and communicate directly with recruiters through an integrated messaging system.

### Key Highlights

✨ **Clean Architecture** - Repository and Unit of Work patterns for maintainable code  
🔐 **Enterprise Security** - ASP.NET Core Identity + OAuth 2.0  
📱 **Responsive Design** - Bootstrap-based UI for all devices  
⚡ **Real-time Features** - Live chat between recruiters and applicants  
📊 **Admin Dashboard** - Centralized management and monitoring  
🧪 **Validated Input** - FluentValidation for data integrity

## ✨ Features

### For Job Seekers

- 🔍 **Browse & Search** - Discover job opportunities with filtering by category and location
- 📄 **Easy Applications** - One-click job applications with CV uploads
- 💬 **Direct Messaging** - Real-time chat with recruiters
- 👤 **Profile Management** - Comprehensive profile with avatar, address, and contact information
- 📊 **Application Tracking** - Monitor the status of all submitted applications

### For Employers

- ✍️ **Job Posting** - Create and manage job listings with detailed descriptions
- 👥 **Applicant Management** - Review, filter, and communicate with candidates
- 💬 **Candidate Communication** - Integrated chat system for direct candidate engagement
- 📈 **Job Analytics** - Track posted jobs and application metrics
- 🔧 **Job Editing** - Modify job details and requirements anytime

### For Administrators

- 👨‍💼 **User Management** - Monitor and manage all user accounts
- 📋 **Job Oversight** - Review and moderate all job postings
- 🔒 **Security Controls** - Manage permissions and user roles
- 📊 **System Monitoring** - View system statistics and activity logs

### Technical Features

- 🔐 **Secure Authentication** - ASP.NET Core Identity with password policies
- 🌐 **Social Login** - Google OAuth 2.0 integration
- ✅ **Input Validation** - FluentValidation for robust data validation
- 🛡️ **Exception Handling** - Global exception middleware with error tracking
- 📚 **API Documentation** - Swagger UI for API exploration
- 💾 **Database Migrations** - EF Core migrations for version control

## Tech Stack

- **Framework:** ASP.NET Core 8.0 MVC
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity, Google OAuth 2.0
- **Validation:** FluentValidation
- **API Docs:** Swagger (Swashbuckle)
- **Environment Management:** DotNetEnv
- **UI:** Razor Views, Bootstrap

## 📁 Project Structure

```
JobBoard/
├── Controllers/              # Request handlers and application logic routing
│   ├── AccountController.cs       # Authentication & user profile management
│   ├── JobsController.cs          # Job CRUD operations
│   ├── ApplicationsController.cs   # Application management
│   ├── ChatController.cs          # Messaging endpoints
│   ├── AdminController.cs         # Admin operations
│   └── HomeController.cs          # Home page
│
├── Models/                   # Data models and DTOs
│   ├── Entities/             # Database entity classes
│   │   ├── ApplicationUser.cs
│   │   ├── Job.cs
│   │   ├── JobApplication.cs
│   │   ├── ChatMessage.cs
│   │   └── Category.cs
│   ├── DTOs/                 # Data Transfer Objects
│   │   ├── Account/
│   │   ├── Jobs/
│   │   ├── Applications/
│   │   ├── Chat/
│   │   └── Auth/
│   └── ErrorViewModel.cs
│
├── Services/                 # Business logic layer
│   ├── JobService.cs
│   ├── ApplicationService.cs
│   ├── ChatService.cs
│   ├── AdminService.cs
│   └── Interfaces/           # Service contracts
│
├── Repositories/             # Data access layer
│   ├── JobRepository.cs
│   ├── ApplicationRepository.cs
│   ├── ChatRepository.cs
│   ├── UnitOfWork.cs
│   └── Interfaces/           # Repository contracts
│
├── Data/
│   ├── AppDbContext.cs       # EF Core DbContext
│   └── SeedData.cs           # Database seeding
│
├── Migrations/               # EF Core migrations
├── Views/                    # Razor templates (UI)
├── Middleware/               # Custom pipeline middleware
├── Validators/               # FluentValidation rules
├── wwwroot/                  # Static assets
├── Properties/
│   └── launchSettings.json   # Server configuration
├── appsettings.json          # Application settings
├── appsettings.Development.json
├── .env                      # Environment variables
├── Program.cs                # Application startup
├── JobBoard.csproj           # Project file
└── .gitignore
```

## 🚀 Getting Started

### Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) - The application framework
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) - Database server
  - **For development:** SQL Server LocalDB (included with Visual Studio) or Express Edition
  - **For production:** Full SQL Server instance
- [Git](https://git-scm.com/) - Version control
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) - Code editor

### Quick Start

1. **Clone the repository:**

   ```bash
   git clone https://github.com/Muhammad-Daud-0/JobBoard.git
   cd JobBoard
   ```

2. **Restore dependencies:**

   ```bash
   dotnet restore
   ```

3. **Configure the environment** (see [Configuration](#configuration) section below)

4. **Apply database migrations:**

   ```bash
   dotnet ef database update
   ```

5. **Run the application:**

   ```bash
   dotnet run
   ```

6. **Access the application:**
   - Open your browser and navigate to: `https://localhost:5001`
   - API Swagger documentation: `https://localhost:5001/swagger`

## ⚙️ Configuration

### Environment Variables

Create a `.env` file in the root directory with the following variables:

```env
# Database Connection
ConnectionStrings__DefaultConnection=Server=(localdb)\\mssqllocaldb;Database=JobBoardDb;Trusted_Connection=true;

# Google OAuth (optional, for social login)
Authentication__Google__ClientId=your_google_client_id
Authentication__Google__ClientSecret=your_google_client_secret

# Application Settings
ASPNETCORE_ENVIRONMENT=Development
```

### appsettings.json

Update `appsettings.json` for your environment:

```json
{
	"ConnectionStrings": {
		"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=JobBoardDb;Trusted_Connection=true;"
	},
	"Logging": {
		"LogLevel": {
			"Default": "Information",
			"Microsoft.EntityFrameworkCore": "Warning"
		}
	}
}
```

## 💾 Database

### Database Schema

The application uses the following main entities:

- **ApplicationUser** - Extended Identity user with profile information
- **Job** - Job postings with descriptions, requirements, and metadata
- **JobApplication** - Applications submitted by candidates
- **ChatMessage** - Messages exchanged between recruiters and applicants
- **Category** - Job categories for organization

### Migrations

View existing migrations:

```bash
dotnet ef migrations list
```

Create a new migration after model changes:

```bash
dotnet ef migrations add MigrationName
```

Apply pending migrations:

```bash
dotnet ef database update
```

Rollback to a previous migration:

```bash
dotnet ef database update PreviousMigrationName
```

## 📚 API Documentation

Once the application is running, access the interactive Swagger UI:

```
https://localhost:5001/swagger
```

### Main API Endpoints

#### Jobs

- `GET /api/jobs` - List all jobs
- `GET /api/jobs/{id}` - Get job details
- `POST /api/jobs` - Create a new job (requires employer role)
- `PUT /api/jobs/{id}` - Update a job
- `DELETE /api/jobs/{id}` - Delete a job

#### Applications

- `GET /api/applications` - List applications
- `POST /api/applications` - Submit a new application
- `GET /api/applications/{id}` - Get application details
- `PUT /api/applications/{id}/status` - Update application status

#### Chat

- `GET /api/chat/messages/{userId}` - Get messages with a user
- `POST /api/chat/messages` - Send a message
- `GET /api/chat/conversations` - Get all conversations

#### Account

- `POST /api/account/register` - Register a new user
- `POST /api/account/login` - User login
- `GET /api/account/profile` - Get user profile
- `PUT /api/account/profile` - Update user profile

## 👥 Usage Guide

### For Job Seekers

1. **Register/Login**
   - Create an account or use Google OAuth
   - Complete your profile with contact information

2. **Browse Jobs**
   - Navigate to the Jobs page
   - Filter by category or use search functionality
   - Click on a job to view full details

3. **Apply for Jobs**
   - Click "Apply" on any job listing
   - Upload your CV in the application form
   - Submit your application

4. **Track Applications**
   - Visit "My Applications" to see all submissions
   - Monitor the status of each application

5. **Chat with Recruiters**
   - Click the chat icon when an employer initiates contact
   - Send and receive messages in real-time

### For Employers

1. **Register as Employer**
   - Create account and select employer role during registration
   - Complete company information

2. **Post a Job**
   - Navigate to "Create Job"
   - Fill in job title, description, requirements
   - Set salary range and category
   - Publish the job

3. **Manage Applications**
   - Go to "My Jobs" to view all postings
   - Click on a job to see all applicants
   - Review CVs and application details

4. **Communicate**
   - Click on an application to view applicant details
   - Start a chat conversation with interested candidates
   - Exchange messages in real-time

### For Administrators

1. **User Management**
   - Access Admin Dashboard
   - View, manage, and modify user accounts
   - Assign or revoke admin roles

2. **Job Moderation**
   - Review all job postings
   - Remove inappropriate job listings
   - Monitor job posting activity

3. **System Oversight**
   - View system statistics and activity
   - Monitor application trends
   - Track platform usage metrics

## 👤 User Roles

The application supports three main user roles:

### Job Seeker

- Default role for new users
- Can browse and search job listings
- Can submit job applications
- Can chat with recruiters
- Can manage personal profile and CV

### Employer

- Can create, edit, and delete job postings
- Can manage and review job applications
- Can communicate with job seekers
- Can track job posting performance
- Access to applicant management dashboard

### Administrator

- Full system access
- Can manage all users and assign roles
- Can moderate content (jobs, applications)
- Can view system-wide statistics
- Can access audit logs and monitoring

## 🔧 Troubleshooting

### Common Issues

#### Database Connection Fails

```
Error: "Cannot open database requested by the login"
```

**Solution:**

- Verify SQL Server is running: `sqlcmd -S (localdb)\mssqllocaldb`
- Check connection string in `appsettings.json`
- Ensure the database user has proper permissions

#### Port Already in Use

```
Error: "Unable to bind to http://127.0.0.1:5001"
```

**Solution:**

- Change the port in `Properties/launchSettings.json`
- Or kill the process: `netstat -ano | findstr :5001`

#### Migrations Not Applied

```
Error: "No database provider has been configured"
```

**Solution:**

```bash
dotnet ef database update
```

#### Google OAuth Not Working

**Solution:**

- Verify `ClientId` and `ClientSecret` in `.env` or `appsettings.json`
- Ensure redirect URI is configured in Google Console
- Check that credentials match your environment

#### Build Errors

**Solution:**

```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:

1. **Fork the repository**

   ```bash
   git clone https://github.com/yourusername/JobBoard.git
   ```

2. **Create a feature branch**

   ```bash
   git checkout -b feature/YourFeatureName
   ```

3. **Make your changes**
   - Follow existing code style and conventions
   - Add comments for complex logic
   - Test your changes thoroughly

4. **Commit with descriptive messages**

   ```bash
   git commit -m "Add: description of your changes"
   ```

5. **Push to your fork**

   ```bash
   git push origin feature/YourFeatureName
   ```

6. **Create a Pull Request**
   - Provide a clear description of changes
   - Reference any related issues
   - Include any relevant screenshots or logs

### Code Style

- Follow [C# naming conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/naming-conventions)
- Use meaningful variable and method names
- Keep methods focused and single-responsibility
- Add XML documentation comments for public methods
- Maintain consistent indentation (4 spaces)

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

The MIT License permits:

- ✅ Commercial use
- ✅ Modification
- ✅ Distribution
- ✅ Private use

With the conditions:

- ⚠️ Include license and copyright notice

## 📞 Support & Contact

### Getting Help

- **Issues**: Report bugs or request features on [GitHub Issues](https://github.com/Muhammad-Daud-0/JobBoard/issues)
- **Discussions**: Join [GitHub Discussions](https://github.com/Muhammad-Daud-0/JobBoard/discussions) for Q&A

### Contact Information

- **Author**: Muhammad Daud
- **Email**: [contact@example.com](mailto:contact@example.com)
- **GitHub**: [@Muhammad-Daud-0](https://github.com/Muhammad-Daud-0)

---

### Acknowledgments

- ASP.NET Core team for the excellent framework
- Entity Framework Core for ORM capabilities
- FluentValidation for validation framework
- Bootstrap community for UI components

### Project Roadmap

- [ ] Enhanced search and filtering
- [ ] Advanced job recommendations AI
- [ ] Video interview integration
- [ ] Notification system (email, SMS)
- [ ] Payment integration for premium features
- [ ] Mobile app (iOS/Android)
- [ ] API rate limiting and security enhancements
- [ ] Performance optimization and caching

---

**Happy coding! 🚀**
