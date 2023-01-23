#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Notification-Microservice/NotificationMicroservice.csproj", "Notification-Microservice/"]
COPY ["Configuration/NotificationMicroservice.Configuration.csproj", "Configuration/"]
COPY ["Models/NotificationMicroservice.Models.csproj", "Models/"]
COPY ["Operation/NotificationMicroservice.OperationLayer.csproj", "Operation/"]
COPY ["DataAccess/NotificationMicroservice.DataAccessLayer.csproj", "DataAccess/"]
RUN dotnet restore "Notification-Microservice/NotificationMicroservice.csproj"
COPY . .
WORKDIR "/src/Notification-Microservice"
RUN dotnet build "NotificationMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NotificationMicroservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationMicroservice.dll"]