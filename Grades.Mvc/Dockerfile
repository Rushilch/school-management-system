# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory for the specific project
WORKDIR /src/Grades.Mvc

# Copy project file(s)
COPY ./Grades.Mvc/*.csproj ./
# Copy shared project so restore can find project references
COPY ./School.Shared/School.Shared.csproj ../School.Shared/

# Restore dependencies for the project
RUN dotnet restore Grades.Mvc.csproj

# Copy the rest of the project files
COPY ./Grades.Mvc/. ./
COPY ./School.Shared/. ../School.Shared/

# Publish
RUN dotnet publish Grades.Mvc.csproj -c Release -o /app/publish

# Stage 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Grades.Mvc.dll"]