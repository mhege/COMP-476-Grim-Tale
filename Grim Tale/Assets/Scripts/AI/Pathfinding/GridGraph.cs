using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Pathfinding
{
    /// <summary>
    /// Very quick basic graph implementation that was created to be used only for COMP 476 Lab on pathfinding.
    /// It is most likely not suitable for more practical use cases without modification.
    /// </summary>
    public class GridGraph : MonoBehaviour
    {
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject openPointPrefab;
        [SerializeField] private GameObject closedPointPrefab;
        [SerializeField] private GameObject pathPointPrefab;
        
        private List<GridGraphNode> nodes = new List<GridGraphNode>();

        private void Start()
        {
            GenerateGrid();
        }

        public void Clear()
        {
            nodes.Clear();
            gameObject.DestroyChildren();
        }

        public void Remove(GridGraphNode node)
        {
            if (node == null || !nodes.Contains(node)) return;

            foreach (var n in node.adjacencyList)
            {
                n.adjacencyList.Remove(node);
            }

            nodes.Remove(node);
        }

        public void GenerateGrid(bool checkCollisions = true)
        {
            Clear();

            var nodeGrid = new GridGraphNode[generationGridRows, generationGridColumns];

            var width = (generationGridColumns > 0 ? generationGridColumns - 1 : 0) * generationGridCellSize;
            var height = (generationGridRows > 0 ? generationGridRows - 1 : 0) * generationGridCellSize;
            var genPosition = new Vector3(transform.position.x - width / 2, transform.position.y, transform.position.z - height / 2);

            // first pass : generate nodes
            for (var r = 0; r < generationGridRows; ++r)
            {
                var startingX = genPosition.x;
                for (var c = 0; c < generationGridColumns; ++c)
                {
                    if (checkCollisions)
                    {
                        var extent = generationObstacleAvoidance;
                        if (Physics.CheckBox(genPosition, new Vector3(extent, extent, extent), Quaternion.identity, LayerMask.GetMask("Obstacle")))
                        {
                            genPosition = new Vector3(genPosition.x + generationGridCellSize, genPosition.y, genPosition.z);
                            continue;
                        }
                    }

                    var obj = nodePrefab == null ? new GameObject("Node", typeof(GridGraphNode)) : Instantiate(nodePrefab);

                    obj.name = $"Node ({nodes.Count})";
                    obj.tag = "Node";
                    obj.transform.parent = transform;
                    obj.transform.position = genPosition;

                    var addedNode = obj.GetComponent<GridGraphNode>();                
                    nodes.Add(addedNode);
                    nodeGrid[r, c] = addedNode;

                    genPosition = new Vector3(genPosition.x + generationGridCellSize, genPosition.y, genPosition.z);
                }
                genPosition = new Vector3(startingX, genPosition.y, genPosition.z + generationGridCellSize);
            }

            // second pass : create adjacency lists (edges)
            var operations = new [,] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 }, { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };
            for (var r = 0; r < generationGridRows; ++r)
            {
                for (var c = 0; c < generationGridColumns; ++c)
                {
                    if (nodeGrid[r, c] == null) continue;

                    for (var i = 0; i < operations.GetLength(0); ++i)
                    {
                        var neighborId = new int[] { r + operations[i, 0], c + operations[i, 1] };

                        // check to see if operation brings us out of bounds
                        if (neighborId[0] < 0 || neighborId[0] >= nodeGrid.GetLength(0) || neighborId[1] < 0 || neighborId[1] >= nodeGrid.GetLength(1))
                            continue;

                        var neighbor = nodeGrid[neighborId[0], neighborId[1]];

                        if (neighbor == null) continue;
                        
                        if (checkCollisions)
                        {
                            var direction = neighbor.transform.position - nodeGrid[r, c].transform.position;
                            if (Physics.Raycast(nodeGrid[r, c].transform.position, direction, direction.magnitude, LayerMask.GetMask("Obstacle"))) continue;
                        }

                        nodeGrid[r, c].adjacencyList.Add(neighbor);
                    }
                }
            }
        }

        public List<GridGraphNode> GetNeighbors(GridGraphNode node)
        {
            return node.adjacencyList;
        }

        public List<GridGraphNode> Nodes => nodes;
        public GameObject OpenPointPrefab => openPointPrefab;
        public GameObject ClosedPointPrefab => closedPointPrefab;
        public GameObject PathPointPrefab => pathPointPrefab;
        public int Count => nodes.Count;

        #region grid_generation_properties

        // grid generation properties
        [HideInInspector, Min(0)] public int generationGridColumns = 1;
        [HideInInspector, Min(0)] public int generationGridRows = 1;
        [HideInInspector, Min(0)] public float generationGridCellSize = 1f;
        [HideInInspector, Min(0)] public float generationObstacleAvoidance = 1f;

#if UNITY_EDITOR
        [Header("Gizmos")] 
        private Color edgeGizmoColor = Color.white;

        private void OnDrawGizmos()
        {
            if (nodes == null) return;

            // nodes
            foreach (var node in nodes.Where(node => node != null))
            {
                Gizmos.color = node._nodeGizmoColor;

                Gizmos.color = edgeGizmoColor;
                var neighbors = GetNeighbors(node);
                foreach (var neighbor in neighbors)
                {
                    Gizmos.DrawLine(node.transform.position, neighbor.transform.position);
                }
            }
        }
#endif
        #endregion
    }
}
