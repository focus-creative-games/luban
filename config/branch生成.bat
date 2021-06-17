..\src\Luban.Client\bin\Debug\net5.0\Luban.Client.exe ^
	-h %LUBAN_SERVER_IP% ^
	-j cfg ^
	-- ^
	-d Defines/__root__.xml ^
	--input_data_dir Datas ^
	--output_data_dir output_lua ^
	-s client ^
	--gen_types data_lua ^
	--export_test_data ^
	--branch cn ^
	--branch_input_data_dir Datas/l10n/cn

pause