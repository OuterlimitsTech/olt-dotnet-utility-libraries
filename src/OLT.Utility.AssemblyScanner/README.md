<img src="https://user-images.githubusercontent.com/1365728/127748628-47575d74-a2fb-4539-a31e-74d8b435fc21.png" width="30%" >

# .NET Utility Library to scan for referenced assemblies 

<code>OltAssemblyScanBuilder</code> is a flexible utility for filtering and loading .NET assemblies dynamically. Using a builder pattern, it allows you to include or exclude assemblies based on specified criteria, making it ideal for scenarios where you need to manage assembly loading at runtime.

[![Nuget](https://img.shields.io/nuget/v/OLT.Utility.AssemblyScanner)](https://www.nuget.org/packages/OLT.Utility.AssemblyScanner)


[![CI](https://github.com/OuterlimitsTech/olt-dotnet-utility-libraries/actions/workflows/build.yml/badge.svg)](https://github.com/OuterlimitsTech/olt-dotnet-utility-libraries/actions/workflows/build.yml) 


### Features
- Filter Assemblies: Include or exclude assemblies based on name patterns.
- Dynamic Loading: Optionally load assemblies into the current AppDomain.
- Chainable API: Utilizes a builder pattern for flexible configuration.
- Distinct List: Ensures a distinct list of assemblies is returned after filtering


### Installation

Add the AssemblyScanner source files to your .NET project or include it as a class library.

```bash
dotnet add package OLT.Utility.AssemblyScanner
```


### Usage

Here's a basic example of how to use the <code>OltAssemblyScanBuilder</code> to filter and load assemblies:
```csharp
using OLT.Utility

class Program
{
    static void Main()
    {
        var assemblies = new OltAssemblyScanBuilder()
            .IncludeFilters("OLT.", "MyApp.")
            .LoadAssemblies()
            .Build();
        
        foreach (var assembly in assemblies)
        {
            Console.WriteLine($"Loaded Assembly: {assembly.FullName}");
        }
    }
}
```

### Including and Excluding Assemblies

You can include and exclude assemblies using patterns:
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
            .DeepScan()
            .LoadAssemblies()
            .Build();
        
        foreach (var assembly in assemblies)
        {
            Console.WriteLine($"Loaded Assembly: {assembly.FullName}");
        }
    }
}
```

### Loading Assemblies
The LoadAssemblies() method will attempt to load the filtered assemblies into the current AppDomain. You can remove this step if you only need the assembly names without actually loading them.

### Methods

#### IncludeFilter
Adds a filter to include assemblies based on a name pattern.
```csharp
IncludeFilter(string filter);
```

#### ExcludeFilter
Adds a filter to exclude assemblies based on a name pattern.
```csharp
ExcludeFilter(string filter);
```

#### DeepScan
Scans all referenced assemblies for the included filter
```csharp
DeepScan(string filter);
```

#### Build
Filters the assemblies based on the specified criteria and returns a distinct list.
```csharp
IEnumerable<Assembly> Build();
```
