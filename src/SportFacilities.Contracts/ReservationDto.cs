namespace SportFacilities.Contracts
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public int SportFacilityId { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string Description { get; set; }
    }
}
