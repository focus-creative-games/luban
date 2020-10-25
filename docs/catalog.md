
* start up
    - [下载 dotnet sdk 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
    - [下载 luban.client&luban.server](https://github.com/focus-creative-games/luban/releases/tag/v1.0)
        - 启动 luban.server
    - 创建游戏配置
        - root.xml
        - 新增一个表定义
        - 新增一个excel文件
    - 生成
        - c# dotnet core
            - gen.client 命令行
        - c# unity
        - c# unity + ILRuntime
        - lua unity xlua
        - lua unity tolua
        - 其他lua
        - java
        - go
        - cpp
        - typescript
        - python 2.7及3.0
* 进阶
    - 游戏配置
        - excel 篇
            - 标题头定义
            - 定义和配置更多基础类型
            - 定义枚举
            - 定义bean
            - 数据格式
            - 指定某个或者多个sheet或者文件
            - list 或者其他类型
            - group 分组导出
            - tag 
            - sep 
            - multi_rows
            - 多级标题头
            - 单例表
            - 行表与列表
            - 可空变量
            - datetime
            - convert
            - 多态bean
        - json 篇
        - lua 
        - xml 
        - 一个同时包含 excel 、 json 配置的项目
    - 数据校验
        - ref 
        - path
    - 导出格式
        - bin 格式
        - lua 格式
    - 本地化
* gen.client & gen.server 工作模型
* 定义 + 数据的 抽象模型
* 定义
    * 根定义文件
        * group
        * service
        * topmodule
        * import
    * 子模块定义
        * 类型系统
            - 基础类型
            - 可空类型
            - 枚举
            - const
            - bean
                - field
					- convert 常量替换
                - 多态
        * table
        * module
* 源数据格式
    - excel
    - json
    - lua
    - xml
* 导出数据格式
    - bin
    - json
    - lua
        - emmylua anntations
* input 多种数据文件来源
* group 数据分组
* tag 
* 资源列表导出
* validator 数据检验
* 本地化
* editor 相关。导出符合luban数据约束的文件。
* ide支持
    - emmylua anntations

- luban.client 命令行介绍
    - luban.client 命令行参数
    - job cfg 命令行参数
    
* 各个语言和引擎及平台下的项目搭建
    - c# + dotnet core
    - c# + unity
    - c# + unity + ILRuntime
    - 其他 c# 平台
    - lua + unity + tolua
    - lua + unity + xlua
    - lua + unity + slua
    - lua + unreal + unlua
    - lua + cocos2dx
    - 其他使用 lua 的 平台
    - c++ + unreal
    - c++ + 其他平台
    - go
    - java
    - python 2.7
    - python 3.0
    - typescript + 微信小游戏
    - typescript + 其他平台

* luban 开发
    -  git clone 项目
    -  luban 构建与发布
        - 普通构建
        - docker
    - 代码结构介绍
    - 自定义 job