FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ODataApi/ODataApi.csproj", "ODataApi/"]
RUN dotnet restore "ODataApi/ODataApi.csproj"
COPY . .
WORKDIR "/src/ODataApi"
RUN dotnet build "ODataApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ODataApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ODataApi.dll"]
