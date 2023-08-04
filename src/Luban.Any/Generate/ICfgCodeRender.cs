namespace Luban.Any.Generate;

interface ICfgCodeRender : ICodeRender<DefTable>, IRender
{
    string Render(DefBean b);

    string Render(DefTable c);
}