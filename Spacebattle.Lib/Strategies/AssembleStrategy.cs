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
        string assemblyName = interfaceType.ToString() + "Adapter";
        var defaultCompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var defaultReferences = new[]{
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("Spacebattle.Lib").Location)};
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: new[] { CSharpSyntaxTree.ParseText(code) },
            references: defaultReferences,
            options: defaultCompilationOptions);
        using var memoryStream = new MemoryStream();
        compilation.Emit(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return Assembly.Load(memoryStream.ToArray());
    }
}
