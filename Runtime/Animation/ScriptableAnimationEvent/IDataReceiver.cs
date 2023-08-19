#nullable enable

namespace UnityExtras
{
    public interface IDataReceiver<in T>
    {
        T value { set; }
    }
}
