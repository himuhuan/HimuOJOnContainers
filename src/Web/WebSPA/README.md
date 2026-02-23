# HimuOJ WebSPA

`WebSPA` 是 HimuOJ 的 SPA 入口项目：负责托管 Vue 前端、提供 Duende BFF（Cookie + OIDC）能力，并作为 YARP 反向代理网关把请求转发到下游服务。

## 这个项目做什么

- 从 `wwwroot/` 提供 SPA 静态文件，并将所有非 API 路由回退到 `index.html`（前端路由）。
- 通过 `Duende.BFF` 提供 BFF 端点（登录/登出/用户会话）。
- 通过 YARP 反向代理把 `/api/**` 转发到内部服务（Problems、Submits、Identity）。
- 提供少量聚合接口（`Controller/BffController.cs`），减少前端多次请求。

## 技术栈

- 后端：.NET 8（`Microsoft.NET.Sdk.Web`）
- BFF/认证：`Duende.BFF` + `OpenIdConnect` + Cookie 会话
- 网关：YARP（`Duende.BFF.Yarp`）
- BFF 内部调用下游 API：Refit（`Refit.HttpClientFactory`）
- 前端：Vue 3 + Vite + TypeScript + Pinia + Naive UI
- 实时：SignalR 客户端（`@microsoft/signalr`）用于提交状态推送

## 目录结构（本目录）

- `Program.cs`：启动入口。
- `HostingExtensions.cs`：DI 注册 + 中间件流水线（BFF、认证、反向代理、SPA fallback）。
- `Controller/BffController.cs`：题目/提交/用户相关的聚合接口。
- `Services/*.cs`：下游 API 的 Refit 接口。
- `Models/BffModels.cs`：BFF 聚合返回用的模型。
- `Filters/BffGatewayRefitExceptionFilter.cs`：将 Refit `ApiException` 转成更友好的 JSON 错误响应。
- `Client/`：Vue SPA（Vite 开发服务器、组件、路由、store、前端 API 封装）。

## 对外 HTTP 面（概览）

- SPA：
  - `GET /` 以及其他非 API 路由 -> `wwwroot/index.html`
- BFF 管理端点（Duende）：
  - `GET /bff/login`（触发 OIDC 登录）
  - `GET /bff/user`（当前用户 claims）
  - 登出 URL 通过 claim `bff:logout_url` 下发（见 `Client/src/stores/user.ts`）
- 聚合 API（由本项目实现）：
  - `GET /api/problems/list` -> `GET /api/bff/problems-list`
  - `GET /api/submissions` -> `GET /api/bff/submissions-list`
  - `GET /api/submissions/{id}/detail` -> `GET /api/bff/submissions-detail/{id}`
  - `GET /api/users/{id}/detail` -> `GET /api/bff/users-detail/{id}`
- 反向代理 API（YARP）：
  - `/api/problems/**` -> Problems 服务
  - `/api/submissions/**` 以及 `/submitshub` -> Submits 服务（含 SignalR）
  - `/api/users/**` 以及 `/static/users/**` -> Identity 服务

说明：多数反代路由都配置了 BFF metadata（`OptionalUserToken` + `AntiforgeryCheck`）。前端请求会固定带 `X-CSRF: 1`（见 `Client/src/modules/HttpClient.ts`）。

## 配置

- `HostingExtensions.cs` 依赖 `IdentityServer` 配置段：
  - 代码读取 `IdentityServer:ExternalUrl` 与 `IdentityServer:Scopes`。
- `ReverseProxy` 配置段定义 YARP 的 routes/clusters。
  - 默认 `appsettings.json` 使用 docker-compose 的服务名作为地址，例如 `http://problems-api` / `http://submits-api` / `http://identity` / `http://webspa-bff`。

## 本地开发

依赖：

- .NET 8 SDK
- Node.js + Yarn（前端）

前端（Vite）：

```bash
cd Client
yarn install
yarn dev
```

Vite 默认运行在 `http://localhost:6100`，并将 `/api`、`/bff`、`/submitshub`、`/static` 代理到 `Client/vite.config.ts` 里配置的后端地址。

后端（BFF + 反向代理）：

```bash
dotnet run --project WebSPA.csproj
```

如果你不是在 docker-compose 环境启动，通常需要：

- 提供 `IdentityServer:ExternalUrl`（代码按这个 key 读取）。
- 覆盖 `ReverseProxy:Clusters:*:Destinations:*:Address` 指向你本机的各服务 URL。

## 生产构建说明

后端从 `wwwroot/` 提供 SPA 文件。一般的发布流程是：

1. 构建前端：`cd Client && yarn build`
2. 将 `Client/dist/` 拷贝到 `wwwroot/`
3. `dotnet publish`

当前目录下未看到自动执行「前端构建/拷贝到 wwwroot」的 MSBuild 步骤。

## 常用测试请求

参考 `WebSPA.http`：

- `GET /api/problems/list?page=1&pageSize=10`
- `GET /api/submissions/{id}/detail`
- `GET /api/users/{id}/detail`
