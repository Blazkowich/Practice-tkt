CREATE TABLE [tkt].[Event]
(
    [EventId]       INT           IDENTITY (1, 1) NOT NULL,
    [VenueId]       INT           NOT NULL,
    [Name]          NVARCHAR(200) NOT NULL,
    [Description]   NVARCHAR(MAX) NULL,
    [EventDate]     DATETIME2 (0) NOT NULL,
    [DoorsOpenAt]   DATETIME2 (0) NULL,
    [EventStatusId] TINYINT       NOT NULL CONSTRAINT [DF_Event_EventStatusId] DEFAULT 1,
    [CreatedAt]     DATETIME2 (0) NOT NULL CONSTRAINT [DF_Event_CreatedAt]     DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Event]             PRIMARY KEY CLUSTERED ([EventId] ASC),
    CONSTRAINT [FK_Event_Venue]       FOREIGN KEY ([VenueId])       REFERENCES [tkt].[Venue]       ([VenueId]),
    CONSTRAINT [FK_Event_EventStatus] FOREIGN KEY ([EventStatusId]) REFERENCES [tkt].[EventStatus] ([EventStatusId])
);
