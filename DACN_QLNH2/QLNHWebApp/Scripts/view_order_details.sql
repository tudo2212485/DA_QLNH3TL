-- Query để xem chi tiết đơn hàng với tên món ăn
SELECT 
    o.Id as OrderId,
    o.CustomerName,
    o.Phone,
    o.Date,
    o.Time,
    o.Guests,
    o.TotalPrice,
    o.Status,
    oi.Quantity,
    m.Name as MenuItemName,
    m.Price as UnitPrice,
    (oi.Quantity * oi.Price) as ItemTotal
FROM Orders o
LEFT JOIN OrderItems oi ON o.Id = oi.OrderId
LEFT JOIN MenuItems m ON oi.MenuItemId = m.Id
ORDER BY o.Id DESC, oi.Id;

-- Query để xem tổng quan đơn hàng theo nhóm
SELECT 
    o.Id as OrderId,
    o.CustomerName,
    o.Date,
    o.Time,
    GROUP_CONCAT(m.Name || ' (x' || oi.Quantity || ')') as OrderedItems,
    o.TotalPrice,
    o.Status
FROM Orders o
LEFT JOIN OrderItems oi ON o.Id = oi.OrderId  
LEFT JOIN MenuItems m ON oi.MenuItemId = m.Id
GROUP BY o.Id, o.CustomerName, o.Date, o.Time, o.TotalPrice, o.Status
ORDER BY o.Id DESC;