using CityInfo.API.Entities;
using CityInfo.API.Models;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasData(
                new City("New York City")
                {
                    Id = 1,
                    Description = "City with big park"
                },
                new City("Xamar City")
                {
                    Id = 2,
                    Description = "City with Liidho Beach"
                },
                new City("Paris")
                {
                    Id = 3,
                    Description = "City with Eiffel tower"
                },
                new City("Hargeisa")
                {
                    Id = 4,
                    Description = "North city"
                });


            modelBuilder.Entity<PointOfInterest>()
                .HasData(
                new PointOfInterest("Hazzerdous")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "Food Joint",
                },
                new PointOfInterest("Jumanji")
                {
                    Id = 2,
                    CityId = 1,
                    Description = "Game Park",
                },
                new PointOfInterest("Mosque of NY")
                {
                    Id = 3,
                    CityId = 1,
                    Description = "A prayer mosque",
                },
                new PointOfInterest("Xeebta Liido")
                {
                    Id = 4, // Changed to 4 to ensure uniqueness
                    CityId = 2,
                    Description = "Beach resort",
                },
                new PointOfInterest("Masaajidka Xamar")
                {
                    Id = 5, // Changed to 5 to ensure uniqueness
                    CityId = 2,
                    Description = "A prayer mosque",
                },
                new PointOfInterest("Benzema browns")
                {
                    Id = 6, // Changed to 6 to ensure uniqueness
                    CityId = 3,
                    Description = "Dessert parlor",
                },
                new PointOfInterest("Iniesta")
                {
                    Id = 7,
                    CityId = 3,
                    Description = "Footie Spot",
                },
                new PointOfInterest("Mosque of Paris")
                {
                    Id = 8, 
                    CityId = 3,
                    Description = "A prayer mosque",
                },
                new PointOfInterest("Haraar House")
                {
                    Id = 9, 
                    CityId = 4,
                    Description = "Social area for elders",
                },
                new PointOfInterest("Masaajidka Hargeisa")
                {
                    Id = 10, 
                    CityId = 4,
                    Description = "A prayer mosque",
                });


            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("connectionstring");
        //    base.OnConfiguring(optionsBuilder);
        //}


    }
}
