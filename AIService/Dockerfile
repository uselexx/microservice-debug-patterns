# Stage 1: Build the Application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything from the root context (defined in docker-compose)
COPY . ./

# Move into the AI Service directory to restore and publish
WORKDIR /src/AIService
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# ---

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /app/publish ./

# Expose the internal port (8080 is the .NET 8 default)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "AIService.dll"]