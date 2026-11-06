-- One row per seat per event. Created in bulk from the venue manifest when an event goes on sale.
-- PriceId is locked (set) when the seat moves to Booked.
-- HeldByUserId / HoldUntil track the 15-minute hold window.
CREATE TABLE [tkt].[EventSeat]
(
    [EventSeatId]  BIGINT           IDENTITY (1, 1) NOT NULL,
    [EventId]      INT              NOT NULL,
    [VenueSeatId]  INT              NOT NULL,
    [PriceId]      INT              NULL,
    [SeatStateId]  TINYINT          NOT NULL CONSTRAINT [DF_EventSeat_SeatStateId] DEFAULT 1,
    [HeldByUserId] UNIQUEIDENTIFIER NULL,
    [HoldUntil]    DATETIME2 (0)    NULL,
    [CreatedAt]    DATETIME2 (0)    NOT NULL CONSTRAINT [DF_EventSeat_CreatedAt] DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_EventSeat]                     PRIMARY KEY CLUSTERED ([EventSeatId] ASC),
    CONSTRAINT [UQ_EventSeat_EventId_VenueSeatId] UNIQUE ([EventId], [VenueSeatId]),
    CONSTRAINT [FK_EventSeat_Event]     FOREIGN KEY ([EventId])     REFERENCES [tkt].[Event]     ([EventId]),
    CONSTRAINT [FK_EventSeat_VenueSeat] FOREIGN KEY ([VenueSeatId]) REFERENCES [tkt].[VenueSeat] ([VenueSeatId]),
    CONSTRAINT [FK_EventSeat_Price]     FOREIGN KEY ([PriceId])     REFERENCES [tkt].[Price]     ([PriceId]),
    CONSTRAINT [FK_EventSeat_SeatState] FOREIGN KEY ([SeatStateId]) REFERENCES [tkt].[SeatState] ([SeatStateId])
);
