using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "NewFaction", menuName = "Faction")]
    public class UnitFactionData:ScriptableObject
    {
        [SerializeField] private string name;
        [SerializeField] private UnitData[] startUnits;
        [SerializeField] private Vector2[] startUnitsOffSetPosition;
        [SerializeField] private List<UnitData> spawnableUnits;
        
        public string Name => name;

        public UnitData[] StartUnits => startUnits;

        public Vector2[] StartUnitsOffSetPosition => startUnitsOffSetPosition;

        public List<UnitData> SpawnableUnits => spawnableUnits;
    }
}