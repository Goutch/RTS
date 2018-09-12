using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "NewRace", menuName = "Race")]
    public class UnitRaceInfo:ScriptableObject
    {
        [SerializeField] private string name;
        [SerializeField] private UnitInfo[] startUnits;
        [SerializeField] private Vector2[] startUnitsOffSetPosition;
        [SerializeField] private List<UnitInfo> spawnableUnits;
        

        public string Name => name;

        public UnitInfo[] StartUnits => startUnits;

        public Vector2[] StartUnitsOffSetPosition => startUnitsOffSetPosition;

        public List<UnitInfo> SpawnableUnits => spawnableUnits;
    }
}