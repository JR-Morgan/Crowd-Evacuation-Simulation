``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19042.1110 (20H2/October2020Update)
Intel Core i7-5820K CPU 3.30GHz (Broadwell), 1 CPU, 12 logical and 6 physical cores
.NET SDK=5.0.302
  [Host]     : .NET 5.0.8 (5.0.821.31504), X64 RyuJIT
  DefaultJob : .NET 5.0.8 (5.0.821.31504), X64 RyuJIT


```
|   Method |     Mean |   Error |   StdDev |   Median | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------- |---------:|--------:|---------:|---------:|-----:|------:|------:|------:|----------:|
|  Current | 156.2 μs | 3.63 μs | 10.71 μs | 158.6 μs |    1 |     - |     - |     - |      1 KB |
| Previous | 158.3 μs | 3.77 μs | 11.06 μs | 161.2 μs |    1 |     - |     - |     - |      1 KB |
