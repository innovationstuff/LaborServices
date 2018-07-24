using LaborServices.Entity.Identity;
using LaborServices.Model;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using LaborServices.Model.Identity;
using LaborServices.Models;

namespace LaborServices.Entity
{
    public class LaborServicesDbContext : IdentityDbContext<ApplicationUser, ApplicationRole,
        string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public LaborServicesDbContext()
            : base("LaborServicesDb")
        {
        }

        static LaborServicesDbContext()
        {
            Database.SetInitializer<LaborServicesDbContext>(null);
            //Database.SetInitializer<LaborServicesDbContext>(new LaborServicesDbInitializer());
        }

        public static LaborServicesDbContext Create()
        {
            return new LaborServicesDbContext();
        }

        #region Entity Sets
        public IDbSet<ApplicationGroup> ApplicationGroups { get; set; }
        public IDbSet<ApplicationPage> ApplicationPages { get; set; }
        public IDbSet<Slider> Sliders { get; set; }
        public IDbSet<WebSitePage> WebSitePages { get; set; }
        public IDbSet<Setting> Settings { get; set; }
		public IDbSet<About> Abouts { get; set; }
		public IDbSet<PaymentTransaction> PaymentTransactions { get; set; }
		public IDbSet<ReceiptVoucher> ReceiptVouchers { get; set; }
        public IDbSet<Team> Teams { get; set; }
		public IDbSet<Branche> Branches { get; set; }
		#endregion

		public virtual void Commit()
        {
            base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Make sure to call the base method first:
            base.OnModelCreating(modelBuilder);

            // Map Users to Groups:
            modelBuilder.Entity<ApplicationGroup>()
                .HasMany<ApplicationUserGroup>((ApplicationGroup g) => g.ApplicationUsers)
                .WithRequired()
                .HasForeignKey<string>((ApplicationUserGroup ag) => ag.ApplicationGroupId);

            modelBuilder.Entity<ApplicationUserGroup>()
                .HasKey((ApplicationUserGroup r) =>
                    new
                    {
                        ApplicationUserId = r.ApplicationUserId,
                        ApplicationGroupId = r.ApplicationGroupId
                    }).ToTable("ApplicationUserGroups");

            // Map Roles to Groups:
            modelBuilder.Entity<ApplicationGroup>()
                .HasMany<ApplicationGroupRole>((ApplicationGroup g) => g.ApplicationRoles)
                .WithRequired()
                .HasForeignKey<string>((ApplicationGroupRole ap) => ap.ApplicationGroupId);


            modelBuilder.Entity<ApplicationGroupRole>()
                .HasKey((ApplicationGroupRole gr) =>
                new
                {
                    ApplicationRoleId = gr.ApplicationRoleId,
                    ApplicationGroupId = gr.ApplicationGroupId
                }).ToTable("ApplicationGroupRoles");


            modelBuilder.Entity<ApplicationPage>()
                .HasMany<ApplicationRole>(s => s.ApplicationRoles)
                .WithMany(c => c.ApplicationPages)
                .Map(cs =>
                {
                    cs.MapLeftKey("ApplicationPageId");
                    cs.MapRightKey("ApplicationRoleId");
                    cs.ToTable("ApplicationRolePage");
                });

            modelBuilder.Entity<ApplicationPage>()
                .HasMany(m => m.ParentPages)
                .WithMany(m => m.ChildernPages)
                .Map(m =>
                {
                    m.ToTable("ApplicationPageParents");
                });
        }

		object placeHolderVariable;
	}


}
