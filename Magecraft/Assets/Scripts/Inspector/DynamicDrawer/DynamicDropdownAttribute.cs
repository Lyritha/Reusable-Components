using UnityEngine;

namespace Lyrith.Inspector.DynamicDropdown
{
    /// <summary>
    /// Displays a dropdown in the inspector, populated by invoking a method at draw time.
    /// </summary>
    /// <remarks>
    /// <para><b>Requirements:</b></para>
    /// <list type="bullet">
    /// <item>Target method is on the same object as the field.</item>
    /// <item>Takes no parameters.</item>
    /// <item>Returns <see cref="IEnumerable{T}"/> of (T value, string label) tuples.</item>
    /// </list>
    /// <para><b>Supports:</b></para>
    /// <list type="bullet">
    /// <item>T backed by string, int, float, Unity Object reference, or <c>[SerializeReference]</c>.</item>
    /// <item>Plain serializable structs/classes as T.</item>
    /// </list>
    /// <para><b>Limitations:</b></para>
    /// <list type="bullet">
    /// <item>No parameterized methods.</item>
    /// <item>Non-string second tuple element is rejected.</item>
    /// <item>Invalid return shape shows a fallback label instead of a dropdown.</item>
    /// </list>
    /// </remarks>
    public class DynamicDropdownAttribute : PropertyAttribute
    {
        public string MethodName { get; }

        public DynamicDropdownAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}
