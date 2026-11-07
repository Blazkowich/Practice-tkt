CREATE TABLE [sec].[User]
(
    [UserId]       UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_User_UserId]    DEFAULT NEWSEQUENTIALID(),
    [Email]        NVARCHAR(200)    NOT NULL,
    [PasswordHash] NVARCHAR(500)    NULL,
    [FirstName]    NVARCHAR(100)    NULL,
    [LastName]     NVARCHAR(100)    NULL,
    [CreatedAt]    DATETIME2 (0)    NOT NULL CONSTRAINT [DF_User_CreatedAt] DEFAULT GETUTCDATE(),
    [IsActive]     BIT              NOT NULL CONSTRAINT [DF_User_IsActive]  DEFAULT 1,
    CONSTRAINT [PK_User]         PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [UQ_User_Email]   UNIQUE ([Email])
);
