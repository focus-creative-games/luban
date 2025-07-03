
- [README 中文](./README.md)
- [README English](./README_EN.md)

# Luban

![icon](docs/images/logo.png)

[![license](http://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://opensource.org/licenses/MIT) ![star](https://img.shields.io/github/stars/focus-creative-games/luban?style=flat-square)


luban is a powerful, easy-to-use, elegant, and stable game configuration solution. It is designed to meet the needs of simple to complex game configuration workflows from small to very large game projects.

luban can handle a variety of file types, supports popular languages, can generate multiple export formats, supports rich data inspection functions, has good cross-platform capabilities, and generates extremely fast.
Luban has a clear and elegant generation pipeline design, supports good modularization and plug-in, and is convenient for developers to carry out secondary development. Developers can easily adapt luban to their own configuration format, and customize powerful configuration tools that meet project requirements.

Luban standardizes the game configuration development workflow, which can greatly improve the efficiency of planning and programming.

## Core features

- Rich source data format. Support excel family (csv, xls, xlsx, xlsm), json, xml, yaml, lua, etc.
- Rich export formats. Support generating binary, json, bson, xml, lua, yaml and other format data
- Enhanced excel format. Simple configurations such as simple lists, substructures, structured lists, and arbitrarily complex deep nested structures can be concisely configured
- Complete type system. Not only can it express common specification line lists, but it can flexibly and elegantly express complex GamePlay data such as behavior trees, skills, plots, and dungeons because **supports OOP type inheritance**
- Support multiple languages. Supports generating language codes such as c#, java, go, cpp, lua, python, typescript, php, rust, etc.
- Support popular message schemes. protobuf(schema + binary + json), flatbuffers(schema + json), msgpack(binary)
- Powerful data verification capability. ref reference check, path resource path, range range check, etc.
- Perfect localization support
- Supports all major game engines and platforms. Support Unity, Unreal, Cocos2x, Godot, WeChat games, etc.
- Good cross-platform capability. It can run well on Win, Linux, and Mac platforms.
- Support all mainstream hot update solutions. hybridclr, ilruntime, {x,t,s}lua, puerts, etc.
- The generated code makes no reflection API calls, ensuring compatibility with common code obfuscation and hardening tools such as [Obfuz](https://github.com/focus-creative-games/obfuz), Obfuscator, Confuser, and .NET Refactor.
- Clear and elegant generation pipeline, it is easy to carry out secondary development on the basis of luban, and customize a configuration tool suitable for your own project style.

## Documentation

- [Official Documentation](https://luban.doc.code-philosophy.com/)
- [Quick Start](https://luban.doc.code-philosophy.com/docs/beginner/quickstart)
- **Example Project** ([github](https://github.com/focus-creative-games/luban_examples)) ([gitee](https://gitee.com/focus-creative-games/luban_examples) )

## Support and contact

- QQ group: 692890842 (Luban development exchange group)
- discord: https://discord.gg/dGY4zzGMJ4
- Email: luban@code-philosophy.com


## license

Luban is licensed under the [MIT](https://github.com/focus-creative-games/luban/blob/main/LICENSE) license
