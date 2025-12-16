# ERP.WebAPI
模組化DI重新架構ERP專案

# scaffold(db-first)
dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS;Database=ERP;User Id=sa;Password=sa123;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c ERPDbContext --context-dir Context --use-database-names --no-pluralize --no-onconfiguring --force

# migrations
dotnet tool install --global dotnet-ef
dotnet ef migrations add Add__ERP_2000 --project EntityModels/ERP.EntityModels --startup-project ERP.WebAPI --context ERPContext

dotnet ef database update `
  --project EntityModels\ERP.EntityModels\ERP.EntityModels.csproj `
  --startup-project ERP.WebAPI\ERP.WebAPI.csproj `
  --context ERPContext

dotnet ef database update --startup-project ERP.WebAPI --context ERPContext

# Vue3
npm init vue@lastest
npm install

# SFTP
參考: https://blog.ite2.com/2023/11/22/quick-setup-ftp-sftp-server/
