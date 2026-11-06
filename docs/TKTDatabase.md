# TKTDatabase — SQL Database Project

SQL Server Database Project (SSDT / DACPAC) for the EPM-TKT event ticketing system. Objects are split across two schemas and target **Azure SQL Database (V12)**.

| Schema | Responsibility | Tables |
|---|---|---|
| `sec` | Security, identity, and access control | `Role`, `User`, `UserRole` |
| `tkt` | Ticketing domain — venues, events, seat inventory, orders | everything else |

`tkt` is the project `DefaultSchema` (auto-registered). `sec` is explicitly defined in `Schema Objects\Schemas\sec.sql`.

## Prerequisites

- **Visual Studio 2022** with the *SQL Server Data Tools* (SSDT) workload  
  _or_  
  **sqlpackage** CLI — [download](https://learn.microsoft.com/en-us/sql/tools/sqlpackage/sqlpackage-download)

## Project Structure

```
TKTDatabase/
├── Schema Objects/
│   └── tkt/
│       ├── Tables/           # 20 CREATE TABLE files (one per table)
│       └── Indexes/          # 14 CREATE INDEX files (one per table that has indexes)
├── Scripts/
│   └── Post-Deployment/
│       ├── PostDeployment.sql          # Entry point — :r includes in dependency order
│       ├── Reference Data/tkt/         # 5 lookup-table MERGE scripts (always run)
│       └── Test Data/tkt/              # 15 domain-data MERGE scripts (guarded by DataSource)
└── TKTDatabase.sqlproj
```

## Schema: `tkt`

### Lookup Tables

Seeded on every deploy. Never reference these by integer literal in application code — use the domain enum constants that map to these IDs.

| Table | Purpose | Values |
|---|---|---|
| `SeatType` | How seats in a section are sold | `1` Reserved · `2` GeneralAdmission |
| `SeatState` | Lifecycle state of a seat for an event | `1` Available · `2` Booked · `3` Sold |
| `PriceCategory` | Ticket price tier | `1` Adult · `2` Child · `3` VIP · `4` Student · `5` Senior |
| `EventStatus` | Publication state of an event | `1` Draft · `2` Published · `3` OnSale · `4` Completed · `5` Cancelled |
| `Role` | System user role | `1` User · `2` Moderator · `3` Admin |

---

### Venue Manifest Tables

These define the **physical layout** of a venue. They are created once per venue and reused across all events held there.

#### `Venue`

| Column | Type | Notes |
|---|---|---|
| `VenueId` | `INT IDENTITY` PK | |
| `Name` | `NVARCHAR(200)` | |
| `Address` | `NVARCHAR(500)` | nullable |
| `City` | `NVARCHAR(100)` | nullable |
| `Country` | `NVARCHAR(100)` | nullable |
| `CreatedAt` | `DATETIME2(0)` | default `GETUTCDATE()` |
| `IsActive` | `BIT` | default `1` |

#### `Section`

A named seating area within a venue (e.g., `E35`, `N22`). Each section has a `SeatTypeId` that applies to all seats within it.

| Column | Type | Notes |
|---|---|---|
| `SectionId` | `INT IDENTITY` PK | |
| `VenueId` | `INT` FK → `Venue` | |
| `Name` | `NVARCHAR(100)` | e.g., `E35`, `N22` |
| `SeatTypeId` | `TINYINT` FK → `SeatType` | Reserved or GeneralAdmission |
| `SortOrder` | `INT` | display order |

#### `Row`

A row within a **Reserved** section. General Admission sections have no rows.

| Column | Type | Notes |
|---|---|---|
| `RowId` | `INT IDENTITY` PK | |
| `SectionId` | `INT` FK → `Section` | |
| `Name` | `NVARCHAR(50)` | e.g., `A`, `B`, `1` |
| `SortOrder` | `INT` | |

#### `VenueSeat`

The smallest physical unit in the venue manifest. `RowId` and `SeatNumber` are `NULL` for General Admission seats.

| Column | Type | Notes |
|---|---|---|
| `VenueSeatId` | `INT IDENTITY` PK | |
| `SectionId` | `INT` FK → `Section` | |
| `RowId` | `INT` FK → `Row` | **NULL** for GA |
| `SeatNumber` | `NVARCHAR(20)` | **NULL** for GA |
| `SortOrder` | `INT` | |

---

### Event & Pricing Tables

#### `Event`

| Column | Type | Notes |
|---|---|---|
| `EventId` | `INT IDENTITY` PK | |
| `VenueId` | `INT` FK → `Venue` | |
| `Name` | `NVARCHAR(200)` | |
| `Description` | `NVARCHAR(MAX)` | nullable |
| `EventDate` | `DATETIME2(0)` | |
| `DoorsOpenAt` | `DATETIME2(0)` | nullable |
| `EventStatusId` | `TINYINT` FK → `EventStatus` | default `1` (Draft) |
| `CreatedAt` | `DATETIME2(0)` | default `GETUTCDATE()` |

#### `Offer`

A sale window attached to an event. An event can have multiple offers (e.g., early-bird, general sale).

| Column | Type | Notes |
|---|---|---|
| `OfferId` | `INT IDENTITY` PK | |
| `EventId` | `INT` FK → `Event` | |
| `Name` | `NVARCHAR(200)` | |
| `SaleStartAt` | `DATETIME2(0)` | |
| `SaleEndAt` | `DATETIME2(0)` | nullable |
| `IsActive` | `BIT` | default `1` |
| `CreatedAt` | `DATETIME2(0)` | default `GETUTCDATE()` |

#### `Price`

A price level within an offer. Scoped to a section and optionally a row. `SectionId NULL` means the price applies to all sections; `RowId NULL` means all rows in the section.

| Column | Type | Notes |
|---|---|---|
| `PriceId` | `INT IDENTITY` PK | |
| `OfferId` | `INT` FK → `Offer` | |
| `SectionId` | `INT` FK → `Section` | nullable (applies to all sections if NULL) |
| `RowId` | `INT` FK → `Row` | nullable (applies to all rows if NULL) |
| `PriceCategoryId` | `TINYINT` FK → `PriceCategory` | |
| `Amount` | `DECIMAL(10,2)` | |
| `Currency` | `CHAR(3)` | default `'USD'` |

---

### Identity Tables

#### `User`

The single identity type for all authenticated users — ticket buyers, Event Managers, Moderators, and Admins alike. Role assignments in `UserRole` determine what each user can do.

| Column | Type | Notes |
|---|---|---|
| `UserId` | `UNIQUEIDENTIFIER` PK | default `NEWSEQUENTIALID()` |
| `ExternalUserId` | `NVARCHAR(200)` UNIQUE | ID from the Auth Service |
| `Email` | `NVARCHAR(200)` | |
| `FirstName` | `NVARCHAR(100)` | nullable |
| `LastName` | `NVARCHAR(100)` | nullable |
| `CreatedAt` | `DATETIME2(0)` | default `GETUTCDATE()` |
| `IsActive` | `BIT` | default `1` |

#### `UserRole`

Many-to-many join between `User` and `Role`. Composite PK `(UserId, RoleId)`.

| Column | Type | Notes |
|---|---|---|
| `UserId` | `UNIQUEIDENTIFIER` FK → `User` | PK (part 1) |
| `RoleId` | `TINYINT` FK → `Role` | PK (part 2) |
| `AssignedAt` | `DATETIME2(0)` | default `GETUTCDATE()` |

---

### Transactional Tables

#### `EventSeat`

**The core state table.** One row per physical seat per event, created from the venue manifest when an event goes on sale. The atomic `UPDATE … WHERE state = 'Available'` on this table is what prevents double-booking.

| Column | Type | Notes |
|---|---|---|
| `EventSeatId` | `BIGINT IDENTITY` PK | |
| `EventId` | `INT` FK → `Event` | |
| `VenueSeatId` | `INT` FK → `VenueSeat` | UNIQUE together with `EventId` |
| `PriceId` | `INT` FK → `Price` | **NULL** until seat is Booked (price locked on hold) |
| `SeatStateId` | `TINYINT` FK → `SeatState` | default `1` (Available) |
| `HeldByUserId` | `UNIQUEIDENTIFIER` | **NULL** unless Booked |
| `HoldUntil` | `DATETIME2(0)` | **NULL** unless Booked; 15-minute window |
| `CreatedAt` | `DATETIME2(0)` | default `GETUTCDATE()` |

Key indexes:
- `IX_EventSeat_EventId_SeatStateId` — primary read path for seat availability queries
- `IX_EventSeat_HoldUntil` (filtered, `WHERE HoldUntil IS NOT NULL`) — background worker that releases expired holds

#### `Cart`

A user's temporary basket for a single event.

| Column | Type | Notes |
|---|---|---|
| `CartId` | `UNIQUEIDENTIFIER` PK | default `NEWSEQUENTIALID()` |
| `UserId` | `UNIQUEIDENTIFIER` FK → `sec.User` | |
| `EventId` | `INT` FK → `Event` | |
| `CreatedAt` | `DATETIME2(0)` | default `GETUTCDATE()` |
| `ExpiresAt` | `DATETIME2(0)` | nullable; tracks overall cart expiry |

#### `CartItem`

A seat held in a cart. UNIQUE `(CartId, EventSeatId)` prevents the same seat being added twice.

| Column | Type | Notes |
|---|---|---|
| `CartItemId` | `INT IDENTITY` PK | |
| `CartId` | `UNIQUEIDENTIFIER` FK → `Cart` | |
| `EventSeatId` | `BIGINT` FK → `EventSeat` | |
| `AddedAt` | `DATETIME2(0)` | default `GETUTCDATE()` |

#### `Order`

A completed purchase. Created after payment succeeds.

| Column | Type | Notes |
|---|---|---|
| `OrderId` | `UNIQUEIDENTIFIER` PK | default `NEWSEQUENTIALID()` |
| `UserId` | `UNIQUEIDENTIFIER` FK → `sec.User` | |
| `EventId` | `INT` FK → `Event` | |
| `CartId` | `UNIQUEIDENTIFIER` FK → `Cart` | the cart that was checked out |
| `TotalAmount` | `DECIMAL(10,2)` | |
| `PaymentTransactionId` | `NVARCHAR(200)` | nullable; from Payment Service |
| `CreatedAt` | `DATETIME2(0)` | default `GETUTCDATE()` |

#### `Ticket`

A digital ticket issued after a successful order. UNIQUE `(EventSeatId)` ensures one ticket per seat per event. `CancelledAt` is set by an Event Manager cancellation.

| Column | Type | Notes |
|---|---|---|
| `TicketId` | `UNIQUEIDENTIFIER` PK | default `NEWSEQUENTIALID()` |
| `OrderId` | `UNIQUEIDENTIFIER` FK → `Order` | |
| `EventId` | `INT` FK → `Event` | |
| `EventSeatId` | `BIGINT` FK → `EventSeat` | UNIQUE |
| `UserId` | `UNIQUEIDENTIFIER` FK → `sec.User` | |
| `PriceId` | `INT` FK → `Price` | price at time of purchase |
| `PaidAmount` | `DECIMAL(10,2)` | |
| `IssuedAt` | `DATETIME2(0)` | default `GETUTCDATE()` |
| `CancelledAt` | `DATETIME2(0)` | nullable |
| `CancelledByManagerId` | `UNIQUEIDENTIFIER` | nullable; set by Event Manager |

---

## Seat Lifecycle

The `SeatStateId` on `EventSeat` drives the seat state machine. See [seat-state-machine.png](images/seat-state-machine.png) for the full diagram.

| Transition | Trigger |
|---|---|
| start → Available | Seat created from manifest, price set from section/row |
| Available → Booked | Customer holds seat; price locked; 15-min timer starts |
| Booked → Available | Customer removes from cart |
| Booked → Available | Hold expired or payment retries exhausted (background worker) |
| Booked → Booked | Payment failed, retry (max 3–5) |
| Booked → Sold | Payment succeeded |
| Sold → Available | Ticket cancelled by Event Manager |
| Sold → end | Event finished |

---

## Post-Deployment Scripts

### Reference Data

Scripts in `Scripts/Post-Deployment/Reference Data/tkt/` seed the lookup tables. They run unconditionally on every deploy using idempotent `MERGE` statements. Safe to run multiple times.

### Test Data

Scripts in `Scripts/Post-Deployment/Test Data/tkt/` seed a realistic development dataset. Each script guards its own inserts with:

```sql
IF '$(DataSource)' = 'TestData'
BEGIN
    -- MERGE ...
END
```

The `DataSource` SQLCMD variable defaults to `TestData` in the project. Override it at deploy time to skip test data.

**Test dataset summary:**

| Data | Count | Notable states |
|---|---|---|
| Venues | 1 | Rice-Eccles Stadium |
| Sections | 3 | E35 (Reserved), N22 (GA), S44 (Reserved) |
| Venue seats | 18 | 10 reserved, 5 GA, 3 reserved |
| Events | 2 | Night 1 (OnSale), Night 2 (Published) |
| Event seats | 36 | Night 1: seat 6 Booked, seat 7 Sold; Night 2: all Available |
| Customers | 3 | Alice, Bob, Carol |
| Users | 3 | Admin, Moderator, User |

---

## Deployment

### Visual Studio

Right-click `TKTDatabase` project → **Publish** → configure target connection → Publish.

### CLI (`sqlpackage`)

```bash
# Deploy with test data (default)
sqlpackage /Action:Publish \
  /SourceFile:TKTDatabase\bin\Debug\TKTDatabase.dacpac \
  /TargetConnectionString:"Server=.;Database=TKT;Integrated Security=True;" \
  /v:DataSource=TestData

# Deploy without test data (production)
sqlpackage /Action:Publish \
  /SourceFile:TKTDatabase\bin\Release\TKTDatabase.dacpac \
  /TargetConnectionString:"Server=...;Database=TKT;..." \
  /v:DataSource=Production
```

---

## Adding New Schema Objects

When extending the database, follow this checklist:

1. **Table** — create `Schema Objects/tkt/Tables/tkt.[TableName].sql` with a single `CREATE TABLE` statement. No `GO`, no `CREATE INDEX`.
2. **Indexes** — create `Schema Objects/tkt/Indexes/tkt.[TableName].sql` with `CREATE INDEX` statements. One file per table.
3. **Register in project** — add `<Build Include="..."/>` entries to `TKTDatabase.sqlproj` in dependency order (referenced tables before referencing tables).
4. **Reference data** (if lookup table) — add `Scripts/Post-Deployment/Reference Data/tkt/[TableName].sql` with a `MERGE` statement and add its `:r` include to `PostDeployment.sql`.
5. **Test data** (if domain table) — add `Scripts/Post-Deployment/Test Data/tkt/[TableName].sql` with an `IF '$(DataSource)' = 'TestData' BEGIN ... END` guard and add its `:r` include to `PostDeployment.sql` (after its dependencies).
6. **Update this document** — add the new table to the appropriate section above.
