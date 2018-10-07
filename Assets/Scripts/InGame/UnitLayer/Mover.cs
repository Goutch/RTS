
using UnityEngine;

namespace DefaultNamespace
{
    public class Mover:MonoBehaviour
    {
        public void MoveToward(Vector3 target,float speed)
        {
            this.transform.position=Vector3.MoveTowards(this.transform.position,target,Time.deltaTime*speed);
        }

        public void RotateToward(Vector3 target)
        {
            float AngleRad =
                Mathf.Atan2(target.y - transform.position.y,
                    target.x - transform.position.x);
            //Angle en Degrés
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
            // Rotation
            this.transform.rotation = Quaternion.Euler(0, 0, AngleDeg - 90);
        }

        public void MoveAndRotateToward(Vector3 target,float moveSpeed)
        {
           // todo:optimize :only call rotate when target move
            MoveToward(target,moveSpeed);
            RotateToward(target);
        }
    }
}