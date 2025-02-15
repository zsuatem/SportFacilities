using SportFacilities.Backend.Data;
using SportFacilities.Backend.Entities;
using SportFacilities.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportFacilities.Backend.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly TimeZoneInfo _polishTimeZone;

        public ReservationService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
            _polishTimeZone = GetPolishTimeZone();
        }

        // Metoda pomocnicza – próbuje pobrać strefę Windows, a w razie niepowodzenia – strefę IANA.
        private TimeZoneInfo GetPolishTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw");
            }
        }

        // Konwertuje dowolny DateTimeOffset do polskiego czasu
        private DateTimeOffset ConvertToPolishTime(DateTimeOffset dtoTime)
        {
            return TimeZoneInfo.ConvertTime(dtoTime, _polishTimeZone);
        }

        public async Task<List<ReservationDto>> GetReservationsByFacilityIdAsync(int facilityId)
        {
            return await _context.Reservations
                .Where(r => r.SportFacilityId == facilityId)
                .Include(r => r.User)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    SportFacilityId = r.SportFacilityId,
                    UserId = r.UserId,
                    UserEmail = r.User.Email,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    Description = r.Description
                })
                .ToListAsync();
        }

        public async Task<List<ReservationDto>> GetAllReservationsAsync()
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.SportFacility)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    SportFacilityId = r.SportFacilityId,
                    UserId = r.UserId,
                    UserEmail = r.User.Email,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    Description = r.Description
                })
                .ToListAsync();
        }

        public async Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto, string userId)
        {
            var facility = await _context.SportFacilities
                .Include(f => f.Availabilities)
                .FirstOrDefaultAsync(f => f.Id == dto.SportFacilityId);

            if (facility == null)
                throw new InvalidOperationException("Nie znaleziono obiektu sportowego.");

            // Konwersja do lokalnego czasu dla walidacji
            var localStartTime = ConvertToPolishTime(dto.StartTime);
            var localEndTime = ConvertToPolishTime(dto.EndTime);
            var dayOfWeek = localStartTime.DayOfWeek;

            var availability = facility.Availabilities.FirstOrDefault(a => a.DayOfWeek == dayOfWeek);
            if (availability == null || !availability.IsAvailable)
                throw new InvalidOperationException("Obiekt nie jest dostępny w wybranym dniu.");

            if (localStartTime.TimeOfDay < availability.OpeningTime || localEndTime.TimeOfDay > availability.ClosingTime)
                throw new InvalidOperationException("Wybrane godziny rezerwacji wykraczają poza godziny dostępności obiektu.");

            // Konwersja przekazanych czasów do UTC – tak jak zapisy w bazie
            var dtoStartUtc = dto.StartTime.ToUniversalTime();
            var dtoEndUtc = dto.EndTime.ToUniversalTime();
            
            var overlappingReservations = await _context.Reservations
                .Where(r => r.SportFacilityId == dto.SportFacilityId)
                .ToListAsync();

            foreach (var r in overlappingReservations)
            {
                if ((dtoStartUtc > r.StartTime && dtoStartUtc < r.EndTime) || (dtoEndUtc > r.StartTime && dtoEndUtc < r.EndTime) || (dtoStartUtc == r.StartTime && dtoEndUtc == r.EndTime))
                {
                    throw new InvalidOperationException("W wybranym terminie obiekt jest już zarezerwowany.");
                }
            }

            var reservation = new Reservation
            {
                SportFacilityId = dto.SportFacilityId,
                UserId = userId,
                StartTime = dtoStartUtc,
                EndTime = dtoEndUtc,
                Description = dto.Description
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                string subject = $"Potwierdzenie rezerwacji w {facility.Name}";
                string body = $"Twoja rezerwacja została dodana: <br>" +
                              $"Obiekt: {facility.Name}<br>" +
                              $"Data: {dto.StartTime:yyyy-MM-dd} {dto.StartTime:HH:mm} - {dto.EndTime:HH:mm}<br>" +
                              $"Opis: {dto.Description}";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }

            return new ReservationDto
            {
                Id = reservation.Id,
                SportFacilityId = reservation.SportFacilityId,
                UserId = reservation.UserId,
                UserEmail = (await _context.Users.FindAsync(userId))?.Email,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                Description = reservation.Description
            };
        }

        public async Task<ReservationDto> UpdateReservationAsync(int reservationId, UpdateReservationDto dto, string userId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.SportFacility)
                .ThenInclude(f => f.Availabilities)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null || reservation.UserId != userId)
                return null;

            var facility = reservation.SportFacility;
            var localStartTime = ConvertToPolishTime(dto.StartTime);
            var localEndTime = ConvertToPolishTime(dto.EndTime);
            var dayOfWeek = localStartTime.DayOfWeek;

            var availability = facility.Availabilities.FirstOrDefault(a => a.DayOfWeek == dayOfWeek);
            if (availability == null || !availability.IsAvailable)
                throw new InvalidOperationException("Obiekt nie jest dostępny w wybranym dniu.");

            if (localStartTime.TimeOfDay < availability.OpeningTime || localEndTime.TimeOfDay > availability.ClosingTime)
                throw new InvalidOperationException("Wybrane godziny rezerwacji wykraczają poza godziny dostępności obiektu.");

            var dtoStartUtc = dto.StartTime.ToUniversalTime();
            var dtoEndUtc = dto.EndTime.ToUniversalTime();

            // Logika walidacji przy edycji – wykluczamy aktualizowaną rezerwację i sprawdzamy nakładanie.
            var overlappingReservations = await _context.Reservations
                .Where(r => r.SportFacilityId == facility.Id && r.Id != reservationId)
                .ToListAsync();
            
            foreach (var r in overlappingReservations)
            {
                if ((dtoStartUtc > r.StartTime && dtoStartUtc < r.EndTime) || (dtoEndUtc > r.StartTime && dtoEndUtc < r.EndTime) || (dtoStartUtc == r.StartTime && dtoEndUtc == r.EndTime))
                {
                    throw new InvalidOperationException("W wybranym terminie obiekt jest już zarezerwowany.");
                }
            }

            reservation.StartTime = dtoStartUtc;
            reservation.EndTime = dtoEndUtc;
            reservation.Description = dto.Description;

            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                string subject = $"Zmiana rezerwacji w {reservation.SportFacility.Name}";
                string body = $"Twoja rezerwacja została zmieniona: <br>" +
                              $"Nowa data: {dto.StartTime:yyyy-MM-dd} {dto.StartTime:HH:mm} - {dto.EndTime:HH:mm}<br>" +
                              $"Nowy opis: {dto.Description}";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }

            return new ReservationDto
            {
                Id = reservation.Id,
                SportFacilityId = reservation.SportFacilityId,
                UserId = reservation.UserId,
                UserEmail = (await _context.Users.FindAsync(userId))?.Email,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                Description = reservation.Description
            };
        }

        public async Task<bool> DeleteReservationAsync(int reservationId, string userId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.SportFacility)
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.UserId == userId);

            if (reservation == null) return false;

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);
            if (user != null && reservation.SportFacility != null)
            {
                string subject = $"Usunięcie rezerwacji w {reservation.SportFacility.Name}";
                string body = "Twoja rezerwacja została usunięta.";
                await _emailService.SendEmailAsync(user.Email, subject, body);
            }

            return true;
        }
    }
}
