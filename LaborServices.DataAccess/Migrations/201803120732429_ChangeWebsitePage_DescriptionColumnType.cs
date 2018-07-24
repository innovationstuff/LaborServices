namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeWebsitePage_DescriptionColumnType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WebSitePages", "DescriptionEn", c => c.String(maxLength: 4000));
            AlterColumn("dbo.WebSitePages", "DescriptionAr", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WebSitePages", "DescriptionAr", c => c.String(maxLength: 8000, unicode: false));
            AlterColumn("dbo.WebSitePages", "DescriptionEn", c => c.String(maxLength: 8000, unicode: false));
        }
    }
}
