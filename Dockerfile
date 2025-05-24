# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project file dan restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy semua source code dan build aplikasi
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy hasil build dari stage sebelumnya
COPY --from=build /app/out ./

EXPOSE 8080

# Jalankan aplikasi
ENTRYPOINT ["dotnet", "APITaklimSmart.dll"]
