#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Logger-Microservice/Logger-Microservice.csproj", "Logger-Microservice/"]
COPY ["Logger-Microservice.Common/Logger-Microservice.Common.csproj", "Logger-Microservice.Common/"]
COPY ["Logger-Microservice.Handlers/Logger-Microservice.Handlers.csproj", "Logger-Microservice.Handlers/"]
COPY ["Logger-Microservice.Commands/Logger-Microservice.Commands.csproj", "Logger-Microservice.Commands/"]
COPY ["Logger-Microservice.Queries/Logger-Microservice.Queries.csproj", "Logger-Microservice.Queries/"]
COPY ["Logger-Microservice.Repositories/Logger-Microservice.Repositories.csproj", "Logger-Microservice.Repositories/"]
COPY ["Logger-Microservice.Models/Logger-Microservice.Models.csproj", "Logger-Microservice.Models/"]
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