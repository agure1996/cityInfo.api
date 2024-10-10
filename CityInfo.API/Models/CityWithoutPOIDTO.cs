namespace CityInfo.API.Models
{   
    /// <summary>
    /// DTO for City without any Points of Interests
    /// </summary>
    public class CityWithoutPOIDTO
    {
        /// <summary>
        /// Id of City
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of City
        /// </summary>
        public string Name { get; set; } = string.Empty;


        /// <summary>
        /// Description of City
        /// </summary>
        public string? Description { get; set; }

    }
}
