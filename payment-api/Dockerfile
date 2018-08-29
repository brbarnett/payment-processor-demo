FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["payment-api/payment-api.csproj", "payment-api/"]
RUN dotnet restore "payment-api/payment-api.csproj"
COPY . .
WORKDIR "/src/payment-api"
RUN dotnet build "payment-api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "payment-api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "payment-api.dll"]