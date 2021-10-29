{{~if comment != '' ~}}
'''
{{comment | html.escape}}
'''
{{~end~}}
class {{py_full_name}}(Enum):
    {{~ for item in items ~}}
{{~if item.comment != '' ~}}
    '''
    {{item.escape_comment}}
    '''
{{~end~}}
    {{item.name}} = {{item.value}}
    {{~end~}}
    {{~if (items == empty)~}}
    pass
    {{~end~}}
