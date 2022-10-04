#if NET48
namespace SlnExplorer;

using System;
using System.Diagnostics;
using System.Reflection;
using Contracts;

/// <summary>
/// Provides tools to manipulate types and properties.
/// </summary>
internal static class ReflectionTools
{
    /// <summary>
    /// Gets a type used to manipulate projects and solutions.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <returns>The project in solution type.</returns>
    public static Type GetProjectInSolutionType(string typeName)
    {
        string FullTypeName = $"Microsoft.Build.Construction.{typeName}, Microsoft.Build, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a";
        Contract.RequireNotNull(Type.GetType(FullTypeName, false, false), out Type Result);
        return Result;
    }

    /// <summary>
    /// Gets the property of a given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The property.</returns>
    public static PropertyInfo GetTypeProperty(Type type, string propertyName)
    {
        PropertyInfo? Property = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
        Contract.RequireNotNull(Property, out PropertyInfo Result);

        return Result;
    }

    /// <summary>
    /// Gets the method of a given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="methodName">The property name.</param>
    /// <returns>The method.</returns>
    public static MethodInfo GetTypeMethod(Type type, string methodName)
    {
        MethodInfo? Method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        Contract.RequireNotNull(Method, out MethodInfo Result);

        return Result;
    }

    /// <summary>
    /// Gets the first constructor of a type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The constructor.</returns>
    public static ConstructorInfo GetFirstTypeConstructor(Type type)
    {
        ConstructorInfo[] Constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
        Debug.Assert(Constructors.Length > 0, "At least one constructor is expected");

        return Constructors[0];
    }

    /// <summary>
    /// Gets the value of a given object property.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="obj">The object.</param>
    /// <returns>The property value.</returns>
    public static object GetPropertyValue(PropertyInfo property, object obj)
    {
        object? Value = property.GetValue(obj);
        Contract.RequireNotNull(Value, out object Result);

        return Result;
    }
}
#endif