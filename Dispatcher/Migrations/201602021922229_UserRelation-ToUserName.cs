namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserRelationToUserName : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DispatchRequests", "ProvidingUserId", "dbo.AspNetUsers");
            DropIndex("dbo.DispatchRequests", new[] { "ProvidingUserId" });
            AddColumn("dbo.DispatchRequests", "ProvidingUserName", c => c.String());
            DropColumn("dbo.DispatchRequests", "ProvidingUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DispatchRequests", "ProvidingUserId", c => c.String(maxLength: 128));
            DropColumn("dbo.DispatchRequests", "ProvidingUserName");
            CreateIndex("dbo.DispatchRequests", "ProvidingUserId");
            AddForeignKey("dbo.DispatchRequests", "ProvidingUserId", "dbo.AspNetUsers", "Id");
        }
    }
}
