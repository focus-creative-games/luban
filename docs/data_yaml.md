[//]: # (Author: bug)
[//]: # (Date: 2020-11-01 16:26:41)

### json 数据源

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

<table name="TbDataFromYaml" value="DemoType2" input="test/yaml_datas"/> 
```

递归遍历test/yaml_datas整个目录树，**按文件名排序后**依次将每个yaml数据当作一个记录读入。其中1.yml文件内容如下

```yaml
---
x1: true
x2: 3
x3: 128
x4: 40
x5: 11223344
x6: 1.2
x7: 1.23432
x10: hq
x12:
  x1: 10
x13: B
x14:
  __type__: DemoD2
  x1: 1
  x2: 2
s1:
  key: "/key32"
  text: aabbcc22
v2:
  x: 1
  y: 2
v3:
  x: 1.1
  y: 2.2
  z: 3.4
v4:
  x: 10.1
  y: 11.2
  z: 12.3
  w: 13.4
t1: '1970-01-01 00:00:00'
k1:
- 1
- 2
k2:
- 2
- 3
k8:
- - 2
  - 2
- - 4
  - 10
k9:
- y1: 1
  y2: true
- y1: 2
  y2: false
k15:
- __type__: DemoD2
  x1: 1
  x2: 2

```
