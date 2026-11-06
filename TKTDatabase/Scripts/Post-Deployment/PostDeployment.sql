/*
Post-Deployment Script
--------------------------------------------------------------------------------------
 SQLCMD :r is a preprocessor directive processed before SQL execution.
 The IF '$(DataSource)' = 'TestData' block controls whether the included
 test-data scripts actually execute — the files themselves contain no guards.
--------------------------------------------------------------------------------------
*/

--------------------------------------------------------------------------------------
-- REFERENCE DATA  (always seeded — idempotent MERGE)
--------------------------------------------------------------------------------------
:r ".\Reference Data\tkt\SeatType.sql"
:r ".\Reference Data\tkt\SeatState.sql"
:r ".\Reference Data\tkt\PriceCategory.sql"
:r ".\Reference Data\tkt\EventStatus.sql"
:r ".\Reference Data\sec\Role.sql"

--------------------------------------------------------------------------------------
-- TEST DATA  (runs only when DataSource = TestData)
-- Ordered by dependency: referenced tables before referencing tables.
--------------------------------------------------------------------------------------

IF '$(DataSource)' = 'TestData'
BEGIN
    :r ".\TestData\tkt\Venues.sql"
    :r ".\TestData\tkt\Sections.sql"
    :r ".\TestData\tkt\Rows.sql"
    :r ".\TestData\tkt\VenueSeats.sql"
    :r ".\TestData\tkt\Events.sql"
    :r ".\TestData\tkt\Offers.sql"
    :r ".\TestData\tkt\Prices.sql"
    :r ".\TestData\tkt\EventSeats.sql"
    :r ".\TestData\sec\Users.sql"
    :r ".\TestData\sec\UserRoles.sql"
    :r ".\TestData\tkt\Carts.sql"
    :r ".\TestData\tkt\CartItems.sql"
    :r ".\TestData\tkt\Orders.sql"
    :r ".\TestData\tkt\Tickets.sql"
END
