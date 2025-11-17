namespace ExampleWidgetProvider;

[GeneratedComClass]
public partial class WidgetProviderFactory : IClassFactory
{
#pragma warning disable IDE1006 // Naming Styles

    private const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);
    private const int E_NOINTERFACE = unchecked((int)0x80004002);

#pragma warning restore IDE1006 // Naming Styles

    int IClassFactory.LockServer(bool fLock) => 0;
    int IClassFactory.CreateInstance(nint pUnkOuter, in Guid riid, out nint ppvObject)
    {
        ppvObject = 0;
        if (pUnkOuter != 0)
            return CLASS_E_NOAGGREGATION;

        if (riid == typeof(WidgetProvider).GUID || riid == typeof(IUnknown).GUID)
        {
            ppvObject = MarshalInspectable<IWidgetProvider>.FromManaged(new WidgetProvider());
            return 0;
        }
        return E_NOINTERFACE;
    }
}
