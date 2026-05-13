# Social Media MVC

這是一個使用 ASP.NET Core MVC 與 SQL Server 開發的社群網站範例，並透過 Docker Compose 進行容器化部署。

## 技術棧

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core
- SQL Server 2022
- Nginx（反向代理）
- Docker Compose

## 功能特色（Features）

- 會員註冊（手機號碼 + Email + 密碼）
- 會員登入/登出（Cookie Authentication）
- 發文功能：新增、列表、編輯、刪除（軟刪除）
- 留言功能：針對貼文新增留言、列表顯示
- 權限控制：未登入不可發文/留言/編輯/刪除
- 共用錯誤頁：統一處理 500/404/403/401/400
- Development 啟動自動初始化資料庫（Migration + SP + Seed + Sample Data）

## 後續優化方向

目前系統可用且符合題目需求，但若往真實社群平台規模發展，建議以下優化：

1. 架構優化：由 SSR 漸進式走向前後端分離
- 現況：ASP.NET Core MVC Server-Side Rendering。
- 限制：高互動場景（即時通知、局部刷新、動態載入）開發與維護成本較高。
- 優化：抽離 API 層，前端改為獨立應用（SPA），先從讀取型頁面開始逐步改造，避免一次重寫風險。

2. 驗證機制優化：由 Cookie-based 擴展為 Token-based（JWT/OAuth2）
- 現況：使用 Cookie Authentication，適合單體 MVC。
- 限制：在分散式/多服務架構下，跨服務授權、跨網域存取、行動端整合與水平擴展較不易。
- 優化：導入 JWT + Refresh Token（或整合 OAuth2/OIDC），讓 Web、App、第三方服務共用統一授權模型。

3. 即時能力優化：支援 WebSocket/SignalR
- 現況：頁面刷新後才看到最新資料。
- 限制：社群互動延遲，使用者體驗不佳。
- 優化：新增即時推送（新貼文、留言、通知），減少輪詢成本。

4. 資料存取優化：逐步統一存取策略
- 現況：建立資料使用 Stored Procedure；查詢/更新部分使用 EF Core。
- 限制：資料存取風格混用，後續維運與效能調校成本較高。
- 優化：依情境明確分層（例如 Command 走 SP、Query 走 EF/Dapper），並建立一致的查詢規範與索引策略。

5. 可擴展性優化：快取與非同步化
- 現況：主要依賴同步資料庫讀寫。
- 限制：熱門貼文與高流量時 DB 壓力上升。
- 優化：加入 Redis 快取（熱門 feed、使用者摘要），並以佇列處理非同步工作（通知、稽核、統計）。

6. 可觀測性優化：監控、追蹤、告警
- 現況：有基本錯誤頁與 Request ID。
- 限制：跨服務問題定位困難。
- 優化：導入集中式 log、metrics、distributed tracing，並設定告警門檻（錯誤率、延遲、DB 連線）。

7. 安全性優化：強化生產環境安全基線
- 現況：具備 Anti-Forgery、輸入驗證、參數化查詢。
- 限制：秘密資訊目前仍有明文配置樣例。
- 優化：改用 Secret Manager / Vault / `.env` 注入敏感資訊，補上 CSP、Rate Limit、登入風險控制（IP/裝置維度）。

## 實作題需求對照（依「【新進.Net】玉山銀行軟體工程師實作題 I」）

1. 註冊功能（以手機號碼註冊與登入）
- 你怎麼做：
  - 註冊頁面：`/Account/Register`
  - 登入頁面：`/Account/Login`（以 `PhoneNumber + Password` 驗證）
  - 註冊時先檢查手機或 Email 是否重複，再建立使用者。
- 實作位置：
  - Controller：[AccountController.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Controllers\AccountController.cs)
  - Service：[AccountService.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Services\AccountService.cs)
  - Repository（SP 呼叫）：[UserRepository.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Repositories\UserRepository.cs)

2. 登入驗證（只有登入者可發文或留言）
- 你怎麼做：
  - 使用 ASP.NET Core Cookie Authentication。
  - 需要保護的動作都加上 `[Authorize]`。
  - 發文、留言、編輯、刪除皆要求登入。
- 實作位置：
  - 認證設定：[Program.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Program.cs)
  - 發文控制器授權：[PostsController.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Controllers\PostsController.cs)
  - 留言控制器授權：[CommentsController.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Controllers\CommentsController.cs)

3. 發文功能（新增、列出、編輯、刪除）
- 你怎麼做：
  - 新增發文：`sp_Post_Create`
  - 列出所有發文：Feed 查詢（依時間倒序，排除已刪除）
  - 編輯發文：僅作者本人可編輯
  - 刪除發文：軟刪除（`IsDeleted = true`）
- 實作位置：
  - Controller：[PostsController.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Controllers\PostsController.cs)
  - Service：[PostService.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Services\PostService.cs)
  - Repository：[PostRepository.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Repositories\PostRepository.cs)

4. 留言功能（針對發文新增留言）
- 你怎麼做：
  - 登入者可對貼文新增留言（`sp_Comment_Create`）。
  - 列表頁一次載入多篇貼文留言，減少查詢次數。
- 實作位置：
  - Controller：[CommentsController.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Controllers\CommentsController.cs)
  - Service：[CommentService.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Services\CommentService.cs)
  - Repository：[CommentRepository.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Repositories\CommentRepository.cs)

5. 系統架構要求（C# / ASP.NET 10+ MVC、三層式）
- 你怎麼做：
  - C# + ASP.NET Core MVC (.NET 10)。
  - 三層式：Nginx（Web Server）+ ASP.NET App（Application Server）+ SQL Server（RDBMS）。
  - 後端分層：
    - 展示層：Controllers + Views
    - 業務層：Services
    - 資料層：Repositories + EF Core/Stored Procedure
    - 共用層：`Common/Result` 等
- 實作位置：
  - 啟動設定：[Program.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Program.cs)
  - 容器架構：[docker-compose.yml](C:\Users\dickson\Desktop\social-media-mvc\docker-compose.yml)

6. 技術要求對照
- 使用 Bootstrap 支援 RWD
  - 做法：Layout 引入 Bootstrap CSS/JS，頁面採 Bootstrap 元件與格線。
  - 位置：[Views/Shared/_Layout.cshtml](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Views\Shared\_Layout.cshtml)
- 透過 Stored Procedure 存取資料庫
  - 做法：使用 `sp_User_*`、`sp_Post_Create`、`sp_Comment_Create`。
  - 位置：[stored-procedures.sql](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\DB\stored-procedures.sql)
- 多表異動需 Transaction
  - 做法：刪除貼文時，發文軟刪除 + 該貼文留言軟刪除包在同一個 transaction。
  - 位置：[PostService.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Services\PostService.cs)
- DDL/DML 放在 `DB` 資料夾
  - 做法：`schema.sql`、`stored-procedures.sql`、`seed.sql`、`sample-data.sql` 皆已放置。
  - 位置：`SocialMediaApp.Web/DB/`
- 防止 SQL Injection 與 XSS
  - SQL Injection：Repository 呼叫 Stored Procedure 時皆使用 `SqlParameter` 參數化，不拼接字串 SQL。
  - XSS：Razor 預設輸出會 HTML Encode，且輸入有 DataAnnotations 驗證（長度/格式）與 Anti-Forgery Token。
  - 位置：
    - 參數化查詢：[UserRepository.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Repositories\UserRepository.cs)
    - 防偽驗證（`[ValidateAntiForgeryToken]`）：`AccountController`、`PostsController`、`CommentsController`
    - 輸入驗證：[ViewModels](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\ViewModels)

7. 密碼安全（題目要求加鹽雜湊）
- 你怎麼做：
  - 使用 `PasswordHasher<User>` 實作雜湊與驗證（含 salt 機制，不儲存明碼）。
- 實作位置：
  - [AccountService.cs](C:\Users\dickson\Desktop\social-media-mvc\SocialMediaApp.Web\Services\AccountService.cs)

## 啟動前需要準備什麼

請先安裝以下工具：

1. Git
2. Docker Desktop（需支援 Docker Compose）
3. .NET SDK 10.0（只有在不使用 Docker 本機啟動時才需要）

可選工具：

- SQL Server 用戶端工具（例如 `sqlcmd`，可手動執行 DB 腳本）
- Postman（用於測試 API 或流程）

## 下載專案

```bash
git clone https://github.com/dicksonchai98/social-media-mvc.git
cd social-media-mvc
```

建立本機環境變數檔（不要提交到 Git）：

```bash
cp .env.example .env
```

## 啟動方式（建議使用 Docker）

啟動所有服務：

```bash
docker compose up --build
```

背景執行：

```bash
docker compose up -d --build
```

停止服務：

```bash
docker compose down
```

## 服務位址

- 網站（經 Nginx）：`http://localhost:8080`
- SQL Server：`localhost:1433`
  - 帳號：`sa`
  - 密碼：請使用 `.env` 的 `DB_PASSWORD`
  - 資料庫：`SocialMediaDb`

## 資料庫初始化

在 Development 環境下，應用程式啟動時會自動完成以下初始化：

1. 執行 EF Core Migration（建立/更新資料表）
2. 執行 `SocialMediaApp.Web/DB/stored-procedures.sql`
3. 執行 `SocialMediaApp.Web/DB/seed.sql`
4. 執行 `SocialMediaApp.Web/DB/sample-data.sql`

`seed.sql` 與 `sample-data.sql` 已做重複執行保護，重啟服務不會無限重複插入同一筆資料。

## 不使用 Docker 的本機啟動方式（Local .NET）

1. 先啟動本機 SQL Server（或保持 Docker 的 SQL Server 在執行中）。
2. 確認 `SocialMediaApp.Web/appsettings.Development.json` 的連線字串正確。
3. 執行：

```bash
dotnet restore SocialMediaApp.Web/SocialMediaApp.Web.csproj
dotnet run --project SocialMediaApp.Web/SocialMediaApp.Web.csproj
```

實際本機網址會顯示在終端機輸出（通常為 `http://localhost:xxxx`）。

## 如何確認啟動成功

1. 開啟 `http://localhost:8080`，確認可看到貼文頁面。
2. 查看容器狀態：

```bash
docker compose ps
```

3. 查看服務日誌：

```bash
docker compose logs -f
```

## 專案結構

```text
SocialMediaApp.Web/
  Controllers/
  Data/
  DB/
  Entities/
  Repositories/
  Services/
  Views/
SocialMediaApp.Web.Tests/
nginx/
docker-compose.yml
Dockerfile
```
