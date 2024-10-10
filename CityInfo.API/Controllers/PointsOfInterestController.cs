using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
    [ApiVersion(1)]
    /*
    [Authorize(Policy = "MustBeFromSpij")] after setting authorisation middleware set this controller to check authorisation
                                            * Just to test added policy, refer to program */
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {

        private readonly ILogger<PointsOfInterestController> _logger;
        //private readonly LocalMailService _mailService;
        private readonly IMailService _mailService;
        //private readonly CitiesDataStore _citiesDataStore; //replaced datastore with repository
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Controller for Points of interest object
        /// </summary>
        /// <param name="logger">Logger for logging API requests</param>
        /// <param name="mailService">Service for sending mail</param>
        /// <param name="cityInfoRepository">City repository</param>
        /// <param name="mapper">Mapper mapping entity to City or Point of interest DTO's</param>
        /// <exception cref="ArgumentNullException">If arguments dont exist or are null</exception>
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ??
                throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Get point of interest of a city
        /// </summary>
        /// <param name="cityId">Id of city whose point of interest we are looking at</param>
        /// <returns>list all points of interest of the city, mapped in appropriate dto format</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointsOfInterestDTO>>> GetPointsOfInterest(int cityId)
        {
            var cityNameClaim = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

            if (!await _cityInfoRepository.CityNameMatchesCityClaim(cityNameClaim, cityId))
            {
                return Forbid();
            }
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest.");
                return NotFound();
            }
            var pointsOfInterestForSelectedCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointsOfInterestDTO>>(pointsOfInterestForSelectedCity));
        }

        /// <summary>
        /// Get a single point of interest of a city using the city and the point of interest Id
        /// </summary>
        /// <param name="cityId">Id of city</param>
        /// <param name="PointOfInterestId">Id of point of interest</param>
        /// <returns>Point of interest if it exists</returns>

        [HttpGet("{PointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointsOfInterestDTO>> GetPointOfInterest(int cityId, int PointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var poi = await _cityInfoRepository.GetAPointOfInterestFromCityAsync(cityId, PointOfInterestId);
            if (poi == null) { return NotFound(); }

            return Ok(_mapper.Map<PointsOfInterestDTO>(poi));
        }

        /// <summary>
        /// Create a point of interest for a city
        /// </summary>
        /// <param name="CityId">Id of city</param>
        /// <param name="pointOfInterest">Point of interest object</param>
        /// <returns>Point of interest created</returns>
        [HttpPost]
        public async Task<ActionResult<PointsOfInterestDTO>> CreatePointOfInterest(int CityId, [FromBody] CreatePointOfInterestDTO pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(CityId)) { return NotFound(); }
            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestToCityASync(CityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesASync();

            var createdAPOIReturn = _mapper.Map<Models.PointsOfInterestDTO>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityid = CityId,
                    pointOfInterestId = createdAPOIReturn.Id

                }, createdAPOIReturn);

        }


        [HttpPut("{PointOfInterestId}")]
        public async Task<ActionResult<PointsOfInterestDTO>> UpdatePointOfInterest(int CityId, int PointOfInterestId, UpdatePointOfInterestDTO pointOfInterestBeingUpdated)
        {


            //check if city exists
            if (!await _cityInfoRepository.CityExistsAsync(CityId)) { return NotFound(); }

            //check if poi exists
            var POIEntity = await _cityInfoRepository.GetAPointOfInterestFromCityAsync(CityId, PointOfInterestId);

            //Check if point of interest exists
            if (POIEntity == null)
            {
                _logger.LogWarning($"Point of interest '{POIEntity}' does not exist in database of POI.");
                return NotFound();
            }

            //if it exists we update and map the values of the POI object in our store with the values of POI object we provide
            _mapper.Map(pointOfInterestBeingUpdated, POIEntity);

            //save changes
            await _cityInfoRepository.SaveChangesASync();

            return NoContent();
        }

        [HttpPatch("{PointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int CityId, int PointOfInterestId, JsonPatchDocument<UpdatePointOfInterestDTO> patchDocument)
        {

            //check if city exists
            if (!await _cityInfoRepository.CityExistsAsync(CityId)) { return NotFound(); }

            //check if poi exists
            var POIEntity = await _cityInfoRepository.GetAPointOfInterestFromCityAsync(CityId, PointOfInterestId);

            if (POIEntity == null)
            {
                _logger.LogWarning($"Point of interest '{POIEntity}' does not exist in database of POI.");
                return NotFound();
            }

          //map to poiforupdatedto
            
           var POIToPatch =  _mapper.Map<UpdatePointOfInterestDTO>(POIEntity);


            patchDocument.ApplyTo(POIToPatch, ModelState);

            //Check if the object we are trying to patch and the modelstate are both valid

            if (!ModelState.IsValid || !TryValidateModel(POIToPatch)) return BadRequest(ModelState);

            //if the modelstate and object we are patching is valid
            _mapper.Map(POIToPatch,POIEntity);

            //save changes
            await _cityInfoRepository.SaveChangesASync();



            return NoContent();
        }

        [HttpDelete("{PointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int CityId, int PointOfInterestId)
        {

            //check if city exists
            if (!await _cityInfoRepository.CityExistsAsync(CityId)) { return NotFound(); }


            //check if poi exists
            var POIEntity = await _cityInfoRepository.GetAPointOfInterestFromCityAsync(CityId, PointOfInterestId);

            if (POIEntity == null)
            {
                _logger.LogWarning($"Point of interest '{POIEntity}' does not exist in database of POI.");
                return NotFound();
            }


            //remove POI from City
            _cityInfoRepository.DeletePointOfInterestToCity(POIEntity);


            //save changes
            await _cityInfoRepository.SaveChangesASync();
            
            //once changes have been made
            string message = $"Point of interest {POIEntity.Name} with id {POIEntity.Id} has been deleted.";
            //send mail informing point of interest has been deleted
            _mailService.send("Point of interest deleted.",
                message);
            return NoContent();
        }
    }
}
