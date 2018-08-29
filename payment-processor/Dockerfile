FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["payment-processor/payment-processor.csproj", "payment-processor/"]
RUN dotnet restore "payment-processor/payment-processor.csproj"
COPY . .
WORKDIR "/src/payment-processor"
RUN dotnet build "payment-processor.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "payment-processor.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "payment-processor.dll"]