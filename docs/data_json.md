[//]: # (Author: bug)
[//]: # (Date: 2020-11-01 16:26:41)

# Json 数据

##  说明
- 在一个大型复杂项目里，有些表的数据是以json形式保存，比如技能、AI、剧情等等。常规的导表工具只能处理excel，像xml、json之类的数据一般是程序员自己处理，最终导致游戏内有几套配置加载方案，而且前后端以及
编辑器的开发者还得花费大量时间手写代码去处理这些数据，既麻烦又不容易定位错误。

- luban通过 **定义 + 数据源** 的方式统一所有配置。json数据源用法与excel数据源基本相同，唯一区别在于
输入的数据文件格式由xlsx变成json。实际项目中如果以json为数据格式，为了方便编辑器处理，一般一个记录占一个文件，所有记录统一放在一个目录下，因此数据源变成了目录。如下图中的input="test/json_datas"目录。

- Luban 支持 json 数据为数据源。 一般来说，json数据是由编辑器制作导出的，而不像excel那样由 人工直接编辑。

## 例子
* 我们新增一个定义表,覆盖了常见数据类型。  
```xml
<bean name="DemoType2" >
	<var name="x4" type="int" convert="DemoEnum"/>
	<var name="x1" type="bool"/>
	<var name="x5" type="long" convert="DemoEnum"/>
	<var name="x6" type="float"/>
	<var name="x7" type="double"/>
	<var name="x10" type="string"/>
	<var name="x12" type="DemoType1"/>
	<var name="x13" type="DemoEnum"/>
	<var name="x14" type="DemoDynamic" sep=","/>多态数据结构
	<var name="v2" type="vector2"/>
	<var name="v3" type="vector3"/>
	<var name="v4" type="vector4"/>
	<var name="t1" type="datetime"/>
	<var name="k1" type="array,int"/> 使用;来分隔
	<var name="k2" type="list,int"/>
	<var name="k8" type="map,int,int"/>
	<var name="k9" type="list,DemoE2" sep="," index="y1"/>
	<var name="k15" type="array,DemoDynamic" sep=","/> 
</bean>

<table name="TbDataFromJson" value="DemoType2" input="test/json_datas"/>
```
* 我们在 item目录下 新增一个目录，叫 DemoJsonDatas, 里面放两个数据：  
  ![如图](images/adv/def_43.png)
* json 的内容如下：  
递归遍历test/json_datas整个目录树，**按文件名排序后**依次将每个json数据当作一个记录读入。其中1.json文件内容如下

```json
 {
	"x1":true,
	"x2":3,
	"x3":128,
	"x4":1,
	"x5":11223344,
	"x6":1.2,
	"x7":1.23432,
	"x10":"hq",
	"x12": { "x1":10},
	"x13":"B",
	"x14":{"__type__": "DemoD2", "x1":1, "x2":2},
	"v2":{"x":1, "y":2},
	"v3":{"x":1.1, "y":2.2, "z":3.4},
	"v4":{"x":10.1, "y":11.2, "z":12.3, "w":13.4},
	"t1":"1970-01-01 00:00:00",
	"k1":[1,2],
	"k2":[2,3],
	"k7":[2,3],
	"k8":[[2,2],[4,10]],
	"k9":[{"y1":1, "y2":true},{"y1":2, "y2":false}],
	"k15":[{"__type__": "DemoD2", "x1":1, "x2":2}]
 }
```
* 自行参数每种数据类型 在 json格式的填法，大多数都是合乎常理的。唯一特殊的是map类型。
* 由于json 的 key 类型必须为 string, 所以对于 map类型。数据格式为   
  ```
    [ [key1,value1], [key2, value2], ,,,] 
  ```
