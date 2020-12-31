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
	(1, 1, 'Client 1 | Product A'),
	(1, 1, 'Client 1 | Product B'),
	(1, 1, 'Client 1 | Product C'),
	
	-- Client 2 (2) | niemandr (1)
	(2, 1, 'Client 2 | Product A'),
	(2, 1, 'Client 2 | Product B'),
	(2, 1, 'Client 2 | Product C');
	
	
-- =====================================================================================
-- Projects
-- =====================================================================================
TRUNCATE TABLE `Projects`;

INSERT INTO `Projects`
	(`ClientId`, `ProductId`, `UserId`, `ProjectName`)
VALUES
	-- Client 1 | Product A
	(1, 1, 1, 'Client 1 | Product A | Project 1'),
	(1, 1, 1, 'Client 1 | Product A | Project 2'),
	(1, 1, 1, 'Client 1 | Product A | Project 3'),
	-- Client 1 | Product B
	(1, 2, 1, 'Client 1 | Product B | Project 1'),
	(1, 2, 1, 'Client 1 | Product B | Project 2'),
	(1, 2, 1, 'Client 1 | Product B | Project 3'),
	-- Client 1 | Product C
	(1, 3, 1, 'Client 1 | Product C | Project 1'),
	(1, 3, 1, 'Client 1 | Product C | Project 2'),
	(1, 3, 1, 'Client 1 | Product C | Project 3'),
	
	-- Client 2 | Product A
	(2, 4, 1, 'Client 2 | Product A | Project 1'),
	(2, 4, 1, 'Client 2 | Product A | Project 2'),
	(2, 4, 1, 'Client 2 | Product A | Project 3'),
	-- Client 2 | Product B
	(2, 5, 1, 'Client 2 | Product B | Project 1'),
	(2, 5, 1, 'Client 2 | Product B | Project 2'),
	(2, 5, 1, 'Client 2 | Product B | Project 3'),
	-- Client 2 | Product C
	(2, 6, 1, 'Client 2 | Product C | Project 1'),
	(2, 6, 1, 'Client 2 | Product C | Project 2'),
	(2, 6, 1, 'Client 2 | Product C | Project 3');
		
	