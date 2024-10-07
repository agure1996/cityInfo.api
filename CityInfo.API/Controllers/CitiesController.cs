using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        //replace datastore with repository
        //private readonly CitiesDataStore _citiesDataStore;

        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task <ActionResult<IEnumerable<CityWithoutPOIDTO>>> GetCities()
        {

            var cityEntities = await _cityInfoRepository.GetCitiesAsync();
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
        public async Task<IActionResult> GetCityById(int CityId , bool includePOI = false)
        {
            var city = await _cityInfoRepository.GetCityASync(CityId,includePOI);

            if (city == null) return NotFound();

            if (includePOI)
            {
                return Ok(_mapper.Map<CityDTO>(city));
            }
            return Ok(_mapper.Map<CityWithoutPOIDTO>(city));
        }





        //public JsonResult GetCityById(int id)
        //{
        //    return new JsonResult(
        //        CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));
        //}

    }
}
