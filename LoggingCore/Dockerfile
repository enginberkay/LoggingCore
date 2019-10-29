FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["LoggingCore/LoggingCore.csproj", "LoggingCore/"]
RUN dotnet restore "LoggingCore/LoggingCore.csproj"
COPY . .
WORKDIR "/src/LoggingCore"
RUN dotnet build "LoggingCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoggingCore.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoggingCore.dll"]