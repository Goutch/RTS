using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DefaultNamespace
{
    public class AbilityCaster : MonoBehaviour
    {
        private UnitData data;
        private bool[] AbilitiesAvalable;
        private bool WaitingForDelay;

        private int currentAbilityIndex;

        public int CurrentAbilityIndex => currentAbilityIndex;

        private Vector2 currentTargetPosition;

        public Vector2 CurrentTargetPosition => currentTargetPosition;

        private UnitAnimationController animaitonController;

        private Transform currentTarget;

        public Transform CurrentTarget => currentTarget;

        private UnitAI AI;


        public void Init(UnitData data, UnitAI AI)
        {
            this.AI = AI;
            this.data = data;
            AbilitiesAvalable = new bool[this.data.Abilities.Count];
            for (int i = 0; i < AbilitiesAvalable.Length; i++)
            {
                AbilitiesAvalable[i] = true;
            }

            animaitonController = GetComponentInParent<UnitAnimationController>();
        }

        public void OnCast()
        {
            WaitingForDelay = false;
        }

        public bool CastAbility(int Index)
        {
            if (AbilitiesAvalable[Index])
            {
                AbilitiesAvalable[Index] = false;
                currentTarget = AI.CurrentCommand.TargetTransform;
                currentTargetPosition = AI.CurrentCommand.Target;
                currentAbilityIndex = Index;
                data.Abilities[Index].Init(this);
                if (data.Abilities[Index].HasCastDelay)
                {
                    StartCoroutine(WaitAbilityDelayRoutine(Index));
                }
                else
                {
                    animaitonController.OnCastAbility();
                    StartCoroutine(CastAbilityRoutine(Index));
                }
                return true;
            }

            return false;
        }

        private IEnumerator WaitAbilityDelayRoutine(int index)
        {
            WaitingForDelay = true;
            animaitonController.OnCastAbility();
            while (WaitingForDelay)
            {
                yield return 0;
            }

            StartCoroutine(CastAbilityRoutine(index));
        }

        private IEnumerator CastAbilityRoutine(int Index)
        {
            data.Abilities[Index].Cast();
            yield return new WaitForSeconds(data.Abilities[Index].Cooldown);
            AbilitiesAvalable[Index] = true;
        }
    }
}