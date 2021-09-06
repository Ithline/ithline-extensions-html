using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace Ithline.Extensions.Html
{
    /// <summary>
    /// Represents an immutable list of <see cref="HtmlNode"/> values.
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public sealed class HtmlNodeList : HtmlNode, IReadOnlyList<HtmlNode>
    {
        public static readonly HtmlNodeList Empty = new();
        private readonly ImmutableList<HtmlNode> _nodes;

        private HtmlNodeList(ImmutableList<HtmlNode>? nodes = null)
        {
            _nodes = nodes ?? ImmutableList<HtmlNode>.Empty;
        }

        /// <summary>
        /// Gets the number of nodes in the list.
        /// </summary>
        public int Count => _nodes.Count;
        /// <summary>
        /// Gets the <see cref="HtmlNode"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the node to retrieve.</param>
        /// <returns>The node at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is negative or not less than <see cref="Count"/>.</exception>
        public HtmlNode this[int index] => _nodes[index];

        /// <summary>
        /// Creates a <see cref="HtmlNodeList"/> with the specified nodes.
        /// </summary>
        /// <param name="nodes">List of nodes.</param>
        /// <returns>A <see cref="HtmlNodeList"/> with the specified nodes.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="nodes"/> is <see langword="null"/>.</exception>
        public static HtmlNodeList CreateRange(IEnumerable<HtmlNode?> nodes)
        {
            if (nodes is null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            if (nodes is HtmlNodeList nodeList)
            {
                return nodeList;
            }

            var builder = ImmutableList.CreateBuilder<HtmlNode>();
            foreach (var node in nodes)
            {
                if (node is null)
                {
                    continue;
                }

                if (node is IEnumerable<HtmlNode> range)
                {
                    builder.AddRange(range);
                }
                else
                {
                    builder.Add(node);
                }
            }
            return new HtmlNodeList(builder.ToImmutable());
        }

        /// <summary>
        /// Adds the specified node to the end of the list.
        /// </summary>
        /// <param name="node">Node to add.</param>
        /// <returns>A new <see cref="HtmlNodeList"/> with the node added.</returns>
        public HtmlNodeList Add(HtmlNode? node)
        {
            if (node is null)
            {
                return this;
            }

            if (node is IEnumerable<HtmlNode> list)
            {
                return this.Wrap(_nodes.AddRange(list));
            }

            return this.Wrap(_nodes.Add(node));
        }

        /// <summary>
        /// Returns an empty <see cref="HtmlNodeList"/>.
        /// </summary>
        /// <returns>An empty <see cref="HtmlNodeList"/>.</returns>
        public HtmlNodeList Clear() => Empty;

        /// <summary>
        /// Removes the specified node from the list.
        /// </summary>
        /// <param name="node">Node to remove.</param>
        /// <returns>A new <see cref="HtmlNodeList"/> with the node removed.</returns>
        public HtmlNodeList Remove(HtmlNode node)
        {
            if (node is HtmlNodeList list)
            {
                return this.Wrap(_nodes.RemoveRange(list));
            }

            return this.Wrap(_nodes.Remove(node));
        }

        /// <inheritdoc/>
        public override void WriteTo(TextWriter output)
        {
            foreach (var node in _nodes)
            {
                node.WriteTo(output);
            }
        }

        /// <inheritdoc/>
        public IEnumerator<HtmlNode> GetEnumerator()
        {
            foreach (var node in _nodes)
            {
                yield return node;
            }
        }
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private HtmlNodeList Wrap(ImmutableList<HtmlNode> nodes)
        {
            if (nodes is null || nodes.IsEmpty)
            {
                return Empty;
            }
            else if (nodes == _nodes)
            {
                return this;
            }
            else
            {
                return new HtmlNodeList(nodes);
            }
        }

        private sealed class DebugView
        {
            private readonly HtmlNodeList _list;

            public DebugView(HtmlNodeList list)
            {
                _list = list ?? throw new ArgumentNullException(nameof(list));
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public HtmlNode[] Items
            {
                get
                {
                    var items = new HtmlNode[_list.Count];
                    _list._nodes.CopyTo(items, 0);
                    return items;
                }
            }
        }
    }
}
