using CityInfo.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class City
    {
        /**
         * You will notice in our DTO we dont add requirements
         * And that is because DTOs are only used to get data, not manipulate it to no need for regulations
         */
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)] 
        public string Name { get; set; }

        [MaxLength(200)]
        public string?   Description { get; set; }

        public ICollection<PointOfInterest> PointsOfInterest { get; set; }
        = new List<PointOfInterest>();

        public City(string name) {  Name = name; }
    }
}
