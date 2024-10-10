using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    /*[Authorize]*/ //after setting authorisation middleware set this controller to check authorisation
    [Route("api/v{version:apiVersion}/cities")]
    [ApiVersion("1.0")]
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




        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="searchQuery"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpGet]

        public async Task<ActionResult<IEnumerable<CityWithoutPOIDTO>>> GetCities(
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {


            if (pageSize > MAXCITIESPERPAGE)
            {
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

        /// <summary>
        /// Get a city by id
        /// </summary>
        /// <param name="CityId"> Id of the City</param>
        /// <param name="includePOI">Whether or not to include points of interest</param>
        /// <returns>Return a city with or without points of interests</returns>
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
