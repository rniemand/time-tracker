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
	(1, 'Client 2'),
	(1, 'Client 3'),
	(1, 'Client 4');