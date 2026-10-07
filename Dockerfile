FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["MYMCarRental.API/MYMCarRental.API.csproj", "MYMCarRental.API/"]
COPY ["MYMCarRental.Application/MYMCarRental.Application.csproj", "MYMCarRental.Application/"]
COPY ["MYMCarRental.Domain/MYMCarRental.Domain.csproj", "MYMCarRental.Domain/"]
COPY ["MYMCarRental.Infrastructure/MYMCarRental.Infrastructure.csproj", "MYMCarRental.Infrastructure/"]

RUN dotnet restore "MYMCarRental.API/MYMCarRental.API.csproj"

COPY . .

WORKDIR "/src/MYMCarRental.API"

RUN dotnet publish "MYMCarRental.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MYMCarRental.API.dll"]