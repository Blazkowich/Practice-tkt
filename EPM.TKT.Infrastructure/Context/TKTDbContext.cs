using EPM.TKT.Infrastructure.Entities.Sec;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Context;

internal class TKTDbContext(DbContextOptions<TKTDbContext> options) : DbContext(options), ITKTDbContext
{
    // sec — identity & access
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // tkt — lookup / reference
    public DbSet<SeatType> SeatTypes { get; set; }
    public DbSet<SeatState> SeatStates { get; set; }
    public DbSet<PriceCategory> PriceCategories { get; set; }
    public DbSet<EventStatus> EventStatuses { get; set; }

    // tkt — venue manifest
    public DbSet<Venue> Venues { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<Row> Rows { get; set; }
    public DbSet<VenueSeat> VenueSeats { get; set; }

    // tkt — event & pricing
    public DbSet<Event> Events { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<Price> Prices { get; set; }

    // tkt — transactional
    public DbSet<EventSeat> EventSeats { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    // Explicit interface members not provided by DbContext base
    Task<int> ITKTDbContext.SaveChangesAsync(CancellationToken cancellationToken)
        => base.SaveChangesAsync(cancellationToken);

    public void ClearChangeTracker() => ChangeTracker.Clear();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
    }
}
