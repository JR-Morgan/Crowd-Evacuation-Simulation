using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PedestrianSimulation.Agent.LocalAvoidance;
using UnityEngine;
using UnityEngine.AI;

namespace PedestrianSimulation.Environment
{

    public struct Chunk
    {
        public List<Wall> walls;
    }
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshSurface))]
    public class EnvironmentManager : MonoBehaviour
    {

        public ConcurrentDictionary<int, Chunk> HashedChunks { get; private set; }
        
        private NavMeshSurface navMeshSurface;

        private void Awake()
        {
            HashedChunks = new ConcurrentDictionary<int, Chunk>();
            navMeshSurface = GetComponent<NavMeshSurface>();
        }
        
        public void InitialiseEnvironment()
        {
            Transform[] children = this.GetComponentsInChildren<Transform>(true);
            int layerNumber = (int)(Mathf.Log((uint)navMeshSurface.layerMask.value, 2));
            
            foreach (Transform t in children)
            {
                t.gameObject.layer = layerNumber;
            }
            
            navMeshSurface.BuildNavMesh();
            
            
        }
    }
}
