namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeIdentity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AgentSheet", "AgentId", "dbo.Agent");
            DropForeignKey("dbo.AgentSheetRecord", "AgentSheetId", "dbo.AgentSheet");
            DropIndex("dbo.AgentSheet", new[] { "AgentId" });
            DropIndex("dbo.AgentSheetRecord", new[] { "AgentSheetId" });
            DropPrimaryKey("dbo.Agent");
            DropPrimaryKey("dbo.AgentSheet");
            DropPrimaryKey("dbo.AgentSheetRecord");
            AlterColumn("dbo.Agent", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.AgentSheet", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.AgentSheet", "AgentId", c => c.Int(nullable: false));
            AlterColumn("dbo.AgentSheetRecord", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.AgentSheetRecord", "AgentSheetId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Agent", "Id");
            AddPrimaryKey("dbo.AgentSheet", "Id");
            AddPrimaryKey("dbo.AgentSheetRecord", "Id");
            CreateIndex("dbo.AgentSheet", "AgentId");
            CreateIndex("dbo.AgentSheetRecord", "AgentSheetId");
            AddForeignKey("dbo.AgentSheet", "AgentId", "dbo.Agent", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AgentSheetRecord", "AgentSheetId", "dbo.AgentSheet", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AgentSheetRecord", "AgentSheetId", "dbo.AgentSheet");
            DropForeignKey("dbo.AgentSheet", "AgentId", "dbo.Agent");
            DropIndex("dbo.AgentSheetRecord", new[] { "AgentSheetId" });
            DropIndex("dbo.AgentSheet", new[] { "AgentId" });
            DropPrimaryKey("dbo.AgentSheetRecord");
            DropPrimaryKey("dbo.AgentSheet");
            DropPrimaryKey("dbo.Agent");
            AlterColumn("dbo.AgentSheetRecord", "AgentSheetId", c => c.String(maxLength: 128));
            AlterColumn("dbo.AgentSheetRecord", "Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.AgentSheet", "AgentId", c => c.String(maxLength: 128));
            AlterColumn("dbo.AgentSheet", "Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Agent", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.AgentSheetRecord", "Id");
            AddPrimaryKey("dbo.AgentSheet", "Id");
            AddPrimaryKey("dbo.Agent", "Id");
            CreateIndex("dbo.AgentSheetRecord", "AgentSheetId");
            CreateIndex("dbo.AgentSheet", "AgentId");
            AddForeignKey("dbo.AgentSheetRecord", "AgentSheetId", "dbo.AgentSheet", "Id");
            AddForeignKey("dbo.AgentSheet", "AgentId", "dbo.Agent", "Id");
        }
    }
}
