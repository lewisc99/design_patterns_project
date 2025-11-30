
/// <summary>
/// Asynchronous Factory Method
/// </summary>
public class Foo
{

    private string Name { get; set; }
    protected Foo() { }

    protected Foo(string name)
    {
        this.Name = name;
    }

    public override string ToString()
    {
        return Name;
    }

    private async Task InitAsync()
    {
        await Task.Delay(1000);
    }

    public static async Task<Foo> CreateAsync()
    {
        var result = new Foo();
        await result.InitAsync();
        return result;
    }

    public static async Task<Foo> CreateFooNameAsync(string name)
    {
        var result = new Foo(name);
        await result.InitAsync();
        return result;
    }
}

public class AsynchronousFactoryResult
{
    public async void Result01()
    {
        var foo = await Foo.CreateAsync();
    }

    public async void Result02()
    {
        var foo1 = await Foo.CreateFooNameAsync("Luiz");

        Console.WriteLine(foo1);
    }
}