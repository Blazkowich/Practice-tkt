CREATE INDEX [IX_EventSeat_EventId_SeatStateId] ON [tkt].[EventSeat] ([EventId] ASC, [SeatStateId] ASC);
GO
CREATE INDEX [IX_EventSeat_HoldUntil] ON [tkt].[EventSeat] ([HoldUntil] ASC) WHERE [HoldUntil] IS NOT NULL;
