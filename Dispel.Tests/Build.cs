using Dispel;
using Dispel.AST;
using System;
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
    public void SimpleMessageBody()
    {
        var input = "foo";
        var r = LogParser.AttributedText(input);
        var ast = r.Tree.Build<MessageBody>();

        Assert.Equal("foo", ast.Text);
    }

    [Fact]
    public void ComplexMessageBody()
    {
        var input = "foo\x0002bar";
        var r = LogParser.AttributedText(input);
        var ast = r.Tree.Build<MessageBody>();

        Assert.Equal("foobar", ast.Text);
    }

    [Fact]
    public void SingleMessage()
    {
        var input = "[00:01] <\x000303player1\x000f> foo";
        var r = LogParser.Message(input);
        var ast = r.Tree.Build<Message>();

        Assert.Equal("player1", ast.Header.Username);
        Assert.Equal("00:01", ast.Header.Timestamp);
        Assert.Equal("foo", ast.Body.Text);
    }

    [Fact]
    public void MultilineSession()
    {
        var input = "[00:01] <\x000303player1\x000f> foo" + Environment.NewLine + "[00:02] <\x000303player2\x000f> bar" + Environment.NewLine;
        var r = LogParser.Log(input);
        var ast = r.Tree.Build<Log>();
    }
}
