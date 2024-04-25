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

        bool HashtablesEqual(Hashtable table1, Hashtable table2)
        {
            return table1.Count == table2.Count &&              // сравниваем кол-во ветвлений на текущем уровне
                table1.Cast<DictionaryEntry>().All(entry =>     // проходимся по каждому ветвлению
                    table2.Contains(entry.Key) &&               // проверяем, есть ли текущая вершина в сравниваемой таблице
                    HashtablesEqual((Hashtable)(entry.Value ?? new Hashtable()), // рекурсивно сравниваем следующие уровни дерева
                                    (Hashtable)(table2[entry.Key] ?? new Hashtable())));
        }

        Assert.True(HashtablesEqual(actualTree, shouldBe));
    }
}
