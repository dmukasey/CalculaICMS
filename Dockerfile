# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app

# instalar ICU para contemplar globalizacao PT-BR
RUN apk add --no-cache icu-libs 

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080


# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copia os csproj primeiro (melhor cache)
COPY 01-API/NFE/NFE.csproj 01-API/NFE/
COPY 02-ApplicationServices/NfeService/NfeService.csproj 2-ApplicationServices/NfeService/
COPY 03-Domain/Contracts/ICMS.Contracts.csproj 03-Domain/Contracts/
COPY 03-Domain/Domain/ICMS.Domain.csproj 03-Domain/Domain/
COPY 04-Data/MongoDB/ICMS.MongoDB/ICMS.MongoDBHelper.csproj 04-Data/MongoDB/ICMS.MongoDB/
COPY 04-Data/RabbitMQ/ICMS.RabbitMQ.csproj 04-Data/RabbitMQ/
COPY 04-Data/SQLSERVER/ICMS.SQLSERVER.csproj 04-Data/SQLSERVER/

# restore
RUN dotnet restore 01-API/NFE/NFE.csproj

# copia restante do codigo
COPY . .

WORKDIR /src/01-API/NFE/

RUN dotnet publish NFE.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ---------- FINAL ----------
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "NFE.dll"]