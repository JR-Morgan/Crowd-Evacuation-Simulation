using System;
using System.Collections;
using System.Collections.Generic;
using PedestrianSimulation.Agent;
using PedestrianSimulation.Simulation;
using PedestrianSimulation.Simulation.UpdateStrategies;
using UnityEngine;

namespace PedestrianSimulation
{
    public class AgentUpdater : MonoBehaviour
    {
        public IAgentUpdater<IAgent> Updater { get; set; }
        private IEnumerable<IAgent> agents;
        private float timeStep;
        private void Start()
        {
            SimulationManager.Instance.OnSimulationStart.AddListener(Initialise);
        }

        private void Initialise()
        {
            agents = SimulationManager.Instance.Agents;
            timeStep = SimulationManager.Instance.Settings.timeStep;
        }
        
        void Update()
        {
             Updater.Tick(timeStep, agents);
        }
    }
}
