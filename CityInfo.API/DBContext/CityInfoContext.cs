using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DBContext
{
    /// <summary>
    /// Represents the database context for the CityInfo application.
    /// This class is responsible for managing the entity objects during runtime,
    /// which includes fetching data from the database and saving changes.
    /// </summary>
    public class CityInfoContext : DbContext
    {
        /// <summary>
        /// Gets or sets the collection of cities in the database.
        /// </summary>
        public DbSet<City> Cities { get; set; }

        /// <summary>
        /// Gets or sets the collection of points of interest in the database.
        /// </summary>
        public DbSet<PointOfInterest> PointOfInterests { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CityInfoContext"/> class.
        /// </summary>
        /// <param name="options">The options for configuring the context.</param>
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Configures the model using the <see cref="ModelBuilder"/> to seed initial data.
        /// This method is called by the framework and should not be called directly.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure the context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed initial data for Cities
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

            // Seed initial data for Points of Interest
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
                    Id = 4,
                    CityId = 2,
                    Description = "Beach resort",
                },
                new PointOfInterest("Masaajidka Xamar")
                {
                    Id = 5, 
                    CityId = 2,
                    Description = "A prayer mosque",
                },
                new PointOfInterest("Benzema browns")
                {
                    Id = 6, 
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

        // Will Uncomment and configure the following method if I need to specify the database connection string.
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("connectionstring");
        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}
