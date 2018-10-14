using UnityEditor;
using UnityEngine;

namespace AbilitySystem
{
    public class ProjectileData:ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private float speed;
    }
}