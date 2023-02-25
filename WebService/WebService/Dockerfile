#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#4080 HTTP - 4443 HTTPS

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 4080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebService/WebService.csproj", "WebService/"]
COPY ["WebService.Controllers/WebService.Controllers.csproj", "WebService.Controllers/"]
COPY ["WebService.Commands/WebService.Commands.csproj", "WebService.Commands/"]
COPY ["WebService.Models/WebService.Models.csproj", "WebService.Models/"]
COPY ["WebService.Queries/WebService.Queries.csproj", "WebService.Queries/"]
COPY ["WebService.Repositories/WebService.Repositories.csproj", "WebService.Repositories/"]
COPY ["WebService.Exceptions/WebService.Exceptions.csproj", "WebService.Exceptions/"]
COPY ["WebService.Handlers/WebService.Handlers.csproj", "WebService.Handlers/"]
COPY ["WebService.Helpers/WebService.Helpers.csproj", "WebService.Helpers/"]
COPY ["WebService.ProjectConfigurations/WebService.ProjectConfigurations.csproj", "WebService.ProjectConfigurations/"]
COPY ["WebService.Middlewares/WebService.Middlewares.csproj", "WebService.Middlewares/"]
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