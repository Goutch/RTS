using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace DefaultNamespace
{
    public class UnitAnimationController:MonoBehaviour
    {
        private Animator animator;
        private AbilityCaster caster;
        private Mover mover;
        public void Init(Animator animator, AbilityCaster caster,Mover mover)
        {
            this.animator = animator;
            this.caster = caster;
            mover.OnMovingChange += OnMovingStateChange;
        }

        public void OnCastAbility()
        {
            animator.SetTrigger("Ability" + caster.CurrentAbilityIndex);
        }

        public void OnMovingStateChange(bool moving)
        {
            animator.SetBool("IsMoving",moving);
        }
        
        
    }
}