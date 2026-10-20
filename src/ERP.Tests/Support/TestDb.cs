using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ERP.Tests.Support;

/// <summary>
/// 測試用資料庫：同一條 SQLite 記憶體連線，可建立多個 DbContext 共用同一份資料。
/// </summary>
public sealed class TestDb : IDisposable
{
    public SqliteConnection Connection { get; }

    public TestDb()
    {
        Connection = new SqliteConnection("DataSource=:memory:");
        Connection.Open();

        // 測試重點在業務規則，關閉外鍵檢查以簡化測試資料
        using (var command = Connection.CreateCommand())
        {
            command.CommandText = "PRAGMA foreign_keys = OFF;";
            command.ExecuteNonQuery();
        }

        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public TestERPDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ERPDbContext>()
            .UseSqlite(Connection)
            .Options;
        return new TestERPDbContext(options);
    }

    public void Dispose() => Connection.Dispose();
}

/// <summary>
/// 測試用 DbContext。SQLite 沒有 SQL Server 的 rowversion，也不支援 getdate() 預設值，
/// 這裡改成：移除 SQL 預設值、由程式在儲存時自行更換庫存的 RowVersion，
/// 讓樂觀並行（RowVersion 不符 → DbUpdateConcurrencyException）行為與正式環境一致。
/// </summary>
public class TestERPDbContext : ERPDbContext
{
    /// <summary>
    /// 下一次儲存前執行一次的動作，用來模擬「讀取之後、儲存之前，別人改了庫存」。
    /// </summary>
    public Action<TestERPDbContext>? BeforeNextSave { get; set; }

    public TestERPDbContext(DbContextOptions<ERPDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                property.SetDefaultValueSql(null);
                property.SetComputedColumnSql(null);

                if (entityType.ClrType == typeof(t_4000Inventory) && property.Name == "RowVersion")
                {
                    property.ValueGenerated = ValueGenerated.Never;
                }
            }
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        PrepareSave();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        PrepareSave();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void PrepareSave()
    {
        if (BeforeNextSave != null)
        {
            var action = BeforeNextSave;
            BeforeNextSave = null;
            action(this);
        }

        foreach (var entry in ChangeTracker.Entries<t_4000Inventory>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                // 原始值保持不變，UPDATE 的 WHERE 會用原始 RowVersion 比對
                entry.Property(x => x.RowVersion).CurrentValue = Guid.NewGuid().ToByteArray();
            }
        }
    }
}
