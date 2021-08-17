``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19042.1110 (20H2/October2020Update)
Intel Core i7-5820K CPU 3.30GHz (Broadwell), 1 CPU, 12 logical and 6 physical cores
.NET SDK=5.0.303
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  DefaultJob : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT


```
|             Method |     Mean |   Error |   StdDev | Rank |      Gen 0 |     Gen 1 | Gen 2 | Allocated |
|------------------- |---------:|--------:|---------:|-----:|-----------:|----------:|------:|----------:|
|    ParallelUpdater | 130.5 ms | 0.44 ms |  0.41 ms |    1 | 21600.0000 |  200.0000 |     - |    159 MB |
| SynchronousUpdater | 317.5 ms | 7.45 ms | 21.96 ms |    2 | 18000.0000 |  500.0000 |     - |    138 MB |
|        TaskUpdater | 896.4 ms | 3.36 ms |  3.14 ms |    3 | 41000.0000 | 1000.0000 |     - |    307 MB |
