using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportFacilities.Backend.Entities
{
    public class FacilityAvailability
    {
        [Key] public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public bool IsAvailable { get; set; }
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }
        
        public int SportFacilityId { get; set; }
        [ForeignKey("SportFacilityId")] public SportFacility SportFacility { get; set; }
    }
}
