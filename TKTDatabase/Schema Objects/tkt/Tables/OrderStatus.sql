CREATE TABLE [tkt].[OrderStatus]
(
    [OrderStatusId] TINYINT      NOT NULL,
    [Name]          NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_OrderStatus] PRIMARY KEY CLUSTERED ([OrderStatusId] ASC)
);
