#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 5001
EXPOSE 443
COPY ["MicroserviceDemo-Chap3/videos", "./videos"]

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["MicroserviceDemo-Chap3/MicroserviceDemo-Chap3.csproj", "MicroserviceDemo-Chap3/"]
RUN dotnet restore "MicroserviceDemo-Chap3/MicroserviceDemo-Chap3.csproj"
COPY . .
WORKDIR "/src/MicroserviceDemo-Chap3"
RUN dotnet build "MicroserviceDemo-Chap3.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroserviceDemo-Chap3.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroserviceDemo-Chap3.dll"]