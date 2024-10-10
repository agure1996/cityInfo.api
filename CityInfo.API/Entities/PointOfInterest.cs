using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Entities
{
    /// <summary>
    ///  Entity class representing Point of Interest Object in a Database, is tightly coupled with the Database schema, is required for object to be saved to and retrieved from the Database 
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
        /// since pointofinterest is part of city, adding city property sets up a relation between the two that some dbs can map and use for navigation
        /// we set cityid as the foreign key
        /// </summary>

        [ForeignKey("CityId")]
        public City? City { get; set; }

        /// <summary>
        /// City id FK
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// Constructor of Point of Interest
        /// </summary>
        /// <param name="name">Name of Point of Interest</param>
        public PointOfInterest(string name) { Name = name; }
    }
}
