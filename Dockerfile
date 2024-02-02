FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT="Production"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NFeXMLValidator.csproj", "."]
RUN dotnet restore "NFeXMLValidator.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "NFeXMLValidator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NFeXMLValidator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["../Schemas", "Schemas"]
ENTRYPOINT ["dotnet", "NFeXMLValidator.dll"]