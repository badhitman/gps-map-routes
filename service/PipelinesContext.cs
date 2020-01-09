using System.Data.Entity;
using GpsMapRoutes.models;

namespace GpsMapRoutes.service
{
    public class PipelinesContext : DbContext
    {
        static PipelinesContext()
        {
            Database.SetInitializer(new PipelinesContextInitializer());
        }

        public PipelinesContext() : base("DefaultConnection")
        {

        }

        public DbSet<PipelineModel> Pipelines { get; set; }

        public DbSet<SensorModel> Sensors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PipelineModel>().
               HasMany(x => x.Sensors).
               WithRequired(x => x.Pipeline).
               HasForeignKey(x => x.PipelineId);

            modelBuilder.Entity<SensorModel>().Property(x => x.Lat).IsRequired();
            modelBuilder.Entity<SensorModel>().Property(x => x.Lng).IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }

    public class PipelinesContextInitializer : CreateDatabaseIfNotExists<PipelinesContext>
    {
        protected override void Seed(PipelinesContext db)
        {
            db.Pipelines.AddRange(new PipelineModel[]
            {
                new PipelineModel() { Id = 1, Information = "DEMO Трасса 1" },
                new PipelineModel() { Id = 2, Information = "DEMO Трасса 2" }
            });
            db.SaveChanges();
            db.Sensors.AddRange(new SensorModel[]
            {
                new SensorModel(55.581095, 37.473795, 1){ Information = "Локация 1", OrderIndex = 1 },
                new SensorModel(55.580567, 37.478157, 1){ Information = "Локация 2", OrderIndex = 2 },
                new SensorModel(55.580453, 37.481983, 1){ Information = "Локация 3", OrderIndex = 3 },
                new SensorModel(55.579828, 37.484966, 1){ Information = "Локация 4", OrderIndex = 4 },
                new SensorModel(55.578888, 37.487670, 1){ Information = "Локация 5", OrderIndex = 5 },
                new SensorModel(55.576893, 37.488314, 1){ Information = "Локация 6", OrderIndex = 6 },
                new SensorModel(55.576026, 37.491125, 1){ Information = "Локация 7", OrderIndex = 7 },
                new SensorModel(55.575535, 37.496286, 1){ Information = "Локация 8", OrderIndex = 8 },
                new SensorModel(55.574892, 37.499612, 1){ Information = "Локация 9", OrderIndex = 9 },
                new SensorModel(55.574364, 37.504333, 1){ Information = "Локация 10", OrderIndex = 10 },
                new SensorModel(55.575092, 37.508388, 1){ Information = "Локация 11", OrderIndex = 11 },
                new SensorModel(55.574806, 37.519005, 1){ Information = "Локация 12", OrderIndex = 12 },
                new SensorModel(55.573308, 37.519402, 1){ Information = "Локация 13", OrderIndex = 13 },
                new SensorModel(55.571707, 37.519574, 1){ Information = "Локация 14", OrderIndex = 14 },

                new SensorModel(60.7256304245563, 76.4044198218941, 2){ Information = "Локация 1", OrderIndex = 1 },
                new SensorModel(60.7323558270627, 76.3302621067968, 2){ Information = "Локация 2", OrderIndex = 2 },
                new SensorModel(60.7404244451321, 76.245118063537, 2){ Information = "Локация 3", OrderIndex = 3 },
                new SensorModel(60.7189036089972, 76.1654671843584, 2){ Information = "Локация 4", OrderIndex = 4 },
                new SensorModel(60.7444579913416, 76.107788961505, 2){ Information = "Локация 5", OrderIndex = 5 },
                new SensorModel(60.7565555792057, 76.0308846643671, 2){ Information = "Локация 6", OrderIndex = 6 },
                new SensorModel(60.78476549383, 75.9732064415137, 2){ Information = "Локация 7", OrderIndex = 7 },
                new SensorModel(60.8196575659647, 75.9292611288634, 2){ Information = "Локация 8", OrderIndex = 8 },
                new SensorModel(60.842450994841, 75.8413705035629, 2){ Information = "Локация 9", OrderIndex = 9 },
                new SensorModel(60.8612098195227, 75.7315072219374, 2){ Information = "Локация 10", OrderIndex = 10 },
                new SensorModel(60.8933421094188, 75.6793221631652, 2){ Information = "Локация 11", OrderIndex = 11 },
                new SensorModel(60.9200942380996, 75.6408700145962, 2){ Information = "Локация 12", OrderIndex = 12 },
                new SensorModel(60.9374710484712, 75.5804452097022, 2){ Information = "Локация 13", OrderIndex = 13 },
                new SensorModel(60.9668563000849, 75.5310067329706, 2){ Information = "Локация 14", OrderIndex = 14 },
                new SensorModel(60.9868760957874, 75.498047748483, 2){ Information = "Локация 15", OrderIndex = 15 },
                new SensorModel(61.0068832467939, 75.4705819280766, 2){ Information = "Локация 16", OrderIndex = 16 }
            });
            db.SaveChanges();
        }
    }
}
