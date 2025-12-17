using System.Text;

/// <summary>
/// Object Tracking Factory  
/// </summary>
public interface ITheme
{
    string TextColor { get; }
    string BgrColor { get; }
}

class LightTheme : ITheme
{
    public string TextColor => "black";
    public string BgrColor => "white";
}
class DarkTheme : ITheme
{
    public string TextColor => "white";
    public string BgrColor => "dark gray";
}

public class TrackingThemeFactory
{
    private readonly List<WeakReference<ITheme>> themes = new();

    public ITheme CreateTheme(bool dark)
    {
        ITheme theme = dark ? new DarkTheme() : new LightTheme();
        themes.Add(new WeakReference<ITheme>(theme));
        return theme;
    }

    public string Info
    {
        get
        {
            var sb = new StringBuilder();

            foreach (var weakRef in themes)
            {
                if (weakRef.TryGetTarget(out var theme))
                {
                    if (theme is DarkTheme)
                        sb.AppendLine("Dark theme");
                    else if (theme is LightTheme)
                        sb.AppendLine("Light theme");
                }
            }

            return sb.ToString();
        }
    }
}

public class Ref<T> where T : class
{
    public T Value { get; set; }
    public Ref(T value) { Value = value; }
}

public class ThemeRef : Ref<ITheme>, ITheme
{
    public ThemeRef(ITheme value) : base(value) { }
    public string TextColor => Value.TextColor;
    public string BgrColor => Value.BgrColor;
}

public class ReplaceableThemeFactory
{
    private readonly List<WeakReference<Ref<ITheme>>> themes = new();

    private ITheme createThemeImpl(bool dark)
    {
        return dark ? new DarkTheme() : new LightTheme();
    }
    public Ref<ITheme> CreateTheme(bool dark)
    {
        var r = new Ref<ITheme>(createThemeImpl(dark));
        themes.Add(new(r));
        return r;
    }

    public void ReplaceTheme(bool dark)
    {
        foreach (var weakRef in themes)
        {
            if (weakRef.TryGetTarget(out var reference))
            {
                reference.Value = createThemeImpl(dark);
            }
        }
    }
}

public class ObjectTrackingFactoryResult
{
    public void Result01()
    {
        var factory = new TrackingThemeFactory();
        var theme = factory.CreateTheme(true);
        var theme2 = factory.CreateTheme(false);
        var theme3 = factory.CreateTheme(true);
        Console.WriteLine(factory.Info);
    }

    public void Result02()
    {

        /* In real-world terms, you can think of this functionality as a factory recall: if a car factory finds that cars sold up to now have a defective part, it collects your car and replaces the part, for free. The same principle is applied here:
         */

        var factory = new ReplaceableThemeFactory();
        var magicTheme = factory.CreateTheme(true);
        Console.WriteLine(magicTheme.Value.BgrColor);

        factory.ReplaceTheme(false);
        Console.WriteLine(magicTheme.Value.BgrColor);
    }
}