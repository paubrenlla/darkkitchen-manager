-- =============================================
-- DarkKitchen - Script de datos de prueba
-- Contraseña para todos los usuarios: ValidP@ssw0rd!8X
-- Fechas cubiertas: enero 2026 – julio 2026
-- =============================================

USE DarkKitchen;

-- Limpiar datos existentes
DELETE FROM AuditLogs;
DELETE FROM OrderItems;
DELETE FROM Orders;
DELETE FROM PromotionProducts;
DELETE FROM Promotions;
DELETE FROM ProductImages;
DELETE FROM Products;
DELETE FROM ProductLines;
DELETE FROM ProductCategories;
DELETE FROM ShippingTypes;
DELETE FROM Users;

-- =============================================
-- VARIABLES — USUARIOS
-- =============================================

DECLARE @Admin1Id  UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @Admin2Id  UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111112';

DECLARE @Prep1Id   UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222221';
DECLARE @Prep2Id   UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @Prep3Id   UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222223';

DECLARE @Cli1Id    UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333331';
DECLARE @Cli2Id    UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333332';
DECLARE @Cli3Id    UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @Cli4Id    UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333334';
DECLARE @Cli5Id    UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333335';
DECLARE @Cli6Id    UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333336';
DECLARE @Cli7Id    UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333337';
DECLARE @Cli8Id    UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333338';

-- =============================================
-- VARIABLES — LINEAS Y CATEGORIAS
-- =============================================

DECLARE @LineComboId      UNIQUEIDENTIFIER = 'aaaa0001-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @LineDesayunosId  UNIQUEIDENTIFIER = 'aaaa0002-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @LineMinutasId    UNIQUEIDENTIFIER = 'aaaa0003-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @LinePastaId      UNIQUEIDENTIFIER = 'aaaa0004-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @LinePostresId    UNIQUEIDENTIFIER = 'aaaa0005-aaaa-aaaa-aaaa-aaaaaaaaaaaa';
DECLARE @LineBebidasId    UNIQUEIDENTIFIER = 'aaaa0006-aaaa-aaaa-aaaa-aaaaaaaaaaaa';

DECLARE @CatParrillaId    UNIQUEIDENTIFIER = 'bbbb0001-bbbb-bbbb-bbbb-bbbbbbbbbbbb';
DECLARE @CatBebidasId     UNIQUEIDENTIFIER = 'bbbb0002-bbbb-bbbb-bbbb-bbbbbbbbbbbb';
DECLARE @CatFritosId      UNIQUEIDENTIFIER = 'bbbb0003-bbbb-bbbb-bbbb-bbbbbbbbbbbb';
DECLARE @CatDulcesId      UNIQUEIDENTIFIER = 'bbbb0004-bbbb-bbbb-bbbb-bbbbbbbbbbbb';
DECLARE @CatPastaId       UNIQUEIDENTIFIER = 'bbbb0005-bbbb-bbbb-bbbb-bbbbbbbbbbbb';
DECLARE @CatVerdulasId    UNIQUEIDENTIFIER = 'bbbb0006-bbbb-bbbb-bbbb-bbbbbbbbbbbb';

-- =============================================
-- VARIABLES — PRODUCTOS
-- =============================================

DECLARE @P01 UNIQUEIDENTIFIER = 'cccc0001-cccc-cccc-cccc-cccccccccccc';
DECLARE @P02 UNIQUEIDENTIFIER = 'cccc0002-cccc-cccc-cccc-cccccccccccc';
DECLARE @P03 UNIQUEIDENTIFIER = 'cccc0003-cccc-cccc-cccc-cccccccccccc';
DECLARE @P04 UNIQUEIDENTIFIER = 'cccc0004-cccc-cccc-cccc-cccccccccccc';
DECLARE @P05 UNIQUEIDENTIFIER = 'cccc0005-cccc-cccc-cccc-cccccccccccc';
DECLARE @P06 UNIQUEIDENTIFIER = 'cccc0006-cccc-cccc-cccc-cccccccccccc';
DECLARE @P07 UNIQUEIDENTIFIER = 'cccc0007-cccc-cccc-cccc-cccccccccccc';
DECLARE @P08 UNIQUEIDENTIFIER = 'cccc0008-cccc-cccc-cccc-cccccccccccc';
DECLARE @P09 UNIQUEIDENTIFIER = 'cccc0009-cccc-cccc-cccc-cccccccccccc';
DECLARE @P10 UNIQUEIDENTIFIER = 'cccc0010-cccc-cccc-cccc-cccccccccccc';
DECLARE @P11 UNIQUEIDENTIFIER = 'cccc0011-cccc-cccc-cccc-cccccccccccc';
DECLARE @P12 UNIQUEIDENTIFIER = 'cccc0012-cccc-cccc-cccc-cccccccccccc';
DECLARE @P13 UNIQUEIDENTIFIER = 'cccc0013-cccc-cccc-cccc-cccccccccccc';
DECLARE @P14 UNIQUEIDENTIFIER = 'cccc0014-cccc-cccc-cccc-cccccccccccc';
DECLARE @P15 UNIQUEIDENTIFIER = 'cccc0015-cccc-cccc-cccc-cccccccccccc';
DECLARE @P16 UNIQUEIDENTIFIER = 'cccc0016-cccc-cccc-cccc-cccccccccccc';
DECLARE @P17 UNIQUEIDENTIFIER = 'cccc0017-cccc-cccc-cccc-cccccccccccc';
DECLARE @P18 UNIQUEIDENTIFIER = 'cccc0018-cccc-cccc-cccc-cccccccccccc';
DECLARE @P19 UNIQUEIDENTIFIER = 'cccc0019-cccc-cccc-cccc-cccccccccccc';
DECLARE @P20 UNIQUEIDENTIFIER = 'cccc0020-cccc-cccc-cccc-cccccccccccc';
DECLARE @P21 UNIQUEIDENTIFIER = 'cccc0021-cccc-cccc-cccc-cccccccccccc';
DECLARE @P22 UNIQUEIDENTIFIER = 'cccc0022-cccc-cccc-cccc-cccccccccccc';
DECLARE @P23 UNIQUEIDENTIFIER = 'cccc0023-cccc-cccc-cccc-cccccccccccc';
DECLARE @P24 UNIQUEIDENTIFIER = 'cccc0024-cccc-cccc-cccc-cccccccccccc';
DECLARE @P25 UNIQUEIDENTIFIER = 'cccc0025-cccc-cccc-cccc-cccccccccccc';

-- =============================================
-- VARIABLES — PROMOCIONES
-- =============================================

DECLARE @Promo1Id UNIQUEIDENTIFIER = 'dddd0001-dddd-dddd-dddd-dddddddddddd';
DECLARE @Promo2Id UNIQUEIDENTIFIER = 'dddd0002-dddd-dddd-dddd-dddddddddddd';
DECLARE @Promo3Id UNIQUEIDENTIFIER = 'dddd0003-dddd-dddd-dddd-dddddddddddd';
DECLARE @Promo4Id UNIQUEIDENTIFIER = 'dddd0004-dddd-dddd-dddd-dddddddddddd';
DECLARE @Promo5Id UNIQUEIDENTIFIER = 'dddd0005-dddd-dddd-dddd-dddddddddddd';
DECLARE @Promo6Id UNIQUEIDENTIFIER = 'dddd0006-dddd-dddd-dddd-dddddddddddd';
DECLARE @Promo7Id UNIQUEIDENTIFIER = 'dddd0007-dddd-dddd-dddd-dddddddddddd';

-- =============================================
-- VARIABLES — TIPOS DE ENVIO
-- =============================================

DECLARE @ShipExpressId   UNIQUEIDENTIFIER = 'eeee0001-eeee-eeee-eeee-eeeeeeeeeeee';
DECLARE @ShipEstandarId  UNIQUEIDENTIFIER = 'eeee0002-eeee-eeee-eeee-eeeeeeeeeeee';
DECLARE @ShipProgramadoId UNIQUEIDENTIFIER = 'eeee0003-eeee-eeee-eeee-eeeeeeeeeeee';
DECLARE @ShipRecogerLocal UNIQUEIDENTIFIER = 'eeee0004-eeee-eeee-eeee-eeeeeeeeeeee';

-- =============================================
-- VARIABLES — PEDIDOS
-- =============================================

-- Enero 2026
DECLARE @O001 UNIQUEIDENTIFIER = 'ffff0001-ffff-ffff-ffff-ffffffffffff';
DECLARE @O002 UNIQUEIDENTIFIER = 'ffff0002-ffff-ffff-ffff-ffffffffffff';
DECLARE @O003 UNIQUEIDENTIFIER = 'ffff0003-ffff-ffff-ffff-ffffffffffff';
DECLARE @O004 UNIQUEIDENTIFIER = 'ffff0004-ffff-ffff-ffff-ffffffffffff';
DECLARE @O005 UNIQUEIDENTIFIER = 'ffff0005-ffff-ffff-ffff-ffffffffffff';
-- Febrero 2026
DECLARE @O006 UNIQUEIDENTIFIER = 'ffff0006-ffff-ffff-ffff-ffffffffffff';
DECLARE @O007 UNIQUEIDENTIFIER = 'ffff0007-ffff-ffff-ffff-ffffffffffff';
DECLARE @O008 UNIQUEIDENTIFIER = 'ffff0008-ffff-ffff-ffff-ffffffffffff';
DECLARE @O009 UNIQUEIDENTIFIER = 'ffff0009-ffff-ffff-ffff-ffffffffffff';
DECLARE @O010 UNIQUEIDENTIFIER = 'ffff0010-ffff-ffff-ffff-ffffffffffff';
-- Marzo 2026
DECLARE @O011 UNIQUEIDENTIFIER = 'ffff0011-ffff-ffff-ffff-ffffffffffff';
DECLARE @O012 UNIQUEIDENTIFIER = 'ffff0012-ffff-ffff-ffff-ffffffffffff';
DECLARE @O013 UNIQUEIDENTIFIER = 'ffff0013-ffff-ffff-ffff-ffffffffffff';
DECLARE @O014 UNIQUEIDENTIFIER = 'ffff0014-ffff-ffff-ffff-ffffffffffff';
DECLARE @O015 UNIQUEIDENTIFIER = 'ffff0015-ffff-ffff-ffff-ffffffffffff';
-- Abril 2026
DECLARE @O016 UNIQUEIDENTIFIER = 'ffff0016-ffff-ffff-ffff-ffffffffffff';
DECLARE @O017 UNIQUEIDENTIFIER = 'ffff0017-ffff-ffff-ffff-ffffffffffff';
DECLARE @O018 UNIQUEIDENTIFIER = 'ffff0018-ffff-ffff-ffff-ffffffffffff';
DECLARE @O019 UNIQUEIDENTIFIER = 'ffff0019-ffff-ffff-ffff-ffffffffffff';
DECLARE @O020 UNIQUEIDENTIFIER = 'ffff0020-ffff-ffff-ffff-ffffffffffff';
-- Mayo 2026
DECLARE @O021 UNIQUEIDENTIFIER = 'ffff0021-ffff-ffff-ffff-ffffffffffff';
DECLARE @O022 UNIQUEIDENTIFIER = 'ffff0022-ffff-ffff-ffff-ffffffffffff';
DECLARE @O023 UNIQUEIDENTIFIER = 'ffff0023-ffff-ffff-ffff-ffffffffffff';
DECLARE @O024 UNIQUEIDENTIFIER = 'ffff0024-ffff-ffff-ffff-ffffffffffff';
DECLARE @O025 UNIQUEIDENTIFIER = 'ffff0025-ffff-ffff-ffff-ffffffffffff';
-- Junio 2026
DECLARE @O026 UNIQUEIDENTIFIER = 'ffff0026-ffff-ffff-ffff-ffffffffffff';
DECLARE @O027 UNIQUEIDENTIFIER = 'ffff0027-ffff-ffff-ffff-ffffffffffff';
DECLARE @O028 UNIQUEIDENTIFIER = 'ffff0028-ffff-ffff-ffff-ffffffffffff';
DECLARE @O029 UNIQUEIDENTIFIER = 'ffff0029-ffff-ffff-ffff-ffffffffffff';
DECLARE @O030 UNIQUEIDENTIFIER = 'ffff0030-ffff-ffff-ffff-ffffffffffff';
DECLARE @O031 UNIQUEIDENTIFIER = 'ffff0031-ffff-ffff-ffff-ffffffffffff';
DECLARE @O032 UNIQUEIDENTIFIER = 'ffff0032-ffff-ffff-ffff-ffffffffffff';
-- Julio 2026
DECLARE @O033 UNIQUEIDENTIFIER = 'ffff0033-ffff-ffff-ffff-ffffffffffff';
DECLARE @O034 UNIQUEIDENTIFIER = 'ffff0034-ffff-ffff-ffff-ffffffffffff';
DECLARE @O035 UNIQUEIDENTIFIER = 'ffff0035-ffff-ffff-ffff-ffffffffffff';
DECLARE @O036 UNIQUEIDENTIFIER = 'ffff0036-ffff-ffff-ffff-ffffffffffff';
DECLARE @O037 UNIQUEIDENTIFIER = 'ffff0037-ffff-ffff-ffff-ffffffffffff';

-- =============================================
-- USUARIOS
-- =============================================
-- Hash de: ValidP@ssw0rd!8X
DECLARE @PwHash NVARCHAR(256) = '$2a$12$aZGd1nXfOuqRCZd4fV4p4Op4R6bPZvKQAwbD2ODT.7wXQJQIqXPn2';

INSERT INTO Users (Id, Name, Surname, Email, HashedPassword, Role, PhoneCountryPrefix, PhoneNumber) VALUES
(@Admin1Id, 'Carlos',   'Fernandez', 'admin@bmb.com',       @PwHash, 'Administrativo', '+598', '094111222'),
(@Admin2Id, 'Lucia',    'Mendez',    'admin2@bmb.com',      @PwHash, 'Administrativo', '+598', '094112233'),
(@Prep1Id,  'Pepe',     'Ruiz',      'preparador@bmb.com',  @PwHash, 'Preparador',     '+598', '094333444'),
(@Prep2Id,  'Andres',   'Silva',     'preparador2@bmb.com', @PwHash, 'Preparador',     '+598', '094334455'),
(@Prep3Id,  'Natalia',  'Gomez',     'preparador3@bmb.com', @PwHash, 'Preparador',     '+598', '094335566'),
(@Cli1Id,   'Juan',     'Sosa',      'cliente@bmb.com',     @PwHash, 'Cliente',        '+598', '094444555'),
(@Cli2Id,   'Maria',    'Lopez',     'cliente2@bmb.com',    @PwHash, 'Cliente',        '+598', '094445566'),
(@Cli3Id,   'Diego',    'Pereira',   'cliente3@bmb.com',    @PwHash, 'Cliente',        '+598', '094446677'),
(@Cli4Id,   'Sofia',    'Garcia',    'cliente4@bmb.com',    @PwHash, 'Cliente',        '+598', '094447788'),
(@Cli5Id,   'Martin',   'Rodriguez', 'cliente5@bmb.com',    @PwHash, 'Cliente',        '+598', '094448899'),
(@Cli6Id,   'Valentina','Torres',    'cliente6@bmb.com',    @PwHash, 'Cliente',        '+598', '094449900'),
(@Cli7Id,   'Pablo',    'Castro',    'cliente7@bmb.com',    @PwHash, 'Cliente',        '+598', '094450011'),
(@Cli8Id,   'Camila',   'Diaz',      'cliente8@bmb.com',    @PwHash, 'Cliente',        '+598', '094451122');

-- =============================================
-- LINEAS DE PRODUCTO
-- =============================================

INSERT INTO ProductLines (Id, Name) VALUES
(@LineComboId,     'Combo burgers'),
(@LineDesayunosId, 'Desayunos'),
(@LineMinutasId,   'Minutas clasicas'),
(@LinePastaId,     'Pastas artesanales'),
(@LinePostresId,   'Postres y dulces'),
(@LineBebidasId,   'Bebidas y jugos');

-- =============================================
-- CATEGORIAS DE PRODUCTO
-- =============================================

INSERT INTO ProductCategories (Id, Name) VALUES
(@CatParrillaId, 'Parrilla'),
(@CatBebidasId,  'Bebidas'),
(@CatFritosId,   'Fritos'),
(@CatDulcesId,   'Dulces'),
(@CatPastaId,    'Pasta'),
(@CatVerdulasId, 'Verduras y ensaladas');

-- =============================================
-- TIPOS DE ENVIO
-- =============================================

INSERT INTO ShippingTypes (Id, Name, Cost) VALUES
(@ShipExpressId,    'Express',      150.00),
(@ShipEstandarId,   'Estandar',      50.00),
(@ShipProgramadoId, 'Programado',    30.00),
(@ShipRecogerLocal, 'RetiroEnLocal',  0.00);

-- =============================================
-- PRODUCTOS (25)
-- =============================================

INSERT INTO Products (Id, Code, Name, Description, Price, IsActive, ProductLineId, ProductCategoryId) VALUES
(@P01, 'BURG01', 'Hamburguesa Clasica',          'Hamburguesa clasica con queso cheddar y lechuga fresca',         150.00, 1, @LineComboId,     @CatParrillaId),
(@P02, 'BURG02', 'Hamburguesa Doble',             'Doble hamburguesa con queso y bacon ahumado artesanal',          200.00, 1, @LineComboId,     @CatParrillaId),
(@P03, 'BURG03', 'Hamburguesa BBQ',               'Hamburguesa con salsa BBQ casera y cebolla caramelizada',        185.00, 1, @LineComboId,     @CatParrillaId),
(@P04, 'BURG04', 'Hamburguesa Vegana',             'Medallon de legumbres con vegetales frescos y pan integral',    170.00, 1, @LineComboId,     @CatVerdulasId),
(@P05, 'BURG05', 'Hamburguesa Crispy Chicken',    'Pollo crocante con mayonesa de ajo y pepinos encurtidos',        175.00, 1, @LineComboId,     @CatFritosId),
(@P06, 'DESA01', 'Desayuno Completo',             'Cafe, tostadas, jugo natural y fruta de temporada',             120.00, 1, @LineDesayunosId, @CatBebidasId),
(@P07, 'DESA02', 'Desayuno Liviano',              'Yogur con granola y frutos rojos',                               90.00, 1, @LineDesayunosId, @CatDulcesId),
(@P08, 'DESA03', 'Desayuno Ejecutivo',            'Huevos revueltos, tostadas integrales y cafe americano',        145.00, 1, @LineDesayunosId, @CatParrillaId),
(@P09, 'MIN01',  'Milanesa Napolitana',           'Milanesa napolitana con jamon, queso y salsa de tomate',        180.00, 1, @LineMinutasId,   @CatFritosId),
(@P10, 'MIN02',  'Milanesa a la Portuguesa',      'Milanesa con morrones, aceitunas y salsa portuguesa',           190.00, 1, @LineMinutasId,   @CatFritosId),
(@P11, 'MIN03',  'Pollo al Limón',                'Suprema de pollo a la plancha con limon y hierbas',             165.00, 1, @LineMinutasId,   @CatParrillaId),
(@P12, 'MIN04',  'Bife de Chorizo',               'Bife de chorizo con papas fritas y ensalada',                   280.00, 1, @LineMinutasId,   @CatParrillaId),
(@P13, 'PAS01',  'Spaghetti Bolognese',           'Spaghetti con salsa boloñesa de carne vacuna artesanal',        140.00, 1, @LinePastaId,     @CatPastaId),
(@P14, 'PAS02',  'Fetuccine Alfredo',             'Fetuccine con salsa alfredo y parmesano rallado',               135.00, 1, @LinePastaId,     @CatPastaId),
(@P15, 'PAS03',  'Ravioles de Ricota',            'Ravioles caseros rellenos de ricota y espinaca',                155.00, 1, @LinePastaId,     @CatPastaId),
(@P16, 'PAS04',  'Lasagna Clasica',               'Lasagna con carne, bechamel y queso gratinado',                 160.00, 0, @LinePastaId,     @CatPastaId),
(@P17, 'POST01', 'Volcan de Chocolate',           'Bizcocho tibio de chocolate con helado de vainilla',            110.00, 1, @LinePostresId,   @CatDulcesId),
(@P18, 'POST02', 'Tiramisu',                      'Tiramisu clasico con cafe, mascarpone y cacao',                  95.00, 1, @LinePostresId,   @CatDulcesId),
(@P19, 'POST03', 'Cheesecake de Frutos Rojos',    'Cheesecake cremosa con coulis de frutillas y arandanos',        105.00, 1, @LinePostresId,   @CatDulcesId),
(@P20, 'POST04', 'Panqueques con Dulce de Leche', 'Panqueques esponjosos rellenos de dulce de leche artesanal',    85.00, 1, @LinePostresId,   @CatDulcesId),
(@P21, 'BEB01',  'Jugo Natural de Naranja',       'Jugo exprimido al momento de naranjas frescas',                  45.00, 1, @LineBebidasId,   @CatBebidasId),
(@P22, 'BEB02',  'Limonada con Jengibre',         'Limonada artesanal con jengibre fresco y menta',                 50.00, 1, @LineBebidasId,   @CatBebidasId),
(@P23, 'BEB03',  'Smoothie Verde',                'Smoothie de espinaca, banana, kiwi y leche de almendras',        65.00, 1, @LineBebidasId,   @CatVerdulasId),
(@P24, 'BEB04',  'Cafe Especial',                 'Cafe de especialidad con extraccion por goteo',                  55.00, 1, @LineBebidasId,   @CatBebidasId),
(@P25, 'ENSALA1','Ensalada Caesar',               'Ensalada caesar con crutones, parmesano y aderezo casero',       95.00, 0, @LineMinutasId,   @CatVerdulasId);

-- =============================================
-- IMAGENES DE PRODUCTO
-- =============================================

INSERT INTO ProductImages (Id, Url, SizeInBytes, ProductId) VALUES
(NEWID(), 'https://images.dk.com/burg01-a.jpg', 52000, @P01),
(NEWID(), 'https://images.dk.com/burg01-b.jpg', 48000, @P01),
(NEWID(), 'https://images.dk.com/burg02-a.jpg', 55000, @P02),
(NEWID(), 'https://images.dk.com/burg03-a.jpg', 51000, @P03),
(NEWID(), 'https://images.dk.com/burg04-a.jpg', 49000, @P04),
(NEWID(), 'https://images.dk.com/burg05-a.jpg', 53000, @P05),
(NEWID(), 'https://images.dk.com/desa01-a.jpg', 44000, @P06),
(NEWID(), 'https://images.dk.com/desa01-b.jpg', 40000, @P06),
(NEWID(), 'https://images.dk.com/desa02-a.jpg', 38000, @P07),
(NEWID(), 'https://images.dk.com/desa03-a.jpg', 46000, @P08),
(NEWID(), 'https://images.dk.com/min01-a.jpg',  70000, @P09),
(NEWID(), 'https://images.dk.com/min02-a.jpg',  68000, @P10),
(NEWID(), 'https://images.dk.com/min03-a.jpg',  55000, @P11),
(NEWID(), 'https://images.dk.com/min04-a.jpg',  72000, @P12),
(NEWID(), 'https://images.dk.com/pas01-a.jpg',  50000, @P13),
(NEWID(), 'https://images.dk.com/pas02-a.jpg',  48000, @P14),
(NEWID(), 'https://images.dk.com/pas03-a.jpg',  52000, @P15),
(NEWID(), 'https://images.dk.com/pas04-a.jpg',  47000, @P16),
(NEWID(), 'https://images.dk.com/post01-a.jpg', 41000, @P17),
(NEWID(), 'https://images.dk.com/post02-a.jpg', 39000, @P18),
(NEWID(), 'https://images.dk.com/post03-a.jpg', 43000, @P19),
(NEWID(), 'https://images.dk.com/post04-a.jpg', 37000, @P20),
(NEWID(), 'https://images.dk.com/beb01-a.jpg',  30000, @P21),
(NEWID(), 'https://images.dk.com/beb02-a.jpg',  32000, @P22),
(NEWID(), 'https://images.dk.com/beb03-a.jpg',  35000, @P23),
(NEWID(), 'https://images.dk.com/beb04-a.jpg',  28000, @P24),
(NEWID(), 'https://images.dk.com/ensa01-a.jpg', 42000, @P25);

-- =============================================
-- PROMOCIONES
-- =============================================
-- Promo expirada (solo pasado)
-- Promo activa todo el año
-- Promo activa presente y futuro
-- Promo solo junio
-- Promo solo julio (futura)
-- Promo anulada/inactiva

INSERT INTO Promotions (Id, Name, IsActive, DiscountPercentage, StartDate, EndDate) VALUES
(@Promo1Id, 'Lanzamiento Enero',       0,  20, '2026-01-01', '2026-01-31'),  -- expirada
(@Promo2Id, 'Black Friday Extended',   1,  10, '2026-01-01', '2026-12-31'),  -- todo el año
(@Promo3Id, 'Semana de Turismo',       1,  15, '2026-03-29', '2026-04-06'),  -- expirada pero fue activa
(@Promo4Id, 'Dia del Padre',           1,  12, '2026-06-01', '2026-06-30'),  -- junio activa ahora
(@Promo5Id, 'Promo Julio',             1,  18, '2026-07-01', '2026-07-07'),  -- futura (empieza en julio)
(@Promo6Id, 'Combo Desayuno',          1,   8, '2026-05-01', '2026-08-31'),  -- presente y futuro
(@Promo7Id, 'Descuento Postres',       0,  25, '2026-02-14', '2026-02-28'); -- expirada, San Valentin

INSERT INTO PromotionProducts (PromotionId, ProductsId) VALUES
-- Lanzamiento Enero: burgers
(@Promo1Id, @P01), (@Promo1Id, @P02), (@Promo1Id, @P03),
-- Black Friday: todos los combos
(@Promo2Id, @P01), (@Promo2Id, @P02), (@Promo2Id, @P03), (@Promo2Id, @P04), (@Promo2Id, @P05),
-- Semana de Turismo: minutas
(@Promo3Id, @P09), (@Promo3Id, @P10), (@Promo3Id, @P11),
-- Dia del Padre: carnes y minutas
(@Promo4Id, @P09), (@Promo4Id, @P10), (@Promo4Id, @P11), (@Promo4Id, @P12),
-- Promo Julio: pastas
(@Promo5Id, @P13), (@Promo5Id, @P14), (@Promo5Id, @P15),
-- Combo Desayuno: desayunos
(@Promo6Id, @P06), (@Promo6Id, @P07), (@Promo6Id, @P08),
-- Descuento Postres: postres
(@Promo7Id, @P17), (@Promo7Id, @P18), (@Promo7Id, @P19), (@Promo7Id, @P20);

-- =============================================
-- PEDIDOS  (37 pedidos — enero a julio 2026)
-- Estado: Pending | Delayed | Prepared | Shipping | Delivered | NotDelivered | Cancelled
-- =============================================

INSERT INTO Orders (Id, OrderNumber, ClientId, Street, DoorNumber, Apartment, City, Country, CreatedAt, LastTransitionDate, State, ShippingCost) VALUES
-- ---- ENERO 2026 ----
(@O001,  1,  @Cli1Id, 'Rivera',           '1234', NULL, 'Montevideo', 'Uruguay', '2026-01-05 09:00:00', '2026-01-05 11:30:00', 'Delivered',    150.00),
(@O002,  2,  @Cli2Id, 'Bvar Artigas',     '4567', '3B', 'Montevideo', 'Uruguay', '2026-01-10 13:00:00', '2026-01-10 15:00:00', 'Delivered',     50.00),
(@O003,  3,  @Cli3Id, 'Av Italia',        '789',  NULL, 'Montevideo', 'Uruguay', '2026-01-15 08:00:00', '2026-01-15 08:30:00', 'Cancelled',     30.00),
(@O004,  4,  @Cli4Id, 'Av Brasil',        '321',  '2A', 'Montevideo', 'Uruguay', '2026-01-20 18:00:00', '2026-01-21 10:00:00', 'Delivered',    150.00),
(@O005,  5,  @Cli5Id, 'Br España',        '100',  NULL, 'Montevideo', 'Uruguay', '2026-01-28 12:00:00', '2026-01-28 14:00:00', 'NotDelivered',  50.00),
-- ---- FEBRERO 2026 ----
(@O006,  6,  @Cli1Id, 'Rivera',           '1234', NULL, 'Montevideo', 'Uruguay', '2026-02-03 10:00:00', '2026-02-03 12:30:00', 'Delivered',     50.00),
(@O007,  7,  @Cli6Id, 'Larrañaga',        '555',  '1C', 'Montevideo', 'Uruguay', '2026-02-07 09:30:00', '2026-02-07 09:45:00', 'Cancelled',      0.00),
(@O008,  8,  @Cli2Id, 'Bvar Artigas',     '4567', '3B', 'Montevideo', 'Uruguay', '2026-02-14 20:00:00', '2026-02-14 22:00:00', 'Delivered',    150.00),
(@O009,  9,  @Cli7Id, 'Gral Flores',      '222',  NULL, 'Montevideo', 'Uruguay', '2026-02-18 11:00:00', '2026-02-18 13:00:00', 'Delivered',     50.00),
(@O010, 10,  @Cli3Id, 'Av Italia',        '789',  NULL, 'Montevideo', 'Uruguay', '2026-02-25 16:00:00', '2026-02-25 18:30:00', 'Delivered',     30.00),
-- ---- MARZO 2026 ----
(@O011, 11,  @Cli4Id, 'Av Brasil',        '321',  '2A', 'Montevideo', 'Uruguay', '2026-03-02 08:00:00', '2026-03-02 09:00:00', 'Delivered',      0.00),
(@O012, 12,  @Cli8Id, 'Dr Prudencio',     '101',  '4D', 'Montevideo', 'Uruguay', '2026-03-10 14:00:00', '2026-03-10 16:00:00', 'Delivered',     50.00),
(@O013, 13,  @Cli5Id, 'Br España',        '100',  NULL, 'Montevideo', 'Uruguay', '2026-03-15 19:00:00', '2026-03-15 19:30:00', 'Cancelled',    150.00),
(@O014, 14,  @Cli1Id, 'Rivera',           '1234', NULL, 'Montevideo', 'Uruguay', '2026-03-20 10:00:00', '2026-03-20 12:00:00', 'Delayed',       50.00),
(@O015, 15,  @Cli2Id, 'Bvar Artigas',     '4567', '3B', 'Montevideo', 'Uruguay', '2026-03-29 13:00:00', '2026-03-29 15:00:00', 'Delivered',     30.00),
-- ---- ABRIL 2026 ----
(@O016, 16,  @Cli6Id, 'Larrañaga',        '555',  '1C', 'Montevideo', 'Uruguay', '2026-04-01 09:00:00', '2026-04-01 11:00:00', 'Delivered',    150.00),
(@O017, 17,  @Cli7Id, 'Gral Flores',      '222',  NULL, 'Montevideo', 'Uruguay', '2026-04-08 12:30:00', '2026-04-08 14:30:00', 'Delivered',     50.00),
(@O018, 18,  @Cli3Id, 'Av Italia',        '789',  NULL, 'Montevideo', 'Uruguay', '2026-04-15 15:00:00', '2026-04-15 17:00:00', 'NotDelivered',  50.00),
(@O019, 19,  @Cli8Id, 'Dr Prudencio',     '101',  '4D', 'Montevideo', 'Uruguay', '2026-04-22 18:00:00', '2026-04-22 20:00:00', 'Delivered',     30.00),
(@O020, 20,  @Cli4Id, 'Av Brasil',        '321',  '2A', 'Montevideo', 'Uruguay', '2026-04-28 10:00:00', '2026-04-28 10:30:00', 'Cancelled',      0.00),
-- ---- MAYO 2026 ----
(@O021, 21,  @Cli1Id, 'Rivera',           '1234', NULL, 'Montevideo', 'Uruguay', '2026-05-02 09:00:00', '2026-05-02 11:00:00', 'Delivered',    150.00),
(@O022, 22,  @Cli5Id, 'Br España',        '100',  NULL, 'Montevideo', 'Uruguay', '2026-05-08 13:00:00', '2026-05-08 15:30:00', 'Delivered',     50.00),
(@O023, 23,  @Cli2Id, 'Bvar Artigas',     '4567', '3B', 'Montevideo', 'Uruguay', '2026-05-14 11:00:00', '2026-05-14 13:00:00', 'Delayed',       30.00),
(@O024, 24,  @Cli6Id, 'Larrañaga',        '555',  '1C', 'Montevideo', 'Uruguay', '2026-05-20 08:00:00', '2026-05-20 10:00:00', 'Delivered',    150.00),
(@O025, 25,  @Cli7Id, 'Gral Flores',      '222',  NULL, 'Montevideo', 'Uruguay', '2026-05-27 19:00:00', '2026-05-27 21:00:00', 'Delivered',     50.00),
-- ---- JUNIO 2026 (presente) ----
(@O026, 26,  @Cli3Id, 'Av Italia',        '789',  NULL, 'Montevideo', 'Uruguay', '2026-06-01 10:00:00', '2026-06-01 12:00:00', 'Delivered',      0.00),
(@O027, 27,  @Cli8Id, 'Dr Prudencio',     '101',  '4D', 'Montevideo', 'Uruguay', '2026-06-03 14:00:00', '2026-06-03 16:00:00', 'Delivered',     50.00),
(@O028, 28,  @Cli1Id, 'Rivera',           '1234', NULL, 'Montevideo', 'Uruguay', '2026-06-05 09:30:00', '2026-06-05 09:30:00', 'Pending',      150.00),
(@O029, 29,  @Cli4Id, 'Av Brasil',        '321',  '2A', 'Montevideo', 'Uruguay', '2026-06-08 11:00:00', '2026-06-08 11:30:00', 'Prepared',      50.00),
(@O030, 30,  @Cli2Id, 'Bvar Artigas',     '4567', '3B', 'Montevideo', 'Uruguay', '2026-06-09 13:00:00', '2026-06-09 13:00:00', 'Pending',       30.00),
(@O031, 31,  @Cli5Id, 'Br España',        '100',  NULL, 'Montevideo', 'Uruguay', '2026-06-10 08:00:00', '2026-06-10 09:30:00', 'Shipping',     150.00),
(@O032, 32,  @Cli6Id, 'Larrañaga',        '555',  '1C', 'Montevideo', 'Uruguay', '2026-06-12 07:45:00', '2026-06-12 07:45:00', 'Pending',       50.00),
-- ---- JULIO 2026 (futuro hasta el 7) ----
(@O033, 33,  @Cli7Id, 'Gral Flores',      '222',  NULL, 'Montevideo', 'Uruguay', '2026-07-01 10:00:00', '2026-07-01 10:00:00', 'Pending',       30.00),
(@O034, 34,  @Cli3Id, 'Av Italia',        '789',  NULL, 'Montevideo', 'Uruguay', '2026-07-02 11:30:00', '2026-07-02 11:30:00', 'Pending',      150.00),
(@O035, 35,  @Cli8Id, 'Dr Prudencio',     '101',  '4D', 'Montevideo', 'Uruguay', '2026-07-03 09:00:00', '2026-07-03 09:00:00', 'Pending',       50.00),
(@O036, 36,  @Cli1Id, 'Rivera',           '1234', NULL, 'Montevideo', 'Uruguay', '2026-07-05 14:00:00', '2026-07-05 14:00:00', 'Pending',        0.00),
(@O037, 37,  @Cli4Id, 'Av Brasil',        '321',  '2A', 'Montevideo', 'Uruguay', '2026-07-07 08:00:00', '2026-07-07 08:00:00', 'Pending',       50.00);

-- =============================================
-- ITEMS DE PEDIDOS
-- =============================================

INSERT INTO OrderItems (Id, ProductId, Quantity, Price, DiscountPercentage, AppliedPromotionName, OrderId) VALUES
-- O001 (Enero, entregado)
(NEWID(), @P01, 2, 150.00, 20, 'Lanzamiento Enero', @O001),
(NEWID(), @P02, 1, 200.00, 20, 'Lanzamiento Enero', @O001),
(NEWID(), @P21, 2,  45.00,  0, NULL,                @O001),
-- O002
(NEWID(), @P06, 1, 120.00,  0, NULL,  @O002),
(NEWID(), @P24, 1,  55.00,  0, NULL,  @O002),
-- O003
(NEWID(), @P09, 1, 180.00,  0, NULL,  @O003),
-- O004
(NEWID(), @P01, 3, 150.00, 20, 'Lanzamiento Enero', @O004),
(NEWID(), @P22, 2,  50.00,  0, NULL,                @O004),
-- O005
(NEWID(), @P12, 1, 280.00,  0, NULL,  @O005),
(NEWID(), @P21, 1,  45.00,  0, NULL,  @O005),
-- O006 (Febrero)
(NEWID(), @P09, 1, 180.00,  0, NULL,  @O006),
(NEWID(), @P10, 1, 190.00,  0, NULL,  @O006),
(NEWID(), @P24, 1,  55.00,  0, NULL,  @O006),
-- O007
(NEWID(), @P17, 2, 110.00, 25, 'Descuento Postres', @O007),
-- O008
(NEWID(), @P01, 2, 150.00, 10, 'Black Friday Extended', @O008),
(NEWID(), @P03, 2, 185.00, 10, 'Black Friday Extended', @O008),
(NEWID(), @P22, 2,  50.00,  0, NULL,                    @O008),
-- O009
(NEWID(), @P13, 2, 140.00,  0, NULL,  @O009),
(NEWID(), @P18, 1,  95.00, 25, 'Descuento Postres', @O009),
-- O010
(NEWID(), @P11, 1, 165.00,  0, NULL,  @O010),
(NEWID(), @P23, 1,  65.00,  0, NULL,  @O010),
-- O011 (Marzo)
(NEWID(), @P06, 2, 120.00,  0, NULL,  @O011),
(NEWID(), @P07, 1,  90.00,  0, NULL,  @O011),
-- O012
(NEWID(), @P09, 1, 180.00, 15, 'Semana de Turismo', @O012),
(NEWID(), @P10, 1, 190.00, 15, 'Semana de Turismo', @O012),
(NEWID(), @P21, 2,  45.00,  0, NULL,                @O012),
-- O013
(NEWID(), @P02, 1, 200.00, 10, 'Black Friday Extended', @O013),
-- O014
(NEWID(), @P14, 2, 135.00,  0, NULL,  @O014),
(NEWID(), @P24, 2,  55.00,  0, NULL,  @O014),
-- O015
(NEWID(), @P09, 2, 180.00, 15, 'Semana de Turismo', @O015),
(NEWID(), @P22, 1,  50.00,  0, NULL,                @O015),
-- O016 (Abril)
(NEWID(), @P03, 2, 185.00, 10, 'Black Friday Extended', @O016),
(NEWID(), @P05, 1, 175.00, 10, 'Black Friday Extended', @O016),
-- O017
(NEWID(), @P17, 1, 110.00,  0, NULL,  @O017),
(NEWID(), @P19, 1, 105.00,  0, NULL,  @O017),
(NEWID(), @P20, 1,  85.00,  0, NULL,  @O017),
-- O018
(NEWID(), @P12, 1, 280.00,  0, NULL,  @O018),
(NEWID(), @P23, 1,  65.00,  0, NULL,  @O018),
-- O019
(NEWID(), @P13, 1, 140.00,  0, NULL,  @O019),
(NEWID(), @P15, 1, 155.00,  0, NULL,  @O019),
-- O020
(NEWID(), @P01, 1, 150.00, 10, 'Black Friday Extended', @O020),
-- O021 (Mayo)
(NEWID(), @P06, 1, 120.00,  8, 'Combo Desayuno', @O021),
(NEWID(), @P07, 1,  90.00,  8, 'Combo Desayuno', @O021),
(NEWID(), @P21, 2,  45.00,  0, NULL,             @O021),
-- O022
(NEWID(), @P01, 2, 150.00, 10, 'Black Friday Extended', @O022),
(NEWID(), @P04, 1, 170.00, 10, 'Black Friday Extended', @O022),
-- O023
(NEWID(), @P14, 2, 135.00,  0, NULL,  @O023),
(NEWID(), @P18, 2,  95.00,  0, NULL,  @O023),
-- O024
(NEWID(), @P09, 1, 180.00,  0, NULL,  @O024),
(NEWID(), @P11, 1, 165.00,  0, NULL,  @O024),
(NEWID(), @P22, 2,  50.00,  0, NULL,  @O024),
-- O025
(NEWID(), @P17, 2, 110.00,  0, NULL,  @O025),
(NEWID(), @P19, 1, 105.00,  0, NULL,  @O025),
-- O026 (Junio)
(NEWID(), @P08, 1, 145.00,  8, 'Combo Desayuno', @O026),
(NEWID(), @P24, 1,  55.00,  0, NULL,             @O026),
-- O027
(NEWID(), @P09, 1, 180.00, 12, 'Dia del Padre', @O027),
(NEWID(), @P12, 1, 280.00, 12, 'Dia del Padre', @O027),
(NEWID(), @P21, 2,  45.00,  0, NULL,            @O027),
-- O028
(NEWID(), @P01, 3, 150.00, 10, 'Black Friday Extended', @O028),
(NEWID(), @P05, 1, 175.00, 10, 'Black Friday Extended', @O028),
-- O029
(NEWID(), @P13, 2, 140.00,  0, NULL,  @O029),
(NEWID(), @P15, 1, 155.00,  0, NULL,  @O029),
-- O030
(NEWID(), @P06, 2, 120.00,  8, 'Combo Desayuno', @O030),
(NEWID(), @P08, 1, 145.00,  8, 'Combo Desayuno', @O030),
-- O031
(NEWID(), @P02, 2, 200.00, 10, 'Black Friday Extended', @O031),
(NEWID(), @P03, 1, 185.00, 10, 'Black Friday Extended', @O031),
(NEWID(), @P23, 1,  65.00,  0, NULL,                    @O031),
-- O032
(NEWID(), @P17, 1, 110.00,  0, NULL,  @O032),
(NEWID(), @P20, 2,  85.00,  0, NULL,  @O032),
-- O033 (Julio)
(NEWID(), @P13, 2, 140.00, 18, 'Promo Julio', @O033),
(NEWID(), @P14, 1, 135.00, 18, 'Promo Julio', @O033),
-- O034
(NEWID(), @P09, 1, 180.00, 12, 'Dia del Padre', @O034),
(NEWID(), @P11, 2, 165.00, 12, 'Dia del Padre', @O034),
(NEWID(), @P22, 2,  50.00,  0, NULL,            @O034),
-- O035
(NEWID(), @P01, 4, 150.00, 10, 'Black Friday Extended', @O035),
(NEWID(), @P21, 4,  45.00,  0, NULL,                    @O035),
-- O036
(NEWID(), @P06, 1, 120.00,  8, 'Combo Desayuno', @O036),
(NEWID(), @P07, 2,  90.00,  8, 'Combo Desayuno', @O036),
(NEWID(), @P24, 1,  55.00,  0, NULL,             @O036),
-- O037
(NEWID(), @P15, 2, 155.00, 18, 'Promo Julio', @O037),
(NEWID(), @P18, 1,  95.00,  0, NULL,          @O037),
(NEWID(), @P19, 1, 105.00,  0, NULL,          @O037);

-- =============================================
-- LOGS DE AUDITORIA
-- =============================================

INSERT INTO AuditLogs (Id, Timestamp, EntityName, EntityId, ChangeDescription, ResponsibleUser) VALUES
-- Enero
(NEWID(), '2026-01-02 09:00:00', 'Product',   @P01,     'Producto creado: Hamburguesa Clasica',           'admin@bmb.com'),
(NEWID(), '2026-01-02 09:05:00', 'Product',   @P02,     'Producto creado: Hamburguesa Doble',             'admin@bmb.com'),
(NEWID(), '2026-01-02 09:10:00', 'Product',   @P03,     'Producto creado: Hamburguesa BBQ',               'admin@bmb.com'),
(NEWID(), '2026-01-02 09:15:00', 'Product',   @P04,     'Producto creado: Hamburguesa Vegana',            'admin@bmb.com'),
(NEWID(), '2026-01-02 09:20:00', 'Product',   @P05,     'Producto creado: Hamburguesa Crispy Chicken',    'admin@bmb.com'),
(NEWID(), '2026-01-03 10:00:00', 'Promotion', @Promo1Id,'Promocion creada: Lanzamiento Enero 20%',        'admin@bmb.com'),
(NEWID(), '2026-01-15 11:00:00', 'Product',   @P09,     'Producto creado: Milanesa Napolitana',           'admin@bmb.com'),
(NEWID(), '2026-01-15 11:05:00', 'Product',   @P10,     'Producto creado: Milanesa a la Portuguesa',      'admin@bmb.com'),
(NEWID(), '2026-01-15 11:10:00', 'Product',   @P11,     'Producto creado: Pollo al Limon',                'admin@bmb.com'),
(NEWID(), '2026-01-15 11:15:00', 'Product',   @P12,     'Producto creado: Bife de Chorizo',               'admin@bmb.com'),
-- Febrero
(NEWID(), '2026-02-01 08:00:00', 'Product',   @P06,     'Producto creado: Desayuno Completo',             'admin2@bmb.com'),
(NEWID(), '2026-02-01 08:05:00', 'Product',   @P07,     'Producto creado: Desayuno Liviano',              'admin2@bmb.com'),
(NEWID(), '2026-02-01 08:10:00', 'Product',   @P08,     'Producto creado: Desayuno Ejecutivo',            'admin2@bmb.com'),
(NEWID(), '2026-02-13 10:00:00', 'Promotion', @Promo7Id,'Promocion creada: Descuento Postres San Valentin','admin@bmb.com'),
(NEWID(), '2026-02-20 14:00:00', 'Product',   @P01,     'Precio actualizado: 140 -> 150',                 'admin@bmb.com'),
-- Marzo
(NEWID(), '2026-03-01 09:00:00', 'Product',   @P13,     'Producto creado: Spaghetti Bolognese',           'admin2@bmb.com'),
(NEWID(), '2026-03-01 09:05:00', 'Product',   @P14,     'Producto creado: Fetuccine Alfredo',             'admin2@bmb.com'),
(NEWID(), '2026-03-01 09:10:00', 'Product',   @P15,     'Producto creado: Ravioles de Ricota',            'admin2@bmb.com'),
(NEWID(), '2026-03-25 11:00:00', 'Promotion', @Promo3Id,'Promocion creada: Semana de Turismo 15%',        'admin@bmb.com'),
-- Abril
(NEWID(), '2026-04-01 08:00:00', 'Product',   @P17,     'Producto creado: Volcan de Chocolate',           'admin@bmb.com'),
(NEWID(), '2026-04-01 08:05:00', 'Product',   @P18,     'Producto creado: Tiramisu',                      'admin@bmb.com'),
(NEWID(), '2026-04-01 08:10:00', 'Product',   @P19,     'Producto creado: Cheesecake de Frutos Rojos',    'admin@bmb.com'),
(NEWID(), '2026-04-01 08:15:00', 'Product',   @P20,     'Producto creado: Panqueques con Dulce de Leche', 'admin@bmb.com'),
(NEWID(), '2026-04-07 10:00:00', 'Promotion', @Promo3Id,'Promocion expirada: Semana de Turismo',          'admin@bmb.com'),
(NEWID(), '2026-04-15 12:00:00', 'Product',   @P02,     'Precio actualizado: 190 -> 200',                 'admin2@bmb.com'),
-- Mayo
(NEWID(), '2026-05-01 09:00:00', 'Promotion', @Promo6Id,'Promocion creada: Combo Desayuno 8%',            'admin@bmb.com'),
(NEWID(), '2026-05-01 09:05:00', 'Promotion', @Promo2Id,'Promocion actualizada: Black Friday 10%',        'admin@bmb.com'),
(NEWID(), '2026-05-10 10:00:00', 'Product',   @P21,     'Producto creado: Jugo Natural de Naranja',       'admin2@bmb.com'),
(NEWID(), '2026-05-10 10:05:00', 'Product',   @P22,     'Producto creado: Limonada con Jengibre',         'admin2@bmb.com'),
(NEWID(), '2026-05-10 10:10:00', 'Product',   @P23,     'Producto creado: Smoothie Verde',                'admin2@bmb.com'),
(NEWID(), '2026-05-10 10:15:00', 'Product',   @P24,     'Producto creado: Cafe Especial',                 'admin2@bmb.com'),
(NEWID(), '2026-05-20 15:00:00', 'Product',   @P16,     'Producto desactivado: Lasagna Clasica',          'admin@bmb.com'),
-- Junio
(NEWID(), '2026-06-01 08:00:00', 'Promotion', @Promo4Id,'Promocion creada: Dia del Padre 12%',            'admin@bmb.com'),
(NEWID(), '2026-06-05 10:00:00', 'Product',   @P03,     'Precio actualizado: 175 -> 185',                 'admin@bmb.com'),
(NEWID(), '2026-06-10 09:00:00', 'Product',   @P09,     'Descripcion actualizada: Milanesa Napolitana',   'admin2@bmb.com'),
(NEWID(), '2026-06-12 08:30:00', 'Promotion', @Promo5Id,'Promocion creada: Promo Julio 18%',              'admin@bmb.com');