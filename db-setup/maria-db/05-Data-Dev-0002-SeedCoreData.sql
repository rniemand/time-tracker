-- =====================================================================================
-- Users
-- =====================================================================================
TRUNCATE TABLE `Users`;

INSERT INTO `Users`
	(`Username`, `Password`, `UserEmail`, `FirstName`, `LastName`)
VALUES
	('niemandr', '54NzSfLO3E1EdnXLDmvK3w==', 'niemand.richard@gmail.com', 'Richard', 'Niemand');
	
-- =====================================================================================
-- Clients
-- =====================================================================================
TRUNCATE TABLE `Clients`;

INSERT INTO `Clients`
	(`UserId`, `ClientName`)
VALUES
	(1, 'Client 1'),
	(1, 'Client 2');
	

-- =====================================================================================
-- Products
-- =====================================================================================	
TRUNCATE TABLE `Products`;

INSERT INTO `Products`
	(`ClientId`, `UserId`, `ProductName`)
VALUES
	-- Client 1 (1) | niemandr (1)
	(1, 1, 'Product A (Client 1)'),
	(1, 1, 'Product B (Client 1)'),
	(1, 1, 'Product 3 (Client 1)'),
	-- Client 2 (2) | niemandr (1)
	(2, 1, 'Product A (Client 2)'),
	(2, 1, 'Product B (Client 2)'),
	(3, 1, 'Product 3 (Client 2)');