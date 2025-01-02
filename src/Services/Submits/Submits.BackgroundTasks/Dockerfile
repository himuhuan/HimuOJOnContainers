# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y g++
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Submits/Submits.BackgroundTasks/Submits.BackgroundTasks.csproj", "src/Services/Submits/Submits.BackgroundTasks/"]
COPY ["src/Common/Common.WebHostDefaults/Common.WebHostDefaults.csproj", "src/Common/Common.WebHostDefaults/"]
COPY ["src/Services/Submits/Submits.Infrastructure/Submits.Infrastructure.csproj", "src/Services/Submits/Submits.Infrastructure/"]
COPY ["src/Services/Submits/Submits.Domain/Submits.Domain.csproj", "src/Services/Submits/Submits.Domain/"]
COPY ["src/Common/Common.DomainSeedWork/Common.DomainSeedWork.csproj", "src/Common/Common.DomainSeedWork/"]
RUN dotnet restore "./src/Services/Submits/Submits.BackgroundTasks/Submits.BackgroundTasks.csproj"
COPY . .
WORKDIR "/src/src/Services/Submits/Submits.BackgroundTasks"
RUN dotnet build "./Submits.BackgroundTasks.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Submits.BackgroundTasks.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Submits.BackgroundTasks.dll"]