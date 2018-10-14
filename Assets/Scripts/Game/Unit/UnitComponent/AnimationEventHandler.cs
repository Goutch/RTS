using JetBrains.Annotations;
using UnityEngine;

namespace UnitComponent
{
    public class AnimationEventHandler:MonoBehaviour
    {
        private AbilityCaster caster;
        private void Start()
        {
            caster = transform.parent.GetComponentInChildren<AbilityCaster>();
            
        }
        [UsedImplicitly]
        public void OnCast(int index)
        {
            caster.OnCast();
        }
    }
}