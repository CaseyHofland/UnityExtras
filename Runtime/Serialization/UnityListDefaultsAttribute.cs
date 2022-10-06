#nullable enable
using UnityEngine;

namespace UnityExtras
{
    /// <summary>
    /// By default, new items in serialized lists are initialized with their stateless values (e.g. <see cref="bool"/>s reset to <see langword="false"/> regardless of implementation).
    /// <para>Use this attribute on serialized lists in order to reset new items to their default values.</para>
    /// </summary>
    public class UnityListDefaultsAttribute : PropertyAttribute { }
}
