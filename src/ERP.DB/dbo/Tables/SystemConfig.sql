CREATE TABLE dbo.SystemConfig
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ConfigType NVARCHAR(32) NOT NULL,  -- e.g. 'InventoryLocation'
    Code NVARCHAR(32) NULL,        -- e.g. 'A01'
    Name NVARCHAR(64) NOT NULL,        -- e.g. '一號倉'
    [Sort] INT NOT NULL DEFAULT 1,
    IsActive BIT NOT NULL DEFAULT 1
);
