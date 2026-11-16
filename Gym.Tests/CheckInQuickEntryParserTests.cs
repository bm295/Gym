using Gym.Application.CheckIns;

namespace Gym.Tests;

public sealed class CheckInQuickEntryParserTests
{
    [Theory]
    [InlineData("MEM-1001", "MEM-1001")]
    [InlineData("0901234567", "0901234567")]
    [InlineData("checkinMEM-1001", "MEM-1001")]
    [InlineData("check in MEM-1001", "MEM-1001")]
    [InlineData("CHECKIN0901234567", "0901234567")]
    [InlineData("memberMEM-1002", "MEM-1002")]
    [InlineData("checkinMEM1001", "MEM1001")]
    [InlineData("checkin", "checkin")]
    [InlineData("checkinUNKNOWN", "checkinUNKNOWN")]
    [InlineData("not-a-member", "not-a-member")]
    [InlineData("checkinMEM-1001 more", "checkinMEM-1001 more")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    public void Extract_member_lookup_preserves_unrecognized_input(string input, string expected)
    {
        Assert.Equal(expected, CheckInQuickEntryParser.ExtractMemberLookup(input));
    }

    [Fact]
    public void Excessively_long_input_is_not_segmented()
    {
        var input = "checkin" + new string('1', 200);
        Assert.Equal(input, CheckInQuickEntryParser.ExtractMemberLookup(input));
    }

    [Fact]
    public void Parsing_is_deterministic_and_does_not_depend_on_external_dictionary()
    {
        Assert.Equal("MEM-1001", CheckInQuickEntryParser.ExtractMemberLookup("checkcheckinMEM-1001"));
    }
}
