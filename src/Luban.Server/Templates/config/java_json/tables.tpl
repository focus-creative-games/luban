package {{package}};

import com.google.gson.JsonElement;

public final class {{name}}
{
    public  interface  IJsonLoader {
        JsonElement load(String file) throws java.io.IOException;
    }

    {{~for table in tables ~}}
{{~if table.comment != '' ~}}
    /**
     * {{table.comment}}
     */
{{~end~}}
    private final {{table.full_name_with_top_module}} {{table.inner_name}};
    public {{table.full_name_with_top_module}} get{{table.name}}() { return {{table.inner_name}}; }
    {{~end~}}

    public {{name}}(IJsonLoader loader) throws java.io.IOException {
        var tables = new java.util.HashMap<String, Object>();
        {{~for table in tables ~}}
        {{table.inner_name}} = new {{table.full_name_with_top_module}}(loader.load("{{table.output_data_file}}")); 
        tables.put("{{table.full_name}}", {{table.inner_name}});
        {{~end~}}

        {{~ for table in tables ~}}
        {{table.inner_name}}.resolve(tables); 
        {{~end~}}
    }
}
