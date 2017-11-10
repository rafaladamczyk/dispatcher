namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class foreignUserKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DispatchRequests", "CreatorId", c => c.String(maxLength: 128));
            AddColumn("dbo.DispatchRequests", "ProviderId", c => c.String(maxLength: 128));
            CreateIndex("dbo.DispatchRequests", "CreatorId");
            CreateIndex("dbo.DispatchRequests", "ProviderId");
            AddForeignKey("dbo.DispatchRequests", "CreatorId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.DispatchRequests", "ProviderId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DispatchRequests", "ProviderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DispatchRequests", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.DispatchRequests", new[] { "ProviderId" });
            DropIndex("dbo.DispatchRequests", new[] { "CreatorId" });
            DropColumn("dbo.DispatchRequests", "ProviderId");
            DropColumn("dbo.DispatchRequests", "CreatorId");
        }
    }
}
