using System;
using System.Collections.Generic;

namespace SportFacilities.Contracts
{
    public class SportFacilityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

        public List<FacilityAvailabilityDto> Availabilities { get; set; }
    }
}
