#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#4080 HTTP - 4443 HTTPS

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 4080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebService/WebService.csproj", "WebService/"]
COPY ["WebService.Configuration/WebService.Configuration.csproj", "WebService.Configuration/"]
COPY ["WebService.Models/WebService.Models.csproj", "WebService.Models/"]
COPY ["WebService.DataAccessLayer/WebService.DataAccessLayer.csproj", "WebService.DataAccessLayer/"]
COPY ["WebService.Helpers/WebService.Helpers.csproj", "WebService.Helpers/"]
COPY ["WebService.MiddlewareLayer/WebService.MiddlewareLayer.csproj", "WebService.MiddlewareLayer/"]
COPY ["WebService.OperationLayer/WebService.OperationLayer.csproj", "WebService.OperationLayer/"]
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