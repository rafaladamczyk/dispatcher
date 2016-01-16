namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TypoFix : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DistpachRequesters", newName: "DispatchRequesters");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.DispatchRequesters", newName: "DistpachRequesters");
        }
    }
}
