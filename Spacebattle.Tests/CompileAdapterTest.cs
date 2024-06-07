using Hwdtech;
using Hwdtech.Ioc;
using System.Reflection;
using System.Collections;
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
        Hashtable codeTree = new(){
        {typeof(IUObject), new Hashtable(){
            {typeof(Hashtable), @"
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
                }"
            }}
        }};
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.StringCode",
        (object[] args) => ((Hashtable)codeTree[args[0]]!)[args[1]]).Execute();
        var assembleStrategy = new AssembleStrategy();
        var assembly = (Assembly)assembleStrategy.Apply(typeof(IUObject), typeof(Hashtable));
        var adapterInstance = Activator.CreateInstance(assembly.GetType("Spacebattle.Lib.IUObjectAdapter")!)!;
        var adapterType = adapterInstance.GetType();
        adapterType.GetMethod("SetProperty")!.Invoke(adapterInstance, new object[] { "Density", 256 });
        int property = (int)adapterType.GetMethod("GetProperty")!.Invoke(adapterInstance, new object[] { "Density" })!;
        Assert.Equal("Spacebattle.Lib.IUObjectAdapter", adapterType.ToString());
        Assert.Equal(256, property);
    }
}
