#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Notification-Microservice/Notification-Microservice.csproj", "Notification-Microservice/"]
COPY ["Configuration/Configuration.csproj", "Configuration/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["Operation/Operation.csproj", "Operation/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
COPY ["Helpers/Helper.csproj", "Helpers/"]
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