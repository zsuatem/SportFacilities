namespace SportFacilities.Backend.Entities
{
    public class SportFacility
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
        public ICollection<FacilityAvailability> Availabilities { get; set; }
    }
}
