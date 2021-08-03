
## 部属

### 部属 luban-server

- 基于 docker (强烈推荐以这种方式在服务器上部属luban-server，因为可以充分利用缓存机制大幅缩短生成时间)

	docker run -d --rm --name luban-server -p 8899:8899 focuscreativegames/luban-server:latest

- 基于 .net 5 runtime （可跨平台，不需要重新编译）
	- 自行安装 .net 5 runtime.
	- 从[示例项目](https://github.com/focus-creative-games/luban_examples/tree/main/Tools/Luban.Server)拷贝 Luban.Server（**可跨平台，即使在linux、mac平台也不需要重新编译**）
	- 在Luban.Server目录下运行 dotnet Luban.Server.dll (Win平台可以直接运行 Luban.Server.exe)

### 安装 luban-client
- 基于 docker (只推荐与jenkins之类devops工具配合使用，因为docker容器启动会增加一定的延迟)

	docker run --rm -v $PWD/.cache.meta:/bin/.cache.meta  focuscreativegames/luban-client <参数>
	
	提醒！ .cache.meta这个文件用于保存本地生成或者提交到远程的文件md5缓存，**强烈推荐** 添加-v $PWD/.cache.meta:/bin/.cache.meta 映射，不然每次重新计算所有涉及文件的md5,这可能在项目后期会造成多达几秒的延迟。
- 基于 .net 5 runtime （推荐win平台使用，可跨平台，不需要重新编译）
	- 自行安装 .net 5 runtime.
	- 从[示例项目](https://github.com/focus-creative-games/luban_examples/tree/main/Tools/Luban.Client)拷贝 Luban.Client（**可跨平台，即使在linux、mac平台也不需要重新编译**）


-----
## 使用

### luban-server 使用介绍

命令行使用
    
    dotnet Luban.Server.dll [-p <port>] [-l <log level>]

    参数介绍：
    -p <port>           可选参数。 监听端口 <port>，默认为8899。
    -l <log level>      可选参数。 日志级别。默认为INFO。 有效值有: TRACE,DEBUG,INFO,WARN,ERROR,FATAL,OFF

## luban-client 使用介绍

 命令行使用

    dotnet Luban.Client.dll [-h <host>] [-p <port>] [-l <log level>] [-c <cache meta file>] [-w <watch dirs>] [-h ] -j cfg -- <job options>
    
    参数介绍：
    -h,--host <host>            可选参数。 luban-server的地址。默认为 127.0.0.1
    -p,--port <port>            可选参数。 luban-server的端口。默认为 8899
    -j,--job <job>              必选参数。 生成类型。目前支持的生成类型有: cfg,proto,db。 生成配置请取cfg。
    -l,--loglevel <log level>   可选参数。 日志级别。默认为INFO。有效值有: TRACE,DEBUG,INFO,WARN,ERROR,FATAL,OFF
    -c,--cachemetafile <meta file>  可选参数。 meta缓存文件名。 默认为 .cache.meta
    -w,--watch <watch dirs>     可选参数。 监测目录或者目录列表，以逗号','分隔。当开启此选项后，生成结束后不会退出程序，而是进入自动生成模式。监听到目标目录发生变化后，自动重新运行生成。省去改动后手动运行生成脚本的麻烦。
    -h,--help                   可选参数。显示帮助信息
    --  <job options>           必选参数。 从此参数起，便是 不同job的特有选项

----
    
    cfg的<job options>介绍：

    -d,--define_file <root file>            必选参数。 根定义文件名。
    --input_data_dir  <input data dir>      必选参数。 配置数据文件根目录。
    -c,--output_code_dir <output code dir>  可选参数。 生成代码文件的目录。
    -s,--service                            必选参数。生成分组目标。一般来说，会定义client,server,editor等好几个目标，不同目标的生成内容不同。
    --gen_types <type1,type2,,,>            必选参数。生成任务类型。既可以是生成代码也可以是生成数据或者其他。目前支持的有 code_cs_bin,code_cs_json,code_cs_unity_json,code_lua_bin,code_java_bin,code_go_bin,code_go_json,code_cpp_bin,code_python27_json,code_python3_json，code_typescript_bin,code_typescript_json,data_bin,data_lua,data_json,data_json_monolithic
    --output_data_dir <output data dir>     可选参数。 导出的数据文件的目录。
    --validate_root_dir <path validate root dir>. 可选参数。 配置path检查的根目录。
    --export_test_data                      可选参数。 是否导出测试数据。默认为false

    -t,--l10n_timezone <timezone>           可选参数。 指定所在时区。影响datetime类型转换为utc时间。 默认为中国北京时间。
    --input_l10n_text_files <file1,file2..> 可选参数。 本地化的文本映射表。可以有多个。
    --l10n_text_field_name <field name>     可选参数。 文本映射表中，目标映射列的列名，默认为text
    --output_l10n_not_converted_text_file <file> 可选参数。 未被本地化映射的text key和value的输出文件。不提供该参数则不生成
    --branch <branch name>                  可选参数。当前需要生成的分支名称。
    --branch_input_data_dir <branch data root dir> 可选参数。分支数据的根目录。


## 示例

假设 

    luban.server 运行在本机，端口为8899
    luban.client的位置在 d:\tools\Luban.Client
    配置定义在 d:\raw_config\defines
    配置定义的根定义文件为 d:\raw_config\defines\__root__.xml
    配置数据在 d:\raw_configs\datas

    client项目为unity项目，位置在 d:\client
    你期望client生成的代码在 d:\client\Assets\Gen 目录
    你期望client生成的数据在 d:\client\Assets\StreamingAssets\GameData 目录

    你们服务器项目为 dotnet c#项目，位置在d:\server 
    你期望server生成的代码在 d:\server\src\Gen
    你期望server生成的数据在 d:\server\GameData

案例1：
    
    你要为客户端生成代码和数据。
    你期望使用bin格式的导出数据类型
    你为客户端选择的service分组为 client
    当前在开发期，你期望导出数据中包含测试数据
    
    则win下命令为：

    dotnet d:\tools\Luban.Client\Luban.Client.dll ^
        -h 127.0.0.1 ^
        -p 8899 ^
        -j cfg ^
        -- ^
        --define_file d:\raw_config\defines\__root__.xml ^
        --input_data_dir d:\raw_configs\datas ^
        --output_code_dir d:\client\Assets\Gen ^
        --output_data_dir d:\client\Assets\StreamingAssets\GameData ^
        --gen_types code_cs_bin,data_bin ^
        --service client ^
        --export_test_data 

    linux bash命令同理。

案例2：

    你要为客户端生成代码和数据。
    你期望使用json格式导出数据类型。
    你不期望导出数据中包含测试数据
        
    则win下命令为:

    dotnet d:\tools\Luban.Client\Luban.Client.dll ^
        -h 127.0.0.1 ^
        -p 8899 ^
        -j cfg ^
        -- ^
        --define_file d:\raw_config\defines\__root__.xml ^
        --input_data_dir d:\raw_configs\datas ^
        --output_code_dir d:\client\Assets\Gen ^
        --output_data_dir d:\client\Assets\StreamingAssets\GameData ^
        --gen_types code_cs_unity_json,data_json ^
        --service client

案例3：
    
    你要为服务器生成代码和数据。

    你期望使用json导出数据格式。
    你期望包含测试数据。
    你为服务器选择的service为server

    则 win下命令为：

    dotnet d:\tools\Luban.Client\Luban.Client.dll ^
        -h 127.0.0.1 ^
        -p 8899 ^
        -j cfg ^
        -- ^
        --define_file d:\raw_config\defines\__root__.xml ^
        --input_data_dir d:\raw_configs\datas ^
        --output_code_dir d:\server\src\Gen ^
        --output_data_dir d:\server\GameData ^
        --gen_types code_cs_json,data_json ^
        --service server ^
        --export_test_data
案例4：

    luban-server 被你部属在 192.168.1.10这台机器上，端口为1111。其他的如案例3。

    则 win下的生成命令为：

    dotnet d:\tools\Luban.Client\Luban.Client.dll ^
        -h 192.168.1.10 ^
        -p 1111 ^
        -j cfg ^
        -- ^
        --define_file d:\raw_config\defines\__root__.xml ^
        --input_data_dir d:\raw_configs\datas ^
        --output_code_dir d:\server\src\Gen ^
        --output_data_dir d:\server\GameData ^
        --gen_types code_cs_json,data_json ^
        --service server ^
        --export_test_data

        







