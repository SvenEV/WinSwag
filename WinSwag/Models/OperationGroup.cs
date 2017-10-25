using System;
using System.Collections.Generic;

namespace WinSwag.Models
{
    public class OperationGroup
    {
        public string Name { get; }

        public IReadOnlyList<Operation> Operations { get; }

        public OperationGroup(string name, IReadOnlyList<Operation> operations)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Operations = operations ?? throw new ArgumentNullException(nameof(operations));
        }

        public override string ToString() => $"{Name} ({Operations.Count} operations)";
    }
}
