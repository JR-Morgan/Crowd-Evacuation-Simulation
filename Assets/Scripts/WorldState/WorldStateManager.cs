using PedestrianSimulation.Agent;
using PedestrianSimulation.Simulation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public int t;
    }

    public class WorldStateManager : Singleton<WorldStateManager>
    {
        [SerializeField, Range(0, byte.MaxValue)]
        private float sFrameFrequency = 5;

        public List<IEnumerable<AgentStateModel>> AgentStates { get; private set; }

        public List<AgentStateModel> FlatAgentStates { get; private set; }

        private void OnEnable()
        {
            Initialise();
        }



        public void Initialise() => Initialise(sFrameFrequency);

        private void Initialise(float frameFrequency)
        {
            AgentStates = new List<IEnumerable<AgentStateModel>>();
            FlatAgentStates = new List<AgentStateModel>();
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

                    AddSFrame(LegacySimulationManager.Instance);


                }
                OnUpdate.Invoke(currentTime);
            }

        }

        private void AddSFrame(LegacySimulationManager simulationManager)
        {
            if (currentS >= AgentStates.Count)
            {
                IList<LegacyPedestrianAgent> agents = simulationManager.Agents;
                var states = agents.Select(a => a.State);

                AgentStates.Add(states);
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
            currentS = position;
            currentTime = ToTime(position);
            counter = 0;

            IList<LegacyPedestrianAgent> agents = LegacySimulationManager.Instance.Agents;

            int i = 0;
            foreach(AgentStateModel s in AgentStates[position])
            {
                agents[i].State = s;
            }
        }

        private int NearestSFrame(float time) => Mathf.Max(Mathf.Min(Mathf.RoundToInt(time / (Time.fixedDeltaTime * sFrameFrequency)), AgentStates.Count - 1), 0);
        private float ToTime(int position) => position * (Time.fixedDeltaTime * sFrameFrequency);

        public UnityEvent<float> OnUpdate { get; } = new UnityEvent<float>();

    }
}