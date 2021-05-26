using PedestrianSimulation.Agent;
using PedestrianSimulation.Simulation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PedestrianSimulation.Simulation
{
    [Serializable]
    public struct AgentStateModel
    {
        public bool active;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
    }

    public class WorldStateManager : Singleton<WorldStateManager>
    {
        [SerializeField, Range(0, byte.MaxValue)]
        private float sFrameFrequency = 5;
        [SerializeField]
        private List<AgentStateModel[]> agentStates;

        private void OnEnable()
        {
            Initialise();
        }



        public void Initialise() => Initialise(sFrameFrequency);

        private void Initialise(float frameFrequency)
        {
            agentStates = new List<AgentStateModel[]>();
        }




        int currentS = 0;
        float currentTime = 0f;
        int counter = 0;

        public void FixedUpdate()
        {
            if (agentStates != null)
            {
                currentTime += Time.fixedDeltaTime;
                counter++;

                if (counter >= sFrameFrequency)
                {
                    counter = 0;
                    currentS++;

                    AddSFrame(LegacySimulationManager.Instance);


                }
                OnUpdate.Invoke(currentTime);
            }

        }

        private void AddSFrame(LegacySimulationManager simulationManager)
        {
            if (currentS >= agentStates.Count)
            {
                IList<LegacyPedestrianAgent> agents = simulationManager.Agents;
                AgentStateModel[] states = new AgentStateModel[agents.Count];
                for (int i = 0; i < agents.Count; i++)
                {
                    states[i] = agents[i].State;
                }
                agentStates.Add(states);
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
            currentS = position;
            currentTime = ToTime(position);
            counter = 0;

            IList<LegacyPedestrianAgent> agents = LegacySimulationManager.Instance.Agents;
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].State = agentStates[position][i];
            }
        }

        private int NearestSFrame(float time) => Mathf.Max(Mathf.Min(Mathf.RoundToInt(time / (Time.fixedDeltaTime * sFrameFrequency)), agentStates.Count - 1), 0);
        private float ToTime(int position) => position * (Time.fixedDeltaTime * sFrameFrequency);

        public UnityEvent<float> OnUpdate { get; } = new UnityEvent<float>();

    }
}