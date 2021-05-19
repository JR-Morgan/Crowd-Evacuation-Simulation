using DOTS_AI.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS_AI.Systems
{
    public class AgentBehaviourSystem : SystemBase
    {

        //EntityQuery entityQuery;
        //protected override void OnCreate()
        //{
        //    base.OnCreate();
        //    Entities.WithStoreEntityQueryInField(ref entityQuery);
        //}

        protected override void OnUpdate()
        {
            //using NativeArray<AgentComponent> agents = entityQuery.ToComponentDataArray<AgentComponent>(Allocator.Temp);

            //Entities.ForEach((ref AgentComponent agentComponent, ref Translation translation) =>
            //{
            //    agentComponent.translation = translation.Value;
        
            //    float3 drivingForce = SocialForceModel.DrivingForce(agentComponent);
            //    float3 InteractionForce = SocialForceModel.InteractionForce(agentComponent, agents);
       
        
            //}).ScheduleParallel();

        }
    }
}
