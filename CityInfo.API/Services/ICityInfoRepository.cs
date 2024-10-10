using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    /// <summary>
    /// Defines the operations for accessing city and point of interest data.
    /// Implementations of this interface should handle data retrieval and persistence.
    /// </summary>
    public interface ICityInfoRepository
    {
        /// <summary>
        /// Retrieves all cities asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a collection of cities.</returns>
        Task<IEnumerable<City>> GetCitiesAsync();

        /// <summary>
        /// Retrieves cities that match the specified criteria asynchronously.
        /// Supports pagination.
        /// </summary>
        /// <param name="name">The exact name of the city to filter by (optional).</param>
        /// <param name="searchQuery">A search term to filter cities by name or description (optional).</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of items per page for pagination.</param>
        /// <returns>A task that represents the asynchronous operation, containing a tuple of a collection of cities and pagination metadata.</returns>
        Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);

        /// <summary>
        /// Retrieves all points of interest for a specific city asynchronously.
        /// </summary>
        /// <param name="cityId">The identifier of the city.</param>
        /// <returns>A task that represents the asynchronous operation, containing a collection of points of interest.</returns>
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);

        /// <summary>
        /// Retrieves a specific point of interest from a city asynchronously.
        /// </summary>
        /// <param name="cityId">The identifier of the city.</param>
        /// <param name="pointOfInterestId">The identifier of the point of interest.</param>
        /// <returns>A task that represents the asynchronous operation, containing the point of interest if found; otherwise, null.</returns>
        Task<PointOfInterest?> GetAPointOfInterestFromCityAsync(int cityId, int pointOfInterestId);

        /// <summary>
        /// Checks if a city exists by its identifier asynchronously.
        /// </summary>
        /// <param name="cityId">The identifier of the city.</param>
        /// <returns>A task that represents the asynchronous operation, containing true if the city exists; otherwise, false.</returns>
        Task<bool> CityExistsAsync(int cityId);

        /// <summary>
        /// Retrieves a specific city by its identifier asynchronously.
        /// Optionally includes points of interest.
        /// </summary>
        /// <param name="cityId">The identifier of the city.</param>
        /// <param name="includePOI">Whether to include points of interest in the result.</param>
        /// <returns>A task that represents the asynchronous operation, containing the city if found; otherwise, null.</returns>
        Task<City?> GetCityASync(int cityId, bool includePOI = false);

        /// <summary>
        /// Adds a point of interest to a specific city asynchronously.
        /// </summary>
        /// <param name="cityId">The identifier of the city.</param>
        /// <param name="pointOfInterest">The point of interest to add.</param>
        Task AddPointOfInterestToCityASync(int cityId, PointOfInterest pointOfInterest);

        /// <summary>
        /// Checks if a city's name matches the specified claim.
        /// </summary>
        /// <param name="cityName">The name of the city to check.</param>
        /// <param name="cityId">The identifier of the city.</param>
        /// <returns>A task that represents the asynchronous operation, containing true if the names match; otherwise, false.</returns>
        Task<bool> CityNameMatchesCityClaim(string? cityName, int cityId);

        /// <summary>
        /// Deletes a point of interest from a city.
        /// </summary>
        /// <param name="pointOfInterest">The point of interest to delete.</param>
        void DeletePointOfInterestToCity(PointOfInterest pointOfInterest);

        /// <summary>
        /// Saves changes to the data store asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing true if the changes were saved successfully; otherwise, false.</returns>
        Task<bool> SaveChangesASync();
    }
}
