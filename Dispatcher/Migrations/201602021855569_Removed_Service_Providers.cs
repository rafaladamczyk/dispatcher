namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removed_Service_Providers : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.ServiceProviders");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ServiceProviders",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.UserName);
            
        }
    }
}
