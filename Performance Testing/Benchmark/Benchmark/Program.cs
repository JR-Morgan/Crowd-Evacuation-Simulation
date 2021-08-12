using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace PedestrianSimulation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<UpdateBenchmark>(
                ManualConfig.Create(DefaultConfig.Instance)
                    .WithOptions(ConfigOptions.DisableOptimizationsValidator)
                );
        }
    }
}