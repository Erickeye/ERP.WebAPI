using Microsoft.EntityFrameworkCore;
using ERP.Models.AMS;
using ERP.Library.Enums.Login;

namespace ERP.Data
{
	public class AMSContext : DbContext
	{
		public AMSContext(DbContextOptions<AMSContext> options)
			: base(options)
		{
		}
		public DbSet<t_user> t_user { get; set; } = default!;
		public DbSet<t_role> t_role { get; set; } = default!;
        public DbSet<t_permission> t_permission { get; set; }  = default!;
		public DbSet<t_level> t_level { get; set; }  = default!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			modelBuilder.Entity<t_permission>().ToTable("t_permission").HasKey(p => new { p.f_roleId, p.f_pageId });
			modelBuilder.Entity<t_permission>()
				.Property(x => x.f_pageId)
				.HasColumnName("f_pageId")
				.HasConversion(
					v => (int)v,
					v => (PermissionType)v
				);			
		}
	}
}
