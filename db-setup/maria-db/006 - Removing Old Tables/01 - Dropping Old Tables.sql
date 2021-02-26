ALTER TABLE `ToDoList`
	DROP FOREIGN KEY `FK_ToDoList_ToDoCategory`,
	DROP FOREIGN KEY `FK_ToDoList_ToDoSubCategory`,
	DROP FOREIGN KEY `FK_ToDoList_Users`;
	
ALTER TABLE `ToDoCategory`
	DROP FOREIGN KEY `FK_ToDoCategory_Users`;

ALTER TABLE `ToDoSubCategory`
	DROP FOREIGN KEY `FK_ToDoSubCategory_ToDoCategory`,
	DROP FOREIGN KEY `FK_ToDoSubCategory_Users`;
	
DROP TABLE `ToDoSubCategory`;
DROP TABLE `ToDoCategory`;
DROP TABLE `ToDoList`;

DROP TABLE `HangfireAggregatedCounter`;
DROP TABLE `HangfireCounter`;
DROP TABLE `HangfireDistributedLock`;
DROP TABLE `HangfireHash`;
DROP TABLE `HangfireJobParameter`;
DROP TABLE `HangfireJobQueue`;
DROP TABLE `HangfireJobState`;
DROP TABLE `HangfireList`;
DROP TABLE `HangfireServer`;
DROP TABLE `HangfireSet`;
DROP TABLE `HangfireState`;
DROP TABLE `HangfireJob`;

TRUNCATE TABLE `Options`;

DROP TABLE `Timers`;

