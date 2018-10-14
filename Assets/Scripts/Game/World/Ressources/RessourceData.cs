using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace InGame.World.Ressources
{
    [CreateAssetMenu(menuName = "RessourceData")]
    public class RessourceData : ScriptableObject
    {
        [SerializeField] private RessourceType type;
        public RessourceType Type => type;
        [SerializeField] private int value;
        private Vector3Int position;
        private RessourceTileMap ressourceTileMap;

        public void Init(RessourceTileMap ressourceTileMap)
        {
            this.ressourceTileMap = ressourceTileMap;
        }
        public int Harvest(int ammount)
        {
            int over = 0;
            if (value - ammount <= 0)
            {
                over = Mathf.Abs(value - ammount); 
                ressourceTileMap.OnRessourceDepleded(position);
            }
            return ammount - over;
        }
    }
}