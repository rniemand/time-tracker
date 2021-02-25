-- --------------------------------------------------------
-- Host:                         192.168.0.60
-- Server version:               10.4.17-MariaDB-1:10.4.17+maria~bionic-log - mariadb.org binary distribution
-- Server OS:                    debian-linux-gnu
-- HeidiSQL Version:             11.1.0.6116
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- Dumping structure for table TimeTracker.Clients
CREATE TABLE IF NOT EXISTS `Clients` (
  `ClientId` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `DateAddedUtc` datetime NOT NULL DEFAULT utc_timestamp(4),
  `DateUpdatedUtc` datetime DEFAULT NULL,
  `DateDeletedUtc` datetime DEFAULT NULL,
  `ClientName` varchar(128) NOT NULL,
  `ClientEmail` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`ClientId`) USING BTREE,
  KEY `Deleted` (`Deleted`) USING BTREE,
  KEY `UserId` (`UserId`) USING BTREE,
  CONSTRAINT `FK_Clients_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `DailyTasks` (
  `TaskId` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `ClientId` int(11) NOT NULL,
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `DateAddedUtc` datetime NOT NULL DEFAULT utc_timestamp(4),
  `DateUpdatedUtc` datetime DEFAULT NULL,
  `DateDeletedUtc` datetime DEFAULT NULL,
  `TaskName` varchar(128) NOT NULL,
  PRIMARY KEY (`TaskId`) USING BTREE,
  KEY `UserId` (`UserId`) USING BTREE,
  KEY `ClientId` (`ClientId`) USING BTREE,
  KEY `Deleted` (`Deleted`) USING BTREE,
  CONSTRAINT `FK_DailyTasks_Clients` FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`ClientId`),
  CONSTRAINT `FK_DailyTasks_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Options` (
  `OptionId` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL DEFAULT 0,
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `IsCollection` bit(1) NOT NULL DEFAULT b'0',
  `OptionType` varchar(16) NOT NULL,
  `OptionCategory` varchar(128) NOT NULL,
  `OptionKey` varchar(128) NOT NULL,
  `OptionValue` varchar(512) NOT NULL DEFAULT '',
  PRIMARY KEY (`OptionId`) USING BTREE,
  KEY `Deleted` (`Deleted`) USING BTREE,
  KEY `IsCollection` (`IsCollection`) USING BTREE,
  KEY `UserId` (`UserId`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Products` (
  `ProductId` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `ClientId` int(11) NOT NULL,
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `DateAddedUtc` datetime NOT NULL DEFAULT utc_timestamp(4),
  `DateUpdatedUtc` datetime DEFAULT NULL,
  `DateDeletedUtc` datetime DEFAULT NULL,
  `ProductName` varchar(256) NOT NULL,
  PRIMARY KEY (`ProductId`) USING BTREE,
  KEY `ClientId` (`ClientId`) USING BTREE,
  KEY `UserId` (`UserId`) USING BTREE,
  KEY `Deleted` (`Deleted`) USING BTREE,
  CONSTRAINT `FK_Products_Clients` FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`ClientId`),
  CONSTRAINT `FK_Products_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Projects` (
  `ProjectId` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `ClientId` int(11) NOT NULL,
  `ProductId` int(11) NOT NULL,
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `DateAddedUtc` datetime NOT NULL DEFAULT utc_timestamp(4),
  `DateUpdatedUtc` datetime DEFAULT NULL,
  `DateDeletedUtc` datetime DEFAULT NULL,
  `ProjectName` varchar(256) NOT NULL,
  PRIMARY KEY (`ProjectId`) USING BTREE,
  KEY `UserId` (`UserId`) USING BTREE,
  KEY `Deleted` (`Deleted`) USING BTREE,
  KEY `ProductId` (`ProductId`) USING BTREE,
  KEY `ClientId` (`ClientId`) USING BTREE,
  CONSTRAINT `FK_Projects_Clients` FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`ClientId`),
  CONSTRAINT `FK_Projects_Products` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`ProductId`),
  CONSTRAINT `FK_Projects_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Timers` (
  `EntryId` bigint(20) NOT NULL AUTO_INCREMENT,
  `ClientId` int(11) NOT NULL,
  `ProductId` int(11) NOT NULL DEFAULT 0,
  `ProjectId` int(11) NOT NULL DEFAULT 0,
  `TaskId` int(11) NOT NULL DEFAULT 0,
  `UserId` int(11) NOT NULL,
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `Running` bit(1) NOT NULL DEFAULT b'1',
  `EntryType` tinyint(4) NOT NULL DEFAULT 1,
  `EntryState` tinyint(4) NOT NULL DEFAULT 0,
  `TotalSeconds` int(11) NOT NULL DEFAULT 0,
  `StartTimeUtc` timestamp NOT NULL DEFAULT utc_timestamp(4),
  `EndTimeUtc` timestamp NULL DEFAULT NULL,
  `Notes` varchar(512) NOT NULL DEFAULT '',
  PRIMARY KEY (`EntryId`) USING BTREE,
  KEY `UserId` (`UserId`) USING BTREE,
  KEY `ClientId` (`ClientId`),
  KEY `ProductId` (`ProductId`),
  KEY `ProjectId` (`ProjectId`),
  KEY `Deleted` (`Deleted`),
  KEY `Running` (`Running`),
  KEY `EntryType` (`EntryType`),
  KEY `EntryState` (`EntryState`),
  KEY `TaskId` (`TaskId`),
  CONSTRAINT `FK_Timers_Clients` FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`ClientId`),
  CONSTRAINT `FK_Timers_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `TimeSheetEntries` (
  `EntryId` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `ClientId` int(11) NOT NULL,
  `ProductId` int(11) NOT NULL,
  `ProjectId` int(11) NOT NULL,
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `DateAddedUtc` datetime NOT NULL DEFAULT utc_timestamp(4),
  `DateUpdatedUtc` datetime DEFAULT NULL,
  `DateDeletedUtc` datetime DEFAULT NULL,
  `EntryDate` date NOT NULL,
  `EntryVersion` tinyint(4) NOT NULL DEFAULT 1,
  `EntryTimeMin` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`EntryId`) USING BTREE,
  KEY `Deleted` (`Deleted`) USING BTREE,
  KEY `UserId` (`UserId`) USING BTREE,
  KEY `ClientId` (`ClientId`) USING BTREE,
  KEY `ProductId` (`ProductId`) USING BTREE,
  KEY `ProjectId` (`ProjectId`) USING BTREE,
  CONSTRAINT `FK_TimeSheet_Entries_Clients` FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`ClientId`),
  CONSTRAINT `FK_TimeSheet_Entries_Products` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`ProductId`),
  CONSTRAINT `FK_TimeSheet_Entries_Projects` FOREIGN KEY (`ProjectId`) REFERENCES `Projects` (`ProjectId`),
  CONSTRAINT `FK_TimeSheet_Entries_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `ToDoCategory` (
  `CategoryId` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `DateAddedUtc` timestamp NOT NULL DEFAULT utc_timestamp(4),
  `DateUpdatedUtc` timestamp NULL DEFAULT NULL,
  `DateDeletedUtc` timestamp NULL DEFAULT NULL,
  `Category` varchar(128) NOT NULL,
  PRIMARY KEY (`CategoryId`) USING BTREE,
  KEY `UserId` (`UserId`) USING BTREE,
  KEY `Deleted` (`Deleted`),
  CONSTRAINT `FK_ToDoCategory_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `ToDoList` (
  `ToDoId` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `CategoryId` int(11) NOT NULL,
  `SubCategoryId` int(11) NOT NULL,
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `DateAddedUtc` datetime NOT NULL DEFAULT utc_timestamp(4),
  `DateUpdatedUtc` datetime DEFAULT NULL,
  `DateDeletedUtc` datetime DEFAULT NULL,
  `ToDo` varchar(256) NOT NULL,
  PRIMARY KEY (`ToDoId`) USING BTREE,
  KEY `CategoryId` (`CategoryId`) USING BTREE,
  KEY `SubCategoryId` (`SubCategoryId`) USING BTREE,
  KEY `UserId` (`UserId`) USING BTREE,
  KEY `Deleted` (`Deleted`) USING BTREE,
  CONSTRAINT `FK_ToDoList_ToDoCategory` FOREIGN KEY (`CategoryId`) REFERENCES `ToDoCategory` (`CategoryId`),
  CONSTRAINT `FK_ToDoList_ToDoSubCategory` FOREIGN KEY (`SubCategoryId`) REFERENCES `ToDoSubCategory` (`SubCategoryId`),
  CONSTRAINT `FK_ToDoList_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `ToDoSubCategory` (
  `SubCategoryId` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `CategoryId` int(11) NOT NULL,
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `DateAddedUtc` datetime NOT NULL DEFAULT utc_timestamp(4),
  `DateUpdatedUtc` datetime DEFAULT NULL,
  `DateDeletedUtc` datetime DEFAULT NULL,
  `SubCategory` varchar(128) NOT NULL,
  PRIMARY KEY (`SubCategoryId`) USING BTREE,
  KEY `CategoryId` (`CategoryId`) USING BTREE,
  KEY `UserId` (`UserId`) USING BTREE,
  KEY `Deleted` (`Deleted`) USING BTREE,
  CONSTRAINT `FK_ToDoSubCategory_ToDoCategory` FOREIGN KEY (`CategoryId`) REFERENCES `ToDoCategory` (`CategoryId`),
  CONSTRAINT `FK_ToDoSubCategory_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Users` (
  `UserId` int(11) NOT NULL AUTO_INCREMENT,
  `Deleted` bit(1) NOT NULL DEFAULT b'0',
  `DateAddedUtc` datetime NOT NULL DEFAULT utc_timestamp(4),
  `DateUpdatedUtc` datetime DEFAULT NULL,
  `LastLoginDateUtc` datetime DEFAULT NULL,
  `DateDeletedUtc` datetime DEFAULT NULL,
  `FirstName` varchar(64) NOT NULL,
  `LastName` varchar(64) DEFAULT NULL,
  `Username` varchar(64) NOT NULL,
  `Password` varchar(128) NOT NULL,
  `UserEmail` varchar(128) NOT NULL,
  PRIMARY KEY (`UserId`) USING BTREE,
  KEY `Deleted` (`Deleted`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4;

-- Dumping data for table TimeTracker.Users: ~1 rows (approximately)
/*!40000 ALTER TABLE `Users` DISABLE KEYS */;
INSERT INTO `Users` (`UserId`, `Deleted`, `DateAddedUtc`, `DateUpdatedUtc`, `LastLoginDateUtc`, `DateDeletedUtc`, `FirstName`, `LastName`, `Username`, `Password`, `UserEmail`) VALUES
	(1, b'0', '2021-01-06 23:29:27', NULL, '2021-02-25 18:33:52', NULL, 'Richard', 'Niemand', 'niemandr', '54NzSfLO3E1EdnXLDmvK3w==', 'niemand.richard@gmail.com');
/*!40000 ALTER TABLE `Users` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
