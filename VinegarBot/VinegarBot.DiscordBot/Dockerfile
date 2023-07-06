#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["VinegarBot.DiscordBot/VinegarBot.DiscordBot.csproj", "VinegarBot.DiscordBot/"]
RUN dotnet restore "VinegarBot.DiscordBot/VinegarBot.DiscordBot.csproj"
COPY . .
WORKDIR "/src/VinegarBot.DiscordBot"
RUN dotnet build "VinegarBot.DiscordBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VinegarBot.DiscordBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VinegarBot.DiscordBot.dll"]