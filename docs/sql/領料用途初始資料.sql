-- 領料用途（SystemConfig.ConfigType = 'MaterialIssuePurpose'）初始資料
-- 可重複執行：已存在的 Code 不會重複新增。
SET NOCOUNT ON;

DECLARE @Purposes TABLE (Code NVARCHAR(32), Name NVARCHAR(64), Sort INT);
INSERT INTO @Purposes (Code, Name, Sort) VALUES
    (N'PROJECT', N'專案領用', 1),
    (N'REPAIR',  N'維修',     2),
    (N'SAMPLE',  N'樣品',     3),
    (N'INTERNAL', N'自用',    4),
    (N'OTHER',   N'其他',     5);

INSERT INTO dbo.SystemConfig (ConfigType, Code, Name, Sort, IsActive)
SELECT N'MaterialIssuePurpose', p.Code, p.Name, p.Sort, 1
FROM @Purposes AS p
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.SystemConfig AS s
    WHERE s.ConfigType = N'MaterialIssuePurpose'
      AND s.Code = p.Code
);

SELECT Id, ConfigType, Code, Name, Sort, IsActive
FROM dbo.SystemConfig
WHERE ConfigType = N'MaterialIssuePurpose'
ORDER BY Sort;
