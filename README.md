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

# 簽核模組
[ApprovalSettings] => [簽核設定]可以客制化給每一項功能(請假單、加班單等)，且可以隨時切換模組

[ApprovalStep]  => [簽核設定] 底下的步驟，可設置多個步驟，且有3個模式供選擇  
                    1.指定使用者、2.指定使用者(複數)、3.指定角色(可選擇需求數量)  
                    【規則】: 當所有Step完成，該簽核流程結束

[ApprovalStepNumber] =>  1.指定使用者 => 1筆對應user資料  
                         2.指定使用者(複數n) => n筆對應user資料  
                         3.沒有資料(設定數量在ApprovalStep)

[ApprovalRecord] => 1.第一筆Record為申請者自己且自動完成簽核  
                    2.當發送簽和請求時: 假設總計有5人，就會有5筆資料(包含自己)  
                    3.該紀錄有包含單號、使用者userId(roleId)  
                    4.採階段式進行，前一個Step未完成則不能進行簽核
                    
