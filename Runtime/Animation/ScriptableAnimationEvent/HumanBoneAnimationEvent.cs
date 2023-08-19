#nullable enable
using UnityEngine;

namespace UnityExtras
{
    [CreateAssetMenu(fileName = nameof(HumanBoneAnimationEvent), menuName = nameof(ScriptableAnimationEvent) + "/" + nameof(HumanBoneAnimationEvent))]
    public class HumanBoneAnimationEvent : ScriptableAnimationEvent
    {
        [field: SerializeField] public HumanBodyBones bone { get; set; }
        [field: SerializeField] public bool useRootBasedDirection { get; set; }
        [field: SerializeField] public Direction rayDirection { get; set; } = Vector3.down;
        [field: SerializeField, Min(0f)] public float maxDistance { get; set; } = 1f;
        [field: SerializeField] public LayerMask layerMask { get; set; } = ~0;
        [field: SerializeField] public QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore;

        public override void Play(ScriptableAnimationEventListener listener, AnimationEvent animationEvent)
        {
            Texture? hitTexture = null;

            // Perform a raycast from the Bone Transform.
            var boneTransform = listener.animator.GetBoneTransform(bone);
            if (Physics.Raycast(boneTransform.position, (useRootBasedDirection ? listener.transform.rotation : boneTransform.rotation) * rayDirection.value, out var hit, maxDistance, layerMask, queryTriggerInteraction))
            {
                Renderer renderer;
                if (hit.transform.TryGetComponent<Terrain>(out var terrain))
                {
                    // Find hit texture on terrain.
                    var alphamaps = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);

                    var terrainPoint = terrain.transform.InverseTransformPoint(boneTransform.position);
                    var terrainCoord = new Vector2Int
                    {
                        x = (int)(terrainPoint.x / terrain.terrainData.size.x * terrain.terrainData.alphamapWidth),
                        y = (int)(terrainPoint.z / terrain.terrainData.size.z * terrain.terrainData.alphamapHeight)
                    };

                    var highestOpacity = 0f;
                    var highestAlphamapIndex = -1;
                    for (int i = 0; i < terrain.terrainData.alphamapTextureCount; i++)
                    {
                        var opacity = alphamaps[terrainCoord.y, terrainCoord.x, i];
                        if (opacity > highestOpacity)
                        {
                            highestOpacity = opacity;
                            highestAlphamapIndex = i;
                        }
                    }
                    if (highestAlphamapIndex != -1)
                    {
                        hitTexture = terrain.terrainData.GetAlphamapTexture(highestAlphamapIndex);
                    }
                }
                else if ((renderer = hit.transform.GetComponentInChildren<Renderer>()) != null
                    && renderer.sharedMaterial != null)
                {
                    // Find hit texture on renderer.
                    hitTexture = renderer.sharedMaterial.mainTexture;
                }
            }

            // Send the collected data to children requiring it.
            foreach (var childEvent in this)
            {
                if (childEvent is IDataReceiver<RaycastHit> raycastHitReceiver)
                {
                    raycastHitReceiver.value = hit;
                }
                if (childEvent is IDataReceiver<Vector3> hitPointReceiver)
                {
                    hitPointReceiver.value = hit.point;
                }
                if (childEvent is IDataReceiver<Texture?> textureReceiver)
                {
                    textureReceiver.value = hitTexture;
                }
            }

            // Continue playing the event.
            base.Play(listener, animationEvent);
        }
    }
}