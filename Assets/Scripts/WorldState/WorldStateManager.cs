using PedestrianSimulation.Agent;
using PedestrianSimulation.Simulation;
using System;
using System.Collections;
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
        [SerializeField, Range(0, byte.MaxValue)]
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
            if (this.enabled)
            {
                currentS = position;
                currentTime = ToTime(position);
                counter = 0;

                var agents = SimulationManager.Instance.Agents;

                int i = 0;
                Debug.Log($"Jumping to {currentTime} setting {AgentStates[position].Count()} agent states");
                foreach (AgentState s in AgentStates[position])
                {
                    agents[i++].State = s;
                }
            }
        }

        private int NearestSFrame(float time) => Mathf.Max(Mathf.Min(Mathf.RoundToInt(time / (Time.fixedDeltaTime * sFrameFrequency)), AgentStates.Count - 1), 0);
        private float ToTime(int position) => position * (Time.fixedDeltaTime * sFrameFrequency);

        public UnityEvent<float> OnUpdate { get; } = new UnityEvent<float>();

    }
}