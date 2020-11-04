namespace PeopleDirectory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addMigrationInitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        CityName = c.String(nullable: false),
                        CountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CityId)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryId = c.Int(nullable: false, identity: true),
                        CountryName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "dbo.PersonDirectoryModels",
                c => new
                    {
                        ClientId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        SurName = c.String(nullable: false),
                        MobileNo = c.String(nullable: false),
                        Gender = c.String(nullable: false),
                        EmailAddress = c.String(nullable: false),
                        ProfilePic = c.Binary(nullable: false),
                        ClientCityId = c.Int(nullable: false),
                        ClientCountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClientId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cities", "CountryId", "dbo.Countries");
            DropIndex("dbo.Cities", new[] { "CountryId" });
            DropTable("dbo.Users");
            DropTable("dbo.PersonDirectoryModels");
            DropTable("dbo.Countries");
            DropTable("dbo.Cities");
        }
    }
}
