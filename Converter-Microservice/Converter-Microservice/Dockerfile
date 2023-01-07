#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Converter-Microservice/Converter-Microservice.csproj", "Converter-Microservice/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
COPY ["Configuration/Configuration.csproj", "Configuration/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["Operation/Operation.csproj", "Operation/"]
RUN dotnet restore "Converter-Microservice/Converter-Microservice.csproj"
COPY . .
WORKDIR "/src/Converter-Microservice"
RUN dotnet build "Converter-Microservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Converter-Microservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Converter-Microservice.dll"]