#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

RUN apt-get update && apt-get install ffmpeg -y

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Converter-Microservice/ConverterMicroservice.csproj", "Converter-Microservice/"]
COPY ["DataAccess/ConverterMicroservice.DataAccessLayer.csproj", "DataAccess/"]
COPY ["Configuration/ConverterMicroservice.Configuration.csproj", "Configuration/"]
COPY ["Models/ConverterMicroservice.Models.csproj", "Models/"]
COPY ["Operation/ConverterMicroservice.OperationLayer.csproj", "Operation/"]
RUN dotnet restore "Converter-Microservice/ConverterMicroservice.csproj"
COPY . .
WORKDIR "/src/Converter-Microservice"
RUN dotnet build "ConverterMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConverterMicroservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConverterMicroservice.dll"]