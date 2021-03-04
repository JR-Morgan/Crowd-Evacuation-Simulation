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

public class WorldStateManager : MonoBehaviour
{
    #region Singleton and Unity Methods
    private static WorldStateManager _instance;
    public static WorldStateManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    [SerializeField]
    private float sFrameFrequency;

    private List<AgentState[]> agentStates;

    public void Initialise() => Initialise(sFrameFrequency, AgentManager.Instance);

    private void Initialise(float frameFrequency, AgentManager agentManager)
    {
        agentStates = new List<AgentState[]>();
    }




    int currentS = 0;
    float currentTime = 0f;
    int counter = 0;

    public void FixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;
        counter++;

        if (counter >= sFrameFrequency)
        {
            counter = 0;
            currentS++;
            
            AddSFrame(AgentManager.Instance);

            
        }
        OnUpdate.Invoke(currentTime);
    }

    private void AddSFrame(AgentManager agentManager)
    {
        if(currentS >= agentStates.Count)
        {
            IList<AgentBehaviour> agents = agentManager.Agents;
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

        IList<AgentBehaviour> agents = AgentManager.Instance.Agents;
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].State = agentStates[position][i];
        }
    }

    private int Nearest(float time) => Mathf.Max(Mathf.Min(Mathf.RoundToInt(time / (Time.fixedDeltaTime * sFrameFrequency)), agentStates.Count - 1), 0);
    private float ToTime(int position) => position * (Time.fixedDeltaTime * sFrameFrequency);

    public UnityEvent<float> OnUpdate { get; } = new UnityEvent<float>();

}