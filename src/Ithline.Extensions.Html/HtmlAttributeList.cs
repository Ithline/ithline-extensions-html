using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Ithline.Extensions.Html
{
    /// <summary>
    /// Represents an immutable list of <see cref="HtmlAttribute"/> values.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public sealed class HtmlAttributeList : IReadOnlyCollection<HtmlAttribute>
    {
        /// <summary>
        /// Gets an empty <see cref="HtmlAttributeList"/>.
        /// </summary>
        public static readonly HtmlAttributeList Empty = new HtmlAttributeList();
        private readonly ImmutableDictionary<string, HtmlAttribute> _attributes;

        private HtmlAttributeList(ImmutableDictionary<string, HtmlAttribute>? attributes = null)
        {
            _attributes = attributes ?? ImmutableDictionary.Create<string, HtmlAttribute>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the number of attributes in the list.
        /// </summary>
        public int Count => _attributes.Count;
        /// <summary>
        /// Gets the <see cref="HtmlAttribute"/> with the specified name.
        /// </summary>
        /// <param name="name">Name of the attribute.</param>
        /// <returns>The <see cref="HtmlAttribute"/> with the specified name if exists; otherwise, <see langword="null"/>.</returns>
        public HtmlAttribute? this[string name] => _attributes.TryGetValue(name, out var attribute) ? attribute : null;

        /// <summary>
        /// Creates a <see cref="HtmlAttributeList"/> with the specified attribute.
        /// </summary>
        /// <param name="attribute">Attribute to set.</param>
        /// <returns>A <see cref="HtmlAttributeList"/> with the specified attribute.</returns>
        public static HtmlAttributeList Create(HtmlAttribute attribute)
        {
            return Empty.Set(attribute);
        }

        /// <summary>
        /// Creates a <see cref="HtmlAttributeList"/> with the specified attribute.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="value">Attribute value.</param>
        /// <returns>A <see cref="HtmlAttributeList"/> with the specified attribute.</returns>
        /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or empty.</exception>
        public static HtmlAttributeList Create(string name, string? value)
        {
            return Empty.Set(HtmlNode.Attribute(name, value));
        }

        /// <summary>
        /// Creates a <see cref="HtmlAttributeList"/> with the specified attributes.
        /// </summary>
        /// <param name="attributes">List of attributes.</param>
        /// <returns>A <see cref="HtmlAttributeList"/> with the specified attributes.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="attributes"/> is <see langword="null"/>.</exception>
        public static HtmlAttributeList CreateRange(IEnumerable<HtmlAttribute?> attributes)
        {
            if (attributes is null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }

            if (attributes is HtmlAttributeList attributeList)
            {
                return attributeList;
            }

            var builder = ImmutableDictionary.CreateBuilder<string, HtmlAttribute>(StringComparer.OrdinalIgnoreCase);
            foreach (var node in attributes)
            {
                if (node is null)
                {
                    continue;
                }
                builder[node.Name] = node;
            }
            return new HtmlAttributeList(builder.ToImmutable());
        }

        /// <summary>
        /// Sets the specified attribute in the list, possibly overwriting an existing value.
        /// </summary>
        /// <param name="attribute">Attribute to set.</param>
        /// <returns>A new <see cref="HtmlAttributeList"/> that contains the specified attribute.</returns>
        public HtmlAttributeList Set(HtmlAttribute? attribute)
        {
            return attribute is null ? this : this.Wrap(_attributes.SetItem(attribute.Name, attribute));
        }

        /// <summary>
        /// Returns an empty <see cref="HtmlAttributeList"/>.
        /// </summary>
        /// <returns>An empty <see cref="HtmlAttributeList"/>.</returns>
        public HtmlAttributeList Clear() => Empty;

        /// <summary>
        /// Removes the attribute from the <see cref="HtmlAttributeList"/>.
        /// </summary>
        /// <param name="attribute">Attribute to remove.</param>
        /// <returns>A new <see cref="HtmlAttributeList"/> with the specified attribute removed.</returns>
        public HtmlAttributeList Remove(HtmlAttribute? attribute)
        {
            return attribute is null ? this : this.Wrap(_attributes.Remove(attribute.Name));
        }

        /// <inheritdoc/>
        public IEnumerator<HtmlAttribute> GetEnumerator() => _attributes.Values.GetEnumerator();
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private HtmlAttributeList Wrap(ImmutableDictionary<string, HtmlAttribute> attributes)
        {
            if (attributes is null || attributes.IsEmpty)
            {
                return Empty;
            }
            else if (attributes == _attributes)
            {
                return this;
            }
            else
            {
                return new HtmlAttributeList(attributes);
            }
        }

        private sealed class DebugView
        {
            private readonly HtmlAttributeList _list;

            public DebugView(HtmlAttributeList list)
            {
                _list = list ?? throw new ArgumentNullException(nameof(list));
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public HtmlAttribute[] Items
            {
                get
                {
                    var i = 0;
                    var items = new HtmlAttribute[_list.Count];
                    foreach (var item in _list)
                    {
                        items[i++] = item;
                    }
                    return items;
                }
            }
        }
    }
}
