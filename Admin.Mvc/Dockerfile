# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory for the specific project
WORKDIR /src/Admin.Mvc

# Copy project file(s)
COPY ./Admin.Mvc/*.csproj ./
# Copy shared project so restore can find project references
COPY ./School.Shared/School.Shared.csproj ../School.Shared/

# Restore dependencies for the project
RUN dotnet restore Admin.Mvc.csproj

# Copy the rest of the project files
COPY ./Admin.Mvc/. ./
COPY ./School.Shared/. ../School.Shared/

# Publish
RUN dotnet publish Admin.Mvc.csproj -c Release -o /app/publish

# Stage 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Admin.Mvc.dll"]