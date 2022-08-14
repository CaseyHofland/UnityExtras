#nullable enable
using UnityEngine;

namespace UnityExtras
{
    /// <summary>An interace providing serialization hooks for components that want to author values from other components.</summary>
    /// <remarks>
    /// <example>
    /// The proper way to implement this interface through a MonoBehaviour is as follows:
    /// <code>
    /// [<see cref="SerializeField"/>][<see cref="HideInInspector"/>] <see langword="private"/> <see cref="RequiredComponent{T}">RequiredComponent</see>&lt;<see cref="CapsuleCollider2D"/>&gt; _capsuleCollider2D;
    /// <br/><see langword="public"/> <see cref="CapsuleCollider2D"/> capsuleCollider2D => _capsuleCollider2D.<see cref="RequiredComponent{T}.GetComponent(GameObject)">GetComponent</see>(gameObject);
    /// 
    /// [<see cref="SerializeField"/>] <see langword="private"/> <see cref="Vector2"/> _offset;
    /// <br/><see langword="public"/> <see cref="Vector2"/> offset
    /// {
    ///     <see langword="get"/> => capsuleCollider2D.offset;
    ///     <see langword="set"/> => capsuleCollider2D.offset = _offset = value;
    /// }
    /// 
    /// [<see langword="field"/>: <see cref="SerializeField"/>][<see langword="field"/>: <see cref="HideInInspector"/>] <see langword="bool"/> <see cref="IAuthor"/>.isDeserializing { <see langword="get"/>; <see langword="set"/>; }
    /// <br/><see langword="void"/> <see cref="Serialize()"/> => offset = offset;
    /// <br/><see langword="void"/> <see cref="Deserialize()"/> => offset = _offset;
    /// <br/><see langword="void"/> <see cref="DestroyAuthor()"/> => <see cref="ExtraObject.DestroyImmediateSafe">ExtraObject.DestroyImmediateSafe</see>(_capsuleCollider2D);
    /// <br/>
    /// <br/><see langword="void"/> <see href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Reset.html">Reset</see>() => ((<see cref="IAuthor"/>)<see langword="this"/>).<see cref="ResetAuthor">ResetAuthor</see>();
    /// <br/><see langword="void"/> <see href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDestroy.html">OnDestroy</see>() => ((<see cref="IAuthor"/>)<see langword="this"/>).<see cref="DestroyAuthor">DestroyAuthor</see>();
    /// </code>
    /// </example>
    /// </remarks>
    public interface IAuthor : ISerializationCallbackReceiver
    {
        public GameObject gameObject { get; }
        protected bool isDeserializing { get; set; }

        int GetInstanceID();

        void ISerializationCallbackReceiver.OnBeforeSerialize() => OnBeforeSerialize();
        new void OnBeforeSerialize()
        {
            if (gameObject && !isDeserializing)
            {
                Serialize();
            }

#if UNITY_EDITOR
            var instance = (Component)UnityEditor.EditorUtility.InstanceIDToObject(GetInstanceID());
            if (!Application.IsPlaying(instance))
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (!instance)
                    {
                        DestroyAuthor();
                    }
                };
            }
#endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() => OnAfterDeserialize();
        new void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            isDeserializing = true;
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (gameObject)
                {
                    Deserialize();
                }

                isDeserializing = false;
            };
#endif
        }

        void Serialize();
        void Deserialize();
        void ResetAuthor()
        {
            Deserialize();

            if (!isDeserializing)
            {
                Serialize();
            }
        }
        void DestroyAuthor();
    }
}