
    type ProtocolFactory = () => Protocol

    export class {{name}} {
        static readonly Factories = new Map<number, ProtocolFactory>([

        {{~ for proto in protos ~}}
            [{{proto.full_name}}.ID, () => new {{proto.full_name}}()],
        {{~end~}}

        {{~ for rpc in rpcs ~}}
            // TODO RPC .. [{{rpc.full_name}}.ID] = () => new {{rpc.full_name}}(),
        {{~end~}}
        ])
    }
