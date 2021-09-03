using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using PedestrianSimulation.Agent;
using PedestrianSimulation.Agent.LocalAvoidance;
using UnityEngine;
using UnityEngine.AI;

namespace PedestrianSimulation.Simulation.Initialisation.Tests
{
    [TestFixture, TestOf(typeof(UniformAgentDistribution<>))]
    public class UniformAgentDistribution_Test
    {

        private class MockAgent : AbstractAgent
        {
            public override AgentState State { get; set; }

            public override bool TrySetGoal(Vector3 terminalGoal)
            {
                AgentState tmp = State;
                tmp.goal = terminalGoal;
                State = tmp;
                return true;
            }

            public override void Initialise(int id, ILocalAvoidance localAvoidance, AgentEnvironmentModel initialEnvironmentModel)
            {
                AgentState tmp = ConstructState(id, default, default, default, default);
            }
            public override void UpdateIntentions(float timeStep)
            { }
        }

        private static int[] testNumbers = {0,1,2,10};


        private UniformAgentDistribution<MockAgent> distribution;
        private GameObject environment;
        
        [OneTimeSetUp]
        public void SetupDistribution()
        {
            distribution = new UniformAgentDistribution<MockAgent>();
        }

        [SetUp]
        public void SetupEmptyEnvironment()
        {
            environment = new GameObject(nameof(environment), typeof(NavMeshSurface));
        }
        

        [Test]
        [TestCaseSource(nameof(testNumbers))]
        public void EmptyEnvironment(int numberOfAgents)
        {
            var agents = distribution.InstantiateAgents(
                agentParent: new GameObject().transform,
                agentsGoals: new [] { new GameObject().transform },
                agentPrefab: new GameObject(),
                numberOfAgents: numberOfAgents,
                environmentModel: environment
            );
            
            Assert.That(agents, Has.Count.EqualTo(0));
        }
        
        
        
        [Test, TestCaseSource(nameof(testNumbers))]
        public void ValidEnvironment(int numberOfAgents)
        {
            Transform plane = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
            plane.parent = environment.transform;
            plane.rotation = Quaternion.Euler(10, 0, 0); //Set some small rotation such that there is volume to the model 
            
            var agents = distribution.InstantiateAgents(
                agentParent: new GameObject().transform,
                agentsGoals: new [] { new GameObject().transform },
                agentPrefab: new GameObject(),
                numberOfAgents: numberOfAgents,
                environmentModel: environment
            );
            
            Assert.That(agents, Has.Count.EqualTo(numberOfAgents));
        }
        
        [Test, TestCaseSource(nameof(testNumbers))]
        public void CompleteAgentInitialisation(int numberOfAgents)
        {
            Transform plane = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            plane.parent = environment.transform;
            plane.localScale *= 10; //Make the plane large enough to fit many agents

            Transform goal = new GameObject().transform;
            Transform parent = new GameObject().transform;
            
            var agents = distribution.InstantiateAgents(
                agentParent: parent,
                agentsGoals: new [] {goal},
                agentPrefab: new GameObject(),
                numberOfAgents: numberOfAgents,
                environmentModel: environment
            );

            Assert.That(agents, Has.Count.EqualTo(numberOfAgents));
            
            HashSet<int> takenIds = new HashSet<int>();
            foreach (var agent in agents)
            {
                Assert.That(agent.transform.parent, Is.EqualTo(parent), $"Agent {agent.State}'s {nameof(agent.transform.parent)} was incorrectly initialised");
                Assert.That(agent.State.goal, Is.EqualTo(goal.position), $"Agent {agent.State}'s {nameof(agent.State.goal)} was incorrectly initialised");

                Assert.True(takenIds.Add(agent.State.id), $"Agent {agent.State} did not have a unique ID.");
            }
            
        }
        
        
        
    }
}
