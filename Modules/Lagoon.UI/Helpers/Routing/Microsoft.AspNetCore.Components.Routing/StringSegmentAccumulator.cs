// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Lagoon.UI.Helpers.Route;

// This is very similar to Microsoft.Extensions.Primitives.StringValues, except it works in terms
// of ReadOnlyMemory<char> rather than string, so the querystring handling logic doesn't need to
// allocate per-value when tracking things that will be parsed as value types.
internal struct StringSegmentAccumulator
{
    private int _count;
    private ReadOnlyMemory<char> _single;
    private List<ReadOnlyMemory<char>> _multiple;

    public ReadOnlyMemory<char> this[int index]
    {
        get
        {
            if (index >= _count)
            {
                throw new IndexOutOfRangeException();
            }

            return _count == 1 ? _single : _multiple![index];
        }
    }

    public int Count => _count;

    public void SetSingle(ReadOnlyMemory<char> value)
    {
        _single = value;

        if (_count != 1)
        {
            if (_count > 1)
            {
                _multiple = null;
            }

            _count = 1;
        }
    }

    public void Add(ReadOnlyMemory<char> value)
    {
        switch (_count++)
        {
            case 0:
                _single = value;
                break;
            case 1:
                _multiple = new()
                {
                    _single,
                    value
                };
                _single = default;
                break;
            default:
                _multiple!.Add(value);
                break;
        }
    }
}