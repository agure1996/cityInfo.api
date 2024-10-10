using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    /// <summary>
    /// DTO for function for creating a point of interest
    /// </summary>
    public class CreatePointOfInterestDTO 
    {   
        /// Annotation for data is required

        /// <summary>
        /// Name of point of interest
        /// </summary>
        [Required(ErrorMessage = "Provide a name please.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of point of interest
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
