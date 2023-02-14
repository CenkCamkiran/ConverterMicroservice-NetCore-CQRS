#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Logger-Microservice/Logger-Microservice.csproj", "Logger-Microservice/"]
COPY ["Logger-Microservice.Configuration/Logger-Microservice.Configuration.csproj", "Logger-Microservice.Configuration/"]
COPY ["Logger-Microservice.Models/Logger-Microservice.Models.csproj", "Logger-Microservice.Models/"]
COPY ["Logger-Microservice.DataAccessLayer/Logger-Microservice.DataAccessLayer.csproj", "Logger-Microservice.DataAccessLayer/"]
COPY ["Logger-Microservice.OperationLayer/Logger-Microservice.OperationLayer.csproj", "Logger-Microservice.OperationLayer/"]
RUN dotnet restore "Logger-Microservice/Logger-Microservice.csproj"
COPY . .
WORKDIR "/src/Logger-Microservice"
RUN dotnet build "Logger-Microservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Logger-Microservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Logger-Microservice.dll"]