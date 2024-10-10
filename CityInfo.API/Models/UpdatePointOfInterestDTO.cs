using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{   
    /// <summary>
    /// DTO for when update request made for point of interest object
    /// </summary>
    public class UpdatePointOfInterestDTO
    {
        /// <summary>
        /// Name of Point of Interest
        /// </summary>
        //Annotation for data is required
        [Required(ErrorMessage = "Provide a name please.")]
        [MaxLength(50)]

        public string Name { get; set; } = string.Empty;


        /// <summary>
        /// Description of Point of Interest
        /// </summary>
        [Required(ErrorMessage = "Provide some description please.")]
        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
