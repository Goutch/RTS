using AbilitySystem.Abilities;
using UnitComponent;
using UnityEngine;
using UnityEngine.Networking;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace AbilitySystem
{
    public class ProjectileController : NetworkBehaviour
    {
        private Projectile projectile;
        private Vector2 targetPosition;
        private Transform target;
        private bool inited=false;
        private int teamId;

        public void Init(Projectile projectilePrefab, Vector2 targetPosition, int TeamId)
        {
            
            this.teamId = TeamId;
            this.targetPosition = targetPosition;
            projectile = projectilePrefab;
            this.GetComponent<SpriteRenderer>().sprite = projectilePrefab.Sprite;
            GetComponent<CircleCollider2D>().radius = projectilePrefab.CollisionRadius;
            RotateToward(targetPosition);
            Destroy(this.gameObject,projectilePrefab.LifeSpan);
            inited = true;
        }

        private void OnDestroy()
        {
            if(isServer)
                NetworkServer.Destroy(this.gameObject);
        }

        private void Update()
        {
            if (inited)
            {
                transform.Translate(Vector3.up * projectile.Speed * Time.deltaTime);
            }
        }

        public void RotateToward(Vector3 target)
        {
            float AngleRad =
                Mathf.Atan2(target.y - transform.position.y,
                    target.x - transform.position.x);
            //Angle en Degrés
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
            // Rotation
            this.transform.rotation = Quaternion.Euler(0, 0, AngleDeg - 90);
        }
        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(inited)
            if (other.GetComponent<UnitController>().TeamId != this.teamId)
            {
                projectile.OnCollision(other);
                NetworkServer.Destroy(this.gameObject);
            }
        }
    }
}