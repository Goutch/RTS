using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace DefaultNamespace
{
    public delegate void MoverEventHandler(bool moving);

    public class Mover : MonoBehaviour
    {
        public event MoverEventHandler OnMovingChange;
        [NonSerialized]public Transform transformToRotate;
        private Transform transformToMove;
        private UnitAnimationController animsController;
        private List<Vector3> path;
        private bool isMoving = false;
        private bool Stopped = false;

        public bool IsMoving
        {
            get { return isMoving; }
            private set
            {
                if (isMoving != value)
                {
                    isMoving = value;
                    OnMovingChange?.Invoke(value);
                }
            }
        }

        public void Init(Transform move, Transform rotate)
        {
            transformToMove = move;
            transformToRotate = rotate;
            path = new List<Vector3>();
            animsController = GetComponent<UnitAnimationController>();

        }

        public void MoveToward(Vector3 target, float speed)
        {
            transformToMove.position =
                Vector3.MoveTowards(this.transform.position, target, Time.fixedDeltaTime * speed);
        }

        public void CreateNewPath(Vector2 destination)
        {
            path.Clear();
            path = Pathfinder.INSTANCE.FindPath(transform.position, destination);
            //path was impossible

            if (path == null)
            {
                return;
            }

            path.Add(destination);
            IsMoving = true;
        }

        public void ClearPath()
        {
            path.Clear();
            IsMoving = false;
        }

        public void Stop()
        {
            Stopped = true;
            IsMoving = false;
        }

        public void UnStop()
        {
            Stopped = false;
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

        public void MoveAndRotateToward(Vector3 destination, float moveSpeed)
        {
            if (!Stopped)
            {
                IsMoving = true;
                if (path == null || !path.Any())
                    CreateNewPath(destination);
                if (path.Any() && Vector2.Distance(transform.position, path.First()) < 0.2)
                {
                    path.RemoveAt(0);
                    //reached destination
                    if (!path.Any())
                    {
                        IsMoving = false;
                        return;
                    }
                }

                MoveToward(path[0], moveSpeed);
                RotateToward(path[0]);
            }
        }
    }
}