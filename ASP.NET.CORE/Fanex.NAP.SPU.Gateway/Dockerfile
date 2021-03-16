FROM registry.nexdev.net:5050/docker/dotnet/core/sdk:5.0.102-ca-patch-buster-slim AS builder

WORKDIR /ASP.NET.CORE

COPY ["Fanex.NAP.SPU.Middleware", "Fanex.NAP.SPU.Middleware"] 
COPY ["Fanex.NAP.SPU.Gateway/Fanex.NAP.SPU.Gateway.csproj", "Fanex.NAP.SPU.Gateway/"]

WORKDIR "/ASP.NET.CORE/Fanex.NAP.SPU.Gateway"
RUN dotnet restore -v n

COPY ./Fanex.NAP.SPU.Gateway .

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM registry.nexdev.net:5050/docker/dotnet/core/aspnet:5.0-buster-slim AS base

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "Fanex.NAP.SPU.Gateway.dll"]
