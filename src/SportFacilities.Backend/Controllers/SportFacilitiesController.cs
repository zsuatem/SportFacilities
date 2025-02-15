using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportFacilities.Backend.Services;
using SportFacilities.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportFacilities.Backend.Controllers
{
    [ApiController]
    [Route("api/facilities")]
    public class SportFacilitiesController : ControllerBase
    {
        private readonly ISportFacilityService _facilityService;
        private readonly IReservationService _reservationService;

        public SportFacilitiesController(ISportFacilityService facilityService, IReservationService reservationService)
        {
            _facilityService = facilityService;
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SportFacilityDto>>> GetAll()
        {
            var facilities = await _facilityService.GetAllFacilitiesAsync();
            return Ok(facilities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SportFacilityDto>> GetById(int id)
        {
            var facility = await _facilityService.GetFacilityByIdAsync(id);
            if (facility == null) return NotFound();
            return Ok(facility);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<SportFacilityDto>> Create(SportFacilityDto facilityDto)
        {
            var createdFacility = await _facilityService.CreateFacilityAsync(facilityDto);
            return CreatedAtAction(nameof(GetById), new { id = createdFacility.Id }, createdFacility);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(int id, SportFacilityDto facilityDto)
        {
            var result = await _facilityService.UpdateFacilityAsync(id, facilityDto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _facilityService.DeleteFacilityAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        
        [HttpGet("{id}/reservations")]
        public async Task<ActionResult<List<ReservationDto>>> GetReservations(int id)
        {
            if (id == 0)
            {
                var allReservations = await _reservationService.GetAllReservationsAsync();
                return Ok(allReservations);
            }
            else
            {
                var reservations = await _reservationService.GetReservationsByFacilityIdAsync(id);
                if (reservations == null || reservations.Count == 0)
                {
                    return NotFound("Brak rezerwacji dla tego obiektu.");
                }

                return Ok(reservations);
            }
        }
    }
}
