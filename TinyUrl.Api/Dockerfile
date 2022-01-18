#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TinyUrl.Api/TinyUrl.Api.csproj", "TinyUrl.Api/"]
RUN dotnet restore "TinyUrl.Api/TinyUrl.Api.csproj"
COPY . .
WORKDIR "/src/TinyUrl.Api"
RUN dotnet build "TinyUrl.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TinyUrl.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "TinyUrl.Api.dll"]