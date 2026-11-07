using EPM.TKT.Infrastructure.Entities.Sec;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Context.Interfaces;

internal interface ITKTDbContext
{
    // sec — identity & access
    DbSet<Role> Roles { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<UserRole> UserRoles { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }

    // tkt — lookup / reference
    DbSet<SeatType> SeatTypes { get; set; }
    DbSet<SeatState> SeatStates { get; set; }
    DbSet<PriceCategory> PriceCategories { get; set; }
    DbSet<EventStatus> EventStatuses { get; set; }

    // tkt — venue manifest
    DbSet<Venue> Venues { get; set; }
    DbSet<Section> Sections { get; set; }
    DbSet<Row> Rows { get; set; }
    DbSet<VenueSeat> VenueSeats { get; set; }

    // tkt — event & pricing
    DbSet<Event> Events { get; set; }
    DbSet<Offer> Offers { get; set; }
    DbSet<Price> Prices { get; set; }

    // tkt — transactional
    DbSet<EventSeat> EventSeats { get; set; }
    DbSet<Cart> Carts { get; set; }
    DbSet<CartItem> CartItems { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<Ticket> Tickets { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    void ClearChangeTracker();
}
