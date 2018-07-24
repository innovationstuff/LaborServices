namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class NewTbl_Candidate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Candidate",
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
                    Name = c.String(),
                    CreatedOn = c.DateTime(defaultValueSql:"getDate()"),
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
                .ForeignKey("dbo.Agent", t => t.AgentId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .Index(t => t.AgentId)
                .Index(t => t.OwnerId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Candidate", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Candidate", "AgentId", "dbo.Agent");
            DropIndex("dbo.Candidate", new[] { "OwnerId" });
            DropIndex("dbo.Candidate", new[] { "AgentId" });
            DropTable("dbo.Candidate");
        }
    }
}
