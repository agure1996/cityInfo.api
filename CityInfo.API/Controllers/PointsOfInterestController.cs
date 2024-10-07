using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
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
        //private readonly CitiesDataStore _citiesDataStore; //replaced datastore with repository
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;


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


        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointsOfInterestDTO>>> GetPointsOfInterest(int cityId)
        {

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest.");
                return NotFound();
            }
            var pointsOfInterestForSelectedCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointsOfInterestDTO>>(pointsOfInterestForSelectedCity));
        }

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
