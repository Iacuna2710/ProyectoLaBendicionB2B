SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- Crea los 3 roles del sistema directamente en AspNetRoles.
-- Útil si quieres tenerlos ya en la base de datos sin esperar a que la app
-- arranque (Program.cs ya los crea solo al iniciar, vía SeedData.InicializarAsync).
-- Es seguro ejecutarlo varias veces: no duplica roles que ya existan.

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'ADMIN')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'VENTAS')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Ventas', 'VENTAS', NEWID());

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'OPERACIONES')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Operaciones', 'OPERACIONES', NEWID());

SELECT Id, Name FROM AspNetRoles;
