[//]: # (Author: bug)
[//]: # (Date: 2020-11-23 00:21:21)

# 服务端布署

## Docker 镜像布署
- 镜像会自动更新至最新版
- 在有 docker 的环境下，执行下面的指令
``` bash
    docker pull hugebug4ever/luban:latest
    docker run --name luban-server -d -p 8899:8899 hugebug4ever/luban:latest 
```

## Windows 下命令行布署
- 下载 [release](https://github.com/focus-creative-games/luban/releases) 版本
- 解压 Luban.Server.zip
- 执行下面的命令即可
```
    Luban.Server.exe
```