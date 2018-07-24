namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateWebsitePages : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AgentSheets", "AgentId", "dbo.Agents");
            DropForeignKey("dbo.AgentSheets", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AgentSheetRecords", "AgentSheetId", "dbo.AgentSheets");
            DropForeignKey("dbo.AgentSheetRecords", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Candidates", "AgentId", "dbo.Agents");
            DropForeignKey("dbo.Candidates", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Agents", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Agents", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Agents", new[] { "UserId" });
            DropIndex("dbo.Agents", new[] { "OwnerId" });
            DropIndex("dbo.AgentSheets", new[] { "AgentId" });
            DropIndex("dbo.AgentSheets", new[] { "OwnerId" });
            DropIndex("dbo.AgentSheetRecords", new[] { "AgentSheetId" });
            DropIndex("dbo.AgentSheetRecords", new[] { "OwnerId" });
            DropIndex("dbo.Candidates", new[] { "AgentId" });
            DropIndex("dbo.Candidates", new[] { "OwnerId" });
            CreateTable(
                "dbo.WebSitePages",
                c => new
                    {
                        PageId = c.Int(nullable: false, identity: true),
                        PageName = c.Byte(nullable: false),
                        TitleEn = c.String(maxLength: 300),
                        TitleAr = c.String(maxLength: 300),
                        DescriptionEn = c.String(maxLength: 8000, unicode: false),
                        DescriptionAr = c.String(maxLength: 8000, unicode: false),
                        Slug = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.PageId);
            
           
          
            DropTable("dbo.AgentSheetRecords");
            DropTable("dbo.Candidates");
            DropTable("dbo.AgentSheets");
            DropTable("dbo.Agents");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Candidates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        Labor_VisaNumber = c.String(),
                        DateOfBirth = c.DateTime(),
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
                        Crm_CandidateId = c.Int(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        LastModifiedOn = c.DateTime(),
                        LastModifiedBy = c.String(),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.String(),
                        OwnerId = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActivated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AgentSheetRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentSheetId = c.Int(nullable: false),
                        Labor_VisaNumber = c.String(),
                        DateOfBirth = c.DateTime(),
                        Profession = c.String(),
                        Country = c.String(),
                        PassportNumber = c.String(),
                        IssueDate = c.DateTime(),
                        BlockVisaNo = c.String(),
                        AuthorizationNumber = c.String(),
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
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        LastModifiedOn = c.DateTime(),
                        LastModifiedBy = c.String(),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.String(),
                        OwnerId = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActivated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AgentSheets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.Int(nullable: false),
                        Code = c.String(),
                        IsSubmitted = c.Boolean(),
                        SubmitDate = c.DateTime(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        LastModifiedOn = c.DateTime(),
                        LastModifiedBy = c.String(),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.String(),
                        OwnerId = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActivated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Agents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Code = c.String(),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        LastModifiedOn = c.DateTime(),
                        LastModifiedBy = c.String(),
                        DeletedOn = c.DateTime(),
                        DeletedBy = c.String(),
                        OwnerId = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        IsActivated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.WebSitePages");
            CreateIndex("dbo.Candidates", "OwnerId");
            CreateIndex("dbo.Candidates", "AgentId");
            CreateIndex("dbo.AgentSheetRecords", "OwnerId");
            CreateIndex("dbo.AgentSheetRecords", "AgentSheetId");
            CreateIndex("dbo.AgentSheets", "OwnerId");
            CreateIndex("dbo.AgentSheets", "AgentId");
            CreateIndex("dbo.Agents", "OwnerId");
            CreateIndex("dbo.Agents", "UserId");
            AddForeignKey("dbo.Agents", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Agents", "OwnerId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Candidates", "OwnerId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Candidates", "AgentId", "dbo.Agents", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AgentSheetRecords", "OwnerId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AgentSheetRecords", "AgentSheetId", "dbo.AgentSheets", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AgentSheets", "OwnerId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AgentSheets", "AgentId", "dbo.Agents", "Id", cascadeDelete: true);
        }
    }
}
