using Moq;
using Hwdtech;
using Hwdtech.Ioc;
using System.Collections;
namespace Spacebattle.Tests;

public class BuildTreeTests
{
    [Fact]
    public void BuildTreeTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        var tree = new Hashtable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Collision.SetupTree", (object[] args) => tree).Execute();
        var mockVectors = new Mock<IVectorslike>();
        List<List<int>> vectors = File.ReadAllLines("../../../Vectors.txt").Select(vector => vector.Split(' ').Select(int.Parse).ToList()).ToList();
        mockVectors.Setup(t => t.ToActualVectors()).Returns(vectors);
        BuildTree treeBuilding = new BuildTree(mockVectors.Object);
        treeBuilding.Execute();
        Hashtable actualTree = IoC.Resolve<Hashtable>("Game.Collision.SetupTree");

        Hashtable shouldBe = new(){
        {1, new Hashtable(){
            {3, new Hashtable(){
                {5, new Hashtable(){
                    {3, new Hashtable()}}},
                {4, new Hashtable(){
                    {2, new Hashtable()}}}
                }
            }}
        }};
        
    }
}
