RENAME TABLE `RawTimers` TO `TrackedTime`;

ALTER TABLE `TrackedTime`
	CHANGE COLUMN `RawTimerId` `EntryId` BIGINT(20) NOT NULL AUTO_INCREMENT FIRST,
	DROP PRIMARY KEY,
	ADD PRIMARY KEY (`EntryId`) USING BTREE;

ALTER TABLE `TrackedTime`
	DROP COLUMN `ParentTimerId`,
	DROP COLUMN `RootTimerId`;

ALTER TABLE `TrackedTime`
	ADD INDEX `ClientId` (`ClientId`),
	ADD INDEX `ProductId` (`ProductId`),
	ADD INDEX `ProjectId` (`ProjectId`),
	ADD INDEX `Deleted` (`Deleted`);
	
ALTER TABLE `TrackedTime`
	CHANGE COLUMN `TimerNotes` `Notes` VARCHAR(512) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci' AFTER `EntryEndTimeUtc`;
	
ALTER TABLE `TrackedTime`
	CHANGE COLUMN `EntryRunningTimeSec` `RunningTimeSec` INT(11) NOT NULL DEFAULT '0' AFTER `EntryState`,
	CHANGE COLUMN `EntryStartTimeUtc` `StartTimeUtc` TIMESTAMP NOT NULL DEFAULT current_timestamp() AFTER `RunningTimeSec`,
	CHANGE COLUMN `EntryEndTimeUtc` `EndTimeUtc` DATETIME NULL DEFAULT NULL AFTER `StartTimeUtc`;

ALTER TABLE `TrackedTime`
	DROP COLUMN `Completed`,
	DROP COLUMN `Processed`,
	DROP COLUMN `EntryState`;
	
ALTER TABLE `TrackedTime`
	CHANGE COLUMN `RunningTimeSec` ` ElapsedSeconds` INT(11) NOT NULL DEFAULT '0' AFTER `Running`;
	
ALTER TABLE `TrackedTime`
	ADD COLUMN `EndReason` TINYINT NOT NULL DEFAULT 0 AFTER `Running`;
	
ALTER TABLE `TrackedTime`
	CHANGE COLUMN `EndTimeUtc` `EndTimeUtc` TIMESTAMP NULL DEFAULT NULL AFTER `StartTimeUtc`;
	
ALTER TABLE `TrackedTime`
	CHANGE COLUMN ` ElapsedSeconds` `TotalSeconds` INT(11) NOT NULL DEFAULT '0' AFTER `EndReason`;
	
ALTER TABLE `TrackedTime`
	CHANGE COLUMN `EndReason` `EntryState` TINYINT(4) NOT NULL DEFAULT '0' AFTER `Running`;
	
ALTER TABLE `TrackedTime`
	ADD COLUMN `EntryType` TINYINT NOT NULL DEFAULT 1 AFTER `Running`;
	
RENAME TABLE `TrackedTime` TO `Timers`;

