using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Entities
{
    /// <summary>
    /// Entity class representing Point of Interest Object in a Database, is tightly coupled with the Database schema, is required for object to be saved to and retrieved from the Database 
    /// </summary>
    public class PointOfInterest
    {
        /// <summary>
        /// Id of Point of Interest
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Name of Point of Interest
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Description of Point of Interest
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// City navigation property for relational mapping between Point of Interest and City
        /// </summary>
        [ForeignKey("CityId")]
        public City? City { get; set; }

        /// <summary>
        /// City Id representing foreign key to the City entity
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// Constructor for Point of Interest that requires only a name.
        /// </summary>
        /// <param name="name">Name of the Point of Interest</param>
        public PointOfInterest(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Overloaded constructor for Point of Interest that includes Name, CityId, and an optional Description.
        /// </summary>
        /// <param name="name">Name of the Point of Interest</param>
        /// <param name="cityId">The ID of the city this Point of Interest belongs to</param>
        /// <param name="description">Optional description of the Point of Interest</param>
        public PointOfInterest(string name, int cityId, string? description = null)
        {
            Name = name;
            CityId = cityId;
            Description = description;
        }
    }
}
