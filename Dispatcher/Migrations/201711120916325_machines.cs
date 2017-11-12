namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class machines : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Requests", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.Requests", new[] { "CreatorId" });
            AlterColumn("dbo.Requests", "CreatorId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Requests", "CreatorId");
            AddForeignKey("dbo.Requests", "CreatorId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Requests", "CreatorId", "dbo.AspNetUsers");
            DropIndex("dbo.Requests", new[] { "CreatorId" });
            AlterColumn("dbo.Requests", "CreatorId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Requests", "CreatorId");
            AddForeignKey("dbo.Requests", "CreatorId", "dbo.AspNetUsers", "Id");
        }
    }
}
