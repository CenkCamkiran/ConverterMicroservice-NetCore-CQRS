#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LoggerMicroservice/LoggerMicroservice.csproj", "LoggerMicroservice/"]
COPY ["DataAccess/LoggerMicroservice.DataAccessLayer.csproj", "DataAccess/"]
COPY ["Configuration/LoggerMicroservice.Configuration.csproj", "Configuration/"]
COPY ["Models/LoggerMicroservice.Models.csproj", "Models/"]
COPY ["Operation/LoggerMicroservice.OperationLayer.csproj", "Operation/"]
RUN dotnet restore "LoggerMicroservice/LoggerMicroservice.csproj"
COPY . .
WORKDIR "/src/LoggerMicroservice"
RUN dotnet build "LoggerMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoggerMicroservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoggerMicroservice.dll"]