using CityInfo.API.DBContext;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    /// <summary>
    /// Repository for managing City and Point of Interest data.
    /// Provides methods for accessing and manipulating city and point of interest information in the database.
    /// </summary>
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CityInfoRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided context is null.</exception>
        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Retrieves a specific point of interest for a given city.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <param name="pointOfInterestId">The ID of the point of interest.</param>
        /// <returns>The point of interest if found; otherwise, null.</returns>
        public async Task<PointOfInterest?> GetAPointOfInterestFromCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointOfInterests
                .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all cities, ordered by their names.
        /// </summary>
        /// <returns>A collection of cities.</returns>
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        /// <summary>
        /// Retrieves cities with optional filtering, searching, and pagination.
        /// </summary>
        /// <param name="name">Optional name filter for the city.</param>
        /// <param name="searchQuery">Optional search query for city names or descriptions.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A tuple containing a collection of cities and pagination metadata.</returns>
        public async Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            // Start with the initial collection of cities
            var collection = _context.Cities as IQueryable<City>;

            // Apply filtering based on the provided name
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            // Apply search functionality if a search query is provided
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery)
                || (a.Description != null && a.Description.Contains(searchQuery)));
            }

            // Count the total number of items after filtering
            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            // Return paginated results
            var collectionToReturn = await collection.OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        /// <summary>
        /// Retrieves a specific city, optionally including its points of interest.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <param name="includePOI">Whether to include points of interest in the result.</param>
        /// <returns>The city if found; otherwise, null.</returns>
        public async Task<City?> GetCityASync(int cityId, bool includePOI)
        {
            if (includePOI)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }

            return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Checks if a city exists by its ID.
        /// </summary>
        /// <param name="cityId">The ID of the city to check.</param>
        /// <returns>True if the city exists; otherwise, false.</returns>
        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        /// <summary>
        /// Retrieves all points of interest for a given city.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <returns>A collection of points of interest for the specified city.</returns>
        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointOfInterests
                .Where(p => p.CityId == cityId).ToListAsync();
        }

        /// <summary>
        /// Adds a point of interest to a specific city asynchronously.
        /// </summary>
        /// <param name="cityId">The ID of the city to add the point of interest to.</param>
        /// <param name="pointOfInterest">The point of interest to add.</param>
        public async Task AddPointOfInterestToCityASync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityASync(cityId, false);
            if (city != null) { city.PointsOfInterest.Add(pointOfInterest); }
        }

        /// <summary>
        /// Checks if the city name matches the claim associated with the specified city ID.
        /// </summary>
        /// <param name="cityName">The name of the city to check against the claim.</param>
        /// <param name="cityId">The ID of the city associated with the claim.</param>
        /// <returns>True if the name matches; otherwise, false.</returns>
        public async Task<bool> CityNameMatchesCityClaim(string? cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }

        /// <summary>
        /// Saves changes to the database asynchronously.
        /// </summary>
        /// <returns>True if the changes were saved successfully; otherwise, false.</returns>
        public async Task<bool> SaveChangesASync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        /// <summary>
        /// Deletes a point of interest from the context.
        /// </summary>
        /// <param name="pointOfInterest">The point of interest to remove.</param>
        public void DeletePointOfInterestToCity(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
        }
    }
}
