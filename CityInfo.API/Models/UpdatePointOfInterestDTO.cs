using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class UpdatePointOfInterestDTO
    {

        //Annotation for data is required
        [Required(ErrorMessage = "Provide a name please.")]
        [MaxLength(50)]

        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Provide some description please.")]
        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
