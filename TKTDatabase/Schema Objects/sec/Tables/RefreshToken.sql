CREATE TABLE [sec].[RefreshToken]
(
    [TokenId]   UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_RefreshToken_TokenId]   DEFAULT NEWSEQUENTIALID(),
    [UserId]    UNIQUEIDENTIFIER NOT NULL,
    [Token]     NVARCHAR(500)    NOT NULL,
    [ExpiresAt] DATETIME2 (0)    NOT NULL,
    [IsRevoked] BIT              NOT NULL CONSTRAINT [DF_RefreshToken_IsRevoked] DEFAULT 0,
    [CreatedAt] DATETIME2 (0)    NOT NULL CONSTRAINT [DF_RefreshToken_CreatedAt] DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_RefreshToken]       PRIMARY KEY CLUSTERED ([TokenId] ASC),
    CONSTRAINT [UQ_RefreshToken_Token] UNIQUE ([Token]),
    CONSTRAINT [FK_RefreshToken_User]  FOREIGN KEY ([UserId]) REFERENCES [sec].[User] ([UserId])
);
