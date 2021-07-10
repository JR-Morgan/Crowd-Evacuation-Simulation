using System;
using System.Collections;
using System.Collections.Generic;
using PedestrianSimulation.Agent.LocalAvoidance;
using PedestrianSimulation.Environment;
using UnityEngine;

namespace PedestrianSimulation.Environment
{
    #if UNITY_EDITOR
    public class ChunkVisualiser : MonoBehaviour
    {
        private static readonly Color C1 = Color.red, C2 = Color.white, C3 = new Color(1f, 0.5f, 0f, 0f), C4 = Color.green;
        
        public static Dictionary<Chunk, ChunkVisualiser> Dict { get; } = new Dictionary<Chunk, ChunkVisualiser>();
        
        private Vector3 size;

        private Chunk chunk;
        
        public void Initialise(Chunk chunk, Vector2Int index)
        {
            this.chunk = chunk;
            this.size = new Vector3(1, 16, 1); //TODO not hard code
            transform.position = EnvironmentManager.InverseIndexFunction(index);
            Dict.Add(chunk, this);
        }

        private void Update()
        {
            Selected = false;
        }

        public void OnDrawGizmos()
        {
            if (Selected)
            {
                DrawSelected();
                return;
            }
            
            Gizmos.color = C1;
            //Gizmos.DrawWireCube(transform.position, size);
            Gizmos.color = C2;
            foreach (var wall in chunk.walls)
            {
                Gizmos.DrawLine(wall.StartPoint,wall.EndPoint);
            }
        }
        
        public bool Selected { get; set; }
        public void OnDrawGizmosSelected()
        {
            DrawSelected();
        }

        private void DrawSelected()
        {
            Gizmos.color = C3;
            Gizmos.DrawWireCube(transform.position, size);
            Gizmos.color = C4;
            foreach (var wall in chunk.walls)
            {
                Gizmos.DrawLine(wall.StartPoint + new Vector3(0,1,0), wall.EndPoint + new Vector3(0,1,0));
            }
        }
    }
    #endif
}
