[//]: # (Author: bug)
[//]: # (Date: 2020-11-01 16:26:41)

# Lua 数据

## 

* 与 json 相似定义。  
* 唯一区别在于， lua 的table的key支持任意格式，所以 lua 的map 可以直接  {[key1] = value1, [key2] = value2, ,,,}
* ![如图](images/adv/def_42.png)  
* 注意 数据前有一个  return 语句。因为 lua 数据是当作 lua 文件加载的，每个加载后的结果当作一个记录读入。