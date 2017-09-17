using Dispel;
using Xunit;

public class Parse
{
    [Fact]
    public void Line()
    {
        var l1 = @"01[09:20] <03gm> uncolored text";
        var p1 = LogParser.Line(l1);
        Assert.True(p1.IsSuccess);

        var l2 = @"[09:41] <03player> 14colored text";
        var p2 = LogParser.Line(l2);
        Assert.True(p2.IsSuccess);
    }
}
