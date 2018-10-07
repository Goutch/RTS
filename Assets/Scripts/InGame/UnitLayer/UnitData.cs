
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Abilities;

using UnityEngine;


    [CreateAssetMenu(fileName = "Info", menuName = "Info/Unit")]
    public class UnitData : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private UnitAI unitAI;
        [SerializeField] private RuntimeAnimatorController animatorController;
        [SerializeField] private Color color;
        [SerializeField] private string name;
        [Header("Stats")]
        [SerializeField] private float baseHealth;
        [SerializeField] private float baseArmor;
        [SerializeField] private float baseSpeed;
        [SerializeField] private float baseSizeInPixel;
        [SerializeField] private float baseSightRangeInPixel;
        [SerializeField] private float baseAttackRangeInPixel;
        [Tooltip("The abilities the unit will be able to cast, note that the first one is the unit basic Attack")]
        [SerializeField] private List<Ability> abilities; 
        [SerializeField] private float baseMass;

        public string Name => name;

        public Sprite Sprite => sprite;

        public List<Ability>  Abilities=>abilities;

        public UnitAI AI=> unitAI;

        public RuntimeAnimatorController AnimsController=>animatorController;
   
        public Stat health;
        public Stat armor;
        public Stat speed;
        public Stat damage;
        public Stat size;
        public Stat sightRange;
        public Stat attackRange;
        public Stat mass;
        
        public void Init()
        {
            health = new Stat(baseHealth);
            armor = new Stat(baseArmor);
            speed= new Stat(baseSpeed);
            size= new Stat( baseSizeInPixel);
            sightRange= new Stat(baseSightRangeInPixel);
            attackRange= new Stat(baseAttackRangeInPixel);
            mass= new Stat(baseMass);
        }
    }
