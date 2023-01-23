#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#4080 HTTP - 4443 HTTPS

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 4080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebService/WebService.csproj", "WebService/"]
COPY ["APILayer/WebService.APILayer.csproj", "APILayer/"]
COPY ["Models/WebService.Models.csproj", "Models/"]
COPY ["ServiceLayer/WebService.OperationLayer.csproj", "ServiceLayer/"]
COPY ["DataLayer/WebService.DataAccessLayer.csproj", "DataLayer/"]
COPY ["Helpers/WebService.Helpers.csproj", "Helpers/"]
COPY ["Configuration/WebService.Configuration.csproj", "Configuration/"]
COPY ["Middleware/WebService.MiddlewareLayer.csproj", "Middleware/"]
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