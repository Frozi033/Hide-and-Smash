using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyLogic.FieldOfView
{
    public class FieldOfView : MonoBehaviour
    {
        [field: SerializeField, Range(0, 180)] public float ViewAngle { get; set; }
        [field: SerializeField, Range(0, 20)] public float ViewRadius { get; set; }

        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask _obstacleMask;

        [SerializeField] private MeshFilter _viewMeshFilter;

        [SerializeField] private Material _scannerAlarmMat;

        [SerializeField] private float _edgeResolveInterations = 4f;
        [SerializeField] private float _meshResolution = 10f;
        [SerializeField] private float _edgeDistanceTreshold = 0.5f;
        
        private Mesh _viewMesh;
        private MeshRenderer _meshRenderer;
        private Material _scannerDefaultMat;
        
        private Enemy _mainControlls;

        private Transform _meshTransform;

        private void Start()
        {
            _mainControlls = GetComponentInParent<Enemy>();
            
            InitViewMesh();

            StartCoroutine(FOVRoutine());
        }

        private void LateUpdate()
        {
            DrawFieldOfView();
        }

        private IEnumerator FOVRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(0.2f);

            while (true)
            {
                yield return wait;
                FieldOfViewCheck();
            }
        }

        private void FieldOfViewCheck()
        {
            _meshRenderer.material = _scannerDefaultMat;
            
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, ViewRadius, targetMask);

            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < ViewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                    {
                        _meshRenderer.material = _scannerAlarmMat;

                        _mainControlls.PlayerDetected(target);
                    }
                    else
                    {
                        _meshRenderer.material = _scannerDefaultMat;
                    }
                }
                else
                {
                    _meshRenderer.material = _scannerDefaultMat;
                }
            }
            else if (_meshRenderer.material == _scannerAlarmMat)
            {
                _meshRenderer.material = _scannerDefaultMat;
            }
        }


        private void InitViewMesh()
        {
            _viewMesh = new Mesh
            {
                name = "View Mesh"
            };
            
            _viewMeshFilter.mesh = _viewMesh;
            _meshTransform = _viewMeshFilter.transform;
            
            _meshRenderer = _viewMeshFilter.GetComponent<MeshRenderer>();
            
            _scannerDefaultMat = _meshRenderer.material;
        }

        private void DrawFieldOfView()
        {
            var stepCount = Mathf.RoundToInt(ViewAngle * _meshResolution);
            var stepAngleSize = ViewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            var oldViewCast = new ViewCastInfo();
            for (var i = 0; i <= stepCount; i++)
            {
                var angle = _meshTransform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
                var newViewCastInfo = ViewCast(angle);

                if (i > 0)
                {
                    var edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.Distance - newViewCastInfo.Distance) >
                                                        _edgeDistanceTreshold;
                    if (oldViewCast.Hit != newViewCastInfo.Hit ||
                        (oldViewCast.Hit && newViewCastInfo.Hit && edgeDistanceThresholdExceeded))

                    {
                        var edge = FindEdge(oldViewCast, newViewCastInfo);
                        if (edge.PointA != Vector3.zero)
                            viewPoints.Add(edge.PointA);


                        if (edge.PointB != Vector3.zero)
                            viewPoints.Add(edge.PointB);

                    }
                }

                viewPoints.Add(newViewCastInfo.Point);
                oldViewCast = newViewCastInfo;
            }

            var vertexCount = viewPoints.Count + 1;
            var vertices = new Vector3[vertexCount];
            var triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            for (var i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = _meshTransform.InverseTransformPoint(viewPoints[i]);
                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;

                }
            }

            _viewMesh.Clear();
            _viewMesh.vertices = vertices;
            _viewMesh.triangles = triangles;
            _viewMesh.RecalculateNormals();
        }

        private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            var minAngle = minViewCast.Angle;
            var maxAngle = maxViewCast.Angle;
            var minPoint = Vector3.zero;
            var maxPoint = Vector3.zero;
            for (var i = 0; i < _edgeResolveInterations; i++)
            {
                var angle = (minAngle + maxAngle) / 2;
                var newViewCast = ViewCast(angle);

                var edgeDistanceThresholdExceeded =
                    Mathf.Abs(minViewCast.Distance - maxViewCast.Distance) > _edgeDistanceTreshold;
                if (newViewCast.Hit == minViewCast.Hit && !edgeDistanceThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.Point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.Point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }

        private Vector3 GetDirectionFromAngle(float angleInDegrees, bool anglesIsGlobal)
        {
            if (!anglesIsGlobal)
                angleInDegrees += transform.eulerAngles.y;

            var vector = new Vector3(Mathf
                .Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf
                .Cos(angleInDegrees * Mathf.Deg2Rad));

            return vector;
        }

        private ViewCastInfo ViewCast(float globalAngle)
        {
            var direction = GetDirectionFromAngle(globalAngle, true);
            if (Physics.Raycast(_meshTransform.position, direction, out var hit, ViewRadius, _obstacleMask))
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);

            return new ViewCastInfo(false, _meshTransform.position + direction * ViewRadius, ViewRadius, globalAngle);
        }

        private struct ViewCastInfo
        {
            public bool Hit { get; }
            public Vector3 Point { get; }
            public float Distance { get; }
            public float Angle { get; }

            public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
            {
                Hit = hit;
                Point = point;
                Distance = distance;
                Angle = angle;
            }
        }

        private struct EdgeInfo
        {
            public Vector3 PointA { get; }
            public Vector3 PointB { get; }

            public EdgeInfo(Vector3 pointA, Vector3 pointB)
            {
                PointA = pointA;
                PointB = pointB;
            }
        }
    }
}
