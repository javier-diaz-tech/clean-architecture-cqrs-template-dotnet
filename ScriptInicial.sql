USE ArquitecturaBaseDb;
GO

BEGIN TRANSACTION;

-- Roles
INSERT INTO dbo.Roles (Nombre, Activo)
VALUES
    ('Administrador', 1),
    ('Consulta', 1);

-- Menús principales
INSERT INTO dbo.Menus
    (Nombre, Url, Icono, Nivel, MenuPadreId, Activo)
VALUES
    ('Administración', '/administracion', 'settings', 1, NULL, 1),
    ('Consultas', '/consultas', 'search', 1, NULL, 1);

-- Submenú de Administración
INSERT INTO dbo.Menus
    (Nombre, Url, Icono, Nivel, MenuPadreId, Activo)
VALUES
    ('Usuarios', '/administracion/usuarios', 'users', 2,
     (SELECT Id FROM dbo.Menus WHERE Nombre = 'Administración'), 1);

-- Permisos del Administrador: acceso a todos los menús
INSERT INTO dbo.RolesMenu (RolId, MenuId, PuedeVer)
SELECT r.Id, m.Id, 1
FROM dbo.Roles r
CROSS JOIN dbo.Menus m
WHERE r.Nombre = 'Administrador';

-- Permisos del rol Consulta: solo Consultas
INSERT INTO dbo.RolesMenu (RolId, MenuId, PuedeVer)
SELECT r.Id, m.Id, 1
FROM dbo.Roles r
JOIN dbo.Menus m ON m.Nombre = 'Consultas'
WHERE r.Nombre = 'Consulta';

COMMIT TRANSACTION;
GO