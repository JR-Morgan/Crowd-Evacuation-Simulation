using System;
using PedestrianSimulation.Agent;
using System.Collections.Generic;
using System.Linq;
using JMTools;
using UnityEngine;
using UnityEngine.Events;

namespace PedestrianSimulation.Simulation
{
    [AddComponentMenu("Simulation/Managers/World State Manager"), DisallowMultipleComponent]
    public class WorldStateManager : Singleton<WorldStateManager>
    {
        [SerializeField, Range(0, 255)]
        private float sFrameFrequency = 5;

        public List<IEnumerable<AgentState>> AgentStates { get; private set; }

        [SerializeField]
        private List<AgentState> agentStates;
        public List<AgentState> FlatAgentStates { get => agentStates; private set => agentStates = value; }

        private void OnEnable()
        {
            Initialise();
        }


        public void Initialise() => Initialise(sFrameFrequency);

        private void Initialise(float frameFrequency)
        {
            AgentStates = new List<IEnumerable<AgentState>>();
            FlatAgentStates = new List<AgentState>();
        }


        public float CurrentTime => currentTime;

        int currentS = 0;
        float currentTime = 0f;
        int counter = 0;

        public void FixedUpdate()
        {
            if (AgentStates != null)
            {
                currentTime += Time.fixedDeltaTime;
                counter++;

                if (counter >= sFrameFrequency)
                {
                    counter = 0;
                    currentS++;

                    AddSFrame(SimulationManager.Instance);
                }
                OnUpdate.Invoke(currentTime);
            }

        }

        private void AddSFrame(SimulationManager simulationManager)
        {
            if (currentS >= AgentStates.Count)
            {
                var agents = simulationManager.Agents;
                var states = agents.Select(a => a.State);

                AgentStates.Add(states.ToList());
                FlatAgentStates.AddRange(states);
            }
        }

        public float JumpNearest(float time)
        {
            int position = NearestSFrame(time);
            Jump(position);
            return ToTime(position);
        }

        private void Jump(int position)
        {
            if (!this.enabled) return;
            
            
            currentS = position;
            currentTime = ToTime(position);
            counter = 0;

            var agents = SimulationManager.Instance.Agents;

            int i = 0;
            Debug.Log($"Jumping to {currentTime} setting {AgentStates[position].Count()} agent states");
            
            foreach (AgentState s in AgentStates[position])
            {
                //There is a potential for problems to occur, where SimulationManager.CheckSimulationFinished is called each multiple times when an agent's state is changed.
                //There may be an situation where this check thinks that all agents have completed part way through this surrounding for loop
                //Which, when finished, it may be that not all agents are actually completed.
                //This will lead to incorrect termination of the simulation
                //However this is only likely to occur on the penultimate sFrame so will only introduce an error of a single sFrame.
                //For non-deterministic simulations this may to more significant undesirable behaviour.
                var agent = agents[i];
                agent.State = s;
                
                i++;
            }
        }
        

        private int NearestSFrame(float time) => Mathf.Max(Mathf.Min(Mathf.RoundToInt(time / (Time.fixedDeltaTime * sFrameFrequency)), AgentStates.Count - 1), 0);
        private float ToTime(int position) => position * (Time.fixedDeltaTime * sFrameFrequency);

        public UnityEvent<float> OnUpdate { get; } = new UnityEvent<float>();

    }
}