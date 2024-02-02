FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NFeXMLValidator/NFeXMLValidator.csproj", "NFeXMLValidator/"]
RUN dotnet restore "NFeXMLValidator/NFeXMLValidator.csproj"
COPY . .
WORKDIR "/src/NFeXMLValidator"
RUN dotnet build "NFeXMLValidator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NFeXMLValidator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NFeXMLValidator.dll"]