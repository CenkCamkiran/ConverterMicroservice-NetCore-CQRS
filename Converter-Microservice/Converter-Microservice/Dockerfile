#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

RUN apt-get update && apt-get install ffmpeg -y

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Converter-Microservice/Converter-Microservice.csproj", "Converter-Microservice/"]
COPY ["Converter-Microservice.Configuration/Converter-Microservice.Configuration.csproj", "Converter-Microservice.Configuration/"]
COPY ["Converter-Microservice.Models/Converter-Microservice.Models.csproj", "Converter-Microservice.Models/"]
COPY ["Converter-Microservice.DataAccessLayer/Converter-Microservice.DataAccessLayer.csproj", "Converter-Microservice.DataAccessLayer/"]
COPY ["Converter-Microservice.OperationLayer/Converter-Microservice.OperationLayer.csproj", "Converter-Microservice.OperationLayer/"]
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