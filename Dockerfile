FROM mcr.microsoft.com/dotnet/sdk:5.0 as builder

WORKDIR /src

COPY . .

RUN dotnet publish -o output

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as runtime
WORKDIR /app

COPY --from=builder /src/output/ /app

CMD ["dotnet", "/app/csgo-network-log-parser.dll"]