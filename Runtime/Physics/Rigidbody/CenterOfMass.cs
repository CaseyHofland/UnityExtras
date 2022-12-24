#nullable enable
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[ExecuteAlways]
public class CenterOfMass : MonoBehaviour
{
    [SerializeField][Tooltip("The offset from the regular center of mass.")] private Vector3 _offset;
    /// <summary>The offset from the regular center of mass.</summary>
    public Vector3 offset 
    {
        get => _offset;
        set
        {
            _rigidbody!.ResetCenterOfMass();
            _rigidbody.centerOfMass += (_offset = value);
        }
    }

    private Rigidbody? _rigidbody;

    public Rigidbody GetRigidbody() => GetComponent<Rigidbody>();

    private void OnValidate()
    {
        Awake();
    }

    private void Awake()
    {
        _rigidbody = GetRigidbody();
        offset = offset;
    }

    private void OnDestroy()
    {
        _rigidbody!.ResetCenterOfMass();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_rigidbody!.worldCenterOfMass, 0.05f);
    }
}
