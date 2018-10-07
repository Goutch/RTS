using UnityEngine;

namespace DefaultNamespace
{
    public class AnimationEventHandler:MonoBehaviour
    {
        private UnitController controller;
        private void Start()
        {
            controller = GetComponentInParent<UnitController>();
        }
        public void OnCastFinish()
        {
            controller.OnCastFinish();
        }
    }
}