[//]: # (Author: bug)
[//]: # (Date: 2021-09-26 22:25:02)

# binary,json,lua 导出数据格式
- 支持binary,json,lua三种导出数据类型。不同的导出类型只影响导出的数据大小和生成的代码和加载数据的性能，不影响结构定义以及最终加载到内存占用。

- 不同的导出数据类型对程序和策划是透明的，切换不影响数据编辑方式和业务代码中使用配置的方式。
  
# 代码模板

使用scriban模板文件定制导出数据格式。例如生成cs语言bin数据格式的cfg.Tables类的模板如下。

```
using Bright.Serialization;

{{
    name = x.name
    namespace = x.namespace
    tables = x.tables
}}
namespace {{namespace}}
{
   
public sealed class {{name}}
{
    {{~for table in tables ~}}
{{~if table.comment != '' ~}}
    /// <summary>
    /// {{table.comment}}
    /// </summary>
{{~end~}}
    public {{table.full_name}} {{table.name}} {get; }
    {{~end~}}

    public {{name}}(System.Func<string, ByteBuf> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        {{~for table in tables ~}}
        {{table.name}} = new {{table.full_name}}(loader("{{table.output_data_file}}")); 
        tables.Add("{{table.full_name}}", {{table.name}});
        {{~end~}}

        {{~for table in tables ~}}
        {{table.name}}.Resolve(tables); 
        {{~end~}}
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        {{~for table in tables ~}}
        {{table.name}}.TranslateText(translator); 
        {{~end~}}
    }
}

}
```

# 数据模板
- 使用scriban模板文件定制导出数据格式。例如自定义的lua数据模板如下：

```
// {{table.name}}
{{for d in datas}}
	// {{d.impl_type.full_name}}
	{{~i = 0~}}
	{{~for f in d.fields~}}
		{{~if f ~}}
		// {{d.impl_type.hierarchy_export_fields[i].name}} = {{f.value}}
		{{~end~}}
		{{~i = i + 1~}}
	{{~end~}}
{{end}}
```

- 输出数据

```
// TbItem
	// item.Item
		// id = 1
		// name = 钻石
		// major_type = 1
		// minor_type = 101
		// max_pile_num = 9999999
		// quality = 0
		// icon = /Game/UI/UIText/UI_TestIcon_3.UI_TestIcon_3
		
	// item.Item
		// id = 2
		// name = 金币
		// major_type = 1
		// minor_type = 102
		// max_pile_num = 9999999
		// quality = 0
		// icon = /Game/UI/UIText/UI_TestIcon_1.UI_TestIcon_1
```