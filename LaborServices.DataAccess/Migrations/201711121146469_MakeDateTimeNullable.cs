namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeDateTimeNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Agent", "CreatedOn", c => c.DateTime());
            AlterColumn("dbo.AgentSheet", "CreatedOn", c => c.DateTime());
            AlterColumn("dbo.AgentSheetRecord", "DateOfBirth", c => c.DateTime());
            AlterColumn("dbo.AgentSheetRecord", "CreatedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AgentSheetRecord", "CreatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AgentSheetRecord", "DateOfBirth", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AgentSheet", "CreatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Agent", "CreatedOn", c => c.DateTime(nullable: false));
        }
    }
}
