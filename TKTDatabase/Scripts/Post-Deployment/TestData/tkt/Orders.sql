MERGE INTO [tkt].[Order] AS [Target]
USING (VALUES
    (
        '00000001-0000-0000-0000-000000000011',
        '00000012-0000-0000-0000-000000000002',
        1,
        '00000002-0000-0000-0000-000000000002',
        75.00,
        N'PAY-TXN-001'
    )
) AS [Source] ([OrderId], [UserId], [EventId], [CartId], [TotalAmount], [PaymentTransactionId])
ON [Target].[OrderId] = [Source].[OrderId]
WHEN MATCHED THEN
    UPDATE SET
        [UserId]               = [Source].[UserId],
        [EventId]              = [Source].[EventId],
        [CartId]               = [Source].[CartId],
        [TotalAmount]          = [Source].[TotalAmount],
        [PaymentTransactionId] = [Source].[PaymentTransactionId]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([OrderId], [UserId], [EventId], [CartId], [TotalAmount], [PaymentTransactionId])
    VALUES ([Source].[OrderId], [Source].[UserId], [Source].[EventId], [Source].[CartId], [Source].[TotalAmount], [Source].[PaymentTransactionId]);
