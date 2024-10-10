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
    /// <summary>
    /// Manages operations related to Points of Interest for a city.
    /// </summary>
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
    [ApiVersion(1)]
    [ApiController]
    [Authorize] // Enable authorization for the controller
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger; // Logger for logging API requests and responses.
        private readonly IMailService _mailService; // Service used to send email notifications.
        private readonly ICityInfoRepository _cityInfoRepository; // Repository for managing city and point of interest data.
        private readonly IMapper _mapper; // Mapper for transforming entities to DTOs and vice versa.

        /// <summary>
        /// Constructor for PointsOfInterestController.
        /// </summary>
        /// <param name="logger">Logger for logging API requests and responses.</param>
        /// <param name="mailService">Service used to send email notifications.</param>
        /// <param name="cityInfoRepository">Repository for managing city and point of interest data.</param>
        /// <param name="mapper">Mapper for transforming entities to DTOs and vice versa.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the dependencies are null.</exception>
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Check for null dependencies
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieves a list of Points of Interest for the specified city.
        /// </summary>
        /// <param name="cityId">The ID of the city for which the points of interest are being retrieved.</param>
        /// <returns>A list of points of interest for the specified city.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointsOfInterestDTO>>> GetPointsOfInterest(int cityId)
        {
            var cityNameClaim = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

            // Check if the city name matches the claim
            if (!await _cityInfoRepository.CityNameMatchesCityClaim(cityNameClaim, cityId))
            {
                return Forbid(); // Return Forbidden if the claim does not match
            }

            // Check if the city exists
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found.");
                return NotFound(); // Return NotFound if the city does not exist
            }

            // Retrieve points of interest
            var pointsOfInterest = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointsOfInterestDTO>>(pointsOfInterest)); // Return mapped DTOs
        }

        /// <summary>
        /// Retrieves a specific point of interest in a city.
        /// </summary>
        /// <param name="cityId">The ID of the city that contains the point of interest.</param>
        /// <param name="PointOfInterestId">The ID of the point of interest to retrieve.</param>
        /// <returns>The requested point of interest, or a NotFound result if it doesn't exist.</returns>
        [HttpGet("{PointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointsOfInterestDTO>> GetPointOfInterest(int cityId, int PointOfInterestId)
        {
            // Check if the city exists
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound(); // Return NotFound if the city does not exist
            }

            // Retrieve the point of interest
            var pointOfInterest = await _cityInfoRepository.GetAPointOfInterestFromCityAsync(cityId, PointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound(); // Return NotFound if the point of interest does not exist
            }

            return Ok(_mapper.Map<PointsOfInterestDTO>(pointOfInterest)); // Return mapped DTO
        }

        /// <summary>
        /// Creates a new point of interest for a specified city.
        /// </summary>
        /// <param name="CityId">The ID of the city where the point of interest will be created.</param>
        /// <param name="pointOfInterest">The point of interest data to create.</param>
        /// <returns>The created point of interest.</returns>
        [HttpPost]
        public async Task<ActionResult<PointsOfInterestDTO>> CreatePointOfInterest(int CityId, [FromBody] CreatePointOfInterestDTO pointOfInterest)
        {
            // Check if the city exists
            if (!await _cityInfoRepository.CityExistsAsync(CityId))
            {
                return NotFound(); // Return NotFound if the city does not exist
            }

            // Map the incoming DTO to the entity
            var newPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);
            await _cityInfoRepository.AddPointOfInterestToCityASync(CityId, newPointOfInterest); // Add point of interest
            await _cityInfoRepository.SaveChangesASync(); // Save changes

            var createdPointOfInterest = _mapper.Map<PointsOfInterestDTO>(newPointOfInterest); // Map to DTO

            return CreatedAtRoute("GetPointOfInterest", // Return the created point of interest
                                  new { cityid = CityId, pointOfInterestId = createdPointOfInterest.Id },
                                  createdPointOfInterest);
        }

        /// <summary>
        /// Updates an existing point of interest in a city.
        /// </summary>
        /// <param name="CityId">The ID of the city containing the point of interest.</param>
        /// <param name="PointOfInterestId">The ID of the point of interest to update.</param>
        /// <param name="pointOfInterestBeingUpdated">The updated point of interest data.</param>
        /// <returns>No content if successful, or NotFound if the city or point of interest doesn't exist.</returns>
        [HttpPut("{PointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int CityId, int PointOfInterestId, UpdatePointOfInterestDTO pointOfInterestBeingUpdated)
        {
            // Check if the city exists
            if (!await _cityInfoRepository.CityExistsAsync(CityId))
            {
                return NotFound(); // Return NotFound if the city does not exist
            }

            // Retrieve the existing point of interest
            var pointOfInterestEntity = await _cityInfoRepository.GetAPointOfInterestFromCityAsync(CityId, PointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound(); // Return NotFound if the point of interest does not exist
            }

            // Map the updated data to the existing entity
            _mapper.Map(pointOfInterestBeingUpdated, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesASync(); // Save changes

            return NoContent(); // Return NoContent on success
        }

        /// <summary>
        /// Applies a partial update to a point of interest.
        /// </summary>
        /// <param name="CityId">The ID of the city containing the point of interest.</param>
        /// <param name="PointOfInterestId">The ID of the point of interest to update.</param>
        /// <param name="patchDocument">The patch document containing the updates.</param>
        /// <returns>No content if successful, or BadRequest if validation fails.</returns>
        [HttpPatch("{PointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int CityId, int PointOfInterestId, JsonPatchDocument<UpdatePointOfInterestDTO> patchDocument)
        {
            // Check if the city exists
            if (!await _cityInfoRepository.CityExistsAsync(CityId))
            {
                return NotFound(); // Return NotFound if the city does not exist
            }

            // Retrieve the existing point of interest
            var pointOfInterestEntity = await _cityInfoRepository.GetAPointOfInterestFromCityAsync(CityId, PointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound(); // Return NotFound if the point of interest does not exist
            }

            // Prepare to apply patch
            var pointOfInterestToPatch = _mapper.Map<UpdatePointOfInterestDTO>(pointOfInterestEntity);
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState); // Apply the patch

            // Validate the model state
            if (!ModelState.IsValid || !TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState); // Return BadRequest if validation fails
            }

            // Map the patched data back to the existing entity
            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesASync(); // Save changes

            return NoContent(); // Return NoContent on success
        }

        /// <summary>
        /// Deletes a point of interest in a city.
        /// </summary>
        /// <param name="CityId">The ID of the city containing the point of interest.</param>
        /// <param name="PointOfInterestId">The ID of the point of interest to delete.</param>
        /// <returns>No content if the deletion is successful, or NotFound if the city or point of interest doesn't exist.</returns>
        [HttpDelete("{PointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int CityId, int PointOfInterestId)
        {
            // Check if the city exists
            if (!await _cityInfoRepository.CityExistsAsync(CityId))
            {
                return NotFound(); // Return NotFound if the city does not exist
            }

            // Retrieve the existing point of interest
            var pointOfInterestEntity = await _cityInfoRepository.GetAPointOfInterestFromCityAsync(CityId, PointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound(); // Return NotFound if the point of interest does not exist
            }

            // Delete the point of interest
            _cityInfoRepository.DeletePointOfInterestToCity(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesASync(); // Save changes

            string message = $"Point of interest {pointOfInterestEntity.Name} (ID: {pointOfInterestEntity.Id}) has been deleted."; // Log message
            _mailService.send("Point of interest deleted.", message); // Send notification

            return NoContent(); // Return NoContent on success
        }
    }
}
