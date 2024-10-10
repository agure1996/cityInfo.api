using AutoMapper;

namespace CityInfo.API.Profiles
{
    /// <summary>
    /// AutoMapper profile for mapping Point of Interest entities to their corresponding DTOs.
    /// </summary>
    public class PointsOfInterestProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointsOfInterestProfile"/> class.
        /// </summary>
        public PointsOfInterestProfile()
        {
            // Map PointOfInterest entity to PointsOfInterestDTO
            CreateMap<Entities.PointOfInterest, Models.PointsOfInterestDTO>();

            // Map CreatePointOfInterestDTO to PointOfInterest entity for creation
            CreateMap<Models.CreatePointOfInterestDTO, Entities.PointOfInterest>();

            // Map UpdatePointOfInterestDTO to PointOfInterest entity for updates
            CreateMap<Models.UpdatePointOfInterestDTO, Entities.PointOfInterest>();

            // Map PointOfInterest entity to UpdatePointOfInterestDTO for partial updates
            CreateMap<Entities.PointOfInterest, Models.UpdatePointOfInterestDTO>();
        }
    }
}
