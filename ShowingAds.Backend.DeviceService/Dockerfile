#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ShowingAds.Backend.DeviceService/ShowingAds.Backend.DeviceService.csproj", "ShowingAds.Backend.DeviceService/"]
RUN dotnet restore "ShowingAds.Backend.DeviceService/ShowingAds.Backend.DeviceService.csproj"
COPY . .
WORKDIR "/src/ShowingAds.Backend.DeviceService"
RUN dotnet build "ShowingAds.Backend.DeviceService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShowingAds.Backend.DeviceService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShowingAds.Backend.DeviceService.dll"]