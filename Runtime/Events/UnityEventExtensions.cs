#nullable enable
using UnityEngine;
using UnityEngine.Events;

namespace UnityExtras.Events
{
    public static class UnityEventExtensions
    {
        private static int GetPersistentEventIndex(this UnityEventBase unityEvent, string methodName, Object target)
        {
            int i;
            for (i = unityEvent.GetPersistentEventCount() - 1; i >= 0; i--)
            {
                if (methodName == unityEvent.GetPersistentMethodName(i)
                    && target == unityEvent.GetPersistentTarget(i))
                {
                    break;
                }
            }

            return i;
        }

        /// <include file='./UnityEventExtensions.xml' path='docs/GetPersistentEventIndex/*'/>
        public static int GetPersistentEventIndex(this UnityEvent unityEvent, UnityAction call) => call.Target is Object target ? GetPersistentEventIndex(unityEvent, call.Method.Name, target) : -1;
        /// <include file='./UnityEventExtensions.xml' path='docs/GetPersistentEventIndex/*'/>
        public static int GetPersistentEventIndex<T0>(this UnityEvent<T0> unityEvent, UnityAction<T0> call) => call.Target is Object target ? GetPersistentEventIndex(unityEvent, call.Method.Name, target) : -1;
        /// <include file='./UnityEventExtensions.xml' path='docs/GetPersistentEventIndex/*'/>
        public static int GetPersistentEventIndex<T0, T1>(this UnityEvent<T0, T1> unityEvent, UnityAction<T0, T1> call) => call.Target is Object target ? GetPersistentEventIndex(unityEvent, call.Method.Name, target) : -1;
        /// <include file='./UnityEventExtensions.xml' path='docs/GetPersistentEventIndex/*'/>
        public static int GetPersistentEventIndex<T0, T1, T2>(this UnityEvent<T0, T1, T2> unityEvent, UnityAction<T0, T1, T2> call) => call.Target is Object target ? GetPersistentEventIndex(unityEvent, call.Method.Name, target) : -1;
        /// <include file='./UnityEventExtensions.xml' path='docs/GetPersistentEventIndex/*'/>
        public static int GetPersistentEventIndex<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> unityEvent, UnityAction<T0, T1, T2, T3> call) => call.Target is Object target ? GetPersistentEventIndex(unityEvent, call.Method.Name, target) : -1;
    }
}
