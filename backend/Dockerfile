# Stage 1: Build the Application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything, restore, and build
COPY . ./
WORKDIR /src/backend
RUN dotnet restore
RUN dotnet publish -c Debug -o /app/publish

# ---

# Stage 2: Production Runtime (for deployment)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "backend.dll"]

# ---

# Stage 3: Debug Runtime (for remote debugging)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS debug
WORKDIR /app

# 1. Install necessary dependencies for debugging (curl and unzip)
# This is often needed to fetch and unpack the debugger on Debian-based images.
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        unzip \
        curl \
    && rm -rf /var/lib/apt/lists/*

# 2. Download and Install VSDBG (Visual Studio Debugger)
# Use the official Microsoft script to install the latest compatible VSDBG.
# The debugger is installed to /vsdbg inside the container.
ENV VSDBG_ROOT /vsdbg
# The following curl command downloads and executes the installer script,
# fetching the latest VSDBG version compatible with the .NET 8 runtime.
RUN mkdir -p "$VSDBG_ROOT" \
    && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l "$VSDBG_ROOT"

# 3. Copy the published application binaries from the build stage
COPY --from=build /app/publish ./

# 4. Expose the Application Port AND a potential Debugger Port
EXPOSE 5000
EXPOSE 5001
 # Optional: A separate port if you want to attach directly via IP:Port

# 5. Define the application's entrypoint
# The standard entrypoint is usually fine, as VS Code/VS will use 'pipeTransport'
# (like 'docker exec') to run the debugger agent inside the container.
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "backend.dll"]