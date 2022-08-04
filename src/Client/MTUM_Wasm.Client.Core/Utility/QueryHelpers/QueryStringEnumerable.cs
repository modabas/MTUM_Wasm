﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace MTUM_Wasm.Client.Core.Utility.QueryHelpers;

/// <summary>
/// An enumerable that can supply the name/value pairs from a URI query string.
/// </summary>

public readonly struct QueryStringEnumerable
{
    private readonly ReadOnlyMemory<char> _queryString;

    /// <summary>
    /// Constructs an instance of <see cref="QueryStringEnumerable"/>.
    /// </summary>
    /// <param name="queryString">The query string.</param>
    public QueryStringEnumerable(string? queryString)
        : this(queryString.AsMemory())
    {
    }

    /// <summary>
    /// Constructs an instance of <see cref="QueryStringEnumerable"/>.
    /// </summary>
    /// <param name="queryString">The query string.</param>
    public QueryStringEnumerable(ReadOnlyMemory<char> queryString)
    {
        _queryString = queryString;
    }

    /// <summary>
    /// Retrieves an object that can iterate through the name/value pairs in the query string.
    /// </summary>
    /// <returns>An object that can iterate through the name/value pairs in the query string.</returns>
    public Enumerator GetEnumerator()
        => new Enumerator(_queryString);

    /// <summary>
    /// Represents a single name/value pair extracted from a query string during enumeration.
    /// </summary>
    public readonly struct EncodedNameValuePair
    {
        /// <summary>
        /// Gets the name from this name/value pair in its original encoded form.
        /// To get the decoded string, call <see cref="DecodeName"/>.
        /// </summary>
        public readonly ReadOnlyMemory<char> EncodedName { get; }

        /// <summary>
        /// Gets the value from this name/value pair in its original encoded form.
        /// To get the decoded string, call <see cref="DecodeValue"/>.
        /// </summary>
        public readonly ReadOnlyMemory<char> EncodedValue { get; }

        internal EncodedNameValuePair(ReadOnlyMemory<char> encodedName, ReadOnlyMemory<char> encodedValue)
        {
            EncodedName = encodedName;
            EncodedValue = encodedValue;
        }

        /// <summary>
        /// Decodes the name from this name/value pair.
        /// </summary>
        /// <returns>Characters representing the decoded name.</returns>
        public ReadOnlyMemory<char> DecodeName()
            => Decode(EncodedName);

        /// <summary>
        /// Decodes the value from this name/value pair.
        /// </summary>
        /// <returns>Characters representing the decoded value.</returns>
        public ReadOnlyMemory<char> DecodeValue()
            => Decode(EncodedValue);

        private static ReadOnlyMemory<char> Decode(ReadOnlyMemory<char> chars)
        {
            // If the value is short, it's cheap to check up front if it really needs decoding. If it doesn't,
            // then we can save some allocations.
            return chars.Length < 16 && chars.Span.IndexOfAny('%', '+') < 0
                ? chars
                : Uri.UnescapeDataString(SpanHelper.ReplacePlusWithSpace(chars.Span)).AsMemory();
        }
    }

    /// <summary>
    /// An enumerator that supplies the name/value pairs from a URI query string.
    /// </summary>
    public struct Enumerator
    {
        private ReadOnlyMemory<char> _query;

        internal Enumerator(ReadOnlyMemory<char> query)
        {
            Current = default;
            _query = query.IsEmpty || query.Span[0] != '?'
                ? query
                : query.Slice(1);
        }

        /// <summary>
        /// Gets the currently referenced key/value pair in the query string being enumerated.
        /// </summary>
        public EncodedNameValuePair Current { get; private set; }

        /// <summary>
        /// Moves to the next key/value pair in the query string being enumerated.
        /// </summary>
        /// <returns>True if there is another key/value pair, otherwise false.</returns>
        public bool MoveNext()
        {
            while (!_query.IsEmpty)
            {
                // Chomp off the next segment
                ReadOnlyMemory<char> segment;
                var delimiterIndex = _query.Span.IndexOf('&');
                if (delimiterIndex >= 0)
                {
                    segment = _query.Slice(0, delimiterIndex);
                    _query = _query.Slice(delimiterIndex + 1);
                }
                else
                {
                    segment = _query;
                    _query = default;
                }

                // If it's nonempty, emit it
                var equalIndex = segment.Span.IndexOf('=');
                if (equalIndex >= 0)
                {
                    Current = new EncodedNameValuePair(
                        segment.Slice(0, equalIndex),
                        segment.Slice(equalIndex + 1));
                    return true;
                }
                else if (!segment.IsEmpty)
                {
                    Current = new EncodedNameValuePair(segment, default);
                    return true;
                }
            }

            Current = default;
            return false;
        }
    }

    private static class SpanHelper
    {
        private static readonly SpanAction<char, IntPtr> s_replacePlusWithSpace = ReplacePlusWithSpaceCore;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string ReplacePlusWithSpace(ReadOnlySpan<char> span)
        {
            fixed (char* ptr = &MemoryMarshal.GetReference(span))
            {
                return string.Create(span.Length, (IntPtr)ptr, s_replacePlusWithSpace);
            }
        }

        private static unsafe void ReplacePlusWithSpaceCore(Span<char> buffer, IntPtr state)
        {
            fixed (char* ptr = &MemoryMarshal.GetReference(buffer))
            {
                var input = (ushort*)state.ToPointer();
                var output = (ushort*)ptr;

                var i = (nint)0;
                var n = (nint)(uint)buffer.Length;

                if (Sse41.IsSupported && n >= Vector128<ushort>.Count)
                {
                    var vecPlus = Vector128.Create('+');
                    var vecSpace = Vector128.Create(' ');

                    do
                    {
                        var vec = Sse2.LoadVector128(input + i);
                        var mask = Sse2.CompareEqual(vec, vecPlus);
                        var res = Sse41.BlendVariable(vec, vecSpace, mask);
                        Sse2.Store(output + i, res);
                        i += Vector128<ushort>.Count;
                    } while (i <= n - Vector128<ushort>.Count);
                }

                for (; i < n; ++i)
                {
                    if (input[i] != '+')
                    {
                        output[i] = input[i];
                    }
                    else
                    {
                        output[i] = ' ';
                    }
                }
            }
        }
    }
}