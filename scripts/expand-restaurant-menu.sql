-- User-approved portfolio menu additions. INSERT ONLY; existing records/prices are unchanged.
-- Select the existing migrated CafeManagement database explicitly before executing.
SET XACT_ABORT ON;
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;
DECLARE @Menu TABLE(Category nvarchar(120),Product nvarchar(120),Price decimal(18,2));
INSERT @Menu VALUES
(N'Burgers',N'Double Cheeseburger',225),(N'Burgers',N'BBQ Beef Burger',215),
(N'Pizza',N'BBQ Chicken Pizza',195),(N'Pizza',N'Four Cheese Pizza',190),
(N'Grills',N'Chicken Shawarma Plate',195),(N'Grills',N'Kofta Plate',210),
(N'Salads & Sides',N'Caesar Salad',125),(N'Salads & Sides',N'Crispy Onion Rings',65),
(N'Drinks',N'Espresso',40),(N'Drinks',N'Cappuccino',65),(N'Drinks',N'Hot Chocolate',70),(N'Drinks',N'Chocolate Milkshake',85),
(N'Desserts',N'Tiramisu',95),(N'Desserts',N'Apple Pie',85);
IF EXISTS(SELECT 1 FROM @Menu m WHERE NOT EXISTS(SELECT 1 FROM MenuCategories c WHERE c.Name=m.Category AND c.IsVisible=1))
 THROW 51000,'Expected visible restaurant categories are missing; no changes applied.',1;
INSERT Products(CategoryId,Name,Price,IsAvailable)
SELECT c.Id,m.Product,m.Price,1 FROM @Menu m
CROSS APPLY(SELECT TOP(1) Id FROM MenuCategories WHERE Name=m.Category AND IsVisible=1 ORDER BY Id)c
WHERE NOT EXISTS(SELECT 1 FROM Products p WHERE p.CategoryId=c.Id AND p.Name=m.Product);
COMMIT;
