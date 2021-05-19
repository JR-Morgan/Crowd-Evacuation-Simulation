using PedestrianSimulation.Agent;
using PedestrianSimulation.Simulation.UpdateStrategies;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSimulation.Simulation
{
    partial class SimulationManager
    {
        [Header("Updater")]
        [SerializeField]
        private UpdateStrategy updateStrategy;
        [SerializeField]
        private float timeStep = 1f;

        private void InitialiseUpdater(ICollection<PedestrianAgent> agents, IAgentUpdater<PedestrianAgent> updateStrategy)
        {
            updater = updateStrategy;
            updater.Initialise(agents);
        }

        private IAgentUpdater<PedestrianAgent> updater;

        [ContextMenu("Step")]
        private void Step()
        {
            //Asynchronously update agents intentions
            updater.Tick(timeStep, Agents);

            //Synchronously update agents state
            foreach (var a in Agents)
            {
                a.CommitAction();
            }
        }
    }


}
