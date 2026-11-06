CREATE TABLE [sec].[UserRole]
(
    [UserId]     UNIQUEIDENTIFIER NOT NULL,
    [RoleId]     TINYINT          NOT NULL,
    [AssignedAt] DATETIME2 (0)    NOT NULL CONSTRAINT [DF_UserRole_AssignedAt] DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_UserRole]      PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_UserRole_User] FOREIGN KEY ([UserId]) REFERENCES [sec].[User] ([UserId]),
    CONSTRAINT [FK_UserRole_Role] FOREIGN KEY ([RoleId]) REFERENCES [sec].[Role] ([RoleId])
);
