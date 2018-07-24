namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddSettingTblIsEditableColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Settings", "IsEditable", c => c.Boolean(nullable: false, defaultValue: true));
        }

        public override void Down()
        {
            DropColumn("dbo.Settings", "IsEditable");
        }
    }
}
