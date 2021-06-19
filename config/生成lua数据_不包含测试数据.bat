..\src\Luban.Client\bin\Debug\net5.0\Luban.Client.exe ^
	-h %LUBAN_SERVER_IP% ^
	-j cfg ^
	-- ^
	-d Defines/__root__.xml ^
	--input_data_dir Datas ^
	--output_data_dir output_lua_without_test ^
	-s client ^
	--gen_types data_lua 

pause