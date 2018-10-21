using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class Sight : MonoBehaviour
    {
        private CircleCollider2D sightTrigger;
        private List<Transform> enemyUnitsInSight;
        private UnitController myController;
        public List<Transform> EnemyUnitsInSight
        {
            get
            {
                enemyUnitsInSight.RemoveAll(it => it == null);
                return enemyUnitsInSight;
            }
        }

        private List<Transform> allyUnitsInSight;

        public List<Transform> AlliesUnitsInSight
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
            myController=GetComponentInParent<UnitController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Unit")
            {
                if (other.GetComponent<UnitController>().TeamId == myController.TeamId)
                {
                    allyUnitsInSight.Add(other.transform);
                }
                else
                {
                    enemyUnitsInSight.Add(other.transform);
                }
            }
        }
    }
}