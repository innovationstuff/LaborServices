namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameCol_AgentSheetRecord_AuthorizationNumber : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.AgentSheetRecord", "Authorization", "AuthorizationNumber");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.AgentSheetRecord", "AuthorizationNumber", "Authorization");
        }
    }
}
