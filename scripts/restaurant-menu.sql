-- Optional portfolio menu for a FRESH, explicitly selected database after EF migrations.
-- Idempotent: retains old records/prices, publishes these categories and hides others.
-- Do not execute against an unrelated database. No users/accounts are seeded.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
DECLARE @Menu TABLE(Category nvarchar(120),Product nvarchar(120),Price decimal(18,2));
INSERT @Menu VALUES
(N'Burgers',N'Classic Beef Burger',185),(N'Burgers',N'Crispy Chicken Burger',165),
(N'Pizza',N'Margherita Pizza',150),(N'Pizza',N'Pepperoni Pizza',185),
(N'Grills',N'Grilled Chicken',220),(N'Grills',N'Mixed Grill Plate',290),
(N'Salads & Sides',N'Garden Salad',85),(N'Salads & Sides',N'Golden Fries',55),
(N'Drinks',N'Fresh Lemonade',45),(N'Drinks',N'Iced Coffee',65),(N'Drinks',N'Orange Juice',55),
(N'Desserts',N'Chocolate Brownie',75),(N'Desserts',N'Classic Cheesecake',95),
(N'Burgers',N'Double Cheeseburger',225),(N'Burgers',N'BBQ Beef Burger',215),
(N'Pizza',N'BBQ Chicken Pizza',195),(N'Pizza',N'Four Cheese Pizza',190),
(N'Grills',N'Chicken Shawarma Plate',195),(N'Grills',N'Kofta Plate',210),
(N'Salads & Sides',N'Caesar Salad',125),(N'Salads & Sides',N'Crispy Onion Rings',65),
(N'Drinks',N'Espresso',40),(N'Drinks',N'Cappuccino',65),(N'Drinks',N'Hot Chocolate',70),(N'Drinks',N'Chocolate Milkshake',85),
(N'Desserts',N'Tiramisu',95),(N'Desserts',N'Apple Pie',85);
INSERT MenuCategories(Name,IsVisible) SELECT DISTINCT m.Category,1 FROM @Menu m WHERE NOT EXISTS(SELECT 1 FROM MenuCategories c WHERE c.Name=m.Category);
UPDATE c SET IsVisible=CASE WHEN EXISTS(SELECT 1 FROM @Menu m WHERE m.Category=c.Name) THEN 1 ELSE 0 END FROM MenuCategories c;
INSERT Products(CategoryId,Name,Price,IsAvailable) SELECT c.Id,m.Product,m.Price,1 FROM @Menu m CROSS APPLY(SELECT TOP(1) Id FROM MenuCategories WHERE Name=m.Category ORDER BY Id)c WHERE NOT EXISTS(SELECT 1 FROM Products p WHERE p.CategoryId=c.Id AND p.Name=m.Product);
COMMIT;
