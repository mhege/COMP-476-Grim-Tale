using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Pathfinding
{
    public class PathfindingAgent : MonoBehaviour
    {
        public bool debug;

        private delegate float Heuristic(GridGraphNode start, GridGraphNode end);

        private GridGraph graph;
        private GridGraphNode startNode;
        private GridGraphNode goalNode;
        private List<GridGraphNode> path;

        [SerializeField] private float agentRadius = 0.3f;
        [SerializeField] private float maximumSpeed = 2.5f;
        [SerializeField] private float maximumAcceleration = 4f;
        [SerializeField] private float maximumAngularVelocity = 180f;
        [SerializeField] private float maximumAngularAcceleration = 360f;
        [SerializeField] private float arriveOrientation = 0.1f;
        [SerializeField] private float slowDownOrientation = 45f;
        [SerializeField] private float rotationTimeToTarget = 0.5f;

        private float _angularVelocity;
        private Vector3 _velocity;
        private Vector3 _formationVector;
        private Vector3 _velocityForRotation;
        private bool _rotateFirst;
        private bool _isStopped;
        private bool _arrived;
        private bool _controlRotation = true;

        public List<GridGraphNode> Path
        {
            set => path = value;
        }
        
        public float MaximumSpeed
        {
            set => maximumSpeed = value;
        }

        public bool IsStopped
        {
            set => _isStopped = value;
        }

        public bool ControlRotation
        {
            set => _controlRotation = value;
        }

        public Vector3 FormationVector
        {
            get => _formationVector;
            set => _formationVector = value;
        }
        
        public bool Arrived => _arrived;
        
        private void Awake()
        {
            graph = FindObjectOfType<GridGraph>();
        }

        private void Update()
        {
            if (path == null || path.Count == 0)
            {
                _arrived = false;
                return;
            }
            
            var targetNode = PathFollowing();
            var target = targetNode.transform.position;
            var position = transform.position;
            var adjustedTarget = new Vector3(target.x, position.y, target.z);

            _arrived = startNode != goalNode && targetNode == goalNode && Vector3.Distance(adjustedTarget, position) < 0.1f;

            if (_rotateFirst)
            {
                _velocityForRotation = adjustedTarget - transform.position;
                if (Vector3.Angle(_velocityForRotation, transform.forward) < 1f && !_isStopped)
                {
                    Seek(adjustedTarget);
                }

                LookWhereYouAreGoing();
            }
            else
            {
                if(!_isStopped)
                    Seek(adjustedTarget);

                if (_controlRotation)
                {
                    LookWhereYouAreGoing();
                }
            }
        }

        public void SetDestination(Vector3 position, bool rotateFirst = false)
        {
            if (graph == null || graph.Nodes == null || graph.Nodes.Count == 0) return;
            
            startNode = graph.Nodes.OrderBy(n => Vector3.Distance(n.transform.position, transform.position)).First();
            goalNode = graph.Nodes.OrderBy(n => Vector3.Distance(n.transform.position, position)).First();
            
            ClearPoints();
            
            path = FindPath(startNode, goalNode, ManhattanHeuristic);
            _rotateFirst = rotateFirst;
        }
    
        // Actual path finding
        private List<GridGraphNode> FindPath(GridGraphNode start, GridGraphNode goal, Heuristic heuristic = null, bool isAdmissible = true)
        {
            if (graph == null) return new List<GridGraphNode>();

            // if no heuristic is provided then set heuristic = 0
            heuristic ??= (s, e) => 0;

            List<GridGraphNode> path = null;
            var solutionFound = false;

            var gnDict = new Dictionary<GridGraphNode, float> {{start, default}};
            var fnDict = new Dictionary<GridGraphNode, float> {{start, heuristic(start, goal) + gnDict[start]}};
            var pathDict = new Dictionary<GridGraphNode, GridGraphNode> {{start, null}};
            var openList = new List<GridGraphNode> {start};
            var closedODict = new List<GridGraphNode>();

            while (openList.Count > 0)
            {
                // mimic priority queue and remove from the back of the open list (lowest fn value)
                var current = openList[openList.Count - 1];
                openList.RemoveAt(openList.Count - 1);
            
                closedODict.Add(current);
            
                // early exit
                if (current == goal && isAdmissible)
                {
                    solutionFound = true;
                    break;
                }
            
                if (closedODict.Contains(goal))
                {
                    // early exit strategy if heuristic is not admissible (try to avoid this if possible)
                    var gGoal = gnDict[goal];
                    var pathIsTheShortest = openList.All(e => !(gGoal > gnDict[e]));

                    if (pathIsTheShortest) break;
                }

                var neighbors = graph.GetNeighbors(current);
                foreach (var n in neighbors)
                {
                    var movementCost = ManhattanHeuristic(current, n);
                    var gNeighbor = gnDict[current] + movementCost;
                    var fn = gNeighbor + heuristic(n, goal);

                    if (openList.Contains(n) || closedODict.Contains(n))
                    {
                        if (gNeighbor < gnDict[n])
                        {
                            gnDict[n] = gNeighbor;
                        }

                        if (!(fn < fnDict[n])) continue;
                    
                        fnDict[n] = fn;
                        pathDict[n] = current;

                        if (!closedODict.Contains(n)) continue;
                    
                        closedODict.Remove(n);
                        FakePQListInsert(openList, fnDict, n);
                    }
                    else
                    {
                        gnDict.Add(n, gNeighbor);
                        fnDict.Add(n, fn);
                        pathDict.Add(n, current);
                        FakePQListInsert(openList, fnDict, n);
                    }
                }
            }

            // if the closed list contains the goal node then we have found a solution
            if (!solutionFound && closedODict.Contains(goal))
                solutionFound = true;

            if (solutionFound)
            {
                path = new List<GridGraphNode>();

                var node = goal;

                while (!path.Contains(start))
                {
                    path.Add(node);
                    node = pathDict[node];
                }

                // reverse the path since we started adding nodes from the goal 
                path.Reverse();
            }

            if (!debug) return path;
        
            ClearPoints();

            var openListPoints = openList.Select(node => node.transform).ToList();
            SpawnPoints(openListPoints, graph.OpenPointPrefab, Color.magenta);

            var closedListPoints = (from node in closedODict where solutionFound && !path.Contains(node) select node.transform).ToList();
            SpawnPoints(closedListPoints, graph.ClosedPointPrefab, Color.red);

            if (!solutionFound) return null;
        
            var pathPoints = path.Select(node => node.transform).ToList();
            SpawnPoints(pathPoints, graph.PathPointPrefab, Color.green);

            return path;
        }
    
        // The Manhattan distance heuristic
        private float ManhattanHeuristic(GridGraphNode from, GridGraphNode to)
        {
            var fromPosition = from.transform.position;
            var toPosition = to.transform.position;
        
            return Mathf.Abs(fromPosition.x - toPosition.x) + Mathf.Abs(fromPosition.z - toPosition.z);
        }
    
        // Performs the path smoothing and returns the new smoothed path
        private List<GridGraphNode> SmoothPath()
        {
            var smoothedPath = new List<GridGraphNode>();

            if (path.Count < 3) return path;
        
            smoothedPath.Add(path[0]);
            var lastFreeNode = path[1];
            for (var i = 2; i < path.Count; i++)
            {
                var lastNodePosition = smoothedPath.Last().transform.position;
                var nextNodePosition = path[i].transform.position;
                var clear = !Physics.Raycast(lastNodePosition, nextNodePosition - lastNodePosition, Vector3.Distance(lastNodePosition, nextNodePosition), LayerMask.GetMask("Obstacle"));
            
                if (clear)
                {
                    lastFreeNode = path[i];
                }
                else
                {
                    smoothedPath.Add(lastFreeNode);
                }
            }
            smoothedPath.Add(path.Last());
        
            return smoothedPath;
        }

        private void SpawnPoints(List<Transform> points, GameObject prefab, Color color)
        {
            foreach (var t in points)
            {
#if UNITY_EDITOR
                // Scene view visuals
                t.GetComponent<GridGraphNode>()._nodeGizmoColor = color;
#endif

                // Game view visuals
                var obj = Instantiate(prefab, t.position, Quaternion.identity, t);
                obj.name = "DEBUG_POINT";
                obj.transform.localPosition += Vector3.up * 0.5f;
            }
        }

        private void ClearPoints()
        {
            foreach (var node in graph.Nodes)
            {
                for (var c = 0; c < node.transform.childCount; ++c)
                {
                    if (node.transform.GetChild(c).name == "DEBUG_POINT")
                    {
                        Destroy(node.transform.GetChild(c).gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// mimics a priority queue here by inserting at the right position using a loop
        /// not a very good solution but ok for this lab example
        /// </summary>
        /// <param name="pqList"></param>
        /// <param name="fnDict"></param>
        /// <param name="node"></param>
        private void FakePQListInsert(List<GridGraphNode> pqList, Dictionary<GridGraphNode, float> fnDict, GridGraphNode node)
        {
            if (pqList.Count == 0)
                pqList.Add(node);
            else
            {
                for (var i = pqList.Count - 1; i >= 0; --i)
                {
                    if (fnDict[pqList[i]] > fnDict[node])
                    {
                        pqList.Insert(i + 1, node);
                        break;
                    }

                    if (i == 0)
                    {
                        pqList.Insert(0, node);
                    }
                }
            }
        }
        
        // Actual path following
        private GridGraphNode PathFollowing()
        {
            var nearestNode = path.OrderBy(n => Vector3.Distance(n.transform.position, transform.position)).First();
            var index = path.IndexOf(nearestNode);

            var lastFreeNode = nearestNode;
            var target = lastFreeNode;

            // Path smoothing
            for (var i = index + 1; i < path.Count; i++)
            {
                var position = transform.position + 0.1f * Vector3.up;
                var nextNodePosition = path[i].transform.position;
                var nextNodeAdjustedPosition = new Vector3(nextNodePosition.x, position.y, nextNodePosition.z);
                var direction = nextNodeAdjustedPosition - position;
                var distance = Vector3.Distance(position, nextNodeAdjustedPosition);
                var layerMask = LayerMask.GetMask("Obstacle");
                var clear = !Physics.Raycast(position, direction, out _, distance, layerMask) && !Physics.SphereCast(position, agentRadius, direction, out _, distance, layerMask);

                if (clear)
                {
                    lastFreeNode = path[i];

                    if (lastFreeNode == path.Last())
                    {
                        target = lastFreeNode;
                        break;
                    }
                }
                else
                {
                    target = lastFreeNode;
                    break;
                }
            }

            return target;
        }
        
        // Steering seek behaviour
        private void Seek(Vector3 target)
        {
            var position = transform.position;
            var a = maximumAcceleration * (target - position).normalized;
            var v = _velocity + a * Time.deltaTime;

            if (v.magnitude > maximumSpeed)
            {
                v = v.normalized * maximumSpeed;
            }

            _velocity = v + _formationVector;
            _velocityForRotation = _velocity;
        
            var nextPosition = position + _velocity * Time.deltaTime;
            transform.position = nextPosition;
        }
        
        // Agent rotation
        private void LookWhereYouAreGoing()
        {
            if (_velocityForRotation == Vector3.zero) return;
        
            var targetOrientation = Vector3.SignedAngle(transform.forward, _velocityForRotation, Vector3.up);

            if (Mathf.Abs(targetOrientation - arriveOrientation) < arriveOrientation) return;
        
            var sign = targetOrientation < 0 ? -1 : 1;
            var goalAngularVelocity = sign * maximumAngularVelocity * (targetOrientation / slowDownOrientation);
            var angularAcceleration = (goalAngularVelocity - _angularVelocity) / rotationTimeToTarget;
        
            if (angularAcceleration > maximumAngularAcceleration)
            {
                angularAcceleration = maximumAngularAcceleration;
            }

            _angularVelocity += angularAcceleration * Time.deltaTime;

            if (_angularVelocity > maximumAngularVelocity)
            {
                _angularVelocity = maximumAngularVelocity;
            }
        
            transform.RotateAround(transform.position, Vector3.up, sign * _angularVelocity * Time.deltaTime);
        }
    }
}
