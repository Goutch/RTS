using UnityEngine;

namespace InGame.World.Ressources
{
    public class RessourceDrop:MonoBehaviour
    {
        private int value;

        private void Init(Vector2 force,int value)
        {
            GetComponent<Rigidbody2D>().AddForce(force,ForceMode2D.Impulse);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Unit")
            {
                
            }
        }
    }
}