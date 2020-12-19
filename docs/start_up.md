# 快速上手


## 准备工作
1. 安装 .net 5 dsk
1. 下载luban_examples项目

    下载项目 [luban_examples](https://github.com/focus-creative-games/luban_examples)。
    项目中包含测试配置、最新的luban_client&luban_server工作以及大量的示例项目。为方便起见，后续提及到的文件，默认都指这个项目中的文件。

1. 设置全局环境变量 

    set_global_envs.bat 脚本文件，它会设置全局环境变量 LUBAN_SERVER_IP 为 127.0.0.1。如果你在其他机器上部属luban.server，可以将LUBAN_SERVER_IP改为相应地址。
1. 启动luban.server
    
    运行 run_luban_server.bat 脚本文件，启动luban.server程序。由于luban工具使用client/server的工作
    模块，必须有一个运行中的luban.server程序，才能完成生成工作，**请保持luban.server持续运行**。

    新手测试时可以在本机启动luban.server，实际开发中，建议**一个项目部属一个luban.server**。


## 创建起始的游戏配置目录结构
1. 创建配置根目录
    创建一个目录来存放所有游戏配置相关的文件。假设目录名为 Config（名字随意）。 目录下将包含数据目录、定义文件目录、检查脚本、高级自定义检查工程。
1. 在Config目录下创建数据目录Datas(名字随意)。将来用于存放所有游戏原始数据文件。
1. 在Config目录下创建定义目录Defines(名字随意)。目录下将包含根定义及各个模块子定义文件。
1. 在Defines目录下创建\_\_root__.xml 文件。文件内容如下：
    ```xml
    <root>
        <topmodule name="cfg"/> 额外的顶层命名空间

        <group name="c" default="1"/> client 分组
        <group name="s" default="1"/> server 分组
        <group name="e" default="1"/> editor分组

        <import name="."/> 从当前目录导入所有子模块定义文件

        <service name="server" manager="Tables" group="s"/> 输出目标 server，包含s分组的表和字段
        <service name="client" manager="Tables" group="c"/> 输出目标 client，包含c分组的表和字段
        <service name="all" manager="Tables" group="c,s,e"/> 输出目标 editor，包含c,s,e分组的表和字段
    </root>
    ```

    直接从 luban_examples项目的 config/Defines/\_\_root__.xml 拷贝即可。

1. 在配置根目录下，创建 check.bat，内容如下：

```bat
set WORKSPACE=..
set GEN_CLIENT=%WORKSPACE%\Tools\Luban.Client\Luban.Client.exe
set CONF_ROOT=%WORKSPACE%\DesignerConfigs
set DEFINE_FILE=%CONF_ROOT%\Defines\__root__.xml

<luaban.client.exe的路径> -h %LUBAN_SERVER_IP% -j cfg --^
 -d Defines\__root__.xml^
 --input_data_dir Datas ^
 --output_data_dir output_data ^
 --gen_types data_bin ^
 -s all ^
--export_test_data
pause

```

执行 check.bat，如果输出最后为
```
    == succ ==
```
表明命令行是正确的，能够正常生成，否则仔细检查参数。


# 进阶