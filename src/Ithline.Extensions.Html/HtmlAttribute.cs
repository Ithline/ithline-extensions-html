using System;
using System.Diagnostics;

namespace Ithline.Extensions.Html
{
    /// <summary>
    /// Represents a HTML attribute.
    /// </summary>
    [DebuggerDisplay("{Value}", Name = "{Name,nq}")]
    public sealed class HtmlAttribute
    {
        internal HtmlAttribute(string name, string? value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            Name = name;
            Value = value?.Replace("\"", "&quot;");
        }

        /// <summary>
        /// Gets the attribute name.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        public string? Value { get; }
    }
}
