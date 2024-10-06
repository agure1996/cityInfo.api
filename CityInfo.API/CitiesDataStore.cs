using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {

        public List<CityDTO> Cities { get; set; }

        //since cities data store is technically a singleton  we will use singleton service in program to call the datastore
        //public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public CitiesDataStore()
        {

            //Initialising some dummy data

            Cities = new List<CityDTO>()
            {
                new CityDTO()
                {

                    Id = 1,
                    Name = "New York City",
                    Description  = "City with big park",
                    PointsOfInterest = new List<PointsOfInterestDTO>()
                    {
                        new PointsOfInterestDTO()
                        {
                            Id = 1,
                    Name = "Hazzerdous",
                    Description  = "Food Joint",
                        },
                        new PointsOfInterestDTO()
                        {
                            Id = 2,
                    Name = "Jumanji",
                    Description  = "Game Park",
                        },
                        new PointsOfInterestDTO()
                        {
                            Id = 3,
                    Name = "Mosque of NY",
                    Description  = "A prayer mosque",
                        },
                    }

                },
                new CityDTO()
                {

                    Id = 2,
                    Name = "Antwerp",
                    Description  = "Cathedral city",
                    PointsOfInterest = new List<PointsOfInterestDTO>()
                    {
                        new PointsOfInterestDTO()
                        {
                            Id = 1,
                    Name = "Spijken",
                    Description  = "Food park",
                        },
                        new PointsOfInterestDTO()
                        {
                            Id = 2,
                    Name = "skapolo",
                    Description  = "Social circle center",
                        },
                        new PointsOfInterestDTO()
                        {
                            Id = 3,
                    Name = "Mosque of Antilles",
                    Description  = "A prayer mosque",
                        },
                    }

                },
                new CityDTO()
                {

                    Id = 3,
                    Name = "Paris",
                    Description  = "Eiffel tower city",
                    PointsOfInterest = new List<PointsOfInterestDTO>()
                    {
                        new PointsOfInterestDTO()
                        {
                            Id = 2,
                    Name = "Iniesta",
                    Description  = "Footie Spot",
                        },
                        new PointsOfInterestDTO()
                        {
                            Id = 1,
                    Name = "Benzema browns",
                    Description  = "Dessert parlor",
                        },
                        new PointsOfInterestDTO()
                        {
                            Id = 3,
                    Name = "Mosque of Paris",
                    Description  = "A prayer mosque",
                        },
                    }

                },
                new CityDTO()
                {

                    Id = 4,
                    Name = "Hargeisa",
                    Description  = "North city",
                    PointsOfInterest = new List<PointsOfInterestDTO>()
                    {
                        new PointsOfInterestDTO()
                        {
                            Id = 4,
                    Name = "Xeebta Liido",
                    Description  = "Beach resort",
                        },
                        new PointsOfInterestDTO()
                        {
                            Id = 1,
                    Name = "Haraar House",
                    Description  = "Social area for elders",
                        },
                        new PointsOfInterestDTO()
                        {
                            Id = 2,
                    Name = "Masaajidka Hargeisa",
                    Description  = "A prayer mosque",
                        },
                    }

                }

             };

        }
    }
}
