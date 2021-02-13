using Assets.Scripts.Agent.AgentModel;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    [SerializeField]
    private AgentSFM model;
    internal IAgentModel Model => model;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        model = new AgentSFM();
    }

    internal void Tick(IEnumerable<IAgentModel> agents, float deltaTime)
    {
        model.Update(rb.velocity, rb.position);
        rb.velocity = model.CalculateNextVelocity(agents, deltaTime);
        rb.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
    }
}
