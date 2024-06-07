using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Hwdtech;
namespace Spacebattle.Lib;
public class AssembleStrategy
{
    public object Apply(params object[] args)
    {
        Type interfaceType = (Type)args[0];
        Type fieldType = (Type)args[1];
        string code = IoC.Resolve<string>("Game.StringCode", interfaceType, fieldType);
        var compilation = CSharpCompilation.Create(interfaceType.ToString() + "Adapter").AddReferences(new[]{
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
