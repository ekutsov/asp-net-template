FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["A2SEVEN.API/A2SEVEN.API.csproj", "A2SEVEN.API/"]
COPY ["A2SEVEN.Core/A2SEVEN.Core.csproj", "A2SEVEN.Core/"]
COPY ["A2SEVEN.Infrastructure/A2SEVEN.Infrastructure.csproj", "A2SEVEN.Infrastructure/"]
COPY ["A2SEVEN.Domain/A2SEVEN.Domain.csproj", "A2SEVEN.Domain/"]
RUN dotnet restore "A2SEVEN.API/A2SEVEN.API.csproj"
COPY . .
WORKDIR "/src/A2SEVEN.API"
RUN dotnet build "A2SEVEN.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "A2SEVEN.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "A2SEVEN.API.dll"]
