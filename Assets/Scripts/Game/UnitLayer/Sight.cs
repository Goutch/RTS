using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Sight : MonoBehaviour
    {
        private CircleCollider2D sightTrigger;
        private List<Transform> enemyUnitsInSight;

        public List<Transform> EnemyUnitsInSight
        {
            get
            {
                enemyUnitsInSight.RemoveAll(it => it == null);
                return enemyUnitsInSight;
            }
        }

        private List<Transform> allyUnitsInSight;

        public List<Transform> AnnemyUnitsInSight
        {
            get
            {
                allyUnitsInSight.RemoveAll(it => it == null);
                return allyUnitsInSight;
            }
        }

        private void Start()
        {
            allyUnitsInSight = new List<Transform>();
            enemyUnitsInSight = new List<Transform>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {

            if (other.tag == "Unit")
            {
                if (other.GetComponent<UnitController>().TeamId == this.GetComponent<UnitController>().TeamId)
                {
                    allyUnitsInSight.Add(other.transform);
                }
            }
        }
    }
}