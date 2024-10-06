using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {

        private readonly ILogger<PointsOfInterestController> _logger;
        //private readonly LocalMailService _mailService;
        private readonly IMailService _mailService;
        private readonly CitiesDataStore _citiesDataStore;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, CitiesDataStore citiesDataStore )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException( nameof(citiesDataStore));
        }


        [HttpGet]
        public ActionResult<IEnumerable<PointsOfInterestDTO>> GetPointsOfInterest(int cityId)
        {

            try
            {
                //assign the process of getting city to a variable
                var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
                //Checking if City exists
                if (city == null)
                {
                    //log if city isnt found
                    _logger.LogInformation($"City with id {cityId} does not exist");
                    //if city doesnt exist return 404 not found
                }

                //if city exists return city with actionresult of 200 OK
                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Exception while trying to get points of interest for city with id {cityId}", ex);
                return StatusCode(500, $"A problem occured while handling request");
            }
        }

        [HttpGet("{PointOfInterestId}", Name = "GetPointOfInterest")]
        public ActionResult<PointsOfInterestDTO> GetPointOfInterest(int CityId, int PointOfInterestId)
        {
            //assign the process of getting city to a variable
            var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == CityId);
            //Checking if City exists
            if (city == null)
            {
                //log if city isnt found
                _logger.LogInformation($"City with id {CityId} does not exist");
                //if city doesnt exist return 404 not found
                return NotFound();
            }
            //else then we check if city contains point of interest by Id
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == PointOfInterestId);

            //Check if point of interest id exists
            if (pointOfInterest == null)
            {
                //log if city isnt found
                _logger.LogInformation($"Point of Interest with id {PointOfInterestId} does not exist");
                //if point of interest id doesnt exist return 404 not found
                return NotFound();
            }
            //else return the contents of point of interest of specified id
            return Ok(pointOfInterest);
        }


        [HttpPost]
        public ActionResult<PointsOfInterestDTO> CreatePointOfInterest(int CityId, [FromBody] CreatePointOfInterestDTO pointOfInterest)
        {
            //removed the modelstate validation below because the annotations on the poidto do the validation
            //if modelstate (post request) is not valid
            //if (!ModelState.IsValid) return BadRequest();

            //City assigned to variable - where city variable is found via id
            var City = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == CityId);
            //check if city exists
            if (City == null)
            {
                _logger.LogInformation($"City with id {CityId} does not exist");

                return NotFound();
            }

            // Check if a point of interest with the same name already exists in the city
            if (City.PointsOfInterest.Any(p => p.Name == pointOfInterest.Name))
            {
                _logger.LogWarning($"A point of interest with the name '{pointOfInterest.Name}' already exists in the city.");

                return Conflict($"A point of interest with the name '{pointOfInterest.Name}' already exists in the city.");
            }

            //demo purpose - will be improved for now fidning the highest point of id of all points of interest and add one
            //var MaxPOIId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(poi => poi.Id);

            //chatgpt checking of poi already exists
            var MaxPOIId = _citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).DefaultIfEmpty().Max(poi => poi?.Id ?? 0);


            //map poiDTO to end of poiforcreationDTO creating a new point of interest in the poi and adding one to its max count, essentially adding new poi
            var finalPOI = new PointsOfInterestDTO()
            {
                Id = ++MaxPOIId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description,
            };


            //adding new poi to the city
            City.PointsOfInterest.Add(finalPOI);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = CityId,
                    PointOfInterestId = finalPOI.Id
                },
                finalPOI);
        }

        [HttpPut("{PointOfInterestId}")]
        public ActionResult<PointsOfInterestDTO> UpdatePointOfInterest(int CityId, int PointOfInterestId, UpdatePointOfInterestDTO pointOfInterestBeingUpdated)
        {

            //City assigned to variable - where city variable is found via id
            var City = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == CityId);
            //check if city exists
            if (City == null) return NotFound();

            //Check if point of interest exists
            var POIFromStore = City.PointsOfInterest
                            .FirstOrDefault(c => c.Id == PointOfInterestId);
            if (POIFromStore == null)
            {
                _logger.LogWarning($"Point of interest '{POIFromStore}' does bit exist in database of POI.");
                return NotFound();
            }

            //if it exists we update the values of the POI object in our store with the values of POI object we provide
            POIFromStore.Name = pointOfInterestBeingUpdated.Name;
            POIFromStore.Description = pointOfInterestBeingUpdated.Description;



            return NoContent();
        }

        [HttpPatch("{PointOfInterestId}")]
        public ActionResult PartiallyUpdatePointOfInterest(int CityId, int PointOfInterestId, JsonPatchDocument<UpdatePointOfInterestDTO> patchDocument)
        {


            //City assigned to variable - where city variable is found via id
            var City = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == CityId);
            //check if city exists
            if (City == null) return NotFound();

            //Check if point of interest exists
            var POIFromStore = City.PointsOfInterest
                            .FirstOrDefault(c => c.Id == PointOfInterestId);
            if (POIFromStore == null)
            {
                _logger.LogWarning($"Point of interest '{POIFromStore}' does not exist in database of POI.");
                return NotFound();
            };

            //store the patched POI object to a variable
            var POIToPatch = new UpdatePointOfInterestDTO()
            {
                Name = POIFromStore.Name,
                Description = POIFromStore.Description
            };

            patchDocument.ApplyTo(POIToPatch, ModelState);

            //Check if the object we are trying to patch and the modelstate are both valid

            if (!ModelState.IsValid || !TryValidateModel(POIToPatch)) return BadRequest(ModelState);

            //if the modelstate and object we are patching is valid
            POIFromStore.Name = POIToPatch.Name;
            POIFromStore.Description = POIToPatch.Description;


            return NoContent();
        }

        [HttpDelete("{PointOfInterestId}")]
        public ActionResult DeletePointOfInterest(int CityId, int PointOfInterestId)
        {

            //City assigned to variable - where city variable is found via id
            var City = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == CityId);
            //check if city exists
            if (City == null) return NotFound();

            //Check if point of interest exists
            var POIFromStore = City.PointsOfInterest
                            .FirstOrDefault(c => c.Id == PointOfInterestId);
            if (POIFromStore == null) return NotFound();


            //remove POI from City
            City.PointsOfInterest.Remove(POIFromStore);

            string message = $"Point of interest {POIFromStore.Name} with id {POIFromStore.Id} has been deleted.";
            //send mail informing point of interest has been deleted
            _mailService.send("Point of interest deleted.",
                message);
            return NoContent();
        }
    }
}
