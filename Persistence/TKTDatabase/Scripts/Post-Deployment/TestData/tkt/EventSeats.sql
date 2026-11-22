-- SeatStateId: 1=Available  2=Booked  3=Sold
-- IDs  1-18 : Event 1 (Night 1)   IDs 19-36 : Event 2 (Night 2)
-- ID  6 : E35-B-1  Booked by User 1 (Admin), hold sentinel 2099-12-31
-- ID  7 : E35-B-2  Sold  (PriceId=1, Adult $75)
SET IDENTITY_INSERT [tkt].[EventSeat] ON;

MERGE INTO [tkt].[EventSeat] AS [Target]
USING (VALUES
    ( 1, 1,  1, NULL, 1, NULL,                                       NULL),
    ( 2, 1,  2, NULL, 1, NULL,                                       NULL),
    ( 3, 1,  3, NULL, 1, NULL,                                       NULL),
    ( 4, 1,  4, NULL, 1, NULL,                                       NULL),
    ( 5, 1,  5, NULL, 1, NULL,                                       NULL),
    ( 6, 1,  6,    1, 2, '00000011-0000-0000-0000-000000000001', '2099-12-31T23:59:59'),
    ( 7, 1,  7,    1, 3, NULL,                                       NULL),
    ( 8, 1,  8, NULL, 1, NULL,                                       NULL),
    ( 9, 1,  9, NULL, 1, NULL,                                       NULL),
    (10, 1, 10, NULL, 1, NULL,                                       NULL),
    (11, 1, 11, NULL, 1, NULL,                                       NULL),
    (12, 1, 12, NULL, 1, NULL,                                       NULL),
    (13, 1, 13, NULL, 1, NULL,                                       NULL),
    (14, 1, 14, NULL, 1, NULL,                                       NULL),
    (15, 1, 15, NULL, 1, NULL,                                       NULL),
    (16, 1, 16, NULL, 1, NULL,                                       NULL),
    (17, 1, 17, NULL, 1, NULL,                                       NULL),
    (18, 1, 18, NULL, 1, NULL,                                       NULL),
    (19, 2,  1, NULL, 1, NULL,                                       NULL),
    (20, 2,  2, NULL, 1, NULL,                                       NULL),
    (21, 2,  3, NULL, 1, NULL,                                       NULL),
    (22, 2,  4, NULL, 1, NULL,                                       NULL),
    (23, 2,  5, NULL, 1, NULL,                                       NULL),
    (24, 2,  6, NULL, 1, NULL,                                       NULL),
    (25, 2,  7, NULL, 1, NULL,                                       NULL),
    (26, 2,  8, NULL, 1, NULL,                                       NULL),
    (27, 2,  9, NULL, 1, NULL,                                       NULL),
    (28, 2, 10, NULL, 1, NULL,                                       NULL),
    (29, 2, 11, NULL, 1, NULL,                                       NULL),
    (30, 2, 12, NULL, 1, NULL,                                       NULL),
    (31, 2, 13, NULL, 1, NULL,                                       NULL),
    (32, 2, 14, NULL, 1, NULL,                                       NULL),
    (33, 2, 15, NULL, 1, NULL,                                       NULL),
    (34, 2, 16, NULL, 1, NULL,                                       NULL),
    (35, 2, 17, NULL, 1, NULL,                                       NULL),
    (36, 2, 18, NULL, 1, NULL,                                       NULL)
) AS [Source] ([EventSeatId], [EventId], [VenueSeatId], [PriceId], [SeatStateId], [HeldByUserId], [HoldUntil])
ON [Target].[EventSeatId] = [Source].[EventSeatId]
WHEN MATCHED THEN
    UPDATE SET
        [EventId]      = [Source].[EventId],
        [VenueSeatId]  = [Source].[VenueSeatId],
        [PriceId]      = [Source].[PriceId],
        [SeatStateId]  = [Source].[SeatStateId],
        [HeldByUserId] = [Source].[HeldByUserId],
        [HoldUntil]    = [Source].[HoldUntil]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([EventSeatId], [EventId], [VenueSeatId], [PriceId], [SeatStateId], [HeldByUserId], [HoldUntil])
    VALUES ([Source].[EventSeatId], [Source].[EventId], [Source].[VenueSeatId], [Source].[PriceId], [Source].[SeatStateId], [Source].[HeldByUserId], [Source].[HoldUntil]);

SET IDENTITY_INSERT [tkt].[EventSeat] OFF;
