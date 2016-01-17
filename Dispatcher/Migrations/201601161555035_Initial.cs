namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceProviders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DistpachRequesters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DispatchRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        CompletionDate = c.DateTime(nullable: false),
                        RequesterId = c.Int(nullable: false),
                        ProviderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServiceProviders", t => t.ProviderId, cascadeDelete: true)
                .ForeignKey("dbo.DistpachRequesters", t => t.RequesterId, cascadeDelete: true)
                .Index(t => t.RequesterId)
                .Index(t => t.ProviderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DispatchRequests", "RequesterId", "dbo.DistpachRequesters");
            DropForeignKey("dbo.DispatchRequests", "ProviderId", "dbo.ServiceProviders");
            DropIndex("dbo.DispatchRequests", new[] { "ProviderId" });
            DropIndex("dbo.DispatchRequests", new[] { "RequesterId" });
            DropTable("dbo.DispatchRequests");
            DropTable("dbo.DistpachRequesters");
            DropTable("dbo.ServiceProviders");
        }
    }
}
