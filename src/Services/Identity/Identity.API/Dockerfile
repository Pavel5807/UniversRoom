FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY "Services/Identity/Identity.API/Identity.API.csproj" "Services/Identity/Identity.API/Identity.API.csproj"

RUN dotnet restore "Services/Identity/Identity.API/Identity.API.csproj"

COPY . .
WORKDIR /src/Services/Identity/Identity.API
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Identity.API.dll"]
