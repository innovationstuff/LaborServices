namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createManay2ManyMenusRelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationPages", "ParentPageId", "dbo.ApplicationPages");
            DropIndex("dbo.ApplicationPages", new[] { "ParentPageId" });
            CreateTable(
                "dbo.ApplicationPageParents",
                c => new
                    {
                        ApplicationPage_ApplicationPageId = c.Long(nullable: false),
                        ApplicationPage_ApplicationPageId1 = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationPage_ApplicationPageId, t.ApplicationPage_ApplicationPageId1 })
                .ForeignKey("dbo.ApplicationPages", t => t.ApplicationPage_ApplicationPageId)
                .ForeignKey("dbo.ApplicationPages", t => t.ApplicationPage_ApplicationPageId1)
                .Index(t => t.ApplicationPage_ApplicationPageId)
                .Index(t => t.ApplicationPage_ApplicationPageId1);
            
            DropColumn("dbo.ApplicationPages", "ParentPageId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ApplicationPages", "ParentPageId", c => c.Long());
            DropForeignKey("dbo.ApplicationPageParents", "ApplicationPage_ApplicationPageId1", "dbo.ApplicationPages");
            DropForeignKey("dbo.ApplicationPageParents", "ApplicationPage_ApplicationPageId", "dbo.ApplicationPages");
            DropIndex("dbo.ApplicationPageParents", new[] { "ApplicationPage_ApplicationPageId1" });
            DropIndex("dbo.ApplicationPageParents", new[] { "ApplicationPage_ApplicationPageId" });
            DropTable("dbo.ApplicationPageParents");
            CreateIndex("dbo.ApplicationPages", "ParentPageId");
            AddForeignKey("dbo.ApplicationPages", "ParentPageId", "dbo.ApplicationPages", "ApplicationPageId");
        }
    }
}
