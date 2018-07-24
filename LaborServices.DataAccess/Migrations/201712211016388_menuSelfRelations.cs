namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menuSelfRelations : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationRolePages", "ApplicationPageId", "dbo.ApplicationPages");
            DropForeignKey("dbo.ApplicationPages", "ApplicationRole_Id", "dbo.AspNetRoles");
            DropIndex("dbo.ApplicationPages", new[] { "ApplicationRole_Id" });
            DropIndex("dbo.ApplicationRolePages", new[] { "ApplicationPageId" });
            CreateTable(
                "dbo.ApplicationRolePage",
                c => new
                    {
                        ApplicationPageId = c.Long(nullable: false),
                        ApplicationRoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ApplicationPageId, t.ApplicationRoleId })
                .ForeignKey("dbo.ApplicationPages", t => t.ApplicationPageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.ApplicationRoleId, cascadeDelete: true)
                .Index(t => t.ApplicationPageId)
                .Index(t => t.ApplicationRoleId);
            
            AlterColumn("dbo.ApplicationPages", "ParentPageId", c => c.Long());
            CreateIndex("dbo.ApplicationPages", "ParentPageId");
            AddForeignKey("dbo.ApplicationPages", "ParentPageId", "dbo.ApplicationPages", "ApplicationPageId");
            DropColumn("dbo.ApplicationPages", "ApplicationRole_Id");
            DropTable("dbo.ApplicationRolePages");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ApplicationRolePages",
                c => new
                    {
                        ApplicationPageId = c.Long(nullable: false),
                        ApplicationRoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ApplicationPageId, t.ApplicationRoleId });
            
            AddColumn("dbo.ApplicationPages", "ApplicationRole_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ApplicationPages", "ParentPageId", "dbo.ApplicationPages");
            DropForeignKey("dbo.ApplicationRolePage", "ApplicationRoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ApplicationRolePage", "ApplicationPageId", "dbo.ApplicationPages");
            DropIndex("dbo.ApplicationRolePage", new[] { "ApplicationRoleId" });
            DropIndex("dbo.ApplicationRolePage", new[] { "ApplicationPageId" });
            DropIndex("dbo.ApplicationPages", new[] { "ParentPageId" });
            AlterColumn("dbo.ApplicationPages", "ParentPageId", c => c.Long(nullable: false));
            DropTable("dbo.ApplicationRolePage");
            CreateIndex("dbo.ApplicationRolePages", "ApplicationPageId");
            CreateIndex("dbo.ApplicationPages", "ApplicationRole_Id");
            AddForeignKey("dbo.ApplicationPages", "ApplicationRole_Id", "dbo.AspNetRoles", "Id");
            AddForeignKey("dbo.ApplicationRolePages", "ApplicationPageId", "dbo.ApplicationPages", "ApplicationPageId", cascadeDelete: true);
        }
    }
}
