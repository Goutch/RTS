using UnityEngine;

namespace Goutch2D
{
    public class Mover:MonoBehaviour
    {
        public void RotateToward(Vector3 target)
        {
            transform.rotation = Quaternion.Euler(0, 0, Math2D.VectorToDegree(target)- 90);
        }
    }
}