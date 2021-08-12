using System;
using System.Collections;
using System.Collections.Generic;
using PedestrianSimulation.Agent;
using PedestrianSimulation.Simulation.UpdateStrategies;
using UnityEngine;

namespace PedestrianSimulation
{
    public class AgentUpdater : MonoBehaviour
    {
        private IAgentUpdater updater;
        private IEnumerable<AbstractAgent> agents;
        private float timeStep;

        public void Initialise(IAgentUpdater updater, IEnumerable<AbstractAgent> agents, float timeStep)
        {
            this.updater = updater;
            this.agents = agents;
            this.timeStep = timeStep;
        }
        
        void Update()
        {
             updater.Tick(timeStep, agents);
        }
    }
}
