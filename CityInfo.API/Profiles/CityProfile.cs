using AutoMapper;

namespace CityInfo.API.Profiles
{
    /// <summary>
    /// AutoMapper profile for mapping City entities to City DTOs.
    /// </summary>
    public class CityProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CityProfile"/> class.
        /// </summary>
        public CityProfile()
        {
            // Map city entity to CityWithoutPOIDTO without Points of Interest
            CreateMap<Entities.City, Models.CityWithoutPOIDTO>();

            // Map city entity to CityDTO with Points of Interest
            CreateMap<Entities.City, Models.CityDTO>();
        }
    }
}
