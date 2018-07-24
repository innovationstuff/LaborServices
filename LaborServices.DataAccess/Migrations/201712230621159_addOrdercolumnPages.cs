namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addOrdercolumnPages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationPages", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationPages", "Order");
        }
    }
}
