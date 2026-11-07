CREATE INDEX [IX_RefreshToken_UserId] ON [sec].[RefreshToken] ([UserId] ASC);
GO
CREATE INDEX [IX_RefreshToken_ExpiresAt] ON [sec].[RefreshToken] ([ExpiresAt] ASC) WHERE [IsRevoked] = 0;
