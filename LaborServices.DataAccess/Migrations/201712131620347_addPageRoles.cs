namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPageRoles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationPages",
                c => new
                    {
                        ApplicationPageId = c.Long(nullable: false, identity: true),
                        ParentPageId = c.Long(nullable: false),
                        NameAr = c.String(),
                        NameEn = c.String(),
                        Controller = c.String(),
                        Action = c.String(),
                        Area = c.String(),
                        ImageClass = c.String(),
                        PageUrl = c.String(),
                        LinkTarget = c.String(),
                        IsBaseParent = c.Boolean(nullable: false),
                        NoteAr = c.String(),
                        NoteEn = c.String(),
                        ApplicationRole_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ApplicationPageId)
                .ForeignKey("dbo.AspNetRoles", t => t.ApplicationRole_Id)
                .Index(t => t.ApplicationRole_Id);
            
            CreateTable(
                "dbo.ApplicationRolePages",
                c => new
                    {
                        ApplicationPageId = c.Long(nullable: false),
                        ApplicationRoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ApplicationPageId, t.ApplicationRoleId })
                .ForeignKey("dbo.ApplicationPages", t => t.ApplicationPageId, cascadeDelete: true)
                .Index(t => t.ApplicationPageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationPages", "ApplicationRole_Id", "dbo.AspNetRoles");
            DropForeignKey("dbo.ApplicationRolePages", "ApplicationPageId", "dbo.ApplicationPages");
            DropIndex("dbo.ApplicationRolePages", new[] { "ApplicationPageId" });
            DropIndex("dbo.ApplicationPages", new[] { "ApplicationRole_Id" });
            DropTable("dbo.ApplicationRolePages");
            DropTable("dbo.ApplicationPages");
        }
    }
}
