#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ShowingAds.Backend.FileService/ShowingAds.Backend.FileService.csproj", "ShowingAds.Backend.FileService/"]
RUN dotnet restore "ShowingAds.Backend.FileService/ShowingAds.Backend.FileService.csproj"
COPY . .
WORKDIR "/src/ShowingAds.Backend.FileService"
RUN dotnet build "ShowingAds.Backend.FileService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShowingAds.Backend.FileService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShowingAds.Backend.FileService.dll"]