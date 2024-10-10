using CityInfo.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    /// <summary>
    ///  Entity class representing City Object in a Database, is tightly coupled with the Database schema, is required for object to be saved to and retrieved from the Database 
    /// </summary>
    public class City
    {

        /**
         * You will notice in our DTO we dont add requirements but you will find requirements here in this class
         * And that is because DTOs are only used to get data, not manipulate it so hence no need for regulations
         */

        /// <summary>
        /// Id of City (id column in db)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        public int Id { get; set; }

        /// <summary>
        /// Name of city (name column in db)
        /// </summary>
        [Required]
        [MaxLength(50)] 
        public string Name { get; set; }


        /// <summary>
        /// Description of City (description column in db), optional
        /// </summary>
        [MaxLength(200)]
        public string?   Description { get; set; }

        /// <summary>
        /// Points of Interests in the form of List Collection
        /// </summary>
        public ICollection<PointOfInterest> PointsOfInterest { get; set; }
        = new List<PointOfInterest>();

        /// <summary>
        /// Constructor of City class
        /// </summary>
        /// <param name="name">Name of City</param>
        public City(string name) {  Name = name; }
    }
}
