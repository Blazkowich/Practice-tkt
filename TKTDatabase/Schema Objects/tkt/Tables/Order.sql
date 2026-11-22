CREATE TABLE [tkt].[Order]
(
    [OrderId]              UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Order_OrderId] DEFAULT NEWSEQUENTIALID(),
    [UserId]               UNIQUEIDENTIFIER NOT NULL,
    [EventId]              INT              NOT NULL,
    [CartId]               UNIQUEIDENTIFIER NOT NULL,
    [OrderStatusId]        TINYINT          NOT NULL CONSTRAINT [DF_Order_OrderStatusId] DEFAULT 1,
    [TotalAmount]          DECIMAL (10, 2)  NOT NULL,
    [PaymentTransactionId] NVARCHAR(200)    NULL,
    [CreatedAt]            DATETIME2 (0)    NOT NULL CONSTRAINT [DF_Order_CreatedAt] DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Order]             PRIMARY KEY CLUSTERED ([OrderId] ASC),
    CONSTRAINT [FK_Order_User]        FOREIGN KEY ([UserId])        REFERENCES [sec].[User]          ([UserId]),
    CONSTRAINT [FK_Order_Event]       FOREIGN KEY ([EventId])       REFERENCES [tkt].[Event]         ([EventId]),
    CONSTRAINT [FK_Order_Cart]        FOREIGN KEY ([CartId])        REFERENCES [tkt].[Cart]          ([CartId]),
    CONSTRAINT [FK_Order_OrderStatus] FOREIGN KEY ([OrderStatusId]) REFERENCES [tkt].[OrderStatus]  ([OrderStatusId])
);
