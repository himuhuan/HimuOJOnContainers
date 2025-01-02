# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 此阶段用于在快速模式(默认为调试配置)下从 VS 运行时
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 81


# 此阶段用于生成服务项目
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Problems/Problems.API/Problems.API.csproj", "src/Services/Problems/Problems.API/"]
COPY ["src/Common/Common.WebApiComponents/Common.WebApiComponents.csproj", "src/Common/Common.WebApiComponents/"]
COPY ["src/Common/Common.WebHostDefaults/Common.WebHostDefaults.csproj", "src/Common/Common.WebHostDefaults/"]
COPY ["src/Services/Problems/Problems.Instructure/Problems.Infrastructure.csproj", "src/Services/Problems/Problems.Instructure/"]
COPY ["src/Services/Problems/Problems.Domain/Problems.Domain.csproj", "src/Services/Problems/Problems.Domain/"]
COPY ["src/Common/Common.DomainSeedWork/Common.DomainSeedWork.csproj", "src/Common/Common.DomainSeedWork/"]
RUN dotnet restore "./src/Services/Problems/Problems.API/Problems.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Problems/Problems.API"
RUN dotnet build "./Problems.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Problems.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 此阶段在生产中使用，或在常规模式下从 VS 运行时使用(在不使用调试配置时为默认值)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Problems.API.dll"]