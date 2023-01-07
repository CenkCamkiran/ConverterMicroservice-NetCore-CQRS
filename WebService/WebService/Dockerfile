#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebService/WebService.csproj", "WebService/"]
COPY ["APILayer/APILayer.csproj", "APILayer/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["ServiceLayer/ServiceLayer.csproj", "ServiceLayer/"]
COPY ["DataLayer/DataLayer.csproj", "DataLayer/"]
COPY ["Helpers/Helpers.csproj", "Helpers/"]
COPY ["Middleware/Middleware.csproj", "Middleware/"]
RUN dotnet restore "WebService/WebService.csproj"
COPY . .
WORKDIR "/src/WebService"
RUN dotnet build "WebService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebService.dll"]