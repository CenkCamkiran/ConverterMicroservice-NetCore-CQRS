#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

RUN apt-get update && apt-get install ffmpeg -y

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Converter-Microservice/Converter-Microservice.csproj", "Converter-Microservice/"]
COPY ["Converter-Microservice.Handlers/Converter-Microservice.Handlers.csproj", "Converter-Microservice.Handlers/"]
COPY ["Converter-Microservice.Commands/Converter-Microservice.Commands.csproj", "Converter-Microservice.Commands/"]
COPY ["Converter-Microservice.Models/Converter-Microservice.Models.csproj", "Converter-Microservice.Models/"]
COPY ["Converter-Microservice.Queries/Converter-Microservice.Queries.csproj", "Converter-Microservice.Queries/"]
COPY ["Converter-Microservice.Repositories/Converter-Microservice.Repositories.csproj", "Converter-Microservice.Repositories/"]
COPY ["Converter-Microservice.Exceptions/Converter-Microservice.Exceptions.csproj", "Converter-Microservice.Exceptions/"]
COPY ["Converter-Microservice.ProjectConfigurations/Converter-Microservice.ProjectConfigurations.csproj", "Converter-Microservice.ProjectConfigurations/"]
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