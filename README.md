# Guía paso a paso para tener el proyecto funcional

# Paso 1: Script para la base de datos SQL

-- Script SQL principal para crear la base de datos y todas las tablas necesarias para el proyecto
CREATE DATABASE LABORATORIO;
GO

USE LABORATORIO;
GO

--Tablas de negocio--

CREATE TABLE CLIENTES (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    NOMBRE NVARCHAR(200) NOT NULL,
    CEDULA_JURIDICA NVARCHAR(50) NOT NULL,
    CORREO NVARCHAR(100) NOT NULL,
    TELEFONO NVARCHAR(20) NOT NULL,
    DIRECCION NVARCHAR(500) NOT NULL,
    FECHA_REGISTRO DATETIME NOT NULL,
    FECHA_MODIFICACION DATETIME NULL,
    ESTADO BIT NOT NULL
);
GO

CREATE TABLE PRODUCTOS (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    Marca NVARCHAR(100) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL,
    SKU NVARCHAR(50) NOT NULL,
    CantidadEnStock INT NOT NULL,
    Estado BIT NOT NULL,
    FECHA_REGISTRO DATETIME NOT NULL,
    FECHA_MODIFICACION DATETIME NULL,
    -- IVA configurable que usa la app
    PorcentajeIVA DECIMAL(5,2) NOT NULL CONSTRAINT DF_Productos_PorcentajeIVA DEFAULT (13)
);
GO

--ASP.NET Identity--

CREATE TABLE AspNetRoles (
    Id NVARCHAR(128) NOT NULL PRIMARY KEY,
    Name NVARCHAR(256) NOT NULL
);

CREATE TABLE AspNetUsers (
    Id NVARCHAR(128) NOT NULL PRIMARY KEY,
    Email NVARCHAR(256),
    EmailConfirmed BIT NOT NULL,
    PasswordHash NVARCHAR(MAX),
    SecurityStamp NVARCHAR(MAX),
    PhoneNumber NVARCHAR(MAX),
    PhoneNumberConfirmed BIT NOT NULL,
    TwoFactorEnabled BIT NOT NULL,
    LockoutEndDateUtc DATETIME,
    LockoutEnabled BIT NOT NULL,
    AccessFailedCount INT NOT NULL,
    UserName NVARCHAR(256) NOT NULL,
    -- campos extra usados por la app
    NombreCompleto NVARCHAR(MAX),
    Cedula NVARCHAR(MAX),
    Direccion NVARCHAR(MAX),
    FechaRegistro DATETIME NOT NULL,
    EstadoAprobacion NVARCHAR(50) NULL
);

CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(128) NOT NULL,
    RoleId NVARCHAR(128) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserClaims (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(128) NOT NULL,
    ClaimType NVARCHAR(MAX),
    ClaimValue NVARCHAR(MAX),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserLogins (
    LoginProvider NVARCHAR(128) NOT NULL,
    ProviderKey NVARCHAR(128) NOT NULL,
    UserId NVARCHAR(128) NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey, UserId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);
GO

  -- Carrito y cupones--

CREATE TABLE CuponesDescuento (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200) NULL,
    PorcentajeDescuento DECIMAL(5,2) NOT NULL,
    MontoMaximo DECIMAL(18,2) NULL,
    LimiteUso INT NULL,
    UsosRealizados INT NOT NULL DEFAULT 0,
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,
    Activo BIT NOT NULL,
    ProductoId INT NULL,
    FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);
GO

CREATE TABLE Carritos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductoId INT NOT NULL,
    UsuarioId NVARCHAR(256) NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL,
    PorcentajeIVA DECIMAL(5,2) NOT NULL CONSTRAINT DF_Carritos_PorcentajeIVA DEFAULT (13),
    PorcentajeDescuento DECIMAL(5,2) NOT NULL CONSTRAINT DF_Carritos_PorcentajeDescuento DEFAULT (0),
    MontoDescuento DECIMAL(18,2) NOT NULL CONSTRAINT DF_Carritos_MontoDescuento DEFAULT (0),
    CuponDescuentoId INT NULL,
    FechaAgregado DATETIME NOT NULL,
    FOREIGN KEY (ProductoId) REFERENCES Productos(Id),
    FOREIGN KEY (CuponDescuentoId) REFERENCES CuponesDescuento(Id)
);
GO


  -- Creación de las tablas para los Pedidos --

-- Tabla de Pedidos
CREATE TABLE Pedidos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NULL,
    UsuarioId NVARCHAR(128) NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Subtotal DECIMAL(18,2) NOT NULL,
    Impuestos DECIMAL(18,2) NOT NULL,
    Total DECIMAL(18,2) NOT NULL,
    Estado NVARCHAR(50) NOT NULL DEFAULT 'Pendiente', -- Pendiente, Completado o Cancelado
    CONSTRAINT FK_Pedidos_Usuario FOREIGN KEY (UsuarioId) REFERENCES AspNetUsers(Id)
);
GO

-- Tabla de Detalles del Pedido
CREATE TABLE PedidoDetalles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PedidoId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnit DECIMAL(18,2) NOT NULL,
    Descuento DECIMAL(18,2) NOT NULL DEFAULT 0,
    ImpuestoPorc DECIMAL(5,2) NOT NULL, -- Porcentaje del IVA
    TotalLinea DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_PedidoDetalles_Pedido FOREIGN KEY (PedidoId) REFERENCES Pedidos(Id) ON DELETE CASCADE,
    CONSTRAINT FK_PedidoDetalles_Producto FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);
GO

-- Índices para mejor rendimiento
CREATE INDEX IX_Pedidos_UsuarioId ON Pedidos(UsuarioId);
CREATE INDEX IX_Pedidos_Fecha ON Pedidos(Fecha);
CREATE INDEX IX_PedidoDetalles_PedidoId ON PedidoDetalles(PedidoId);
GO


  -- Datos iniciales y helpers--

DELETE FROM AspNetUserRoles WHERE RoleId = (SELECT Id FROM AspNetRoles WHERE Name = 'Cliente');
DELETE FROM AspNetRoles WHERE Name = 'Cliente';

INSERT INTO AspNetRoles (Id, Name) VALUES (NEWID(), 'Administrador');
INSERT INTO AspNetRoles (Id, Name) VALUES (NEWID(), 'Ventas');
INSERT INTO AspNetRoles (Id, Name) VALUES (NEWID(), 'Operaciones');

UPDATE AspNetUsers
SET EstadoAprobacion = 'Aprobado'
WHERE UserName = 'Admin';
GO

# ***IMPORTANTE*** Antes de clonar el proyecto, verificar que se está en la branch o rama llamada "JeffersonAnchia", ya que la master está corrupta y no se le encontró solución.

# Una vez se tiene la base de datos completamente creada, se debe abrir la solución del proyecto en Visual Studio y cambiar dentro del archivo "Web.config" el connectionStrings
# dependiendo de nuestro tipo de conexión ya sea SQL Server, SQL Express, etc.

# Una vez hecho esto, antes de correr el proyecto se debe dar click derecho a la solución dentro del explorador de soluciones, darle click en "Restaurar paquetes de NuGet" para
# evitar problemas de compilación por estos.

# Luego para los seeders utilizados para el identity, se debe correr el proyecto por primera vez y en el buscador ir a la dirección "http://localhost:xxxxxxx/Seed/CrearRolesYUsuarios"
# en donde xxxxxxx es el puerto local en el que se está corriendo el proyecto.

# Una vez salga la vista que dice "Seeders para identity creados correctamente", se puede loggear como admin usando el usuario "admin" todo minúsculas y contraseña "Admin123" con la A mayúscula.
# De igual manera, el proyecto está subido a un host remoto el cual su link es "http://carlosdani0302-001-site1.ltempurl.com/", en donde no es necesario seguir todos estos pasos, sólo contactar
# con el dueño para estar seguros de que el host está funcionando correctamente.
