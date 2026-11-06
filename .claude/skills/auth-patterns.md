## User roles

EPM TKT has two authenticated roles:

- **Customer** — authenticated user who searches events, holds seats, and purchases tickets
- **EventManager** — admin user who manages venues, events, manifests, offers, and can cancel tickets

Public endpoints (event search, price ranges, seat availability reads) require no authentication.

## Controller setup

Every controller must carry `[ApiController]` and a route:

```csharp
[ApiController]
[Route("api/[controller]")]
public class EventsController(ISender sender) : ControllerBase
```

Authorization is applied at the action level, not the controller level, so public and protected actions can coexist on the same controller.

## Action-level authorization

```csharp
// Public — no attribute
[HttpGet("{eventId}/seats")]
public async Task<IActionResult> GetAvailableSeats(Guid eventId, ...)

// Customer-only (Bearer token required — holds seat, makes purchase)
[Authorize(Roles = "Customer")]
[HttpPost("{eventId}/cart/items")]
public async Task<IActionResult> AddToCart(Guid eventId, ...)

// Event Manager only
[Authorize(Roles = "EventManager")]
[HttpPost("{eventId}/cancel-ticket/{ticketId}")]
public async Task<IActionResult> CancelTicket(Guid eventId, Guid ticketId, ...)
```

## Roles source

Roles are stored in the JWT claims issued by the Auth Service. ASP.NET Core's built-in `[Authorize(Roles = "...")]` reads the standard `ClaimTypes.Role` claim. Do not create custom authorization attributes unless a business rule cannot be expressed with roles alone.

## AllowAnonymous override

To open a single action on an otherwise-authorized controller:

```csharp
[AllowAnonymous]
[HttpGet("ping")]
public IActionResult Ping() => Ok();
```
