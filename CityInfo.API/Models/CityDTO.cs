namespace CityInfo.API.Models
{   
    /// <summary>
    /// DTO of City Object
    /// </summary>
    public class CityDTO
    {
        /// <summary>
        /// Id of City
        /// </summary>
        public int Id {  get; set; }

        /// <summary>
        /// Name of City, by default is empty
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of city
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Number of points of interest
        /// </summary>
        public int NumberOfPointsOfInterest
        {
            get
            {
                return PointsOfInterest.Count;
            }
        }
        /// <summary>
        /// Collection of points of interest
        /// </summary>
        public ICollection<PointsOfInterestDTO> PointsOfInterest { get; set; }
        = new List<PointsOfInterestDTO>();
    }
}
