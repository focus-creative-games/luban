package {{namespace_with_top_module}};
{{~if comment != '' ~}}
/**
 * {{comment}}
 */
{{~end~}}
public enum {{name}} {
    {{~ for item in items ~}}
{{~if item.comment != '' ~}}
    /**
     * {{item.comment}}
     */
{{~end~}}
    {{item.name}}({{item.int_value}}),
    {{~end~}}
    ;

    private final int value;

    public int getValue() {
        return value;
    }

    {{name}}(int value) {
        this.value = value;
    }

    public static {{name}} valueOf(int value) {
    {{~ for item in items ~}}
        if (value == {{item.int_value}}) return {{item.name}};
    {{~end~}}
        throw new IllegalArgumentException("");
    }
}
