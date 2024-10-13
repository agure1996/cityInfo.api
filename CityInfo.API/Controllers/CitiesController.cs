using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    /// <summary>
    /// Controller for handling City-related operations, providing API access to retrieve cities with or without Points of Interest (POIs).
    /// Supports API versioning and uses a repository pattern to fetch data.
    /// </summary>
    [ApiController]
    [Authorize]
    // Enable after setting up authorization middleware
    [Route("api/v{version:apiVersion}/cities")]
    [ApiVersion(1)]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        private const int MAXCITIESPERPAGE = 20;

        /// <summary>
        /// Constructor for CitiesController.
        /// </summary>
        /// <param name="cityInfoRepository">Repository for city-related database operations.</param>
        /// <param name="mapper">AutoMapper instance to map entities to DTOs.</param>
        /// <exception cref="ArgumentNullException">Thrown when cityInfoRepository or mapper is null.</exception>
        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieves a paginated list of cities, optionally filtering by name or search query.
        /// </summary>
        /// <param name="name">Optional name filter to search for cities by name.</param>
        /// <param name="searchQuery">Optional search query to filter cities by a keyword in the name or description.</param>
        /// <param name="pageNumber">Page number for pagination (default is 1).</param>
        /// <param name="pageSize">Number of cities per page (default is 10, max is 20).</param>
        /// <response code="200">Returns a paginated list of cities.</response>
        /// <response code="400">Invalid parameters provided.</response>
        /// <response code="404">No cities found.</response>
        /// <returns>A paginated list of cities in the form of CityWithoutPOIDTO.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CityWithoutPOIDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CityWithoutPOIDTO>>> GetCities(
            string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            // Ensure the page size does not exceed the maximum allowed value.
            if (pageSize > MAXCITIESPERPAGE)
            {
                pageSize = MAXCITIESPERPAGE;
            }

            // Get cities from the repository with optional filters and pagination.
            var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            // Add pagination metadata to the response headers.
            Response.Headers.Add("X-Pagination",
                System.Text.Json.JsonSerializer.Serialize(paginationMetadata));

            // Map city entities to DTOs and return the result.
            return Ok(_mapper.Map<IEnumerable<CityWithoutPOIDTO>>(cityEntities));
        }

        /// <summary>
        /// Retrieves a specific city by its ID, with an option to include its Points of Interest (POIs).
        /// </summary>
        /// <response code="200">Returns the requested city information.</response>
        /// <response code="400">Invalid city ID provided.</response>
        /// <response code="404">City not found.</response>
        /// <param name="CityId">ID of the city to retrieve.</param>
        /// <param name="includePOI">Boolean flag to include or exclude POIs. If true, POIs are included.</param>
        /// <returns>Returns the city information, either with or without POIs, depending on the flag.</returns>
        [HttpGet("{CityId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CityDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCityById(int CityId, bool includePOI = false)
        {
            // Retrieve the city from the repository, optionally including its POIs.
            var city = await _cityInfoRepository.GetCityASync(CityId, includePOI);

            // Return 404 if the city is not found.
            if (city == null)
            {
                return NotFound();
            }

            // Return the city with or without POIs, depending on the flag.
            if (includePOI)
            {
                return Ok(_mapper.Map<CityDTO>(city));
            }
            else
            {
                return Ok(_mapper.Map<CityWithoutPOIDTO>(city));
            }
        }
    }
}
