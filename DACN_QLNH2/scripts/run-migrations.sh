#!/bin/bash

# Script để chạy EF Core migrations trong Docker container
echo "Running EF Core migrations..."

# Ensure database directory exists
mkdir -p /app/data

# Run migrations
dotnet ef database update --connection "Data Source=/app/data/QLNHDB.db"

echo "Migrations completed!"