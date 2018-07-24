namespace LaborServices.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class addActiveFlagePage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationPages", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationPages", "Active");
        }
    }
}
