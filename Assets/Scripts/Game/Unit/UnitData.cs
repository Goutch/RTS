
using System.Collections.Generic;
using AbilitySystem;
using DefaultNamespace;

using UnityEngine;


    [CreateAssetMenu(fileName = "UnitData", menuName = "Data/Unit")]
    public class UnitData : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private UnitAI unitAI;
        [SerializeField] private RuntimeAnimatorController animatorController;
        [SerializeField] private string name;
        [Header("Stats")]
        [SerializeField] private float baseHealth;
        [SerializeField] private float baseArmor;
        [SerializeField] private float baseSpeed;
        [SerializeField] private float baseSizeInPixel;
        [SerializeField] private float baseSightRangeInTile;   
        [SerializeField] private float baseMass;
        [SerializeField] private List<Ability> abilities; 


        public string Name => name;

        public Sprite Sprite => sprite;

        public List<Ability>  Abilities=>abilities;

        public UnitAI AI=> unitAI;

        public RuntimeAnimatorController AnimsController=>animatorController;
   
        public Stat health;
        public Stat speed;
        public Stat size;
        public float sightRange=>baseSightRangeInTile;
        public Stat mass;
        
        public void Init()
        {
            health = new Stat(baseHealth);
            speed= new Stat(baseSpeed);
            size= new Stat( baseSizeInPixel);
            mass= new Stat(baseMass);
        }
    }
