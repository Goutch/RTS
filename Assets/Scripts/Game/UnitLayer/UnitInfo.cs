using System;
using System.Collections.Generic;
using DefaultNamespace.Abilities;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Info", menuName = "Info/Unit")]
    public class UnitInfo : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private UnitAI unitAI;
        [SerializeField] private Color color;
        [SerializeField] private string name;
        [Header("Stats")]
        [SerializeField] private float baseHealth;
        [SerializeField] private float baseArmor;
        [SerializeField] private float baseSpeed;
        [SerializeField] private float baseDammage;
        [SerializeField] private float baseSizeInPixel;
        [SerializeField] private float baseSightRangeInPixel;
        [SerializeField] private float baseAttackRangeInPixel;
        [SerializeField] private Ability basicAttack;
        [SerializeField] private List<Ability> abilities; 
        [SerializeField] private float baseMass;

        public string Name => name;

        public Sprite Sprite => sprite;

        public List<Ability>  Abilities=>abilities;

        public UnitAI AI=> unitAI;

        public Color Color => color;

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
            damage = new Stat(baseDammage);
            size= new Stat( baseSizeInPixel);
            sightRange= new Stat(baseSightRangeInPixel);
            attackRange= new Stat(baseAttackRangeInPixel);
            mass= new Stat(baseMass);
            abilities.Insert(0,basicAttack);
        }
    }
}