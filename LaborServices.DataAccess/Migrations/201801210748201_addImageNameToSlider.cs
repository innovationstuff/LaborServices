namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addImageNameToSlider : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sliders", "ImageName", c => c.String(maxLength: 255, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sliders", "ImageName");
        }
    }
}
