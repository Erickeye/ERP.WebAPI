using ERP.EntityModels.Models._1000Company;
using ERP.EntityModels.Models._2000Customer;
using ERP.EntityModels.Models._9000Other;
using ERP.EntityModels.Models.Other;
using ERP.Models.AMS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Data
{
    public class ERPContext : DbContext
    {
        public ERPContext(DbContextOptions<ERPContext> options)
            : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Level> Level { get; set; }
        public DbSet<t_1000Staff> t_1000Staff { get; set; }
        public DbSet<t_1001StaffCertificates> t_1001StaffCertificates { get; set; }
        public DbSet<t_1005ProjectStaff> t_1005ProjectStaff { get; set; }
        public DbSet<t_1020PerformanceTarget> t_1020PerformanceTarget { get; set; }
        public DbSet<t_1030Dayoff> t_1030Dayoff { get; set; }
        public DbSet<t_1039DayoffProxy> t_1039DayoffProxy { get; set; }
        public DbSet<t_1040Document> t_1040Document { get; set; }
        public DbSet<t_1050WorkOver> t_1050WorkOver { get; set; }
        public DbSet<t_1080Company> t_1080Company { get; set; }
        public DbSet<t_1100Department> t_1100Department { get; set; }
        public DbSet<t_1101DepartmentUnit> t_1101DepartmentUnit { get; set; }
        public DbSet<t_1700LoginLog> t_1700LoginLog { get; set; }
        public DbSet<t_1710ActionInfo> t_1710ActionInfo { get; set; }
        public DbSet<t_1200PettyCash> t_1200PettyCash { get; set; }
        public DbSet<t_1201PettyCashDetail> t_1201PettyCashDetail { get; set; }
        public DbSet<t_2000Customer> t_2000Customer { get; set; }
        public DbSet<t_2010Custemploy> t_2010Custemploy { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<ApprovalSettings> ApprovalSettings { get; set; }
        public DbSet<ApprovalStep> ApprovalStep { get; set; }
        public DbSet<ApprovalRecord> ApprovalRecord { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>()
                .HasKey(p => new { p.RoleId, p.PageId }); // 複合主鍵

            base.OnModelCreating(modelBuilder);

            // 對所有 decimal 屬性自動設定 precision
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetPrecision(12); // precision = 12
                        property.SetScale(2);      // scale = 2
                    }
                }
            }
        }
    }
}