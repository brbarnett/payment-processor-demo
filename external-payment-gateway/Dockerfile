FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["external-payment-gateway/external-payment-gateway.csproj", "external-payment-gateway/"]
RUN dotnet restore "external-payment-gateway/external-payment-gateway.csproj"
COPY . .
WORKDIR "/src/external-payment-gateway"
RUN dotnet build "external-payment-gateway.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "external-payment-gateway.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "external-payment-gateway.dll"]