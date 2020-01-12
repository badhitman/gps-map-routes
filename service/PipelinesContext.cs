////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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

            modelBuilder.Entity<SensorModel>().Property(p => p.OrderIndex).HasColumnOrder(0);
            //modelBuilder.Entity<SensorModel>().Property(p => p.Id).HasColumnOrder(1);

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
                new SensorModel(55.581095, 37.473795, 1){ Distance = 0, OrderIndex = 1, Information = "Примечание датчик 1" },
                new SensorModel(55.580567, 37.478157, 1){ Distance = 280, OrderIndex = 2, Information = "Примечание датчик 2" },
                new SensorModel(55.580453, 37.481983, 1){ Distance = 521, OrderIndex = 3, Information = "Примечание датчик 3" },
                new SensorModel(55.579828, 37.484966, 1){ Distance = 721, OrderIndex = 4, Information = "Примечание датчик 4" },
                new SensorModel(55.578888, 37.487670, 1){ Distance = 920, OrderIndex = 5, Information = "Примечание датчик 5" },
                new SensorModel(55.576893, 37.488314, 1){ Distance = 1145, OrderIndex = 6, Information = "Примечание датчик 6" },
                new SensorModel(55.576026, 37.491125, 1){ Distance = 1346, OrderIndex = 7, Information = "Примечание датчик 7" },
                new SensorModel(55.575535, 37.496286, 1){ Distance = 1675, OrderIndex = 8, Information = "Примечание датчик 8" },
                new SensorModel(55.574892, 37.499612, 1){ Distance = 1896, OrderIndex = 9, Information = "Примечание датчик 9" },
                new SensorModel(55.574364, 37.504333, 1){ Distance = 2198, OrderIndex = 10, Information = "Примечание датчик 10" },
                new SensorModel(55.575092, 37.508388, 1){ Distance = 2465, OrderIndex = 11, Information = "Примечание датчик 11" },
                new SensorModel(55.574806, 37.519005, 1){ Distance = 3133, OrderIndex = 12, Information = "Примечание датчик 12" },
                new SensorModel(55.573308, 37.519402, 1){ Distance = 3301, OrderIndex = 13, Information = "Примечание датчик 13" },
                new SensorModel(55.571707, 37.519574, 1){ Distance = 3479, OrderIndex = 14, Information = "Примечание датчик 14" },

                new SensorModel(60.7256304245563, 76.4044198218941, 2){ Distance = 0, OrderIndex = 1, Information = "Примечание датчик 1" },
                new SensorModel(60.7323558270627, 76.3302621067968, 2){ Distance = 4104, OrderIndex = 2, Information = "Примечание датчик 2" },
                new SensorModel(60.7404244451321, 76.245118063537, 2){ Distance = 8822, OrderIndex = 3, Information = "Примечание датчик 3" },
                new SensorModel(60.7189036089972, 76.1654671843584, 2){ Distance = 13773, OrderIndex = 4, Information = "Примечание датчик 4" },
                new SensorModel(60.7444579913416, 76.107788961505, 2){ Distance = 18008, OrderIndex = 5, Information = "Примечание датчик 5" },
                new SensorModel(60.7565555792057, 76.0308846643671, 2){ Distance = 22401, OrderIndex = 6, Information = "Примечание датчик 6" },
                new SensorModel(60.784765493083, 75.9732064415137, 2){ Distance = 26837, OrderIndex = 7, Information = "Примечание датчик 7" },
                new SensorModel(60.8196575659647, 75.9292611288634, 2){ Distance = 31394, OrderIndex = 8, Information = "Примечание датчик 8" },
                new SensorModel(60.842450994841, 75.8413705035629, 2){ Distance = 36794, OrderIndex = 9, Information = "Примечание датчик 9" },
                new SensorModel(60.8612098195227, 75.7315072219374, 2){ Distance = 43104, OrderIndex = 10, Information = "Примечание датчик 10" },
                new SensorModel(60.8933421094188, 75.6793221631652, 2){ Distance = 47662, OrderIndex = 11, Information = "Примечание датчик 11" },
                new SensorModel(60.9200942380996, 75.6408700145962, 2){ Distance = 51294, OrderIndex = 12, Information = "Примечание датчик 12" },
                new SensorModel(60.9374710484712, 75.5804452097022, 2){ Distance = 55090, OrderIndex = 13, Information = "Примечание датчик 13" },
                new SensorModel(60.9668563000849, 75.5310067329706, 2){ Distance = 59312, OrderIndex = 14, Information = "Примечание датчик 14" },
                new SensorModel(60.9868760957874, 75.498047748483, 2){ Distance = 62163, OrderIndex = 15, Information = "Примечание датчик 15" },
                new SensorModel(61.0068832467939, 75.4705819280766, 2){ Distance = 64837, OrderIndex = 16, Information = "Примечание датчик 16" }
            });
            db.SaveChanges();
        }
    }
}
