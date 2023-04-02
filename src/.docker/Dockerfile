FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["EKUTSOV.API/EKUTSOV.API.csproj", "EKUTSOV.API/"]
COPY ["EKUTSOV.Core/EKUTSOV.Core.csproj", "EKUTSOV.Core/"]
COPY ["EKUTSOV.Infrastructure/EKUTSOV.Infrastructure.csproj", "EKUTSOV.Infrastructure/"]
COPY ["EKUTSOV.Domain/EKUTSOV.Domain.csproj", "EKUTSOV.Domain/"]
RUN dotnet restore "EKUTSOV.API/EKUTSOV.API.csproj"
COPY . .
WORKDIR "/src/EKUTSOV.API"
RUN dotnet build "EKUTSOV.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EKUTSOV.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EKUTSOV.API.dll"]
