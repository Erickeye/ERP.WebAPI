CREATE PROCEDURE [dbo].[sp_Inventory_Search]
    @SupplierName NVARCHAR(128) = NULL,
    @LocationName NVARCHAR(64) = NULL,
    @Category NVARCHAR(64) = NULL,
    @Name NVARCHAR(128) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        i.Id,
        i.SupplierId,
        ISNULL(s.[Name], N'') AS SupplierName,
        i.[Name],
        i.LocationId,
        ISNULL(l.[Name], N'') AS LocationName,
        i.Category,
        i.LastPurchaseDate,
        i.[No],
        i.Unit,
        i.Quantity,
        i.Amount,
        i.Total
    FROM dbo.t_4000Inventory AS i
    LEFT JOIN dbo.t_4060Supplier AS s
        ON s.Id = i.SupplierId
    LEFT JOIN dbo.SystemConfig AS l
        ON l.Id = i.LocationId
    WHERE
        @SupplierName IS NULL OR s.[Name] LIKE N'%' + @SupplierName + N'%'
        AND @LocationName IS NULL OR l.[Name] LIKE N'%' + @LocationName + N'%'
        AND @Category IS NULL OR i.Category = @Category
        AND @Name IS NULL OR i.[Name] LIKE N'%' + @Name + N'%';
END
