CREATE TABLE [sec].[User]
(
    [UserId]         UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_User_UserId]    DEFAULT NEWSEQUENTIALID(),
    [ExternalUserId] NVARCHAR(200)    NOT NULL,
    [Email]          NVARCHAR(200)    NOT NULL,
    [FirstName]      NVARCHAR(100)    NULL,
    [LastName]       NVARCHAR(100)    NULL,
    [CreatedAt]      DATETIME2 (0)    NOT NULL CONSTRAINT [DF_User_CreatedAt] DEFAULT GETUTCDATE(),
    [IsActive]       BIT              NOT NULL CONSTRAINT [DF_User_IsActive]  DEFAULT 1,
    CONSTRAINT [PK_User]                PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [UQ_User_ExternalUserId] UNIQUE ([ExternalUserId])
);
