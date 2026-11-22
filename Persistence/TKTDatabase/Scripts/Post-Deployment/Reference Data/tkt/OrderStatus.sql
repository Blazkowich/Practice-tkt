MERGE INTO [tkt].[OrderStatus] AS [Target]
USING (VALUES
    (1, N'Pending'),
    (2, N'Paid'),
    (3, N'Cancelled')
) AS [Source] ([OrderStatusId], [Name])
ON [Target].[OrderStatusId] = [Source].[OrderStatusId]
WHEN MATCHED THEN
    UPDATE SET [Name] = [Source].[Name]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([OrderStatusId], [Name])
    VALUES ([Source].[OrderStatusId], [Source].[Name]);
