namespace LaborServices.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewCol_Candidate_CrmCandidateId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Candidate", "Crm_CandidateId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Candidate", "Crm_CandidateId");
        }
    }
}
