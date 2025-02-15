using Microsoft.AspNetCore.Identity;

namespace SportFacilities.Backend.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        public int SportFacilityId { get; set; }
        public SportFacility SportFacility { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public string Description { get; set; }
    }
}
