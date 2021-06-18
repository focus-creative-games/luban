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
	--input_l10n_text_files l10n/TextTable_CN.xlsx ^
	--l10n_text_field_name text_tw ^
	--output_l10n_not_converted_text_file NotLocalized_CN.txt

pause