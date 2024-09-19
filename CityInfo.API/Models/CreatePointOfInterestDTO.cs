using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{

    public class CreatePointOfInterestDTO 
    {   

        //Annotation for data is required
        [Required(ErrorMessage = "Provide a name please.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
