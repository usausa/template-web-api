namespace Template.ApiServer.Infrastructure.Http;

using Template.ApiServer.Host.Infrastructure.Http;

public sealed class EntityTagTests
{
    [Fact]
    public void FromReturnsQuotedVersion()
    {
        // Act
        var tag = EntityTag.From(3);

        // Assert
        Assert.Equal("\"3\"", tag);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("*")]
    public void TryParseIfMatchWithoutConditionReturnsNull(string? value)
    {
        // Act
        var parsed = EntityTag.TryParseIfMatch(value, out var version);

        // Assert
        Assert.True(parsed);
        Assert.Null(version);
    }

    [Fact]
    public void TryParseIfMatchReturnsVersion()
    {
        // Act
        var parsed = EntityTag.TryParseIfMatch("\"3\"", out var version);

        // Assert
        Assert.True(parsed);
        Assert.Equal(3, version);
    }

    [Theory]
    [InlineData("W/\"3\"")]
    [InlineData("\"abc\"")]
    [InlineData("3")]
    public void TryParseIfMatchWithUnmatchableTagReturnsFalse(string value)
    {
        // Act
        var parsed = EntityTag.TryParseIfMatch(value, out _);

        // Assert
        Assert.False(parsed);
    }
}
