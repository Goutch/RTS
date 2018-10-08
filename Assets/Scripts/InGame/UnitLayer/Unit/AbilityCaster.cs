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
        private bool castingLock;

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

            animaitonController = GetComponent<UnitAnimationController>();
        }

        [UsedImplicitly]
        public void OnCastFinish()
        {
            castingLock = false;
        }

        public bool CastAbility(int Index)
        {
            if (AbilitiesAvalable[Index])
            {
                currentTarget = AI.CurrentCommand.TargetTransform;
                currentTargetPosition = AI.CurrentCommand.Target;
                currentAbilityIndex = Index;
                data.Abilities[Index].Init(this);
                StartCoroutine(StartAbilityRoutine(Index));
                return true;
            }

            return false;
        }

        private IEnumerator StartAbilityRoutine(int Index)
        {
            AbilitiesAvalable[Index] = false;
            castingLock = data.Abilities[Index].CastLock;
            animaitonController.OnCastAbility();
            while (castingLock)
            {
                yield return 0;
            }

            data.Abilities[Index].Cast();
            yield return new WaitForSeconds(data.Abilities[Index].Cooldown);

            AbilitiesAvalable[Index] = true;
        }
    }
}