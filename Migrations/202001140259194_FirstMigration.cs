namespace GpsMapRoutes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PipelineModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderIndex = c.Int(nullable: false),
                        Information = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SensorModels",
                c => new
                    {
                        OrderIndex = c.Int(nullable: false),
                        Id = c.Int(nullable: false, identity: true),
                        PipelineId = c.Int(nullable: false),
                        Lat = c.Double(nullable: false),
                        Lng = c.Double(nullable: false),
                        Distance = c.Double(nullable: false),
                        Information = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PipelineModels", t => t.PipelineId, cascadeDelete: true)
                .Index(t => t.PipelineId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SensorModels", "PipelineId", "dbo.PipelineModels");
            DropIndex("dbo.SensorModels", new[] { "PipelineId" });
            DropTable("dbo.SensorModels");
            DropTable("dbo.PipelineModels");
        }
    }
}
