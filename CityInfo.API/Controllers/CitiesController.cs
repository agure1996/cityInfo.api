using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize] //after setting authorisation middleware set this controller to check authorisation
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        //replace datastore with repository
        //private readonly CitiesDataStore _citiesDataStore;

        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int MAXCITIESPERPAGE = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /**
                 * originally we looped through list 
                 * but now we simply get the contents via async function
                 * then proceed to then use automapper to map city entity in db to the dto object
                        foreach (var city in CitiesJson)
                        {
                            results.Add(new CityWithoutPOIDTO
                            {
                                Id = city.Id,
                                Description = city.Description,
                                Name = city.Name
                            });
                        }
                    return Ok(results);
              * 
              */
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPOIDTO>>> GetCities(
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            

            if (pageSize > MAXCITIESPERPAGE) {
                pageNumber = MAXCITIESPERPAGE; 
            }
            var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
            
            Response.Headers.Add("X-Pagination",
                System.Text.Json.JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPOIDTO>>(cityEntities));
            /**
             * originally we looped through list 
             * but now we simply get the contents via async function
             * then proceed to then use automapper to map city entity in db to the dto object
                    foreach (var city in CitiesJson)
                    {
                        results.Add(new CityWithoutPOIDTO
                        {
                            Id = city.Id,
                            Description = city.Description,
                            Name = city.Name
                        });
                    }
                return Ok(results);
             * 
             */
        }


        //public JsonResult GetCities()
        //{
        //    var CitiesJson = CitiesDataStore.Current.Cities;

        //    return new JsonResult(CitiesJson);
        //}


        [HttpGet("{CityId}")]
        public async Task<IActionResult> GetCityById(int CityId, bool includePOI = false)
        {
            

            var city = await _cityInfoRepository.GetCityASync(CityId, includePOI);

            if (city == null) return NotFound();

            if (includePOI)
            {
                return Ok(_mapper.Map<CityDTO>(city));
            }
            return Ok(_mapper.Map<CityWithoutPOIDTO>(city));
        }

    }
}
