``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19042.1110 (20H2/October2020Update)
Intel Core i7-5820K CPU 3.30GHz (Broadwell), 1 CPU, 12 logical and 6 physical cores
.NET SDK=5.0.302
  [Host]     : .NET 5.0.8 (5.0.821.31504), X64 RyuJIT
  DefaultJob : .NET 5.0.8 (5.0.821.31504), X64 RyuJIT


```
|             Method |      Mean |     Error |    StdDev |    Median | Rank |      Gen 0 |    Gen 1 | Gen 2 | Allocated |
|------------------- |----------:|----------:|----------:|----------:|-----:|-----------:|---------:|------:|----------:|
|        TaskUpdater |  32.47 ms |  0.648 ms |  1.808 ms |  32.68 ms |    1 |  1093.7500 | 468.7500 |     - |      8 MB |
|    ParallelUpdater | 118.11 ms |  0.281 ms |  0.250 ms | 118.13 ms |    2 | 19000.0000 | 125.0000 |     - |    142 MB |
| SynchronousUpdater | 339.75 ms | 10.698 ms | 31.543 ms | 350.02 ms |    3 | 18000.0000 |        - |     - |    138 MB |
