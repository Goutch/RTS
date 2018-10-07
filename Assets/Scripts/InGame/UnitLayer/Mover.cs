
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace DefaultNamespace
{
    public class Mover:MonoBehaviour
    {
        public Transform transformToRotate;
        private Transform transformToMove;
        public void MoveToward(Vector3 target,float speed)
        {
            transformToMove.position=Vector3.MoveTowards(this.transform.position,target,Time.fixedDeltaTime*speed);
        }

        public void RotateToward(Vector3 target)
        {
            float AngleRad =
                Mathf.Atan2(target.y - transform.position.y,
                    target.x - transform.position.x);
            //Angle en Degrés
            float AngleDeg = (180 / Mathf.PI) * AngleRad;
            // Rotation
            transformToRotate.rotation = Quaternion.Euler(0, 0, AngleDeg - 90);
        }

        public void MoveAndRotateToward(Vector3 target,float moveSpeed)
        {
           // todo:optimize :only call rotate when target move
            MoveToward(target,moveSpeed);
            RotateToward(target);
        }

        public void Init(Transform move, Transform rotate)
        {
            transformToMove = move;
            transformToRotate = rotate;
        }
    }
}