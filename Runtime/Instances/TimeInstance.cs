#nullable enable
using UnityEngine;

namespace UnityExtras
{
    [DisallowMultipleComponent]
    public class TimeInstance : Instance<TimeInstance>
    {
        [field: SerializeField] public float timeScale { get; set; } = 1f;

        public override void SetState()
        {
            Time.timeScale = timeScale;
        }
    }
}
