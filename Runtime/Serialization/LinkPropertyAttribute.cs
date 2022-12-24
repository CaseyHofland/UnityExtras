#nullable enable
using UnityEngine;

namespace UnityExtras
{
    public class LinkPropertyAttribute : PropertyAttribute
    {
        public string propertyName;

        public LinkPropertyAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }
    }
}
