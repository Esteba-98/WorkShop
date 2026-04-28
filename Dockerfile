FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Workshop.Api/Workshop.Api.csproj", "src/Workshop.Api/"]
COPY ["src/Workshop.Application/Workshop.Application.csproj", "src/Workshop.Application/"]
COPY ["src/Workshop.Domain/Workshop.Domain.csproj", "src/Workshop.Domain/"]
COPY ["src/Workshop.Infrastructure/Workshop.Infrastructure.csproj", "src/Workshop.Infrastructure/"]
RUN dotnet restore "src/Workshop.Api/Workshop.Api.csproj"
COPY . .
WORKDIR "/src/src/Workshop.Api"
RUN dotnet build "Workshop.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Workshop.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Workshop.Api.dll"]
