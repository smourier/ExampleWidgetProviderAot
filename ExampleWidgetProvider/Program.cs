namespace ExampleWidgetProvider;

internal partial class Program
{
    static void Main()
    {
        Console.WriteLine("Registering Widget Provider");

        var factory = new WidgetProviderFactory();
        var wrappers = new StrategyBasedComWrappers();
        var wrappersUnk = wrappers.GetOrCreateComInterfaceForObject(factory, CreateComInterfaceFlags.None);
        var hr = CoRegisterClassObject(typeof(WidgetProvider).GUID, wrappersUnk, CLSCTX_LOCAL_SERVER, REGCLS_MULTIPLEUSE, out var cookie);
        if (hr < 0)
        {
            Console.WriteLine($"Failed to register class object. HR=0x{hr:X8}");
            return;
        }

        Console.WriteLine("Registered successfully. Press ENTER to exit.");
        Console.ReadLine();

        // Wait until the manager has disposed of the last widget provider.
        using (var emptyWidgetListEvent = WidgetProvider.GetEmptyWidgetListEvent())
        {
            Console.WriteLine("Waiting for all widgets to be removed...");
            emptyWidgetListEvent.WaitOne();
        }

        Console.WriteLine("No more widgets. Exiting.");
        _ = CoRevokeClassObject(cookie);
    }

#pragma warning disable IDE1006 // Naming Styles

    private const uint CLSCTX_LOCAL_SERVER = 4;
    private const uint REGCLS_MULTIPLEUSE = 1;

#pragma warning restore IDE1006 // Naming Styles

    [LibraryImport("OLE32")]
    [PreserveSig]
    private static partial int CoRegisterClassObject(in Guid rclsid, nint pUnk, uint dwClsContext, uint flags, out uint lpdwRegister);

    [LibraryImport("OLE32")]
    [PreserveSig]
    private static partial int CoRevokeClassObject(uint dwRegister);
}
