{{~if comment != '' ~}}
'''
{{comment}}
'''
{{~end~}}
class {{py_full_name}}(Enum):
    {{~ for item in items ~}}
{{~if item.comment != '' ~}}
    '''
    {{item.comment}}
    '''
{{~end~}}
    {{item.name}} = {{item.value}}
    {{~end~}}
    {{~if (items == empty)~}}
    pass
    {{~end~}}
