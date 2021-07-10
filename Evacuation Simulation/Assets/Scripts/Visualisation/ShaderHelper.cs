using PedestrianSimulation.Agent;
using PedestrianSimulation.Simulation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PedestrianSimulation.Visualisation
{

    public static class ShaderHelper
    {
        public static IEnumerable<Vector4> ToHomogeneousCoordinates(IEnumerable<AgentState> agents)
        {
            return agents.Select(a => ToHomogeneousCoordinates(a.position));
        }

        public static IEnumerable<Vector4> ToHomogeneousCoordinates<T>(IEnumerable<T> agents) where T : Component
        {
            return agents.Select(a => ToHomogeneousCoordinates(a.transform.position));

        }

        public static IEnumerable<Vector4> ToHomogeneousCoordinates(IEnumerable<Vector3> positions) => positions.Select(ToHomogeneousCoordinates);

        public static Vector4 ToHomogeneousCoordinates(Vector3 pos) => new Vector4(pos.x, pos.y, pos.z, 1f);
    }
}
