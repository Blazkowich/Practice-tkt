CREATE TABLE [tkt].[CartItem]
(
    [CartItemId]  INT              IDENTITY (1, 1) NOT NULL,
    [CartId]      UNIQUEIDENTIFIER NOT NULL,
    [EventSeatId] BIGINT           NOT NULL,
    [AddedAt]     DATETIME2 (0)    NOT NULL CONSTRAINT [DF_CartItem_AddedAt] DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_CartItem]                   PRIMARY KEY CLUSTERED ([CartItemId] ASC),
    CONSTRAINT [UQ_CartItem_CartId_EventSeatId] UNIQUE ([CartId], [EventSeatId]),
    CONSTRAINT [FK_CartItem_Cart]      FOREIGN KEY ([CartId])      REFERENCES [tkt].[Cart]      ([CartId]),
    CONSTRAINT [FK_CartItem_EventSeat] FOREIGN KEY ([EventSeatId]) REFERENCES [tkt].[EventSeat] ([EventSeatId])
);
