# ERP.WebAPI
模組化DI重新架構ERP專案

# scaffold(db-first)
dotnet ef dbcontext scaffold "Server=DESKTOP-V510SQ3;Database=ERP;User Id=sa;Password=sa123;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c ERPDbContext --project ERP.Db --context-dir Context --use-database-names --no-onconfiguring --force

# migrations
dotnet tool install --global dotnet-ef
dotnet ef migrations add AddNewModels --project EntityModels/AMS.EntityModels --startup-project ERP.WebAPI
dotnet ef database update