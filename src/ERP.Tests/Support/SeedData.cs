using ERP.EntityModels.Models;
using ERP.Tests.Support;

namespace ERP.Tests.Support;

/// <summary>
/// 測試資料建立工具
/// </summary>
public static class SeedData
{
    public record Basics(int PurposeId, int LocationId, int OtherLocationId, int StaffId, int SupplierId, int CompanyId = 0);

    public static Basics CreateBasics(TestERPDbContext db)
    {
        var purpose = new SystemConfig { ConfigType = "MaterialIssuePurpose", Name = "專案領用", Sort = 1, IsActive = true };
        var location = new SystemConfig { ConfigType = "InventoryLocation", Name = "A 倉", Sort = 1, IsActive = true };
        var otherLocation = new SystemConfig { ConfigType = "InventoryLocation", Name = "B 倉", Sort = 2, IsActive = true };
        var staff = new t_1000Staff
        {
            ChineseName = "領料人",
            Gender = 1,
            Account = "receiver",
            ContactPhone = "0900000000",
            BankAccount = "000",
            ContactAddress = "測試地址",
            Email = "receiver@example.com",
            Birthday = new DateTime(1990, 1, 1),
            HighestEducation = "大學",
            OnBoardDate = new DateTime(2020, 1, 1),
            IdCard = "A123456789",
            LaborPension = 0,
            BloodType = 1
        };
        var company = CreateCompany(db, "預設公司", "00000000", isActive: true);
        db.SystemConfig.AddRange(purpose, location, otherLocation);
        db.t_1000Staff.Add(staff);
        db.SaveChanges();

        return new Basics(purpose.Id, location.Id, otherLocation.Id, staff.StaffId, SupplierId: 1, CompanyId: company.Id);
    }

    /// <summary>
    /// 建立一間公司（單據公司必填，種子資料先建一間，其他測試需要時再自行建立）
    /// </summary>
    public static t_1080Company CreateCompany(TestERPDbContext db, string attribName, string taxInvoiceNumber, bool isActive = true)
    {
        var company = new t_1080Company
        {
            AttribName = attribName,
            Name = $"{attribName}股份有限公司",
            TaxInvoiceNumber = taxInvoiceNumber,
            TaxSerialNumber = "000000000",
            Owner = "負責人",
            RegisteredAddress = "測試地址",
            IsActive = true
        };
        db.t_1080Company.Add(company);
        db.SaveChanges();

        // IsActive 的資料表預設值是 true，false 會被 EF 視為「未指定」；要停用就先存成啟用再改
        if (!isActive)
        {
            company.IsActive = false;
            db.SaveChanges();
        }
        return company;
    }

    public static t_4000Inventory CreateInventory(
        TestERPDbContext db, Basics basics, string no, decimal quantity, int? locationId = null)
    {
        var inventory = new t_4000Inventory
        {
            SupplierId = basics.SupplierId,
            Name = $"品項{no}",
            LocationId = locationId ?? basics.LocationId,
            No = no,
            Unit = "個",
            Quantity = quantity
        };
        db.t_4000Inventory.Add(inventory);
        db.SaveChanges();
        return inventory;
    }
}
