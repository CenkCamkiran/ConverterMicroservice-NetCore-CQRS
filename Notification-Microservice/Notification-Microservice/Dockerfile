#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Notification-Microservice/Notification-Microservice.csproj", "Notification-Microservice/"]
COPY ["Notification-Microservice.Commands/Notification-Microservice.Commands.csproj", "Notification-Microservice.Commands/"]
COPY ["Notification-Microservice.Handlers/Notification-Microservice.Handlers.csproj", "Notification-Microservice.Handlers/"]
COPY ["Notification-Microservice.Queries/Notification-Microservice.Queries.csproj", "Notification-Microservice.Queries/"]
COPY ["Notification-Microservice.Models/Notification-Microservice.Models.csproj", "Notification-Microservice.Models/"]
COPY ["Notification-Microservice.Repositories/Notification-Microservice.Repositories.csproj", "Notification-Microservice.Repositories/"]
COPY ["Notification-Microservice.Common/Notification-Microservice.Common.csproj", "Notification-Microservice.Common/"]
RUN dotnet restore "Notification-Microservice/Notification-Microservice.csproj"
COPY . .
WORKDIR "/src/Notification-Microservice"
RUN dotnet build "Notification-Microservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notification-Microservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification-Microservice.dll"]