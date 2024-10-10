namespace CityInfo.API.Models
{   

    /// <summary>
    /// DTO for Point of Interest Object
    /// </summary>
    public class PointsOfInterestDTO
    {
        /// <summary>
        /// Id of Point of Interest
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of Point of Interest
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of Point of Interest
        /// </summary>
        public string? Description { get; set; }
        
      
    }
}
