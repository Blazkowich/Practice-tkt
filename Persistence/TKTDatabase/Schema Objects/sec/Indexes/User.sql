CREATE INDEX [IX_User_Email] ON [sec].[User] ([Email] ASC);
GO
CREATE INDEX [IX_User_IsActive] ON [sec].[User] ([IsActive] ASC) WHERE [IsActive] = 1;
GO
CREATE INDEX [IX_User_CreatedAt] ON [sec].[User] ([CreatedAt] ASC);
