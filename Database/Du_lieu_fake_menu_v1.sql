/* insert data to table product_category */
INSERT INTO Product_category (Product_category_title, isDelete) VALUES
(N'Đồ ăn', 0),
(N'Đồ ăn nhanh', 0),
(N'Đồ uống', 0);

/* insert data to table Unit */
INSERT INTO Unit (Unit_title, isDelete) VALUES
(N'Phần', 0),
(N'Chai/Lon', 0),
(N'Cốc', 0);

/* Insert data to table menu */
INSERT INTO Menu (Product_name, Product_category_id, Cost_price, Sell_price, Sale_price, Product_vat, Description, Unit_id, IsAvailable, Status, IsDelete)
VALUES
(N'Cơm rang dưa bò', 1, 30000, 45000, 45000, 10, N'Món cơm rang với dưa bò', 1, 1, 1, 0),
(N'Cơm rang gà', 1, 28000, 42000, 42000, 10, N'Cơm rang với thịt gà', 1, 1, 1, 0),
(N'Phở tái chín', 1, 35000, 50000, 50000, 10, N'Phở bò tái và chín', 1, 1, 1, 0),
(N'Bánh mì thịt nướng', 2, 15000, 25000, 25000, 10, N'Bánh mì nhân thịt nướng', 1, 1, 1, 0),
(N'Coca Cola 330ml', 3, 8000, 15000, 15000, 5, N'Nước giải khát Coca Cola', 2, 1, 1, 0),
(N'Pepsi 330ml', 3, 8000, 15000, 15000, 5, N'Nước giải khát Pepsi', 2, 1, 1, 0),
(N'Bún bò Huế', 1, 40000, 60000, 60000, 10, N'Bún bò Huế truyền thống', 1, 1, 1, 0),
(N'Mì xào hải sản', 1, 45000, 65000, 65000, 10, N'Mì xào với hải sản', 1, 1, 1, 0),
(N'Cháo gà', 1, 20000, 35000, 35000, 10, N'Cháo gà nóng hổi', 1, 1, 1, 0),
(N'Nước cam ép', 3, 10000, 20000, 20000, 5, N'Nước cam ép nguyên chất', 2, 1, 1, 0),
(N'Cà phê sữa đá', 3, 15000, 30000, 30000, 5, N'Cà phê pha cùng sữa đặc', 3, 1, 1, 0),
(N'Cà phê đen đá', 3, 12000, 25000, 25000, 5, N'Cà phê đen truyền thống', 3, 1, 1, 0),
(N'Trà đào', 3, 12000, 25000, 25000, 5, N'Trà đào mát lạnh', 3, 1, 1, 0),
(N'Mì xào bò', 1, 40000, 60000, 60000, 10, N'Mì xào thịt bò thơm ngon', 1, 1, 1, 0),
(N'Gỏi cuốn', 2, 15000, 25000, 25000, 10, N'Gỏi cuốn tôm thịt', 1, 1, 1, 0),
(N'Nem rán', 2, 20000, 35000, 35000, 10, N'Nem rán giòn thơm', 1, 1, 1, 0),
(N'Bún chả', 1, 30000, 50000, 50000, 10, N'Bún chả Hà Nội', 1, 1, 1, 0),
(N'Bún đậu mắm tôm', 1, 25000, 45000, 45000, 10, N'Bún đậu đặc sản', 1, 1, 1, 0),
(N'Cơm tấm sườn', 1, 40000, 60000, 60000, 10, N'Cơm tấm sườn nướng', 1, 1, 1, 0),
(N'Cơm gà xối mỡ', 1, 35000, 55000, 55000, 10, N'Cơm gà xối mỡ thơm phức', 1, 1, 1, 0),
(N'Gà rán', 2, 30000, 50000, 50000, 10, N'Gà rán giòn cay', 1, 1, 1, 0),
(N'Pizza hải sản', 2, 70000, 120000, 120000, 10, N'Pizza hải sản sốt đặc biệt', 1, 1, 1, 0),
(N'Pizza bò', 2, 65000, 110000, 110000, 10, N'Pizza bò phô mai', 1, 1, 1, 0);
