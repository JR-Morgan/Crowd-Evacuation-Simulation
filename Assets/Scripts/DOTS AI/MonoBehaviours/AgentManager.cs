using DOTS_AI.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace DOTS_AI.Scripts
{
    public class AgentManager : MonoBehaviour
    {

        public void Start()
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityArchetype agentArchetype = entityManager.CreateArchetype(
                typeof(AgentComponent),
                typeof(Translation)
            );

            NativeArray<Entity> agents = new NativeArray<Entity>(2, Allocator.Temp);
            entityManager.CreateEntity(agentArchetype, agents);

            


            foreach (Entity agent in agents)
            {
                entityManager.SetComponentData(agent, new AgentComponent() { desiredSpeed = 1.4f});
                //RenderMeshDescription renderMeshDesc = new RenderMeshDescription();
                //RenderMeshUtility.AddComponents(agent, entityManager, renderMeshDesc);
            }

            agents.Dispose();
        }

    }
}