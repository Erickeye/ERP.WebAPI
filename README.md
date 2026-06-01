# ERP.WebAPI

ERP.WebAPI 是一個以 ASP.NET Core Web API 為後端、Vue 3 為前端的 ERP 練習專案。專案主要用來整理常見後台系統會遇到的功能，例如登入驗證、角色權限、基本資料維護、庫存/採購、請假與審核流程、操作紀錄、Redis 驗證碼，以及 SFTP 檔案操作。

## 專案結構

```text
ERP.WebAPI
├─ src
│  ├─ ERP.WebAPI              # ASP.NET Core Web API 入口、Controller、Middleware、Swagger 設定
│  ├─ ERP.Service             # 主要商業邏輯與服務層
│  ├─ ERP.Library             # ViewModel、Enum、共用 Helper、Extension
│  ├─ EntityModels
│  │  └─ ERP.EntityModels     # EF Core DbContext 與資料表 Entity
│  ├─ ERP.DB                  # SQL Server Database Project
│  └─ ERP.Vue                 # Vue 3 前端專案
├─ dbData                     # 本機資料庫備份資料
└─ document                   # 開發文件、筆記、AI Prompt
```

## 主要功能

- JWT 登入驗證與 Refresh Token
- 角色、使用者與權限管理
- Swagger API 文件與 API 分組
- 全域授權與統一回傳模型
- 全域例外處理 Middleware
- 操作紀錄與登入紀錄
- Redis 驗證碼服務
- SFTP 檔案上傳、下載與清單查詢
- 公司、人員、客戶、庫存、採購等 ERP 基礎資料功能
- 請假申請與特休天數計算
- 審核流程設定、通知與審核紀錄
- Vue 3 + Element Plus 後台介面

## 技術棧

### 後端

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- Swagger / Swashbuckle
- Serilog
- StackExchange.Redis
- SSH.NET
- SkiaSharp

### 前端(尚未深入)

- Vue 3
- Vue Router
- Element Plus
- Axios
- Vite

## 開發環境

建議安裝以下工具：

- .NET 8 SDK
- Node.js
- SQL Server / SQL Server Express
- Redis
- Visual Studio 2022 或 Rider

如果需要建置 `ERP.DB.sqlproj`，Visual Studio 需安裝 SQL Server Data Tools (SSDT)。一般 Web API 開發可直接建置 `ERP.WebAPI.csproj`。


## 資料庫

此專案使用 EF Core DB-first 方式產生 Entity 與 DbContext。重新 scaffold 可參考：

```powershell
dotnet ef dbcontext scaffold `
  "Server=.\SQLEXPRESS;Database=ERP;User Id=<user>;Password=<password>;TrustServerCertificate=True;" `
  Microsoft.EntityFrameworkCore.SqlServer `
  -o Models `
  -c ERPDbContext `
  --context-dir Context `
  --use-database-names `
  --no-pluralize `
  --no-onconfiguring `
  --force
```
- 範例: dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS;Database=ERP;User Id=sa;Password=sa123;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c ERPDbContext --context-dir Context --use-database-names --no-pluralize --no-onconfiguring --force

## 設定檔

本機開發主要使用：

```text
src/ERP.WebAPI/appsettings.Development.json
```

需要設定的項目包含：

- `ConnectionStrings:ERP`
- `JwtSettings`
- `RedisSettings`
- `SftpConfig`


安裝 EF Core CLI：

```powershell
dotnet tool install --global dotnet-ef
```

## 備註

