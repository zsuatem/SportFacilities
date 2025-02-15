using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportFacilities.Backend.Entities;

namespace SportFacilities.Backend.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<SportFacility> SportFacilities { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<FacilityAvailability> FacilityAvailabilities { get; set; }

}
