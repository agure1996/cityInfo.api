using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
namespace CityInfo.API.DBContext
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Cities { get; set; }

        public DbSet<PointOfInterest> PointOfInterests { get; set; }
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
        : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("connectionstring");
        //    base.OnConfiguring(optionsBuilder);
        //}


    }
}
