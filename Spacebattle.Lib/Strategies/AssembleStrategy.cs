using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
namespace Spacebattle.Lib;
public class AssembleStrategy
{
    public object Apply(params object[] args)
    {
        var code = (string)args[0];
        var adapterType = (Type)args[1];
        var compilation = CSharpCompilation.Create(adapterType.ToString() + "Adapter").AddReferences(new[]{
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("Spacebattle.Lib").Location)})
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));
        Assembly assembly;
        using (var ms = new MemoryStream())
        {
            var result = compilation.Emit(ms);
            ms.Seek(0, SeekOrigin.Begin);
            assembly = Assembly.Load(ms.ToArray());
        }
        return assembly;
    }
}
