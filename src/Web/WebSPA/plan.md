# WebSPA 前端重构计划（仅前端视角）

本文只覆盖 `Client/`（Vue + Vite + TS）侧的适配与开发体验改进；后端从 IdentityServer 迁移到 OpenIddict 的实现细节不在本文范围内。

## 目标

1. IdentityServer 弃用后（后端改为 OpenIddict），前端在“尽量不改业务代码”的前提下继续完成登录态获取、登录/登出跳转、权限判断等。
2. 前端开发不再强依赖后端在线：引入 `MockApi`，支持纯前端启动、可控数据、可复现状态，并尽量覆盖当前页面用到的接口。

## 现状梳理（前端）

- SPA：Vue3 + Vite，路由在 `Client/src/routers/index.ts`。
- API：`Client/src/modules/HttpClient.ts`（axios + `X-CSRF: 1` 头） + `Client/src/services/*Api.ts`。
- 登录态：`Client/src/stores/user.ts` 通过 `GET /bff/user` 读取 claims 并推导 `isLogin/roles/userLogoutUrl`。
- 登录入口：导航栏 `Client/src/components/shared/HimuNavBar.vue` 直接跳转 `/bff/login`。
- 权限：路由 meta `requireAuthentication` + `userState.isLogin`；部分功能依赖 `role` claim（Distributor/Administrator）。
- 实时：提交列表 `Client/src/components/submissions/SubmissionList.vue` 通过 `/submitshub` 建立 SignalR 连接监听状态。

## 计划 1：弃用 IdentityServer，后端改 OpenIddict 后的前端适配

### 设计原则

- 前端尽量继续只依赖 BFF 对外的稳定面：`/bff/login`、`/bff/user`、`bff:logout_url`。
- 如果后端迁移后 BFF 端点与 claim 形状保持不变：前端无需大改。
- 如果 claim 类型/字段发生变化：通过“兼容映射层”消化差异，避免全站散落判断。

### 需要关注的潜在变化点

1. 登录/登出入口
   - 仍推荐维持 `/bff/login` 和 `bff:logout_url`；否则需要在 `HimuNavBar.vue` 与 `user.ts` 调整跳转逻辑。
2. claims 类型差异（最容易踩坑）
   - 现有实现使用：
     - `sub`（用户 id）
     - `unique_name`（用户名）
     - `email`、`avatar`
     - `role`（角色数组）
     - `bff:logout_url`
   - OpenIddict/不同 OIDC provider 可能使用：`name`、`preferred_username`、`email`、`picture`、`roles` 等。

### 改动清单（前端）

- `Client/src/stores/user.ts`
  - 新增 claims 兼容映射：为 `id/userName/email/avatar/roles/logoutUrl` 提供多 key fallback（例如 `unique_name` -> `preferred_username` -> `name`）。
  - 将“从 claims 推导业务字段”的逻辑集中封装，避免组件侧直接找 claim。
- `Client/src/routers/index.ts`
  - 保持不变；若未来引入“需要登录但可自动跳转登录”的体验，可把 `permission-denied` 改成跳转 `/bff/login?returnUrl=...`（取决于后端 BFF 支持）。

### 验收标准

- 后端完成 OpenIddict 迁移后：
  - `GET /bff/user` 未登录返回 401 时，前端仍能显示“未登录”状态。
  - 登录后导航栏能正确显示用户名/头像；登出按钮可用。
  - 角色判断（Distributor/Administrator）在新 claims 下仍能生效（通过映射层）。

## 计划 2：MockApi（前端可脱离后端开发）

### 方案选择

采用 MSW（Mock Service Worker）：

- 优点：
  - 在浏览器层拦截请求，对 axios 透明；不需要改 `HttpClient.ts`。
  - 可按“接口路径”精确 mock（`/api/...`、`/bff/user`）。
  - 可逐步覆盖：先覆盖核心页面接口，再补全编辑/上传等。
- 约束：
  - SignalR WebSocket 难以完全 1:1 mock；需要做降级或模拟推送。

### 接入方式（建议）

1. 新增开关：`VITE_USE_MOCK=true` 或 `vite --mode mock`。
2. 在 `Client/src/main.ts` 中，在 `createApp` 前按开关启动 MSW worker。
3. 新增目录：
   - `Client/src/mocks/browser.ts`（worker 启动）
   - `Client/src/mocks/handlers/*.ts`（按领域拆分 handlers：auth/problems/submissions/users）
   - `Client/src/mocks/data/*.ts`（假数据与生成器，保证可重复）

### 需要覆盖的接口（按当前页面依赖优先级）

P0（能跑通首页/题目/提交/用户页）：

- 认证：
  - `GET /bff/user`（返回 claims；支持“已登录/未登录”两种模式）
- 题目：
  - `GET /api/problems/list?page=&pageSize=`
  - `GET /api/problems/{id}/detail`
- 提交：
  - `GET /api/submissions?page=&pageSize=&problemId=&submitterId=`
  - `GET /api/submissions/{id}/detail`
- 用户：
  - `GET /api/users/{id}/detail`
  - `POST /api/users`（注册）

P1（题目编辑/资源相关，覆盖更多开发场景）：

- `GET /api/problems/{id}`（编辑页加载 full problem）
- `POST /api/problems`、`PUT /api/problems/{id}`
- `DELETE /api/problems/{id}`、`DELETE /api/problems/{id}/testpoints`
- `POST /api/problems/{id}/resources/{resourceType}`（上传）
- `GET /api/problems/{id}/resources/{resourceName}`（下载：可返回 blob 占位）
- `GET /api/problems/{id}/testpoints/{testPointId}/{type}`（下载：可返回 blob 占位）

P2（提交状态实时更新）：

- `SubmissionList.vue` 的 `/submitshub`：
  - mock 模式下建议不连接 SignalR。
  - 用定时器在前端本地模拟状态变化（例如 Running -> Accepted/WrongAnswer），或提供“刷新/轮询”替代。
  - 通过 `VITE_USE_MOCK` 走分支，避免影响真实环境。

### 开发体验增强（可选但建议）

- 在 mock 模式下提供可切换“身份/角色”的控制面板（仅开发用）：
  - 例如 `localStorage` 保存当前 mock 用户（匿名/普通用户/Distributor/Admin）。
  - `GET /bff/user` 根据该状态返回不同 claims。

### scripts 规划（前端）

- `yarn dev`：默认走真实后端（保持现状）。
- `yarn dev:mock`：开启 mock 模式（仅前端即可跑）。
- `yarn build`：不启用 mock。

### 验收标准

- 在“后端完全离线”的情况下：`cd Client && yarn dev:mock` 能打开并完成：
  - 题目列表/题目详情可浏览
  - 提交列表/提交详情可浏览（状态可静态或模拟变化）
  - 用户主页可展示统计信息
  - 注册页可走通（写入 mock 数据或返回固定结果）
- 切回真实后端时（`yarn dev`）：不需要改代码即可恢复请求透传。

## 风险与注意事项

- claims 兼容：如果后端在 OpenIddict 迁移时改变了 claim 命名/是否下发头像等字段，前端必须通过映射层兜底，否则会出现“已登录但不显示用户名/头像/权限”问题。
- CSRF：mock 模式下仍会带 `X-CSRF: 1`，handlers 需允许该头部存在（一般无影响）。
- 文件下载/上传：mock 可先返回占位结果（字符串文件名/空 blob），只要不阻塞 UI 开发。
- SignalR：建议先做 mock 降级，不追求完全模拟 WebSocket 协议。

## 里程碑（建议）

1. M1：引入 MSW + 覆盖 P0 接口 + 新增 `yarn dev:mock`。
2. M2：claims 映射层（为 OpenIddict 迁移做前端兜底）。
3. M3：补齐 P1 接口 + mock 用户/角色切换。
4. M4：提交状态 mock 降级方案落地（定时器/轮询/手动刷新）。
