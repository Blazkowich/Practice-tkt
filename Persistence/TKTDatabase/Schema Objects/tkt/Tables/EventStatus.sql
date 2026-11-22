CREATE TABLE [tkt].[EventStatus]
(
    [EventStatusId] TINYINT      NOT NULL,
    [Name]          NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_EventStatus] PRIMARY KEY CLUSTERED ([EventStatusId] ASC)
);
