using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityExtras.Playables;

namespace UnityExtras.Timeline
{
    [TrackClipType(typeof(LightControlAsset))]
    [TrackBindingType(typeof(Light))]
    public class LightControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) => ScriptPlayable<LightControlMixerBehaviour>.Create(graph, inputCount);
    }
}
