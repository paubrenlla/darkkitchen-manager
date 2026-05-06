-- =============================================
-- DarkKitchen - Script de estructura vacía
-- Contraseña del admin: ValidP@ssw0rd!8X
-- =============================================

USE DarkKitchenDB;

-- Limpiar datos existentes
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
-- USUARIO ADMIN INICIAL
-- Contraseña: ValidP@ssw0rd!8X
-- =============================================

INSERT INTO Users (Id, Name, Surname, Email, HashedPassword, Role, PhoneCountryPrefix, PhoneNumber)
VALUES
(
    '11111111-1111-1111-1111-111111111111',
    'Admin',
    'Jefe',
    'admin@bmb.com',
    '$2a$12$aZGd1nXfOuqRCZd4fV4p4Op4R6bPZvKQAwbD2ODT.7wXQJQIqXPn2',
    'Administrativo',
    '+598',
    '094222333'
);