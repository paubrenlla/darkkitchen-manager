-- =============================================
-- DarkKitchen - Script de datos de prueba
-- Contraseña para todos los usuarios: ValidP@ssw0rd!8X
-- =============================================

USE DarkKitchenDB;

-- Limpiar datos existentes
DELETE FROM ShippingTypes;
DELETE FROM PromotionProducts;
DELETE FROM OrderItems;
DELETE FROM Orders;
DELETE FROM Promotions;
DELETE FROM ProductImages;
DELETE FROM Products;
DELETE FROM ProductLines;
DELETE FROM ProductCategories;
DELETE FROM Users;
DELETE FROM AuditLogs;

-- =============================================
-- VARIABLES
-- =============================================

DECLARE @AdminId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @PreparadorId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @ClienteId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';

DECLARE @LineComboId UNIQUEIDENTIFIER = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @LineDesayunosId UNIQUEIDENTIFIER = 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb';
DECLARE @LineMinutasId UNIQUEIDENTIFIER = 'cccccccc-cccc-cccc-cccc-cccccccccccc';

DECLARE @CatParrillaId UNIQUEIDENTIFIER = 'dddddddd-dddd-dddd-dddd-dddddddddddd';
DECLARE @CatBebidasId UNIQUEIDENTIFIER = 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee';
DECLARE @CatFritosId UNIQUEIDENTIFIER = 'ffffffff-ffff-ffff-ffff-ffffffffffff';

DECLARE @Prod1Id UNIQUEIDENTIFIER = 'a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1';
DECLARE @Prod2Id UNIQUEIDENTIFIER = 'b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2';
DECLARE @Prod3Id UNIQUEIDENTIFIER = 'c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3';
DECLARE @Prod4Id UNIQUEIDENTIFIER = 'd4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4';
DECLARE @Prod5Id UNIQUEIDENTIFIER = 'e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5';

DECLARE @Promo1Id UNIQUEIDENTIFIER = 'f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6';
DECLARE @Promo2Id UNIQUEIDENTIFIER = 'a7a7a7a7-a7a7-a7a7-a7a7-a7a7a7a7a7a7';

DECLARE @Order1Id UNIQUEIDENTIFIER = '10000001-0000-0000-0000-000000000001';
DECLARE @Order2Id UNIQUEIDENTIFIER = '10000002-0000-0000-0000-000000000002';
DECLARE @Order3Id UNIQUEIDENTIFIER = '10000003-0000-0000-0000-000000000003';

DECLARE @ShipExpressId UNIQUEIDENTIFIER = '00000001-0000-0000-0000-000000000001';
DECLARE @ShipDiaSigId UNIQUEIDENTIFIER = '00000002-0000-0000-0000-000000000002';

-- =============================================
-- USUARIOS
-- =============================================

INSERT INTO Users (Id, Name, Surname, Email, HashedPassword, Role, PhoneCountryPrefix, PhoneNumber)
VALUES
    (
        @AdminId,
        'Admin',
        'Jefe',
        'admin@bmb.com',
        '$2a$12$aZGd1nXfOuqRCZd4fV4p4Op4R6bPZvKQAwbD2ODT.7wXQJQIqXPn2',
        'Administrativo',
        '+598',
        '094222333'
    ),
    (
        @PreparadorId,
        'Pepe',
        'Ruiz',
        'preparador@bmb.com',
        '$2a$12$aZGd1nXfOuqRCZd4fV4p4Op4R6bPZvKQAwbD2ODT.7wXQJQIqXPn2',
        'Preparador',
        '+598',
        '094333444'
    ),
    (
        @ClienteId,
        'Juan',
        'Sosa',
        'cliente@bmb.com',
        '$2a$12$aZGd1nXfOuqRCZd4fV4p4Op4R6bPZvKQAwbD2ODT.7wXQJQIqXPn2',
        'Cliente',
        '+598',
        '094444555'
    );

-- =============================================
-- LINEAS DE PRODUCTO
-- =============================================

INSERT INTO ProductLines (Id, Name)
VALUES
    (@LineComboId, 'Combo burgers'),
    (@LineDesayunosId, 'Desayunos'),
    (@LineMinutasId, 'Minutas clasicas');

-- =============================================
-- CATEGORIAS DE PRODUCTO
-- =============================================

INSERT INTO ProductCategories (Id, Name)
VALUES
    (@CatParrillaId, 'Parrilla'),
    (@CatBebidasId, 'Bebidas'),
    (@CatFritosId, 'Fritos');

-- =============================================
-- PRODUCTOS
-- =============================================

INSERT INTO Products (Id, Code, Name, Description, Price, IsActive, ProductLineId, ProductCategoryId)
VALUES
    (
        @Prod1Id,
        'BURG01',
        'Hamburguesa Clasica',
        'Hamburguesa clasica con queso cheddar y lechuga fresca',
        150.00,
        1,
        @LineComboId,
        @CatParrillaId
    ),
    (
        @Prod2Id,
        'BURG02',
        'Hamburguesa Doble Grande',
        'Hamburguesa doble con queso y bacon ahumado artesanal',
        200.00,
        1,
        @LineComboId,
        @CatParrillaId
    ),
    (
        @Prod3Id,
        'DESA01',
        'Desayuno Completo Grande',
        'Desayuno con cafe tostadas y jugo natural de naranja',
        120.00,
        1,
        @LineDesayunosId,
        @CatBebidasId
    ),
    (
        @Prod4Id,
        'MIN01',
        'Milanesa Napolitana',
        'Milanesa napolitana con jamon queso y salsa de tomate',
        180.00,
        1,
        @LineMinutasId,
        @CatFritosId
    ),
    (
        @Prod5Id,
        'BURG03',
        'Hamburguesa Vegana Nueva',
        'Hamburguesa vegana con medallon de legumbres y vegetales frescos',
        170.00,
        0,
        @LineComboId,
        @CatParrillaId
    );

-- =============================================
-- IMAGENES DE PRODUCTO
-- =============================================

INSERT INTO ProductImages (Id, Url, SizeInBytes, ProductId)
VALUES
    (NEWID(), 'https://example.com/burg01.jpg', 50000, @Prod1Id),
    (NEWID(), 'https://example.com/burg01-2.jpg', 60000, @Prod1Id),
    (NEWID(), 'https://example.com/burg02.jpg', 55000, @Prod2Id),
    (NEWID(), 'https://example.com/desa01.jpg', 45000, @Prod3Id),
    (NEWID(), 'https://example.com/min01.jpg', 70000, @Prod4Id),
    (NEWID(), 'https://example.com/burg03.jpg', 48000, @Prod5Id);

-- =============================================
-- PROMOCIONES
-- =============================================

INSERT INTO Promotions (Id, Name, IsActive, DiscountPercentage, StartDate, EndDate)
VALUES
    (
        @Promo1Id,
        'Black Friday',
        1,
        10,
        '2026-01-01',
        '2026-12-31'
    ),
    (
        @Promo2Id,
        'Semana de Turismo',
        1,
        15,
        '2026-03-29',
        '2026-12-31'
    );

INSERT INTO PromotionProducts (PromotionId, ProductsId)
VALUES
    (@Promo1Id, @Prod1Id),
    (@Promo1Id, @Prod2Id),
    (@Promo2Id, @Prod3Id);

-- =============================================
-- TIPOS DE ENVIO
-- =============================================

INSERT INTO ShippingTypes (Id, Name, Cost)
VALUES
    (@ShipExpressId, 'Express', 150.00),
    (@ShipDiaSigId, 'TwentyFourHours', 50.00);

-- =============================================
-- PEDIDOS
-- =============================================

INSERT INTO Orders (Id, OrderNumber, ClientId, Street, DoorNumber, Apartment, City, Country, Type, CreatedAt, LastTransitionDate, State, ShippingCost)
VALUES
    (
        @Order1Id,
        1,
        @ClienteId,
        'Rivera',
        '1234',
        NULL,
        'Montevideo',
        'Uruguay',
        'Express',
        '2026-01-15 10:00:00',
        '2026-01-15 12:00:00',
        'Delivered',
        150.00
    ),
    (
        @Order2Id,
        2,
        @ClienteId,
        'Bvar Artigas',
        '4567',
        '3B',
        'Montevideo',
        'Uruguay',
        'TwentyFourHours',
        '2026-02-10 14:30:00',
        '2026-02-10 14:30:00',
        'Pending',
        50.00
    ),
    (
        @Order3Id,
        3,
        @ClienteId,
        'Av Italia',
        '789',
        NULL,
        'Montevideo',
        'Uruguay',
        'Express',
        '2026-02-20 09:15:00',
        '2026-02-20 09:30:00',
        'Cancelled',
        150.00
    );

-- =============================================
-- ITEMS DE PEDIDOS
-- =============================================

INSERT INTO OrderItems (Id, ProductId, Quantity, Price, DiscountPercentage, AppliedPromotionName, OrderId)
VALUES
    (NEWID(), @Prod1Id, 2, 150.00, 10, 'Black Friday', @Order1Id),
    (NEWID(), @Prod2Id, 1, 200.00, 0, NULL, @Order1Id),
    (NEWID(), @Prod3Id, 1, 120.00, 0, NULL, @Order2Id),
    (NEWID(), @Prod1Id, 3, 150.00, 10, 'Black Friday', @Order3Id);

-- =============================================
-- LOGS DE AUDITORIA
-- =============================================

INSERT INTO AuditLogs (Id, Timestamp, EntityName, EntityId, ChangeDescription, ResponsibleUser)
VALUES
    (NEWID(), '2026-04-25 10:00:00', 'Product', @Prod1Id, 'Producto antiguo creado', 'admin@bmb.com'),
    (NEWID(), '2026-05-01 08:30:00', 'Product', @Prod1Id, 'Alta de Hamburguesa Clasica', 'admin@bmb.com'),
    (NEWID(), '2026-05-01 15:00:00', 'Promotion', @Promo1Id, 'Nueva promo Black Friday configurada', 'admin@bmb.com'),
    (NEWID(), '2026-05-02 10:00:00', 'Promotion', @Promo2Id, 'Nueva promo Semana de Turismo configurada', 'admin@bmb.com'),
    (NEWID(), '2026-05-05 12:00:00', 'Product', @Prod1Id, 'Modificación de precio: de 140 a 150', 'admin@bmb.com'),
    (NEWID(), '2026-05-06 09:00:00', 'Promotion', @Promo1Id, 'Descuento actualizado: de 10% a 12%', 'admin@bmb.com');