using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using PedestrianSimulation.Agent;
using PedestrianSimulation.Agent.LocalAvoidance;
using PedestrianSimulation.Agent.LocalAvoidance.SFM;
using PedestrianSimulation.Simulation.UpdateStrategies;
using UnityEngine;
using Random = System.Random;

namespace PedestrianSimulation
{
    [MemoryDiagnoser]
    [HtmlExporter]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class UpdateBenchmark
    {
        private readonly int ticks, numberOfAgents, numberOfWalls;
        private readonly Random random;

        public UpdateBenchmark() : this(new Random(255),1000,2000,500)
        { }
        
        public UpdateBenchmark(Random random, int ticks, int numberOfAgents, int numberOfWalls)
        {
            this.ticks = ticks;
            this.numberOfAgents = numberOfAgents;
            this.numberOfWalls = numberOfWalls;
            this.random = random;
        }
        
        
        #region Benchmarks
        
        [Benchmark]
        public void ParallelUpdater()
        {
            Run(new ParallelForUpdater());
        }
        
        [Benchmark]
        public void TaskUpdater()
        {
            Run(new AsyncTaskUpdater());
        }
        
        [Benchmark]
        public void SynchronousUpdater()
        {
            Run(new SynchronousUpdater());
        }

        private void Run(IAgentUpdater updater, float timeStep = 1f)
        {
            var agents = GenerateAgents(numberOfAgents, numberOfWalls, random);
           
            for (int i = 0; i < ticks; i++)
            {
                updater.Tick(timeStep, agents);
            }
        }
        #endregion

        private static List<TestAgent> GenerateAgents(int numberOfAgents, int numberOfWalls, System.Random random)
        {
            List<TestAgent> agents = new(numberOfAgents);

            //Create Agents
            for (int i = 0; i < numberOfAgents; i++)
            {
                agents.Add(new TestAgent(i, random));
            }

            //Create Walls
            List<Wall> walls = new(numberOfWalls);
            for (int i = 0; i < numberOfWalls; i++)
            {
                Vector3 pos1 = RandomPos(random), pos2 = RandomPos(random);
                walls.Add(new Wall(pos1, pos2));
            }
            
            //Set agent model
            foreach (var agent in agents)
            {
                agent.Model = new AgentEnvironmentModel();
            }

            return agents;
        }
        private static Vector3 RandomPos(System.Random random, float positionScale = 200) => new Vector3(random.Next(), random.Next(), random.Next()) * positionScale;
        
        private sealed class TestAgent : IAgent
        {
            
            public TestAgent(int id, Random random)
            {
                var pos = RandomPos(random);
                State = new AgentState(id, true, default, default, default, pos, default, default, default);
            }

            public bool TrySetGoal(Vector3 terminalGoal) => false;

            public void UpdateIntentions(float timeStep)
            {
                SocialForceModel.CalculateNextVelocity(State, Model);
            }

            public AgentState State { get; set; }
            public event Action<AbstractAgent> GoalComplete;
            public event Action<AbstractAgent> GoalRegress;
            public AgentEnvironmentModel Model { get; set; }
        }
        
        
    }
}
