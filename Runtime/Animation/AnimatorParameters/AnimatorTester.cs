#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityExtras;

[RequireComponent(typeof(Animator))]
public class AnimatorTester : MonoBehaviour
{
    public List<FloatParameter> floatParameters = new();
    public List<IntParameter> intParameters = new();
    public List<BoolParameter> boolParameters = new();
    public List<TriggerParameter> triggerParameters = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Animator _animator;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Animator GetAnimator() => GetComponent<Animator>();

    private void Start()
    {
        _animator = GetAnimator();
    }

    private void Update()
    {
        floatParameters.ForEach(parameter => _animator.SetFloat(parameter, Time.deltaTime));
        intParameters.ForEach(parameter => _animator.SetInteger(parameter));
        boolParameters.ForEach(parameter => _animator.SetBool(parameter));
    }
}