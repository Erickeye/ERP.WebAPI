using System;
using System.Collections.Generic;
using ERP.EntityModels.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.EntityModels.Context;

public partial class ERPDbContext : DbContext
{
    public ERPDbContext(DbContextOptions<ERPDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApprovalRecord> ApprovalRecord { get; set; }

    public virtual DbSet<ApprovalSettings> ApprovalSettings { get; set; }

    public virtual DbSet<ApprovalStep> ApprovalStep { get; set; }

    public virtual DbSet<ApprovalStepNumber> ApprovalStepNumber { get; set; }

    public virtual DbSet<Level> Level { get; set; }

    public virtual DbSet<Notification> Notification { get; set; }

    public virtual DbSet<Permission> Permission { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<User> User { get; set; }

    public virtual DbSet<t_1000Staff> t_1000Staff { get; set; }

    public virtual DbSet<t_1001StaffCertificates> t_1001StaffCertificates { get; set; }

    public virtual DbSet<t_1005ProjectStaff> t_1005ProjectStaff { get; set; }

    public virtual DbSet<t_1020PerformanceTarget> t_1020PerformanceTarget { get; set; }

    public virtual DbSet<t_1030Dayoff> t_1030Dayoff { get; set; }

    public virtual DbSet<t_1039DayoffProxy> t_1039DayoffProxy { get; set; }

    public virtual DbSet<t_1040Document> t_1040Document { get; set; }

    public virtual DbSet<t_1050WorkOver> t_1050WorkOver { get; set; }

    public virtual DbSet<t_1080Company> t_1080Company { get; set; }

    public virtual DbSet<t_1100Department> t_1100Department { get; set; }

    public virtual DbSet<t_1101DepartmentUnit> t_1101DepartmentUnit { get; set; }

    public virtual DbSet<t_1200PettyCash> t_1200PettyCash { get; set; }

    public virtual DbSet<t_1201PettyCashDetail> t_1201PettyCashDetail { get; set; }

    public virtual DbSet<t_1700LoginLog> t_1700LoginLog { get; set; }

    public virtual DbSet<t_1710ActionInfo> t_1710ActionInfo { get; set; }

    public virtual DbSet<t_2000Customer> t_2000Customer { get; set; }

    public virtual DbSet<t_2010Custemploy> t_2010Custemploy { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApprovalRecord>(entity =>
        {
            entity.HasIndex(e => e.ApprovalStepId, "IX_ApprovalRecord_ApprovalStepId");

            entity.Property(e => e.Memo).HasMaxLength(256);
            entity.Property(e => e.TableId).HasMaxLength(32);

            entity.HasOne(d => d.ApprovalStep).WithMany(p => p.ApprovalRecord).HasForeignKey(d => d.ApprovalStepId);
        });

        modelBuilder.Entity<ApprovalSettings>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(64);
        });

        modelBuilder.Entity<ApprovalStep>(entity =>
        {
            entity.HasIndex(e => e.ApprovalSettingsId, "IX_ApprovalStep_ApprovalSettingsId");

            entity.HasOne(d => d.ApprovalSettings).WithMany(p => p.ApprovalStep).HasForeignKey(d => d.ApprovalSettingsId);
        });

        modelBuilder.Entity<ApprovalStepNumber>(entity =>
        {
            entity.HasIndex(e => e.ApprovalStepId, "IX_ApprovalStepNumber_ApprovalStepId");

            entity.HasOne(d => d.ApprovalStep).WithMany(p => p.ApprovalStepNumber).HasForeignKey(d => d.ApprovalStepId);
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.PermissionId);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(e => e.Message).HasMaxLength(256);
            entity.Property(e => e.TargetId).HasMaxLength(32);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PageId });

            entity.HasOne(d => d.Role).WithMany(p => p.Permission).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleName).HasMaxLength(32);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_User_RoleId");

            entity.Property(e => e.Account).HasMaxLength(32);
            entity.Property(e => e.Name).HasMaxLength(32);
            entity.Property(e => e.Pwd).HasMaxLength(128);

            entity.HasOne(d => d.Role).WithMany(p => p.User).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<t_1000Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId);

            entity.Property(e => e.Account).HasMaxLength(32);
            entity.Property(e => e.BankAccount).HasMaxLength(32);
            entity.Property(e => e.BankName).HasMaxLength(64);
            entity.Property(e => e.BusinessPhone).HasMaxLength(32);
            entity.Property(e => e.ChineseName).HasMaxLength(32);
            entity.Property(e => e.ContactAddress).HasMaxLength(64);
            entity.Property(e => e.ContactPhone).HasMaxLength(32);
            entity.Property(e => e.Email).HasMaxLength(64);
            entity.Property(e => e.EmergencyContact1Address).HasMaxLength(64);
            entity.Property(e => e.EmergencyContact1Name).HasMaxLength(32);
            entity.Property(e => e.EmergencyContact1Phone).HasMaxLength(32);
            entity.Property(e => e.EmergencyContact1Relationship).HasMaxLength(16);
            entity.Property(e => e.EmergencyContact2Address).HasMaxLength(64);
            entity.Property(e => e.EmergencyContact2Name).HasMaxLength(32);
            entity.Property(e => e.EmergencyContact2Phone).HasMaxLength(32);
            entity.Property(e => e.EmergencyContact2Relationship).HasMaxLength(16);
            entity.Property(e => e.EnglishName).HasMaxLength(32);
            entity.Property(e => e.HighestEducation).HasMaxLength(32);
            entity.Property(e => e.IdCard).HasMaxLength(16);
            entity.Property(e => e.LaborPension).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.LineId).HasMaxLength(64);
            entity.Property(e => e.OfficialEmail).HasMaxLength(64);
            entity.Property(e => e.SubBankName).HasMaxLength(32);
        });

        modelBuilder.Entity<t_1001StaffCertificates>(entity =>
        {
            entity.HasIndex(e => e.StaffId, "IX_t_1001StaffCertificates_StaffId");

            entity.Property(e => e.CertificateName).HasMaxLength(128);

            entity.HasOne(d => d.Staff).WithMany(p => p.t_1001StaffCertificates).HasForeignKey(d => d.StaffId);
        });

        modelBuilder.Entity<t_1005ProjectStaff>(entity =>
        {
            entity.HasKey(e => e.f_ProjectStaff_ID);

            entity.Property(e => e.f_ProjectStaff_Account).HasMaxLength(20);
            entity.Property(e => e.f_ProjectStaff_BankAccount).HasMaxLength(30);
            entity.Property(e => e.f_ProjectStaff_BankName).HasMaxLength(30);
            entity.Property(e => e.f_ProjectStaff_ChineseName).HasMaxLength(20);
            entity.Property(e => e.f_ProjectStaff_ContactAddress).HasMaxLength(50);
            entity.Property(e => e.f_ProjectStaff_ContactPhone).HasMaxLength(20);
            entity.Property(e => e.f_ProjectStaff_ContractID).HasMaxLength(20);
            entity.Property(e => e.f_ProjectStaff_EC1Address).HasMaxLength(50);
            entity.Property(e => e.f_ProjectStaff_EC1Cellphone).HasMaxLength(20);
            entity.Property(e => e.f_ProjectStaff_EC1Name).HasMaxLength(20);
            entity.Property(e => e.f_ProjectStaff_EC1Relationship).HasMaxLength(10);
            entity.Property(e => e.f_ProjectStaff_Email).HasMaxLength(50);
            entity.Property(e => e.f_ProjectStaff_EnglishName).HasMaxLength(20);
            entity.Property(e => e.f_ProjectStaff_IDCard).HasMaxLength(10);
            entity.Property(e => e.f_ProjectStaff_LaborPension).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.f_ProjectStaff_LineID).HasMaxLength(50);
            entity.Property(e => e.f_ProjectStaff_SubBankName).HasMaxLength(20);
        });

        modelBuilder.Entity<t_1020PerformanceTarget>(entity =>
        {
            entity.HasKey(e => e.f_PTarget_ID);

            entity.Property(e => e.f_PTarget_Achieve).HasColumnType("decimal(12, 2)");
        });

        modelBuilder.Entity<t_1030Dayoff>(entity =>
        {
            entity.HasIndex(e => e.Applicant, "IX_t_1030Dayoff_Applicant");

            entity.HasIndex(e => e.LeaveTaker, "IX_t_1030Dayoff_LeaveTaker");

            entity.HasIndex(e => e.Proxy, "IX_t_1030Dayoff_Proxy");

            entity.Property(e => e.ProxySignature).HasMaxLength(128);
            entity.Property(e => e.Reason).HasMaxLength(64);

            entity.HasOne(d => d.ApplicantNavigation).WithMany(p => p.t_1030DayoffApplicantNavigation).HasForeignKey(d => d.Applicant);

            entity.HasOne(d => d.LeaveTakerNavigation).WithMany(p => p.t_1030DayoffLeaveTakerNavigation).HasForeignKey(d => d.LeaveTaker);

            entity.HasOne(d => d.ProxyNavigation).WithMany(p => p.t_1030DayoffProxyNavigation).HasForeignKey(d => d.Proxy);
        });

        modelBuilder.Entity<t_1039DayoffProxy>(entity =>
        {
            entity.HasIndex(e => e.DayoffId, "IX_t_1039DayoffProxy_DayoffId");

            entity.HasOne(d => d.Dayoff).WithMany(p => p.t_1039DayoffProxy).HasForeignKey(d => d.DayoffId);
        });

        modelBuilder.Entity<t_1040Document>(entity =>
        {
            entity.Property(e => e.Attachment).HasMaxLength(32);
            entity.Property(e => e.Authorizator).HasMaxLength(32);
            entity.Property(e => e.Company).HasMaxLength(64);
            entity.Property(e => e.ContactPerson).HasMaxLength(32);
            entity.Property(e => e.Contract).HasMaxLength(32);
            entity.Property(e => e.Original).HasMaxLength(64);
            entity.Property(e => e.Recipient).HasMaxLength(64);
            entity.Property(e => e.Remark2).HasMaxLength(64);
            entity.Property(e => e.Subject).HasMaxLength(256);
        });

        modelBuilder.Entity<t_1050WorkOver>(entity =>
        {
            entity.Property(e => e.Applicant).HasMaxLength(32);
            entity.Property(e => e.Authorizator).HasMaxLength(32);
            entity.Property(e => e.Department).HasMaxLength(32);
            entity.Property(e => e.JobTitle).HasMaxLength(32);
            entity.Property(e => e.OverTimeHours).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Reason).HasMaxLength(64);
            entity.Property(e => e.SignaturePath).HasMaxLength(64);
        });

        modelBuilder.Entity<t_1080Company>(entity =>
        {
            entity.Property(e => e.AttribName).HasMaxLength(32);
            entity.Property(e => e.BankName).HasMaxLength(32);
            entity.Property(e => e.CheckingAccount).HasMaxLength(32);
            entity.Property(e => e.ContactPhone).HasMaxLength(32);
            entity.Property(e => e.DeliveryAddress).HasMaxLength(64);
            entity.Property(e => e.FaxPhone).HasMaxLength(32);
            entity.Property(e => e.InvoiceForm).HasMaxLength(8);
            entity.Property(e => e.Name).HasMaxLength(32);
            entity.Property(e => e.Owner).HasMaxLength(32);
            entity.Property(e => e.PayDays).HasMaxLength(8);
            entity.Property(e => e.RegisteredAddress).HasMaxLength(64);
            entity.Property(e => e.RemittanceAccount).HasMaxLength(32);
            entity.Property(e => e.TaxInvoiceAddress).HasMaxLength(64);
            entity.Property(e => e.TaxInvoiceNumber).HasMaxLength(8);
            entity.Property(e => e.TaxSerialNumber).HasMaxLength(9);
        });

        modelBuilder.Entity<t_1100Department>(entity =>
        {
            entity.Property(e => e.Id).HasMaxLength(16);
            entity.Property(e => e.Name).HasMaxLength(32);
        });

        modelBuilder.Entity<t_1101DepartmentUnit>(entity =>
        {
            entity.HasIndex(e => e.DepartmentId, "IX_t_1101DepartmentUnit_DepartmentId");

            entity.HasIndex(e => e.StaffId, "IX_t_1101DepartmentUnit_StaffId");

            entity.Property(e => e.DepartmentId).HasMaxLength(16);
            entity.Property(e => e.JobTitle).HasMaxLength(16);

            entity.HasOne(d => d.Department).WithMany(p => p.t_1101DepartmentUnit).HasForeignKey(d => d.DepartmentId);

            entity.HasOne(d => d.Staff).WithMany(p => p.t_1101DepartmentUnit).HasForeignKey(d => d.StaffId);
        });

        modelBuilder.Entity<t_1200PettyCash>(entity =>
        {
            entity.Property(e => e.Id)
                .HasMaxLength(32)
                .HasDefaultValue("");
            entity.Property(e => e.Authorizator).HasMaxLength(32);
            entity.Property(e => e.Company).HasMaxLength(64);
            entity.Property(e => e.Filler)
                .HasMaxLength(32)
                .HasDefaultValue("");
            entity.Property(e => e.Payee)
                .HasMaxLength(32)
                .HasDefaultValue("");
            entity.Property(e => e.Reason).HasMaxLength(64);
            entity.Property(e => e.Supervisor).HasMaxLength(32);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(12, 2)");
        });

        modelBuilder.Entity<t_1201PettyCashDetail>(entity =>
        {
            entity.HasIndex(e => e.PettyCashId, "IX_t_1201PettyCashDetail_PettyCashId");

            entity.Property(e => e.Amount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.PettyCashId)
                .HasMaxLength(32)
                .HasDefaultValue("");
            entity.Property(e => e.Tax).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.PettyCash).WithMany(p => p.t_1201PettyCashDetail).HasForeignKey(d => d.PettyCashId);
        });

        modelBuilder.Entity<t_1700LoginLog>(entity =>
        {
            entity.Property(e => e.IpAddress).HasDefaultValue("");
        });

        modelBuilder.Entity<t_1710ActionInfo>(entity =>
        {
            entity.Property(e => e.Account)
                .HasMaxLength(32)
                .HasDefaultValue("");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(64)
                .HasDefaultValue("");
            entity.Property(e => e.KeyId).HasMaxLength(16);
            entity.Property(e => e.Location).HasMaxLength(512);
            entity.Property(e => e.Memo).HasMaxLength(512);
        });

        modelBuilder.Entity<t_2000Customer>(entity =>
        {
            entity.Property(e => e.Advance).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.AttribName).HasMaxLength(32);
            entity.Property(e => e.BankName).HasMaxLength(32);
            entity.Property(e => e.CheckingAccount).HasMaxLength(32);
            entity.Property(e => e.ContactPhone).HasMaxLength(32);
            entity.Property(e => e.CreditBalance).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.CreditLine).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.DeliveryAddress).HasMaxLength(64);
            entity.Property(e => e.FaxPhone).HasMaxLength(32);
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasDefaultValue("");
            entity.Property(e => e.Owner)
                .HasMaxLength(32)
                .HasDefaultValue("");
            entity.Property(e => e.PayDays).HasMaxLength(8);
            entity.Property(e => e.RegisteredAddress)
                .HasMaxLength(64)
                .HasDefaultValue("");
            entity.Property(e => e.RemittanceAccount).HasMaxLength(32);
            entity.Property(e => e.StaffChineseName).HasMaxLength(32);
            entity.Property(e => e.TaxInvoiceAddress).HasMaxLength(64);
            entity.Property(e => e.TaxInvoiceNumber).HasMaxLength(8);
        });

        modelBuilder.Entity<t_2010Custemploy>(entity =>
        {
            entity.HasIndex(e => e.CustomerId, "IX_t_2010Custemploy_CustomerId");

            entity.Property(e => e.Account).HasMaxLength(32);
            entity.Property(e => e.Department).HasMaxLength(32);
            entity.Property(e => e.Email).HasMaxLength(64);
            entity.Property(e => e.ExtNum).HasMaxLength(32);
            entity.Property(e => e.Job).HasMaxLength(32);
            entity.Property(e => e.JobTitle).HasMaxLength(32);
            entity.Property(e => e.MobilePhone).HasMaxLength(32);
            entity.Property(e => e.Momo).HasMaxLength(256);
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasDefaultValue("");

            entity.HasOne(d => d.Customer).WithMany(p => p.t_2010Custemploy).HasForeignKey(d => d.CustomerId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
