namespace SportFacilities.Contracts
{
    public class UpdateReservationDto
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string Description { get; set; }
    }
}
