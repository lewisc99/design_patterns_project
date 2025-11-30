/// <summary>
/// Delegate Factories in IoC
/// </summary>
public class Service
{
    public string DoSomething(int value)
    {
        return $"I have {value}";
    }
}

public class DomainObject
{
    private Service service;

    private int value;

    public delegate DomainObject Factory(int value);


    public DomainObject(Service service, int value)
    {
        this.service = service;
        this.value = value;
    }
    public override string ToString()
    {
        return service.DoSomething(value);
    }
    
}

public class Container
{
    private Service _service = new Service();

    public DomainObject.Factory Resolve<DomainObjectFactory>()
    {
        // Return a factory delegate that creates DomainObject with the injected Service
        return value => new DomainObject(_service, value);
    }
}


public class DelegateFactoriesResult
{
    public void Result01()
    {
        var service = new Service();
        DomainObject.Factory factory = value => new DomainObject(service, value);

        var result01 = factory(10);
        var result02 = factory(20);

        Console.WriteLine(result01);
        Console.WriteLine(result02);
    }

    public void Result02()
    {

        var container = new Container();
        var factory = container.Resolve<DomainObject.Factory>();
        var dobj2 = factory(42);
        Console.WriteLine(dobj2); // I have 42

        var doc3 = factory(10);
        Console.WriteLine(doc3);
    }
}