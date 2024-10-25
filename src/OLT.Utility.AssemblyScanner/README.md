# .NET Utility Library to scan for referenced assemblies 


[![Nuget](https://img.shields.io/nuget/v/OLT.Utility.AssemblyScanner)](https://www.nuget.org/packages/OLT.Utility.AssemblyScanner)


[![CI](https://github.com/OuterlimitsTech/olt-dotnet-utility-libraries/actions/workflows/build.yml/badge.svg)](https://github.com/OuterlimitsTech/olt-dotnet-utility-libraries/actions/workflows/build.yml) 


```shell
dotnet add package OLT.Utility.AssemblyScanner
```


```csharp
using OLT.Utility

class Program
{
    static void Main()
    {
        var assemblies = new OltAssemblyScanBuilder()
            .IncludeFilters("OLT.", "MyApp.")
            .ExcludeFilter("System.")
            .ExcludeFilter("Microsoft.")
            .LoadAssemblies()
            .Build();
        
        foreach (var assembly in assemblies)
        {
            Console.WriteLine($"Loaded Assembly: {assembly.FullName}");
        }
    }
}


```

