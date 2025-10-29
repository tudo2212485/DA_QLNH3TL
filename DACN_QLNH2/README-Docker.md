# Docker Setup cho Restaurant Management System

## Yêu cầu
- Docker Desktop đã được cài đặt và chạy
- .NET 9.0 SDK (để development)

## Cách sử dụng

### 1. Build và chạy với Docker Compose
```bash
# Build và start tất cả services
docker-compose up --build

# Chạy trong background
docker-compose up -d --build
```

### 2. Access các services

- **Ứng dụng chính**: http://localhost:5000
- **SQLite Web GUI**: http://localhost:8081 (để quản lý database)
- **Adminer**: http://localhost:8080 (alternative database manager)

### 3. Quản lý Database

#### SQLite Web GUI (Recommended)
- Truy cập: http://localhost:8081
- File database: `/data/QLNHDB.db`
- Giao diện web thân thiện để xem và chỉnh sửa data

#### Adminer
- Truy cập: http://localhost:8080
- System: SQLite
- Server: để trống
- Database: đường dẫn đến file database

### 4. Development Commands

```bash
# Stop tất cả containers
docker-compose down

# Rebuild và restart
docker-compose up --build

# Xem logs
docker-compose logs qlnh-webapp

# Chạy migrations (nếu cần)
docker-compose exec qlnh-webapp dotnet ef database update
```

### 5. Database Management

Database SQLite được lưu trong thư mục `./data/QLNHDB.db` và được mount vào container.

**EF Core Tools** đã được setup để quản lý database schema:
- Migrations tự động chạy khi container start
- Data persistence thông qua Docker volumes
- Backup database bằng cách copy file từ thư mục `./data/`

### 6. File Structure
```
├── Dockerfile                 # Container definition cho ASP.NET app
├── docker-compose.yml         # Multi-container setup
├── .dockerignore             # Files to ignore trong build context
├── data/                     # Database files (mounted volume)
│   └── QLNHDB.db            # SQLite database
└── scripts/                  # Helper scripts
    ├── run-migrations.sh     # Linux migration script
    └── run-migrations.bat    # Windows migration script
```

### 7. Troubleshooting

- Nếu database không có data, chạy: `./scripts/run-migrations.bat`
- Nếu port bị conflict, thay đổi port trong docker-compose.yml
- Check logs bằng: `docker-compose logs`