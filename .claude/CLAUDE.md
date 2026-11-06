# EPM-TKT — Event Ticketing System

## Project Overview

An online event ticketing system built with .NET 10 / ASP.NET Core Web API. Customers search for events, pick seats, hold them, pay, and receive digital tickets. Event Managers configure venues, manifests, and offers.

## Solution Structure

| Project | Layer | Responsibility |
|---|---|---|
| `EPM.TKT.WebApi` | Presentation | HTTP controllers, middleware, OpenAPI, app entry point |
| `EPM.TKT.App` | Application | Commands, queries, use-case handlers (MediatR pattern) |
| `EPM.TKT.Domain` | Domain | Entities, value objects, domain events, business rules |
| `EPM.TKT.Infrastructure` | Infrastructure | EF Core DbContext, repositories, DB migrations |
| `EPM.TKT.External` | Integration | Clients for Payment Service and Notification Service |
| `EPM.TKT.Mediator` | Cross-cutting | MediatR pipeline behaviours (validation, logging) |
| `EPM.TKT.Tests` | Tests | Unit and integration tests |

Solution file: `EPM.TKT.WebApi.slnx` (root)

## Domain Vocabulary

- **Event** – container for venue, date/time, and metadata
- **Venue** – physical location where an event takes place
- **Manifest** – complete seat map for a venue; hierarchy is `Section → Row → Seat`
- **Seat** – smallest purchasable unit; either *Reserved* (specific row/seat) or *General Admission* (anywhere in a section)
- **Offer** – configuration of how seats are sold (price levels, rules)
- **Price** – ticket price level (Adult, Child, VIP, etc.); set at section or row level
- **Ticket** – digital/printed proof of purchase containing event, venue, seat, and date info
- **Customer** – authenticated user who searches and buys tickets
- **Event Manager** – admin user who configures venues, manifests, and offers

## Seat State Machine

```
start ──► Available ◄──────────────────────────────┐
              │                                     │
  customer    │ hold seat (price locked, 15-min     │ ticket cancelled
  adds to     │ timer starts)                       │ by Event Manager
  cart        ▼                                     │
           Booked ──────────────────────────► Sold ─┘
              │  payment succeeded                  │
              │                                     │ event finished
  hold expired│ or customer removes from cart       ▼
  (bg worker) └──────────────────────────────────► end
```

Transitions:
1. `start → Available`: seat created from manifest, price set from section/row
2. `Available → Booked`: customer holds seat; price locked; 15-min hold timer starts
3. `Booked → Available`: customer removes from cart
4. `Booked → Available`: hold expired or payment retries exhausted (background worker)
5. `Booked → Booked`: payment failed, retry (max 3–5 retries, background worker)
6. `Booked → Sold`: payment succeeded
7. `Sold → Available`: ticket cancelled by Event Manager
8. `Sold → end`: event finished

## Architecture

Two client types (Web UI / Admin UI) communicate via REST/HTTPS with the Backend API, which dispatches to internal services:

| # | Service | Responsibility |
|---|---|---|
| 1 | Auth Service | Authentication (Customer + Event Manager) and authorization |
| 2 | Event & Venue Service | Event metadata and Venue definitions |
| 3 | Manifest & Offer Service | Seat maps (Reserved & General Admission) and pricing config |
| 4 | Search Service | Event discovery and seat availability reads |
| 5 | Seat Inventory Service | Atomic seat locking/holding/releasing (prevents double-booking) |
| 6 | Cart Service | Temporary seat/offer selection before purchase |
| 7 | Order & Ticket Service | Final purchase, digital ticket generation, cancellations |
| 8 | Payment Service (Ext) | Card validation and financial transaction processing |
| 9 | Notification Service (Ext) | Email/SMS confirmations with generated tickets |

## Key Customer Flow (Sequence)

1. **Show price ranges** — `GET /events/{eventId}/price-ranges`
2. **Find available seats** — `GET /events/{eventId}/seats?minPrice=&maxPrice=&orderBy=`
3. **Hold seat (add to cart)** — `POST /events/{eventId}/cart/items {seatId}` (Bearer token required); DB: `UPDATE event_seats SET state='Booked', holdUntil=now+15min WHERE state='Available'`; if 0 rows → seat taken, show next cheapest
4. **Pay** — `POST /events/{eventId}/orders {cartId, paymentToken}` (Bearer token required); on success: `UPDATE state='Sold'` + `INSERT ticket` + send notification; on failure: release seat back to Available

## Use Cases

**Customer:** Search Events · Check Seat Availability · Manage Cart (Add / Remove / View) · Make Purchase · Payment · Authentication · Be Notified

**Event Manager:** Authentication · Authorization · Cancel Ticket · Manage Venues · Manage Events

## Build & Run

```bash
# restore & build
dotnet build EPM.TKT.WebApi.slnx

# run web API
dotnet run --project EPM.TKT.WebApi

# run tests
dotnet test EPM.TKT.WebApi.slnx
```

## Tech Stack

- **.NET 10** / **ASP.NET Core** Web API
- **OpenAPI** (via `Microsoft.AspNetCore.OpenApi`)
- **MediatR** (command/query dispatching via `EPM.TKT.Mediator`)
- **Entity Framework Core** (infrastructure layer)
- Nullable reference types enabled; implicit usings enabled
