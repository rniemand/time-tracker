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
