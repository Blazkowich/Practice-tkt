CREATE TABLE [sec].[Role]
(
    [RoleId] TINYINT      NOT NULL,
    [Name]   NVARCHAR(50) NOT NULL,

    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleId] ASC)
);
