using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Physics/Crappy Slider Joint")]
[RequireComponent(typeof(Rigidbody))]
[ExecuteAlways]
public class CrappySliderJoint : MonoBehaviour
{
    private Rigidbody? _rigidbody;
    public new Rigidbody rigidbody => _rigidbody ? _rigidbody! : (_rigidbody = GetComponent<Rigidbody>());

    public Vector3 anchor;
    public Vector3 connectedAnchor;
    public Quaternion angle;
    public float minDistance = float.NegativeInfinity;
    public float maxDistance = float.PositiveInfinity;

    [SerializeField][HideInInspector] private ConfigurableJoint configurableJoint;

    private void Awake()
    {
        if (!configurableJoint)
        {
            configurableJoint = gameObject.AddComponent<ConfigurableJoint>();
            configurableJoint.hideFlags = HideFlags.NotEditable; // HideFlags.HideInInspector;
        }
    }

    private void Update()
    {
        configurableJoint.anchor = anchor;
        configurableJoint.axis = angle * Vector3.right;
        configurableJoint.autoConfigureConnectedAnchor = false;
        configurableJoint.connectedAnchor = connectedAnchor + configurableJoint.axis * (maxDistance + minDistance) * 0.5f;
        configurableJoint.secondaryAxis = angle * Vector3.up;

        configurableJoint.yMotion = configurableJoint.zMotion = configurableJoint.angularXMotion = configurableJoint.angularYMotion = configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;

        if (maxDistance == float.PositiveInfinity && minDistance == float.NegativeInfinity)
        {
            configurableJoint.xMotion = ConfigurableJointMotion.Free;
        }
        else
        {
            configurableJoint.xMotion = ConfigurableJointMotion.Limited;

            var linearLimit = configurableJoint.linearLimit;
            linearLimit.limit = (maxDistance - minDistance) * 0.5f;
            configurableJoint.linearLimit = linearLimit;
        }
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        if (!Application.IsPlaying(this))
        {
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(configurableJoint);
            }
        }
        else
#endif
        {
            Destroy(configurableJoint);
        }
    }
}
