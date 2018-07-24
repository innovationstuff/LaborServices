namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAgent_AgentSheet_AgentSheetRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agent",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(maxLength: 128),
                        Code = c.String(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(nullable: false, defaultValueSql:"getDate()" ),
                        CreatedBy = c.String(),
                        LastModifiedOn = c.DateTime(),
                        LastModifiedBy = c.String(),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.String(),
                        OwnerId = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                        IsActivated = c.Boolean(nullable: false, defaultValue: true),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.AgentSheet",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AgentId = c.String(maxLength: 128),
                        Code = c.String(),
                        IsSubmitted = c.Boolean(),
                        SubmitDate = c.DateTime(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(nullable: false, defaultValueSql: "getDate()"),
                        CreatedBy = c.String(),
                        LastModifiedOn = c.DateTime(),
                        LastModifiedBy = c.String(),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.String(),
                        OwnerId = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                        IsActivated = c.Boolean(nullable: false, defaultValue: true),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Agent", t => t.AgentId)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .Index(t => t.AgentId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.AgentSheetRecord",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AgentSheetId = c.String(maxLength: 128),
                        Labor_VisaNumber = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        Profession = c.String(),
                        Country = c.String(),
                        PassportNumber = c.String(),
                        IssueDate = c.DateTime(),
                        BlockVisaNo = c.String(),
                        Authorization = c.String(),
                        ProjectCode = c.String(),
                        ProjectName = c.String(),
                        FlightNumber = c.String(),
                        AirCompany = c.String(),
                        DepartureTime = c.String(),
                        ArrivalDate = c.DateTime(),
                        ArrivalTime = c.String(),
                        ArrivalPlace = c.String(),
                        BasicSalary = c.Decimal(precision: 18, scale: 2),
                        Foodallowance = c.Decimal(precision: 18, scale: 2),
                        HousingAllowance = c.Decimal(precision: 18, scale: 2),
                        OtherAllowance = c.Decimal(precision: 18, scale: 2),
                        TransportationAllowance = c.Decimal(precision: 18, scale: 2),
                        ExpectedArrivalDate = c.DateTime(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(nullable: false, defaultValueSql: "getDate()"),
                        CreatedBy = c.String(),
                        LastModifiedOn = c.DateTime(),
                        LastModifiedBy = c.String(),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.String(),
                        OwnerId = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false, defaultValue: false),
                        IsActivated = c.Boolean(nullable: false, defaultValue: true),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AgentSheet", t => t.AgentSheetId)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .Index(t => t.AgentSheetId)
                .Index(t => t.OwnerId);
            
            AddColumn("dbo.AspNetUsers", "Name", c => c.String());
            AddColumn("dbo.AspNetUsers", "CreatedOn", c => c.DateTime(nullable: false, defaultValueSql: "getDate()"));
            AddColumn("dbo.AspNetUsers", "CreatedBy", c => c.String());
            AddColumn("dbo.AspNetUsers", "LastModifiedOn", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "LastModifiedBy", c => c.String());
            AddColumn("dbo.AspNetUsers", "DeletedOn", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "DeletedBy", c => c.String());
            AddColumn("dbo.AspNetUsers", "OwnerId", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "IsDeleted", c => c.Boolean(nullable: false, defaultValue:false));
            AddColumn("dbo.AspNetUsers", "IsActivated", c => c.Boolean(nullable: false,defaultValue:true));
            CreateIndex("dbo.AspNetUsers", "OwnerId");
            AddForeignKey("dbo.AspNetUsers", "OwnerId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Agent", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Agent", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AgentSheetRecord", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AgentSheetRecord", "AgentSheetId", "dbo.AgentSheet");
            DropForeignKey("dbo.AgentSheet", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AgentSheet", "AgentId", "dbo.Agent");
            DropIndex("dbo.AgentSheetRecord", new[] { "OwnerId" });
            DropIndex("dbo.AgentSheetRecord", new[] { "AgentSheetId" });
            DropIndex("dbo.AspNetUsers", new[] { "OwnerId" });
            DropIndex("dbo.AgentSheet", new[] { "OwnerId" });
            DropIndex("dbo.AgentSheet", new[] { "AgentId" });
            DropIndex("dbo.Agent", new[] { "OwnerId" });
            DropIndex("dbo.Agent", new[] { "UserId" });
            DropColumn("dbo.AspNetUsers", "IsActivated");
            DropColumn("dbo.AspNetUsers", "IsDeleted");
            DropColumn("dbo.AspNetUsers", "OwnerId");
            DropColumn("dbo.AspNetUsers", "DeletedBy");
            DropColumn("dbo.AspNetUsers", "DeletedOn");
            DropColumn("dbo.AspNetUsers", "LastModifiedBy");
            DropColumn("dbo.AspNetUsers", "LastModifiedOn");
            DropColumn("dbo.AspNetUsers", "CreatedBy");
            DropColumn("dbo.AspNetUsers", "CreatedOn");
            DropColumn("dbo.AspNetUsers", "Name");
            DropTable("dbo.AgentSheetRecord");
            DropTable("dbo.AgentSheet");
            DropTable("dbo.Agent");
        }
    }
}
