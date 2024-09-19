using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<CityDTO>> GetCities()
        {
            var CitiesJson = CitiesDataStore.Current.Cities;
           
            return Ok(CitiesJson);
        }
        //public JsonResult GetCities()
        //{
        //    var CitiesJson = CitiesDataStore.Current.Cities;
           
        //    return new JsonResult(CitiesJson);
        //}


        [HttpGet("{CityId}")]
        public ActionResult<CityDTO> GetCityById(int CityId)
        {
           var CityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == CityId);

            if(CityToReturn == null) return NotFound();

            return Ok(CityToReturn);
        }
        //public JsonResult GetCityById(int id)
        //{
        //    return new JsonResult(
        //        CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id));
        //}

    }
}
