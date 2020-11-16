..\src\Luban.Client\bin\Debug\net5.0\Luban.Client.exe ^
	-h 127.0.0.1 ^
	-j cfg ^
	-- ^
	-d Defines/root.xml ^
	--input_data_dir Datas ^
	--output_code_dir output_code ^
	--output_data_dir output_data ^
	-s server ^
	--gen_types code_cs_bin,data_bin ^
	--export_test_data

pause