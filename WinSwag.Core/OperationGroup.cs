using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WinSwag.Core
{
    public class OperationGroup : IReadOnlyList<Operation>
    {
        private readonly ImmutableList<Operation> _operations;

        public string Name { get; }

        public int Count => _operations.Count;

        public Operation this[int index] => _operations[index];

        public IEnumerator<Operation> GetEnumerator() => _operations.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public OperationGroup(string name, IEnumerable<Operation> operations)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _operations = operations?.ToImmutableList() ?? throw new ArgumentNullException(nameof(operations));
        }
    }
}
