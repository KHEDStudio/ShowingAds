#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["ShowingAds.DevicesService/ShowingAds.DevicesService.csproj", "ShowingAds.DevicesService/"]
RUN dotnet restore "ShowingAds.DevicesService/ShowingAds.DevicesService.csproj"
COPY . .
WORKDIR "/src/ShowingAds.DevicesService"
RUN dotnet build "ShowingAds.DevicesService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShowingAds.DevicesService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShowingAds.DevicesService.dll"]