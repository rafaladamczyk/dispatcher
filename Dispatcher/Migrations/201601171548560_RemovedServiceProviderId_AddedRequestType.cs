namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedServiceProviderId_AddedRequestType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DispatchRequests", "ProviderId", "dbo.ServiceProviders");
            DropIndex("dbo.DispatchRequests", new[] { "ProviderId" });
            RenameColumn(table: "dbo.DispatchRequests", name: "ProviderId", newName: "ProviderName");
            DropPrimaryKey("dbo.ServiceProviders");
            AddColumn("dbo.DispatchRequests", "Type", c => c.Int(nullable: false));
            AlterColumn("dbo.ServiceProviders", "Name", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.DispatchRequests", "ProviderName", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.ServiceProviders", "Name");
            CreateIndex("dbo.DispatchRequests", "ProviderName");
            AddForeignKey("dbo.DispatchRequests", "ProviderName", "dbo.ServiceProviders", "Name");
            DropColumn("dbo.ServiceProviders", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ServiceProviders", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.DispatchRequests", "ProviderName", "dbo.ServiceProviders");
            DropIndex("dbo.DispatchRequests", new[] { "ProviderName" });
            DropPrimaryKey("dbo.ServiceProviders");
            AlterColumn("dbo.DispatchRequests", "ProviderName", c => c.Int(nullable: false));
            AlterColumn("dbo.ServiceProviders", "Name", c => c.String());
            DropColumn("dbo.DispatchRequests", "Type");
            AddPrimaryKey("dbo.ServiceProviders", "Id");
            RenameColumn(table: "dbo.DispatchRequests", name: "ProviderName", newName: "ProviderId");
            CreateIndex("dbo.DispatchRequests", "ProviderId");
            AddForeignKey("dbo.DispatchRequests", "ProviderId", "dbo.ServiceProviders", "Id", cascadeDelete: true);
        }
    }
}
