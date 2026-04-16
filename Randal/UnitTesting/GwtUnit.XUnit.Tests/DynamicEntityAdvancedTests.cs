namespace GwtUnit.XUnit.Tests;

public sealed class DynamicEntityComparerTests
{
	[Fact, PositiveTest]
	public void ShouldFindMember_WhenAccessingWithDifferentCase_GivenDefaultComparer()
	{
		var entity = new DynamicEntity(MissingMemberBehavior.ReturnsNull);
		entity["FirstName"] = "Alice";

		entity.TestForMember("firstname").Should().BeTrue();
		entity.TestForMember("FIRSTNAME").Should().BeTrue();
		entity.TestForMember("FirstName").Should().BeTrue();
	}

	[Fact, PositiveTest]
	public void ShouldReturnValue_WhenDynamicAccessUsesLowerCase_GivenDefaultComparer()
	{
		dynamic entity = new DynamicEntity(MissingMemberBehavior.ReturnsNull);
		entity.FirstName = "Alice";

		string result = entity.firstname;

		result.Should().Be("Alice");
	}

	[Fact, PositiveTest]
	public void ShouldFindMemberByOriginalCase_WhenUsingCaseSensitiveComparer()
	{
		var entity = new DynamicEntity(MissingMemberBehavior.ReturnsNull, null, StringComparer.Ordinal);
		entity["FirstName"] = "Alice";

		entity.TestForMember("FirstName").Should().BeTrue();
		entity.TestForMember("firstname").Should().BeFalse();
		entity.TestForMember("FIRSTNAME").Should().BeFalse();
	}

	[Fact, PositiveTest]
	public void ShouldReturnNull_WhenDynamicAccessUsesWrongCase_GivenCaseSensitiveComparer()
	{
		dynamic entity = new DynamicEntity(MissingMemberBehavior.ReturnsNull, null, StringComparer.Ordinal);
		entity.FirstName = "Alice";

		string? result = entity.firstname;

		result.Should().BeNull();
	}

	[Fact, PositiveTest]
	public void ShouldReturnCorrectValue_WhenDynamicAccessMatchesExactCase_GivenCaseSensitiveComparer()
	{
		dynamic entity = new DynamicEntity(MissingMemberBehavior.ReturnsNull, null, StringComparer.Ordinal);
		entity.FirstName = "Alice";

		string result = entity.FirstName;

		result.Should().Be("Alice");
	}
}
