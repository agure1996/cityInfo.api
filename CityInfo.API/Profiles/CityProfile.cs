using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class CityProfile : Profile
    {

        public CityProfile()
        {   
            //Map city entity to City without Points of Interest DTO 
            CreateMap<Entities.City, Models.CityWithoutPOIDTO>();

            //Map city entity to City with Points of Interest DTO 
            CreateMap<Entities.City, Models.CityDTO>();
        }
    }
}
