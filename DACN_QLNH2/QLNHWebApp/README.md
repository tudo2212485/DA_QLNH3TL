# 🍽️ QLNHWebApp - Restaurant Management Backend

ASP.NET Core 9.0 Web Application với MVC pattern, Entity Framework Core và SQLite database.

## 🚀 Quick Start

```bash
# Restore packages
dotnet restore

# Apply migrations
dotnet ef database update

# Run application
dotnet run
```

## 📦 Dependencies

- **ASP.NET Core 9.0** - Web Framework
- **Entity Framework Core** - ORM
- **SQLite** - Database (Located at: `../data/QLNHDB.db`)
- **BCrypt.Net** - Password Hashing
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI Documentation

## 🗂️ Project Structure

```
QLNHWebApp/
├── Controllers/              # MVC & API Controllers
│   ├── AdminController.cs
│   ├── AuthController.cs
│   ├── OrderManagementController.cs
│   ├── AdminMenuController.cs
│   ├── AdminCustomerController.cs
│   ├── SettingsController.cs
│   └── Api/                  # RESTful API
│       ├── OrdersApiController.cs
│       ├── MenuApiController.cs
│       └── TableApiController.cs
├── Models/                   # Data Models
│   ├── RestaurantModels.cs
│   └── RestaurantDbContext.cs
├── Views/                    # Razor Views
│   ├── Admin/
│   ├── Auth/
│   ├── OrderManagement/
│   └── Settings/
├── Services/
│   └── DataSeederService.cs  # Seed initial data
├── Migrations/               # EF Core Migrations
├── wwwroot/                  # Static files
└── Program.cs                # App configuration
```

## 🔑 Default Accounts

| Username | Password | Role | Access Level |
|----------|----------|------|--------------|
| `admin` | `admin123` | Admin | Full access |
| `nhanvien` | `nv123` | Nhân viên | Orders, Bookings, Menu |
| `daubep` | `db123` | Đầu bếp | View Menu only |
| `dotrungb` | `dotrungb123` | Nhân viên | Orders, Bookings, Menu |

## ✨ Features

### 🔐 Authentication & Authorization
- Login/Logout with BCrypt hashing
- Role-based access control (Admin, Nhân viên, Đầu bếp)
- Session-based authentication
- Policy-based authorization

### 📊 Dashboard
- Revenue by month (Chart.js Line Chart)
- Orders by status (Pie Chart)
- Top 5 selling items (Bar Chart)
- Summary statistics

### 🍽️ Menu Management
- Full CRUD operations
- Image upload support (JPG/PNG, max 1MB)
- Search, filter, sort
- Category management

### 📋 Order Management
- Current orders & history
- Add/Edit/Remove items
- Table transfer
- Payment processing
- Card & Table view modes

### 👥 Employee Management
- CRUD operations
- Role assignment
- Statistics tracking

### 👤 Customer Management
- Auto-created from orders
- Statistics (Total, New, Loyal, VIP)
- Order history

### ⚙️ System Settings
- Restaurant information
- Operating hours
- Tax rate configuration
- Reset to defaults

### 🔌 RESTful API
- Server-side pagination
- Swagger UI (`/swagger`)
- JSON responses
- Data validation

## 🌐 API Endpoints

### Orders API (Paginated)
```http
GET /api/Orders?page=1&pageSize=10&search=&status=&sortBy=date&sortOrder=desc
```

### Statistics API
```http
GET /api/Orders/statistics
```

### Menu API
```http
GET /api/Menu
GET /api/Menu/{id}
```

### Table API
```http
GET /api/Table/GetTablesByFloor?floor=Tầng 1&guests=4
```

## 🔧 Configuration

### Database Connection
File: `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=../data/QLNHDB.db"
  }
}
```

### Session Settings
- Timeout: 30 minutes
- Cookie name: `.AspNetCore.Session`

### Authentication
- Scheme: `AdminAuth` (Cookie-based)
- Login path: `/Auth/Login`
- Logout path: `/Auth/Logout`

## 🗄️ Database

### Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

### Seed Data
Data seeding is automatically triggered on first run via `DataSeederService`:
- 4 employees (1 Admin, 2 Staff, 1 Chef)
- 20+ menu items
- 19 tables (3 floors)
- 15 sample orders (6 months of data)

## 🧪 Testing

### Test with Swagger
1. Run app: `dotnet run`
2. Navigate to: http://localhost:5000/swagger
3. Try endpoints directly from UI

### Test Authentication
1. Navigate to: http://localhost:5000/Auth/Login
2. Login with default credentials
3. Access admin features

## 📝 Common Commands

```bash
# Clean build
dotnet clean
dotnet build

# Watch mode (auto-reload)
dotnet watch run

# Create new migration
dotnet ef migrations add YourMigrationName

# Update database
dotnet ef database update

# Kill stuck processes (Windows)
taskkill /F /IM dotnet.exe
taskkill /F /IM QLNHWebApp.exe
```

## 🐛 Troubleshooting

### Port already in use
```bash
# Kill existing processes
taskkill /F /IM dotnet.exe
taskkill /F /IM QLNHWebApp.exe

# Then run again
dotnet run
```

### Database not found
```bash
# Ensure migrations are applied
dotnet ef database update
```

### Build cache issues
```bash
# Clean and rebuild
dotnet clean
Remove-Item -Recurse -Force bin,obj
dotnet restore
dotnet build
```

## 📖 More Info

See main `README.md` in repository root for complete documentation.

## 📞 Support

For issues and questions, please contact the development team.

---

**Made with ❤️ using ASP.NET Core 9.0**
