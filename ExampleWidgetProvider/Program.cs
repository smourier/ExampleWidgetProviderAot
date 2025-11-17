
namespace ExampleWidgetProvider;

internal partial class Program
{
    static void Main()
    {
        Console.WriteLine("Registering Widget Provider");

        var provider = new WidgetProviderFactory();
        PInvoke.CoRegisterClassObject(typeof(WidgetProvider).GUID, provider, CLSCTX.CLSCTX_LOCAL_SERVER, REGCLS.REGCLS_MULTIPLEUSE, out var cookie);
        Console.WriteLine("Registered successfully. Press ENTER to exit.");
        Console.ReadLine();

        if (PInvoke.GetConsoleWindow() != 0)
        {
            Console.WriteLine("Registered successfully. Press ENTER to exit.");
            Console.ReadLine();
        }
        else
        {
            // Wait until the manager has disposed of the last widget provider.
            using (var emptyWidgetListEvent = WidgetProvider.GetEmptyWidgetListEvent())
            {
                emptyWidgetListEvent.WaitOne();
            }

            _ = PInvoke.CoRevokeClassObject(cookie);
        }
    }
}
