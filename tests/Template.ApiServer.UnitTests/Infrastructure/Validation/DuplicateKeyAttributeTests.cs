namespace Template.ApiServer.Infrastructure.Validation;

using BunnyTail.MemberAccessor;

[GenerateAccessor]
public sealed class DuplicateKeyEntry
{
    public string? Code { get; set; }

    public int Branch { get; set; }

    public IReadOnlyList<string>? Tags { get; set; }
}

public sealed class DuplicateKeyWithoutAccessorEntry
{
    public string? Code { get; set; }
}

public sealed class DuplicateKeyAttributeTests
{
    [Fact]
    public void UniqueKeysAreValid()
    {
        // Arrange
        var attribute = new DuplicateKeyAttribute(nameof(DuplicateKeyEntry.Code));
        var entries = new[] { new DuplicateKeyEntry { Code = "A" }, new DuplicateKeyEntry { Code = "B" }, new DuplicateKeyEntry(), null };

        // Act
        var valid = attribute.IsValid(entries);

        // Assert
        Assert.True(valid);
    }

    [Fact]
    public void DuplicateKeysAreInvalid()
    {
        // Arrange
        var attribute = new DuplicateKeyAttribute(nameof(DuplicateKeyEntry.Code));
        var entries = new[] { new DuplicateKeyEntry { Code = "A" }, new DuplicateKeyEntry { Code = "A" } };

        // Act
        var valid = attribute.IsValid(entries);

        // Assert
        Assert.False(valid);
    }

    [Fact]
    public void CompositeKeysAreCompared()
    {
        // Arrange
        var attribute = new DuplicateKeyAttribute(nameof(DuplicateKeyEntry.Code), nameof(DuplicateKeyEntry.Branch));
        var unique = new[] { new DuplicateKeyEntry { Code = "A", Branch = 1 }, new DuplicateKeyEntry { Code = "A", Branch = 2 } };
        var duplicate = new[] { new DuplicateKeyEntry { Code = "A", Branch = 1 }, new DuplicateKeyEntry { Code = "A", Branch = 1 } };

        // Act
        var valid = attribute.IsValid(unique);
        var invalid = attribute.IsValid(duplicate);

        // Assert
        Assert.True(valid);
        Assert.False(invalid);
    }

    [Fact]
    public void ElementsOfListKeyAreCompared()
    {
        // Arrange
        var attribute = new DuplicateKeyAttribute(nameof(DuplicateKeyEntry.Tags));
        var unique = new[] { new DuplicateKeyEntry { Tags = ["a", "b"] }, new DuplicateKeyEntry { Tags = ["c"] } };
        var duplicate = new[] { new DuplicateKeyEntry { Tags = ["a", "b"] }, new DuplicateKeyEntry { Tags = ["b"] } };

        // Act
        var valid = attribute.IsValid(unique);
        var invalid = attribute.IsValid(duplicate);

        // Assert
        Assert.True(valid);
        Assert.False(invalid);
    }

    [Fact]
    public void FormatErrorMessageUsesMembers()
    {
        // Act
        var message = new DuplicateKeyAttribute(nameof(DuplicateKeyEntry.Code), nameof(DuplicateKeyEntry.Branch)).FormatErrorMessage("Entries");

        // Assert
        Assert.Equal("The field Entries must not contain duplicate Code, Branch.", message);
    }

    [Fact]
    public void ElementWithoutAccessorThrows()
    {
        // Arrange
        var attribute = new DuplicateKeyAttribute(nameof(DuplicateKeyWithoutAccessorEntry.Code));
        var entries = new[] { new DuplicateKeyWithoutAccessorEntry { Code = "A" } };

        // Act
        var exception = Record.Exception(() => attribute.IsValid(entries));

        // Assert
        Assert.IsType<InvalidOperationException>(exception);
    }
}
