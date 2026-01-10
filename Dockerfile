# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY EbikeRental.sln ./
COPY EbikeRental.Web/EbikeRental.Web.csproj EbikeRental.Web/
COPY EbikeRental.Application/EbikeRental.Application.csproj EbikeRental.Application/
COPY EbikeRental.Infrastructure/EbikeRental.Infrastructure.csproj EbikeRental.Infrastructure/
COPY EbikeRental.Domain/EbikeRental.Domain.csproj EbikeRental.Domain/
COPY EbikeRental.Shared/EbikeRental.Shared.csproj EbikeRental.Shared/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build and publish the Web project
WORKDIR /src/EbikeRental.Web
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Expose port (Railway uses PORT environment variable)
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "EbikeRental.Web.dll"]
