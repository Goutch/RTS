using UnityEngine;

namespace DefaultNamespace
{
    public class AnimationEventHandler:MonoBehaviour
    {
        private AbilityCaster caster;
        private void Start()
        {
            caster = GetComponentInParent<AbilityCaster>();
        }
        public void OnCastFinish()
        {
            caster.OnCastFinish();
        }
    }
}