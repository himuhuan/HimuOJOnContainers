# HimuOJ

## 运行环境

- Docker Desktop（含 Docker Compose）
- Node.js 18+ 与 Yarn 1.22+（用于前端开发服务）
- PostgreSQL（宿主机 `5432`，账号 `postgres`，密码 `liuhuan123`）

> `docker-compose.override.yml` 默认通过 `host.docker.internal:5432` 连接 PostgreSQL。  
> `.env` 默认配置 `HIMUOJ_EXTERNAL_DNS_NAME_OR_IP=host.docker.internal`，一般无需修改。

## 1. 准备数据库

在 PostgreSQL 中创建以下数据库（已存在可跳过）：

- `HimuOJIdentityDB`
- `HimuOJProblemsDB`
- `HimuOJSubmitsDB`

示例 SQL：

```sql
CREATE DATABASE "HimuOJIdentityDB";
CREATE DATABASE "HimuOJProblemsDB";
CREATE DATABASE "HimuOJSubmitsDB";
```

## 2. 启动后端（Docker）

在项目根目录执行：

```powershell
docker compose up -d --build
docker compose ps
```

主要服务端口：

- Identity: `http://localhost:5001`
- Problems API: `http://localhost:5100`
- Submits API: `http://localhost:5200`
- WebSPA BFF: `http://localhost:6000`
- MinIO API: `http://localhost:9000`
- MinIO Console: `http://localhost:9090`

## 3. 启动前端（本地开发服务）

在另一个终端执行：

```powershell
cd src/Web/WebSPA/Client
yarn install
yarn dev --host 127.0.0.1 --port 6100
```

前端访问地址：

- `http://127.0.0.1:6100`

## 4. 常用排查命令

```powershell
docker compose logs -f identity problems-api submits-api submits-backgroundtasks webspa-bff
docker compose ps
```

## 5. 停止服务

```powershell
docker compose down
```

前端开发服务在其终端按 `Ctrl + C` 停止。
