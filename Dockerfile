# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj file and restore dependencies
# This step is optimized for Docker caching: if only code changes, this layer isn't rebuilt
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . .

# Publish the application for production
# -c Release: Build in Release configuration
# -o /app/publish: Output to /app/publish directory
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET ASP.NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Set the working directory
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose port 8080 (Render's default for web services)
EXPOSE 8080

# Set the ASP.NET Core environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Define the entry point for the application
# Replace 'Whisper.dll' with the actual name of your compiled application DLL
ENTRYPOINT ["dotnet", "Whisper.dll"]
