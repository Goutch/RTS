using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Abilities;
using DefaultNamespace;
using JetBrains.Annotations;
using Player;
using UnityEngine;
using UnityEngine.Networking;

namespace UnitComponent
{
    public class AbilityCaster : NetworkBehaviour
    {
        [SerializeField] private GameObject projectilePrefabObject;
        private UnitData data;
        private bool[] AbilitiesAvalable;
        private bool WaitingForDelay;
        private int currentAbilityIndex;
        private AbilityTargetType targetType;
        private Vector2 currentTargetPosition;
        private Transform currentTarget;
        private UnitAnimationController animaitonController;

        public int CurrentAbilityIndex => currentAbilityIndex;
        public Vector2 CurrentTargetPosition => currentTargetPosition;
        public Transform CurrentTarget => currentTarget;

        public AbilityTargetType TargetType => targetType;

        private UnitAI AI;
        private Spawner spawner;
        private int teamId;


        public void Init(UnitData data, UnitAI AI, Spawner spawner)
        {
            this.AI = AI;
            this.data = data;
            this.spawner = spawner;
            teamId = spawner.GetComponent<NetworkPlayerConnection>().TeamId;
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
            if (AbilitiesAvalable[Index] && data.Abilities[Index].CanCast(this))
            {
                AbilitiesAvalable[Index] = false;

                if (AI.CurrentCommand.TargetTransform != null)
                {
                    currentTarget = AI.CurrentCommand.TargetTransform;
                    currentTargetPosition = currentTarget.position;
                }
                else
                {
                    currentTargetPosition = AI.CurrentCommand.Target;
                }

                currentAbilityIndex = Index;
                data.Abilities[Index].StartAbility(this);
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

        public void SpawnProjectile(Projectile projectile)
        {
            if (hasAuthority)
                CmdShootProjectile(data.Abilities.IndexOf(projectile));
        }

        public void SpawnUnit(UnitData data, Vector2 position)
        {
            spawner.SpawnUnit(data, position);
        }

        [Command]
        private void CmdShootProjectile(int abilityIndex)
        {
            spawner.SpawnProjectile((Projectile) data.Abilities[abilityIndex], transform.position,
                CurrentTargetPosition, teamId);
        }

        [ClientRpc]
        private void RpcShootProjectile(int abilityIndex)
        {
            ProjectileController projectile =
                Instantiate(projectilePrefabObject, transform.position, transform.rotation)
                    .GetComponent<ProjectileController>();
            projectile.Init((Projectile) data.Abilities[abilityIndex], CurrentTargetPosition, teamId);
        }
    }
}