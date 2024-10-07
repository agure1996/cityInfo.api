using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class PointsOfInterestProfile : Profile
    {

        public PointsOfInterestProfile()
        {
            CreateMap<Entities.PointOfInterest,Models.PointsOfInterestDTO>();
            //since we are using put statement we map model to entity
            CreateMap<Models.CreatePointOfInterestDTO,Entities.PointOfInterest>();
            //since we are using update statement we map model to entity
            CreateMap<Models.UpdatePointOfInterestDTO,Entities.PointOfInterest>();
            //since we are using patch statement we map model to entity
            CreateMap<Entities.PointOfInterest,Models.UpdatePointOfInterestDTO>();
        }
    }
}
