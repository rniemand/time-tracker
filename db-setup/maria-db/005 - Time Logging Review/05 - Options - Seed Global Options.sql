INSERT INTO `Options`
	(`UserId`, `OptionType`, `OptionCategory`, `OptionKey`, `OptionValue`)
VALUES
	(0, 'bool', 'TimeSheet.Cron.GenerateDates', 'Enabled', 'true'),
	(0, 'int',  'TimeSheet.Cron.GenerateDates', 'DaysAhead', '8');