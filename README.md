# ERP.WebAPI
模組化DI重新架構ERP專案

# scaffold(db-first)
dotnet ef dbcontext scaffold "Server=DESKTOP-V510SQ3;Database=ERP;User Id=sa;Password=sa123;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c ERPDbContext --project ERP.Db --context-dir Context --use-database-names --no-onconfiguring --force

# migrations
dotnet tool install --global dotnet-ef
dotnet ef migrations add Add__ERP_2000 --project EntityModels/ERP.EntityModels --startup-project ERP.WebAPI
dotnet ef migrations add AddNewModels --project EntityModels/AMS.EntityModels --startup-project ERP.WebAPI

dotnet ef database update `
  --project EntityModels\ERP.EntityModels\ERP.EntityModels.csproj `
  --startup-project ERP.WebAPI\ERP.WebAPI.csproj `
  --context ERPContext

dotnet ef database update --startup-project ERP.WebAPI