MERGE INTO [tkt].[Ticket] AS [Target]
USING (VALUES
    (
        '00000001-0000-0000-0000-000000000021',
        '00000001-0000-0000-0000-000000000011',
        1,
        7,
        '00000012-0000-0000-0000-000000000002',
        1,
        75.00,
        NULL,
        NULL
    )
) AS [Source] ([TicketId], [OrderId], [EventId], [EventSeatId], [UserId], [PriceId], [PaidAmount], [CancelledAt], [CancelledByManagerId])
ON [Target].[TicketId] = [Source].[TicketId]
WHEN MATCHED THEN
    UPDATE SET
        [OrderId]              = [Source].[OrderId],
        [EventId]              = [Source].[EventId],
        [EventSeatId]          = [Source].[EventSeatId],
        [UserId]               = [Source].[UserId],
        [PriceId]              = [Source].[PriceId],
        [PaidAmount]           = [Source].[PaidAmount],
        [CancelledAt]          = [Source].[CancelledAt],
        [CancelledByManagerId] = [Source].[CancelledByManagerId]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([TicketId], [OrderId], [EventId], [EventSeatId], [UserId], [PriceId], [PaidAmount], [CancelledAt], [CancelledByManagerId])
    VALUES ([Source].[TicketId], [Source].[OrderId], [Source].[EventId], [Source].[EventSeatId], [Source].[UserId], [Source].[PriceId], [Source].[PaidAmount], [Source].[CancelledAt], [Source].[CancelledByManagerId]);
