package {{package}};

import bright.serialization.*;

public final class {{name}}
{
    public  static interface  IByteBufLoader {
        ByteBuf load(String file) throws java.io.IOException;
    }

    {{~for table in tables ~}}
{{~if table.comment != '' ~}}
    /**
     * {{table.comment}}
     */
{{~end~}}
    public final {{table.full_name_with_top_module}} {{table.name}};
    {{~end~}}

    public {{name}}(IByteBufLoader loader) throws java.io.IOException {
        var tables = new java.util.HashMap<String, Object>();
        {{~for table in tables ~}}
        {{table.name}} = new {{table.full_name_with_top_module}}(loader.load("{{table.output_data_file}}")); 
        tables.put("{{table.full_name}}", {{table.name}});
        {{~end~}}

        {{~ for table in tables ~}}
        {{table.name}}.resolve(tables); 
        {{~end~}}
    }
}
