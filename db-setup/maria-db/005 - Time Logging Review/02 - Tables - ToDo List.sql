CREATE TABLE `ToDoCategory` (
	`CategoryId` INT(11) NOT NULL AUTO_INCREMENT,
	`UserId` INT(11) NOT NULL DEFAULT '0',
	`Deleted` BIT(1) NOT NULL DEFAULT b'0',
	`DateCreatedUtc` TIMESTAMP NOT NULL DEFAULT current_timestamp(),
	`DateModifiedUtc` TIMESTAMP NULL DEFAULT NULL,
	`Category` VARCHAR(128) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	PRIMARY KEY (`CategoryId`) USING BTREE,
	INDEX `UserId` (`UserId`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `ToDoSubCategory` (
	`SubCategoryId` INT(11) NOT NULL AUTO_INCREMENT,
	`CategoryId` INT(11) NOT NULL DEFAULT '0',
	`UserId` INT(11) NOT NULL DEFAULT '0',
	`Deleted` BIT(1) NOT NULL DEFAULT b'0',
	`DateCreatedUtc` DATETIME NOT NULL DEFAULT current_timestamp(),
	`DateModifiedUtc` DATETIME NULL DEFAULT NULL,
	`SubCategory` VARCHAR(128) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	PRIMARY KEY (`SubCategoryId`) USING BTREE,
	INDEX `CategoryId` (`CategoryId`) USING BTREE,
	INDEX `UserId` (`UserId`) USING BTREE,
	INDEX `Deleted` (`Deleted`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `ToDoList` (
	`ToDoId` BIGINT(20) NOT NULL AUTO_INCREMENT,
	`CategoryId` INT(11) NOT NULL DEFAULT '0',
	`SubCategoryId` INT(11) NOT NULL DEFAULT '0',
	`UserId` INT(11) NOT NULL DEFAULT '0',
	`Deleted` BIT(1) NOT NULL DEFAULT b'0',
	`DateAddedUtc` DATETIME NOT NULL DEFAULT current_timestamp(),
	`DateModifiedUtc` DATETIME NULL DEFAULT NULL,
	`ToDo` VARCHAR(256) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	PRIMARY KEY (`ToDoId`) USING BTREE,
	INDEX `CategoryId` (`CategoryId`) USING BTREE,
	INDEX `SubCategoryId` (`SubCategoryId`) USING BTREE,
	INDEX `UserId` (`UserId`) USING BTREE,
	INDEX `Deleted` (`Deleted`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
