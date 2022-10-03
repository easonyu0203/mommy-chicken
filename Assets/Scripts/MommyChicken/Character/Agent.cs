using System;
using System.Collections;
using UnityEngine;

namespace MommyChicken
{
    public class Agent : MonoBehaviour
    {
        private Transform _transform;
        private Rigidbody _rigidbody;
        private AgentManager _agentManager;
        private float _initY;

        private void Awake()
        {
            _transform = transform;
            _initY = _transform.position.y;
            _rigidbody = GetComponent<Rigidbody>();
            _agentManager = AgentManager.Instance;
        }

        private void Start()
        {
            StartCoroutine(BoidsBehavior());
            StartCoroutine(FixPhysicBug());
        }

        private IEnumerator BoidsBehavior()
        {
            var waitForFixedUpdate = new WaitForFixedUpdate();

            // initial to max speed
            _rigidbody.velocity = _transform.forward * _agentManager.MaxSpeed;

            Collider[] colliders = new Collider[_agentManager.MaxHitCollider];
            Collider[] walls = new Collider[_agentManager.MaxHitCollider];
            Collider[] obstacles = new Collider[_agentManager.MaxHitCollider];
            Collider[] agents = new Collider[_agentManager.MaxHitCollider];
            Collider queen = new Collider();
            while (true)
            {
                // get nearby information (other boid & wall)
                var (_, agentCnt, wallCnt, obstacleCnt, seeQueen) =
                    GetEnvInfo(ref colliders, ref walls, ref agents, ref obstacles, ref queen);

                // avoid obstacle
                Vector3 v0 = AvoidWallObstacle(walls, wallCnt, obstacles, obstacleCnt);
                Vector3 v1 = Cohesion(agents, agentCnt);
                Vector3 v2 = Separation(agents, agentCnt);
                Vector3 v3 = Alignment(agents, agentCnt);
                Vector3 v4 = UpToSpeed();
                Vector3 v5 = QueenEffect(seeQueen, queen);

                // apply force
                Vector3 allVec = (v0 + v1 + v2 + v3 + v4 + v5) * _agentManager.MaxAcceleration;
                _rigidbody.AddForce(allVec, ForceMode.Acceleration);

                // gizmos draw force
                DrawVelocity(v0, v1, v2, v3, v4);

                // look forward
                if (_rigidbody.velocity != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);

                yield return waitForFixedUpdate;
            }
        }

        private (int colliderCnt, int agentCnt, int wallCnt, int obstacleCnt, bool seeQueen) GetEnvInfo(
            ref Collider[] colliders,
            ref Collider[] walls, ref Collider[] agents, ref Collider[] obstacles, ref Collider queen)
        {
            int colliderCnt = 0, agentCnt = 0, wallCnt = 0, obstacleCnt = 0;
            bool seeQueen = false;
            colliderCnt = Physics.OverlapSphereNonAlloc(_transform.position, _agentManager.VisibleRadius, colliders,
                (1 << 6) + (1 << 7) + (1 << 8) + (1 << 9), QueryTriggerInteraction.Collide);
            for (int i = 0; i < colliderCnt; i++)
            {
                if (colliders[i].gameObject.layer == 6)
                {
                    walls[wallCnt++] = colliders[i];
                }
                else if (colliders[i].gameObject.layer == 7 && colliders[i].gameObject != gameObject)
                {
                    agents[agentCnt++] = colliders[i];
                }
                else if (colliders[i].gameObject.layer == 8)
                {
                    seeQueen = true;
                    queen = colliders[i];
                }
                else if (colliders[i].gameObject.layer == 9)
                {
                    obstacles[obstacleCnt++] = colliders[i];
                }
            }

            return (colliderCnt, agentCnt, wallCnt, obstacleCnt, seeQueen);
        }

        private Vector3 AvoidWallObstacle(Collider[] walls, int wallCnt, Collider[] obstacles, int obstacleCnt)
        {
            Vector3 v0 = Vector3.zero;
            if (wallCnt != 0)
            {
                // only apply force if will hit obstacle
                // if will not hit, just stay away from wall, treat it as avoid collision with agent
                var useFallOut =
                    Physics.Raycast(_transform.position, _rigidbody.velocity, _agentManager.VisibleRadius,
                        1 << 6)
                        ? _agentManager.AvoidWallFallOut
                        : _agentManager.DistWallFallOut;
                for (int i = 0; i < wallCnt; i++)
                {
                    Vector3 disVec = _transform.position - walls[i].transform.position;
                    float m = Vector3.Dot(walls[i].transform.forward, disVec);
                    v0 += walls[i].transform.forward *
                          useFallOut.Evaluate(m / _agentManager.VisibleRadius);
                }

                v0 /= wallCnt;
            }

            Vector3 v1 = Vector3.zero;
            if (obstacleCnt != 0)
            {
                for (int i = 0; i < obstacleCnt; i++)
                {
                    Vector3 disVec = obstacles[i].transform.position - _transform.position;
                    v1 += -disVec.normalized *
                          _agentManager.AvoidObstacleFallOut.Evaluate(disVec.magnitude / _agentManager.VisibleRadius);
                }

                v1 /= obstacleCnt;
            }


            return v0 + v1;
        }

        private Vector3 Cohesion(Collider[] agents, int agentCnt)
        {
            Vector3 v1 = Vector3.zero;
            if (agentCnt != 0)
            {
                for (int i = 0; i < agentCnt; i++)
                {
                    v1 += agents[i].transform.position;
                }

                v1 /= agentCnt;
                Vector3 toCenterVec = v1 - _transform.position;
                v1 = toCenterVec.normalized *
                     _agentManager.CohesionFallOut.Evaluate(toCenterVec.magnitude / _agentManager.VisibleRadius);
            }

            return v1;
        }

        private Vector3 Separation(Collider[] agents, int agentCnt)
        {
            Vector3 v2 = Vector3.zero;
            if (agentCnt != 0)
            {
                for (int i = 0; i < agentCnt; i++)
                {
                    Vector3 disVec = agents[i].transform.position - _transform.position;
                    disVec = disVec.normalized *
                             _agentManager.SeparationFallOut.Evaluate(disVec.magnitude /
                                                                      _agentManager.VisibleRadius);
                    v2 -= disVec;
                }

                v2 /= agentCnt;
            }

            return v2;
        }

        private Vector3 Alignment(Collider[] agents, int agentCnt)
        {
            Vector3 v3 = Vector3.zero;
            if (agentCnt != 0)
            {
                for (int i = 0; i < agentCnt; i++)
                {
                    v3 += agents[i].GetComponent<Rigidbody>().velocity;
                }

                v3 /= agentCnt;
                v3 -= _rigidbody.velocity;
                v3 = v3.normalized *
                     _agentManager.AlignmentFallout.Evaluate(v3.magnitude / (_agentManager.MaxSpeed));
            }

            return v3;
        }

        private Vector3 UpToSpeed()
        {
            return (_agentManager.MaxSpeed - _rigidbody.velocity.magnitude) * _rigidbody.velocity.normalized;
        }

        private Vector3 QueenEffect(bool seeQueen, Collider queen)
        {
            Vector3 v = Vector3.zero;
            if (seeQueen)
            {
                Vector3 disVec = queen.transform.position - _transform.position;
                v = disVec.normalized *
                    (_agentManager.MomChickEffectFallOut.Evaluate(disVec.magnitude / _agentManager.VisibleRadius) *
                     _agentManager.MomChickPullForce);
            }

            return v;
        }

        private IEnumerator FixPhysicBug()
        {
            var waitFix = new WaitForFixedUpdate();
            while (true)
            {
                // let agent stay at same y position
                var position = _transform.position;
                position = new Vector3(position.x, _initY, position.z);
                _transform.position = position;
                yield return waitFix;
            }
        }

        private Vector3[] _vs = null;

        private void DrawVelocity(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            _vs = new[] { v0, v1, v2, v3, v4 };
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if (_agentManager != null)
                Gizmos.DrawWireSphere(transform.position, _agentManager.VisibleRadius);
            if (_vs != null && _vs.Length == 5)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, transform.position + _vs[0]);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, transform.position + _vs[1]);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + _vs[2]);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + _vs[3]);
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, transform.position + _vs[4]);
            }
        }
    }
}