using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public delegate void HealthEventHandler();

    public class Health : NetworkBehaviour
    {
        [SerializeField] private RectTransform healthFillAmount;
        private Stat health;
        public event HealthEventHandler OnDeath;
        public event HealthEventHandler OnHealthChange;

        public void Init(UnitData data)
        {
            health = data.health;
        }
        [Server]
        public void Damage(float amount)
        {
            CmdChangeHealth(health.Value - amount);
        }

        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Ability")
            {
            }
        }

        [Command]
        private void CmdChangeHealth(float newHealth)
        {
            if (newHealth < 0)
                CmdKillUnit();
            else
            {
                RpcChangeHealth(newHealth);
            }
        }


        [ClientRpc]
        private void RpcChangeHealth(float newHealth)
        {
            OnHealthChange?.Invoke();
            health.Value = newHealth;
            healthFillAmount.localScale =
                Vector3.one - Vector3.right + (Vector3.right * (health.Value / health.MaxValue));
        }

        [Command]
        private void CmdKillUnit()
        {
            RpcKillUnit();
        }

        [ClientRpc]
        private void RpcKillUnit()
        {
            OnDeath?.Invoke();
            if (hasAuthority)
                NetworkServer.Destroy(this.gameObject);
        }
    }
}