#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["OpenWorld.Server/OpenWorld.Server.csproj", "OpenWorld.Server/"]
RUN dotnet restore "OpenWorld.Server/OpenWorld.Server.csproj"
COPY . .
WORKDIR "/src/OpenWorld.Server"
RUN dotnet build "OpenWorld.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenWorld.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenWorld.Server.dll"]