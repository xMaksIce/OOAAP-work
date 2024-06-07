using Hwdtech;
using Hwdtech.Ioc;
using System.Reflection;
namespace Spacebattle.Tests;
public class CompileAdapterTest
{
    public CompileAdapterTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }
    [Fact]
    public void CodeCompiles()
    {
        var code = @"
        using System.Collections;
        namespace Spacebattle.Lib;
        public class IUObjectAdapter : IUObject
        {
            readonly private Hashtable props = new();
            public IUObjectAdapter(){}
            public object GetProperty(string name)
            {
                return props[name]!;
            }
            public void SetProperty(string name, object value)
            {
                props[name] = value;
            }
        }";
        var assembleStrategy = new AssembleStrategy();
        var assembly = (Assembly)assembleStrategy.Apply(code, typeof(IUObject));
        var adapterInstance = Activator.CreateInstance(assembly.GetType("Spacebattle.Lib.IUObjectAdapter")!)!;
        var adapterType = adapterInstance.GetType();
        adapterType.GetMethod("SetProperty")!.Invoke(adapterInstance, new object[] { "KnockKnock", "Who's there?" });
        string property = (string)adapterType.GetMethod("GetProperty")!.Invoke(adapterInstance, new object[] { "KnockKnock" })!;
        Assert.Equal("Spacebattle.Lib.IUObjectAdapter", adapterType.ToString());
        Assert.Equal("Who's there?", property);
    }
}
