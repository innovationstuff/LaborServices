namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcrmUserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CrmUserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "CrmUserId");
        }
    }
}
