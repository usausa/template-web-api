namespace Template.ApiServer.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
public sealed class DuplicateKeyAttribute : ValidationAttribute
{
    private readonly string[] members;

    public IReadOnlyList<string> Members => members;

    public override object TypeId => this;

    public DuplicateKeyAttribute(params string[] members)
        : base(static () => ValidationMessages.DuplicateKey)
    {
        this.members = members;
    }

    public override bool IsValid(object? value)
    {
        if (members.Length == 0)
        {
            throw new InvalidOperationException("Member is not specified.");
        }

        if ((value is not IEnumerable values) || (value is string))
        {
            return true;
        }

        return members.Length == 1 ? IsUnique(values, members[0]) : IsUniqueComposite(values, members);
    }

    public override string FormatMessage(string format, string name) =>
        String.Format(CultureInfo.CurrentCulture, format, name, String.Join(", ", members));

    private static bool IsUnique(IEnumerable values, string member)
    {
        var keys = new HashSet<object>();
        foreach (var item in values)
        {
            if (item is null)
            {
                continue;
            }

            var key = ValidationHelper.GetValue(item, member);
            if ((key is IEnumerable elements) && (key is not string))
            {
                foreach (var element in elements)
                {
                    if ((element is not null) && !keys.Add(element))
                    {
                        return false;
                    }
                }
            }
            else if ((key is not null) && !keys.Add(key))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsUniqueComposite(IEnumerable values, string[] members)
    {
        var keys = new HashSet<object?[]>(CompositeKeyComparer.Instance);
        foreach (var item in values)
        {
            if (item is null)
            {
                continue;
            }

            var key = new object?[members.Length];
            for (var i = 0; i < members.Length; i++)
            {
                key[i] = ValidationHelper.GetValue(item, members[i]);
            }

            if (key.All(static x => x is not null) && !keys.Add(key))
            {
                return false;
            }
        }

        return true;
    }

    private sealed class CompositeKeyComparer : IEqualityComparer<object?[]>
    {
        public static CompositeKeyComparer Instance { get; } = new();

        public bool Equals(object?[]? x, object?[]? y) =>
            StructuralComparisons.StructuralEqualityComparer.Equals(x, y);

        public int GetHashCode(object?[] obj) =>
            StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
    }
}
