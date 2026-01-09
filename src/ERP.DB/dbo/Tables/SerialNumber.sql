CREATE TABLE [dbo].[SerialNumber]
(
	Id            INT IDENTITY PRIMARY KEY,
    Prefix        NVARCHAR(8) NOT NULL,
    DateKey       CHAR(6) NOT NULL,   -- 2601 (年+月)
    CurrentNo     INT NOT NULL,
    CONSTRAINT UX_Serial UNIQUE (Prefix, DateKey)
)
