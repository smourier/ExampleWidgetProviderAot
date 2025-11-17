namespace ExampleWidgetProvider;

[GeneratedComInterface, Guid("00000001-0000-0000-c000-000000000046")]
public partial interface IClassFactory
{
    [PreserveSig]
    int CreateInstance(nint pUnkOuter, in Guid riid, out nint ppvObject);

    [PreserveSig]
    int LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock);
}
