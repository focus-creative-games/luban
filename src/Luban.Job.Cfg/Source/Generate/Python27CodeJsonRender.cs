using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    class Python27CodeJsonRender : PythonCodeRenderBase
    {

        [ThreadStatic]
        private static Template t_beanRender;
        public override string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(@"

{{
    name = x.py_full_name
    is_abstract_type = x.is_abstract_type
    parent_def_type = x.parent_def_type
    export_fields = x.export_fields
    hierarchy_export_fields = x.hierarchy_export_fields
}}

'''
{{x.comment}}
'''
class {{name}} {{if parent_def_type}}({{parent_def_type.py_full_name}}){{end}}:
{{~if x.is_abstract_type~}}
    _childrenTypes = None

    @staticmethod
    def fromJson(_json_):
        childrenTypes = {{name}}._childrenTypes
        if not childrenTypes:
            childrenTypes = {{name}}._childrenTypes = {
        {{~ for child in x.hierarchy_not_abstract_children~}}
            '{{child.name}}': {{child.py_full_name}},
        {{~end~}}
    }
        type = _json_['__type__']
        child = {{name}}._childrenTypes.get(type)
        if child != None:
            return  child(_json_)
        else:
            raise Exception()
{{~end~}}

    def __init__(self, _json_):
        {{~if parent_def_type~}}
        {{parent_def_type.py_full_name}}.__init__(self, _json_)
        {{~end~}}
        {{~ for field in export_fields ~}}
        {{~if !field.ctype.is_nullable~}}
        if _json_['{{field.name}}'] == None: raise Exception()
        {{~end~}}
        {{py27_deserialize ('self.' + field.py_style_name) ('_json_[""' + field.name + '""]') field.ctype}}
        {{~end~}}
        {{~if export_fields.empty?}}
        pass
        {{~end~}}
");
            var result = template.RenderCode(b);

            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        public override string Render(DefTable p)
        {
            var template = t_tableRender ??= Template.Parse(@"
{{ 
    name = x.py_full_name
    key_type = x.key_ttype
    key_type1 =  x.key_ttype1
    key_type2 =  x.key_ttype2
    value_type =  x.value_ttype
}}
'''
{{x.comment}}
'''
class {{name}}:
    {{~if x.is_map_table ~}}

    def __init__(self, _json_ ):
        self._dataMap = {}
        self._dataList = []
        
        for _json2_ in _json_:
            {{py27_deserialize '_v' '_json2_' value_type}}
            self._dataList.append(_v)
            self._dataMap[_v.{{x.index_field.py_style_name}}] = _v

    def getDataMap(self) : return self._dataMap
    def getDataList(self) : return self._dataList

    def get(self, key) : return self._dataMap.get(key)

    {{~else~}}

    def __init__(self, _json_):
        if (len(_json_) != 1): raise Exception('table mode=one, but size != 1')
        {{py27_deserialize 'self._data' '_json_[0]' value_type}}

    def getData(self) : return self._data

    {{~ for field in value_type.bean.hierarchy_export_fields ~}}
    '''
    {{field.comment}}
    '''
    def {{field.py_style_name}}(self) : return self._data.{{field.py_style_name}}
    {{~end~}}
    {{~end~}}
");
            var result = template.RenderCode(p);

            return result;
        }
    }
}
