#nullable enable
using System.Collections;
using UnityEngine;

namespace UnityExtras
{
    public static partial class AnimatorExtensions
    {
        public static void SetTrigger(this Animator animator, TriggerParameter parameter) => animator.SetTrigger(parameter.parameterName);
        public static void ResetTrigger(this Animator animator, TriggerParameter parameter) => animator.ResetTrigger(parameter.parameterName);

        public static bool GetBool(this Animator animator, BoolParameter parameter) => animator.GetBool(parameter.parameterName);
        public static void SetBool(this Animator animator, BoolParameter parameter) => animator.SetBool(parameter.parameterName, parameter.value);

        public static int GetInteger(this Animator animator, IntParameter parameter) => animator.GetInteger(parameter.parameterName);
        public static void SetInteger(this Animator animator, IntParameter parameter) => animator.SetInteger(parameter.parameterName, parameter.value);

        public static float GetFloat(this Animator animator, FloatParameter parameter) => animator.GetFloat(parameter.parameterName);
        public static void SetFloat(this Animator animator, FloatParameter parameter) => animator.SetFloat(parameter.parameterName, parameter.value);
        public static void SetFloat(this Animator animator, FloatParameter parameter, float deltaTime) => animator.SetFloat(parameter.parameterName, parameter.value, parameter.dampTime, deltaTime);

        public static IEnumerator SetFloatCoroutine(this Animator animator, FloatParameter parameter)
        {
            do
            {
                animator.SetFloat(parameter, Time.deltaTime);
                yield return null;
            }
            while (!Mathf.Approximately(animator.GetFloat(parameter), parameter.value));
        }
    }
}
