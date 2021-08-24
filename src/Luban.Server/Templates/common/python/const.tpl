{{~if x.comment != '' ~}}
'''
{{x.comment}}
'''
{{~end~}}
class {{x.py_full_name}}:
    {{~ for item in x.items ~}}
{{~if item.comment != '' ~}}
    '''
    {{item.comment}}
    '''
{{~end~}}
    {{item.name}} = {{py_const_value item.ctype item.value}}
    {{~end~}}
    {{~if (x.items == empty)~}}
    pass
    {{~end~}}
