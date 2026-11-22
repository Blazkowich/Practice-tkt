CREATE TABLE [tkt].[Ticket]
(
    [TicketId]             UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Ticket_TicketId] DEFAULT NEWSEQUENTIALID(),
    [OrderId]              UNIQUEIDENTIFIER NOT NULL,
    [EventId]              INT              NOT NULL,
    [EventSeatId]          BIGINT           NOT NULL,
    [UserId]               UNIQUEIDENTIFIER NOT NULL,
    [PriceId]              INT              NOT NULL,
    [PaidAmount]           DECIMAL (10, 2)  NOT NULL,
    [IssuedAt]             DATETIME2 (0)    NOT NULL CONSTRAINT [DF_Ticket_IssuedAt] DEFAULT GETUTCDATE(),
    [CancelledAt]          DATETIME2 (0)    NULL,
    [CancelledByManagerId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Ticket]             PRIMARY KEY CLUSTERED ([TicketId] ASC),
    CONSTRAINT [UQ_Ticket_EventSeatId] UNIQUE ([EventSeatId]),
    CONSTRAINT [FK_Ticket_Order]       FOREIGN KEY ([OrderId])     REFERENCES [tkt].[Order]     ([OrderId]),
    CONSTRAINT [FK_Ticket_Event]       FOREIGN KEY ([EventId])     REFERENCES [tkt].[Event]     ([EventId]),
    CONSTRAINT [FK_Ticket_EventSeat]   FOREIGN KEY ([EventSeatId]) REFERENCES [tkt].[EventSeat] ([EventSeatId]),
    CONSTRAINT [FK_Ticket_User]        FOREIGN KEY ([UserId])      REFERENCES [sec].[User]      ([UserId]),
    CONSTRAINT [FK_Ticket_Price]       FOREIGN KEY ([PriceId])     REFERENCES [tkt].[Price]     ([PriceId])
);
