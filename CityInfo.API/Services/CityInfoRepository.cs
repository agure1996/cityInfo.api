using CityInfo.API.DBContext;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services;

public class CityInfoRepository : ICityInfoRepository
{

    private readonly CityInfoContext _context;
    public CityInfoRepository(CityInfoContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<PointOfInterest?> GetAPointOfInterestFromCityAsync(int cityId, int pointOfInterestId)
    {
        return await _context.PointOfInterests.Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
               .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
    }
    public async Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize) 
    {
      
        //collection to start from (look into what defered execution is)
        var collection = _context.Cities as IQueryable<City>;

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            collection = collection.Where(c => c.Name == name);
        }
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection.Where(a => a.Name.Contains(searchQuery) 
            || (a.Description != null && a.Description.Contains(searchQuery)));
        }

        var totalItemCount = await collection.CountAsync();

        var paginatonMetadata = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

        //if city name is found filter for it, update: added paging functionality
        var collectionToReturn = await collection.OrderBy(c => c.Name)
            .Skip(pageSize * (pageNumber -1))
            .Take(pageSize)
            .ToListAsync();

        return (collectionToReturn, paginatonMetadata);
    }

    public async Task<City?> GetCityASync(int cityId, bool includePOI)
    {
        if (includePOI)
        {
            return await _context.Cities.Include(c => c.PointsOfInterest)
                .Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();

    }

    public async Task<bool> CityExistsAsync(int cityId)
    {
        return await _context.Cities.AnyAsync(c => c.Id == cityId);
    }

    public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
    {
        return await _context.PointOfInterests
            .Where(p => p.CityId == cityId).ToListAsync();
    }

    public async Task AddPointOfInterestToCityASync(int cityId, PointOfInterest pointOfInterest)
    {
        var city = await GetCityASync(cityId, false);
        if (city != null) { city.PointsOfInterest.Add(pointOfInterest); }
    }

    public async Task<bool> SaveChangesASync()
    {
        return (await _context.SaveChangesAsync() >= 0);
    }

    public void DeletePointOfInterestToCity(PointOfInterest pointOfInterest)
    {
        _context.PointOfInterests.Remove(pointOfInterest);
    }
}
