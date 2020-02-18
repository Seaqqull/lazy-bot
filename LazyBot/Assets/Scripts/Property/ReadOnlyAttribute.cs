using UnityEngine;


/// <summary>
/// Displays a read-only field in the inspector.
/// Doesn't work with CustomPropertyDrawers.
/// For group read-only restriction use <see cref="BeginReadOnlyGroupAttribute"/>
/// and <see cref="EndReadOnlyGroupAttribute"/>.
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute { }

/// <summary>
/// Begins a read-only group of one or more fields in the inspector.
/// Used with <see cref="EndReadOnlyGroupAttribute"/> to close the group.
/// Works with CustomPropertyDrawers.
/// </summary>
public class BeginReadOnlyGroupAttribute : PropertyAttribute { }

/// <summary>
/// Closes the read-only group and resume editable fields.
/// Used with <see cref="BeginReadOnlyGroupAttribute"/>.
/// </summary>
public class EndReadOnlyGroupAttribute : PropertyAttribute { }
