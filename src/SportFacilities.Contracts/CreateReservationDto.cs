namespace SportFacilities.Contracts
{
    public class CreateReservationDto
    {
        public int SportFacilityId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string Description { get; set; }
    }
}
