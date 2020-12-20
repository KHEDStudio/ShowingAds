#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["ShowingAds.FilesService/ShowingAds.FilesService.csproj", "ShowingAds.FilesService/"]
RUN dotnet restore "ShowingAds.FilesService/ShowingAds.FilesService.csproj"
COPY . .
WORKDIR "/src/ShowingAds.FilesService"
RUN dotnet build "ShowingAds.FilesService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShowingAds.FilesService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShowingAds.FilesService.dll"]