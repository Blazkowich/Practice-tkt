CREATE TABLE [tkt].[Cart]
(
    [CartId]    UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Cart_CartId] DEFAULT NEWSEQUENTIALID(),
    [UserId]    UNIQUEIDENTIFIER NOT NULL,
    [EventId]   INT              NOT NULL,
    [CreatedAt] DATETIME2 (0)    NOT NULL CONSTRAINT [DF_Cart_CreatedAt] DEFAULT GETUTCDATE(),
    [ExpiresAt] DATETIME2 (0)    NULL,
    CONSTRAINT [PK_Cart]       PRIMARY KEY CLUSTERED ([CartId] ASC),
    CONSTRAINT [FK_Cart_User]  FOREIGN KEY ([UserId])  REFERENCES [sec].[User] ([UserId]),
    CONSTRAINT [FK_Cart_Event] FOREIGN KEY ([EventId]) REFERENCES [tkt].[Event] ([EventId])
);
