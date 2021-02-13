using Assets.Scripts.Agent.AgentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Agent
{
    class AgentUpdateManager : MonoBehaviour
    {
        private IList<AgentController> agentControllers;
        private IList<IAgentModel> agentModels;

        private void Start()
        {
            RefreshAgents();
        }

        public void RefreshAgents()
        {
            agentControllers = (IList<AgentController>)FindObjectsOfType(typeof(AgentController));
            agentModels = new List<IAgentModel>(agentControllers.Count());
            foreach (AgentController agent in agentControllers)
            {
                agentModels.Add(agent.Model);
            }
        }

        void Update()
        {
            foreach (AgentController agent in agentControllers)
            {
                agent.Tick(agentModels, Time.deltaTime);
            }
        }
    }
}
