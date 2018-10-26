using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace UnitComponent
{
    public delegate void MoverEventHandler(bool moving);

    public class Mover : MonoBehaviour
    {
        public event MoverEventHandler OnMovingChange;
        [NonSerialized] public Transform transformToRotate;
        [SerializeField] private float Precision = 0.8f;
        private Rigidbody2D rigidbody2D;
        private Pathfinder pathFinder;
        private Transform transformToMove;
        private UnitAnimationController animsController;
        private Vector2 leaderOffSet;
        private List<Vector3> path;
        private Vector2 currentDir;
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
            pathFinder = WorldMap.INSTANCE.pathFinder;
            rigidbody2D = transformToMove.GetComponent<Rigidbody2D>();
        }
        public void MoveToward(Vector3 target, float speed)
        {
            transformToMove.position =
                Vector3.MoveTowards(this.transform.position, target, Time.fixedDeltaTime * speed);
        }

        private void MoveFoward(float speed)
        {
            rigidbody2D.velocity = currentDir*speed;
           // rigidbody2D.MovePosition(transform.position+(Vector3)currentDir*speed);
        }

        public void CreateNewPath(Vector2 destination)
        {
            if (path == null)
                pathFinder.FindPath(transform.position, destination);
            path.Clear();
            path = pathFinder.FindPath(transform.position, destination);
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
            rigidbody2D.velocity = Vector2.zero;
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
            float AngleDeg =  Mathf.Rad2Deg * AngleRad;
            // Rotation
            currentDir = new Vector2(Mathf.Cos(AngleRad), Mathf.Sin(AngleRad));
            transformToRotate.rotation = Quaternion.Euler(0, 0, AngleDeg - 90);
        }

        public void MoveAndRotateToward(Vector3 destination, float moveSpeed)
        {
            if (!Stopped)
            {
                IsMoving = true;
                if (path == null || path.Count==0)
                    CreateNewPath(destination);
                if (path.Count>0 && Vector2.Distance(transform.position, path[0]) <= Precision)
                {
                    path.RemoveAt(0);
                    
                    //reached destination
                    if (path.Count==0)
                    {
                        IsMoving = false;
                        rigidbody2D.velocity = Vector2.zero;
                        return;
                    }
                    
                }

                RotateToward(path[0]);
                MoveFoward(moveSpeed);
            }
        }

        private void OnDrawGizmos()
        {
            for (int i = 1; i < path.Count; i++)
            {
                Gizmos.DrawLine((Vector3) path[i - 1], path[i]);
            }
        }
    }
}