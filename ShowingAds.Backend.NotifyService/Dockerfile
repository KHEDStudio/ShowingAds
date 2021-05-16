#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ShowingAds.Backend.NotifyService/ShowingAds.Backend.NotifyService.csproj", "ShowingAds.Backend.NotifyService/"]
RUN dotnet restore "ShowingAds.Backend.NotifyService/ShowingAds.Backend.NotifyService.csproj"
COPY . .
WORKDIR "/src/ShowingAds.Backend.NotifyService"
RUN dotnet build "ShowingAds.Backend.NotifyService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShowingAds.Backend.NotifyService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShowingAds.Backend.NotifyService.dll"]