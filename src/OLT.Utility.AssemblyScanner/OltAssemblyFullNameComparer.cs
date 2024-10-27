using System.Reflection;

namespace OLT.Utility.AssemblyScanner;


#if NET6_0_OR_GREATER

/// <summary>
/// Compares two <see cref="Assembly"/> objects for equality based on their full names, ignoring case differences.
/// </summary>
public class OltAssemblyFullNameComparer : IEqualityComparer<Assembly>
{

    /// <summary>
    /// Determines whether the specified <see cref="Assembly"/> objects are equal by comparing their full names, ignoring case.
    /// </summary>
    /// <param name="x">The first <see cref="Assembly"/> to compare.</param>
    /// <param name="y">The second <see cref="Assembly"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the full names of the specified <see cref="Assembly"/> objects are equal; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(Assembly? x, Assembly? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
        return string.Equals(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="Assembly"/>, based on its full name.
    /// </summary>
    /// <param name="obj">The <see cref="Assembly"/> for which a hash code is to be generated.</param>
    /// <returns>
    /// A hash code for the <see cref="Assembly"/>, or 0 if the full name is <c>null</c>.
    /// </returns>
    public int GetHashCode(Assembly obj)
    {
        return obj.FullName?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;
    }
}

#else
/// <summary>
/// Compares two <see cref="Assembly"/> objects for equality based on their full names, ignoring case differences.
/// </summary>
public class OltAssemblyFullNameComparer : IEqualityComparer<Assembly>
{
    /// <summary>
    /// Determines whether the specified <see cref="Assembly"/> objects are equal by comparing their full names, ignoring case.
    /// </summary>
    /// <param name="x">The first <see cref="Assembly"/> to compare.</param>
    /// <param name="y">The second <see cref="Assembly"/> to compare.</param>
    /// <returns>
    /// <c>true</c> if the full names of the specified <see cref="Assembly"/> objects are equal; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(Assembly? x, Assembly? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
        return string.Equals(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns a hash code for the specified <see cref="Assembly"/>, based on its full name.
    /// </summary>
    /// <param name="obj">The <see cref="Assembly"/> for which a hash code is to be generated.</param>
    /// <returns>
    /// A hash code for the <see cref="Assembly"/>, or 0 if the full name is <c>null</c>.
    /// </returns>
    public int GetHashCode(Assembly obj)
    {
        return obj.FullName?.GetHashCode() ?? 0;
    }
}

#endif
