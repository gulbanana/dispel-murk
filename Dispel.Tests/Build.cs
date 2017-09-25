using Dispel;
using Dispel.AST;
using System;
using System.Linq;
using Xunit;

public class Build
{
    [Fact]
    public void MessageHeader()
    {
        var input = "[00:01] <\x000303player1\x000f> ";
        var r = LogParser.Header(input);
        var ast = r.Tree.Build<MessageHeader>();

        Assert.Equal("player1", ast.Username);
        Assert.Equal("00:01", ast.Timestamp);
    }

    [Fact]
    public void MessageHeaderWithReset()
    {
        var input = "\x000301[00:01] <\x000303player1\x000f> ";
        var r = LogParser.Header(input);
        var ast = r.Tree.Build<MessageHeader>();

        Assert.Equal("player1", ast.Username);
        Assert.Equal("00:01", ast.Timestamp);
    }

    [Fact]
    public void SimpleBody()
    {
        var input = "foo";
        var r = LogParser.AttributedText(input);
        var ast = r.Tree.Build<MessageBody>();

        Assert.Equal("foo", ast.Flatten());
    }

    [Fact]
    public void ComplexBody()
    {
        var input = "foo\x0002bar";
        var r = LogParser.AttributedText(input);
        var ast = r.Tree.Build<MessageBody>();

        Assert.Equal("foobar", ast.Flatten());
    }

    [Fact]
    public void SimpleMessage()
    {
        var input = "[00:01] <\x000303player1\x000f> foo" + Environment.NewLine;
        var r = LogParser.MessageLine(input);
        var ast = r.Tree.Build<Message>();

        Assert.Equal("player1", ast.Header.Username);
        Assert.Equal("00:01", ast.Header.Timestamp);
        Assert.Equal("foo", ast.Body.Flatten());
    }

    [Fact]
    public void ComplexMessage()
    {
        var input = @"[10:15] <03Quaker> 13Claudio doesn’t move. Both he and Bianca are too polite to do anything except smile pleasantly. “Good evening.” 13The man spoke in English, and Claudio does as well: crisp, only very slightly accented. " + Environment.NewLine;
        var r = LogParser.MessageLine(input);

        var ast = r.Tree.Build<Message>();
        Assert.Equal(3, ast.Body.Runs.Length);

        var r0 = ast.Body.Runs[0];
        Assert.Single(r0.Attributes);
        Assert.Equal(AttributeType.Color, r0.Attributes.Single().Flag);
        Assert.Equal("13", r0.Attributes.Single().Options);

        var r1 = ast.Body.Runs[1];
        Assert.Single(r1.Attributes);
        Assert.Equal(AttributeType.Color, r0.Attributes.Single().Flag);
        Assert.Null(r1.Attributes.Single().Options);
    }

    [Fact]
    public void MultilineSession()
    {
        var input = "[00:01] <\x000303player1\x000f> foo" + Environment.NewLine + "[00:02] <\x000303player2\x000f> bar" + Environment.NewLine;
        var r = LogParser.SessionBody(input);
        var ast = r.Tree.Build<SessionBody>();
    }
}
