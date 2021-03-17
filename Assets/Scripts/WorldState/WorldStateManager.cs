using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct AgentState
{
    public bool active;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
}

public class WorldStateManager : Singleton<WorldStateManager>
{
    [SerializeField]
    private float sFrameFrequency = 5;

    private List<AgentState[]> agentStates;

    private void OnEnable()
    {
        Initialise();
    }



    public void Initialise() => Initialise(sFrameFrequency, SimulationManager.Instance);

    private void Initialise(float frameFrequency, SimulationManager simulationManager)
    {
        agentStates = new List<AgentState[]>();
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

                AddSFrame(SimulationManager.Instance);


            }
            OnUpdate.Invoke(currentTime);
        }

    }

    private void AddSFrame(SimulationManager simulationManager)
    {
        if(currentS >= agentStates.Count)
        {
            IList<AgentBehaviour> agents = simulationManager.Agents;
            AgentState[] states = new AgentState[agents.Count];
            for (int i = 0; i < agents.Count; i++)
            {
                states[i] = agents[i].State;
            }
            agentStates.Add(states);
        }
    }

    public float JumpNearest(float time)
    {
        int position = Nearest(time);
        Jump(position);
        return ToTime(position);
    }

    private void Jump(int position)
    {
        currentS = position;
        currentTime = ToTime(position);
        counter = 0;

        IList<AgentBehaviour> agents = SimulationManager.Instance.Agents;
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].State = agentStates[position][i];
        }
    }

    private int Nearest(float time) => Mathf.Max(Mathf.Min(Mathf.RoundToInt(time / (Time.fixedDeltaTime * sFrameFrequency)), agentStates.Count - 1), 0);
    private float ToTime(int position) => position * (Time.fixedDeltaTime * sFrameFrequency);

    public UnityEvent<float> OnUpdate { get; } = new UnityEvent<float>();

}