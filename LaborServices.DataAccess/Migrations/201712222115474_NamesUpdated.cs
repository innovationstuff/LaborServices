namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NamesUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationPages", "NamesUpdated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationPages", "NamesUpdated");
        }
    }
}
