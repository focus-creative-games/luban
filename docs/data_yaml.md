[//]: # (Author: bug)
[//]: # (Date: 2021-09-26 22:22:28)

# YAML 数据

## 例子
```xml
<table name="TbDataFromYaml" value="DemoType2" input="test/yaml_datas"/> 
```
其中 1.yml 文件内容如下
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