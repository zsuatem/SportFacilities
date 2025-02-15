using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SportFacilities.Contracts;
using SportFacilities.Backend.Services;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SportFacilities.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ControllerBase> _logger;

        public ReservationsController(IReservationService reservationService, UserManager<IdentityUser> userManager, ILogger<ControllerBase> logger)
        {
            _reservationService = reservationService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("facility/{facilityId}")]
        public async Task<ActionResult<List<ReservationDto>>> GetReservationsForFacility(int facilityId)
        {
            var reservations = await _reservationService.GetReservationsByFacilityIdAsync(facilityId);
            return Ok(reservations);
        }

        [HttpPost]
        public async Task<ActionResult<ReservationDto>> CreateReservation(CreateReservationDto dto)
        {
            _logger.LogError("{dto}", dto);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            try
            {
                var reservation = await _reservationService.CreateReservationAsync(dto, user.Id);
                return CreatedAtAction(nameof(GetReservationsForFacility), new { facilityId = dto.SportFacilityId }, reservation);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ReservationDto>> UpdateReservation(int id, UpdateReservationDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            try
            {
                var updatedReservation = await _reservationService.UpdateReservationAsync(id, dto, user.Id);
                if (updatedReservation == null)
                    return NotFound(new { message = "Rezerwacja nie została znaleziona lub nie masz uprawnień do jej edycji." });

                return Ok(updatedReservation);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await _reservationService.DeleteReservationAsync(id, user.Id);
            if (!result)
                return NotFound(new { message = "Rezerwacja nie została znaleziona lub nie masz uprawnień do jej usunięcia." });

            return NoContent();
        }
    }
}
