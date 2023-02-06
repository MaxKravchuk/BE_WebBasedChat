#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BE_Chat/BE_Chat.csproj", "BE_Chat/"]
RUN dotnet restore "BE_Chat/BE_Chat.csproj"
COPY . .
WORKDIR "/src/BE_Chat"
RUN dotnet build "BE_Chat.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BE_Chat.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BE_Chat.dll"]