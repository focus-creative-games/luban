
{{x.typescript_namespace_begin}}
{{~if x.comment != '' ~}}
/**
 * {{x.comment}}
 */
{{~end~}}
export class {{x.name}} {
	readonly id: number
	readonly name: string
	readonly alias: string
	readonly comment: string
	constructor(id: number, name: string, alias: string, comment: string) {
		this.id = id
		this.name = name
		this.alias= alias
		this.comment = comment
	}

    {{~for item in x.items ~}}
    static readonly {{item.name}} = new {{x.name}}({{item.int_value}}, `{{item.name}}`, `{{item.alias}}`, `{{item.comment}}`)
    {{~end~}}
}

{{x.typescript_namespace_end}}