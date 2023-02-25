#nullable enable
using System;

namespace UnityExtras
{
    public class CircularReferenceException : Exception
    {
        public CircularReferenceException() { }
        public CircularReferenceException(string message) : base(message) { }
        public CircularReferenceException(string message, Exception inner) : base(message, inner) { }
    }
}
