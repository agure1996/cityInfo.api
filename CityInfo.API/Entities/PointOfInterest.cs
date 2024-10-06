using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        //since pointofinterest is part of city, adding city property sets up a relation between the two that some dbs can map and use for navigation, we set cityid as the foreign key that it is
        [ForeignKey("CityId")]
        public City? City { get; set; }

        public int CityId { get; set; }

        public PointOfInterest(string name) { Name = name; }
    }
}
