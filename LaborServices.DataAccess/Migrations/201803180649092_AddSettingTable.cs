namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSettingTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WebSitePages", "TitleEn", c => c.String(nullable: false, maxLength: 300));
            AlterColumn("dbo.WebSitePages", "TitleAr", c => c.String(nullable: false, maxLength: 300));
            AlterColumn("dbo.WebSitePages", "DescriptionEn", c => c.String());
            AlterColumn("dbo.WebSitePages", "DescriptionAr", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WebSitePages", "DescriptionAr", c => c.String(maxLength: 4000));
            AlterColumn("dbo.WebSitePages", "DescriptionEn", c => c.String(maxLength: 4000));
            AlterColumn("dbo.WebSitePages", "TitleAr", c => c.String(maxLength: 300));
            AlterColumn("dbo.WebSitePages", "TitleEn", c => c.String(maxLength: 300));
        }
    }
}
