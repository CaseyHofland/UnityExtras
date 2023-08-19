using System;
using UnityEngine;
using UnityEngine.Playables;

namespace UnityExtras.Playables
{
    public class LightControlAsset : PlayableAsset
    {
        public LightControlBehaviour template;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) => ScriptPlayable<LightControlBehaviour>.Create(graph, template);
    }

    [Serializable]
    public class LightControlBehaviour : PlayableBehaviour
    {
        public Color color = Color.white;
        public float intensity = 1f;

        // Temperature has been omitted as these are permanent filters based on the light type (candlelight, lightbulbs, sunlight, etc.)
    }

    public class LightControlMixerBehaviour : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var light = (Light)playerData;
            if (light == null)
            {
                return;
            }

            light.color = default;
            light.intensity = default;
            for (int port = playable.GetInputCount() - 1; port >= 0; port--)
            {
                var input = (ScriptPlayable<LightControlBehaviour>)playable.GetInput(port);
                var behaviour = input.GetBehaviour();
                var weight = playable.GetInputWeight(port);

                light.color += behaviour.color * weight;
                light.intensity += behaviour.intensity * weight;
            }
        }
    }
}