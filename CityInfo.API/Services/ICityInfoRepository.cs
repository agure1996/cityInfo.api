using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task <IEnumerable<City>> GetCitiesAsync();
        Task <(IEnumerable<City>,PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
        Task <IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task <PointOfInterest?> GetAPointOfInterestFromCityAsync(int cityId, int pointOfInterestId);
        Task<bool> CityExistsAsync(int cityId);
        Task <City?> GetCityASync(int cityId, bool includePOI = false);
        Task AddPointOfInterestToCityASync(int cityId,PointOfInterest pointOfInterest);
        void DeletePointOfInterestToCity(PointOfInterest pointOfInterest);

        Task<bool> SaveChangesASync();

    }
}
