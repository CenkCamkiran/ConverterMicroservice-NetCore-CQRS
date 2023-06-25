#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Initilization-Microservice/Initilization-Microservice.csproj", "Initilization-Microservice/"]
COPY ["Initilization-Microservice.Common/Initilization-Microservice.Common.csproj", "Initilization-Microservice.Common/"]
COPY ["Initilization-Microservice.Operation/Initilization-Microservice.Operation.csproj", "Initilization-Microservice.Operation/"]
COPY ["Initilization-Microservice.Repository/Initilization-Microservice.Repository.csproj", "Initilization-Microservice.Repository/"]
COPY ["Initilization-Microservice.Helpers/Initilization-Microservice.Helpers.csproj", "Initilization-Microservice.Helpers/"]
COPY ["Initilization-Microservice.Models/Initilization-Microservice.Models.csproj", "Initilization-Microservice.Models/"]
RUN dotnet restore "Initilization-Microservice/Initilization-Microservice.csproj"
COPY . .
WORKDIR "/src/Initilization-Microservice"
RUN dotnet build "Initilization-Microservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Initilization-Microservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Initilization-Microservice.dll"]