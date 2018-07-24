namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addChildernRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationPageParents", "ApplicationPage_ApplicationPageId", "dbo.ApplicationPages");
            DropForeignKey("dbo.ApplicationPageParents", "ApplicationPage_ApplicationPageId1", "dbo.ApplicationPages");
            DropIndex("dbo.ApplicationPageParents", new[] { "ApplicationPage_ApplicationPageId" });
            DropIndex("dbo.ApplicationPageParents", new[] { "ApplicationPage_ApplicationPageId1" });
            AddColumn("dbo.ApplicationPages", "childern_childern_id", c => c.Long());
            AddColumn("dbo.ApplicationPages", "parent_parent_id", c => c.Long());
            CreateIndex("dbo.ApplicationPages", "childern_childern_id");
            CreateIndex("dbo.ApplicationPages", "parent_parent_id");
            AddForeignKey("dbo.ApplicationPages", "childern_childern_id", "dbo.ApplicationPages", "ApplicationPageId");
            AddForeignKey("dbo.ApplicationPages", "parent_parent_id", "dbo.ApplicationPages", "ApplicationPageId");
            DropTable("dbo.ApplicationPageParents");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ApplicationPageParents",
                c => new
                    {
                        ApplicationPage_ApplicationPageId = c.Long(nullable: false),
                        ApplicationPage_ApplicationPageId1 = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationPage_ApplicationPageId, t.ApplicationPage_ApplicationPageId1 });
            
            DropForeignKey("dbo.ApplicationPages", "parent_parent_id", "dbo.ApplicationPages");
            DropForeignKey("dbo.ApplicationPages", "childern_childern_id", "dbo.ApplicationPages");
            DropIndex("dbo.ApplicationPages", new[] { "parent_parent_id" });
            DropIndex("dbo.ApplicationPages", new[] { "childern_childern_id" });
            DropColumn("dbo.ApplicationPages", "parent_parent_id");
            DropColumn("dbo.ApplicationPages", "childern_childern_id");
            CreateIndex("dbo.ApplicationPageParents", "ApplicationPage_ApplicationPageId1");
            CreateIndex("dbo.ApplicationPageParents", "ApplicationPage_ApplicationPageId");
            AddForeignKey("dbo.ApplicationPageParents", "ApplicationPage_ApplicationPageId1", "dbo.ApplicationPages", "ApplicationPageId");
            AddForeignKey("dbo.ApplicationPageParents", "ApplicationPage_ApplicationPageId", "dbo.ApplicationPages", "ApplicationPageId");
        }
    }
}
