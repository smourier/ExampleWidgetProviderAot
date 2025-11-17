namespace ExampleWidgetProvider;

[GeneratedComClass]
public partial class WidgetProviderFactory : IClassFactory
{
    HRESULT IClassFactory.LockServer(BOOL fLock) => HRESULT.S_OK;
    unsafe HRESULT IClassFactory.CreateInstance(object pUnkOuter, Guid* riid, out object ppvObject)
    {
        ppvObject = 0;
        if (pUnkOuter != null)
            return HRESULT.CLASS_E_NOAGGREGATION;

        if (*riid == typeof(WidgetProvider).GUID || *riid == typeof(IUnknown).GUID)
        {
            ppvObject = MarshalInspectable<IWidgetProvider>.FromManaged(new WidgetProvider());
            return HRESULT.S_OK;
        }
        return HRESULT.E_NOINTERFACE;
    }
}
