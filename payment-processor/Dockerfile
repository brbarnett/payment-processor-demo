FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["article-page/article-page.csproj", "article-page/"]
RUN dotnet restore "article-page/article-page.csproj"
COPY . .
WORKDIR "/src/article-page"
RUN dotnet build "article-page.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "article-page.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "article-page.dll"]