using Dispel;
using System;
using Xunit;

namespace Parse
{
    public class AttributedText
    {
        [Fact]
        public void UnattributedRun()
        {
            var text = "foo";
            var r = LogParser.AttributedText(text);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void AttributedRun()
        {
            var text = "\x0002foo";
            var r = LogParser.AttributedText(text);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void MultipleAttributes()
        {
            var text = "\x0002\x001Dfoo";
            var r = LogParser.AttributedText(text);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void MultipleRuns()
        {
            var text = "foo\x0002bar";
            var r = LogParser.AttributedText(text);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void FinalAttribute()
        {
            var text = "foo\x0002";
            var r = LogParser.AttributedText(text);
            Assert.True(r.IsSuccess);
            Assert.Empty(r.Remainder);
        }

        [Fact]
        public void FinalAttributeWithoutRuns()
        {
            var text = "\x0002";
            var r = LogParser.AttributedText(text);
            Assert.True(r.IsSuccess);
            Assert.Empty(r.Remainder);
        }
    }

    public class Color
    {
        [Fact]
        public void Reset()
        {
            var text = "\x0003";
            var r = LogParser.SetColor(text);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void SingleDigit()
        {
            var text = "\x00031";
            var r = LogParser.SetColor(text);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void DoubleDigit()
        {
            var text = "\x000301";
            var r = LogParser.SetColor(text);
            Assert.True(r.IsSuccess);
        }

        [Theory, InlineData("1,1"), InlineData("01,1"), InlineData("1,01"), InlineData("01,01")]
        public void WithBackground(string input)
        {
            input = "\x0003" + input;
            var r = LogParser.SetColor(input);
            Assert.True(r.IsSuccess);
            Assert.Empty(r.Remainder);
        }
    }

    public class Header
    {
        [Fact]
        public void Basic()
        {
            var line = "[09:41] <\x000303player\x000f> ";
            var r = LogParser.Header(line);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void WithReset()
        {
            var line = "\x000301[09:20] <\x000303gm\x000f> ";
            var r = LogParser.Header(line);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void Chanop()
        {
            var line = "[09:41] <\x000303@player\x000f> ";
            var r = LogParser.Header(line);
            Assert.True(r.IsSuccess);
        }
    }

    public class Line
    {
        [Fact]
        public void NarrativeAfterDialogue()
        {
            var input = @"[13:02] <03Quaker> 1“Please, lead the way.” 13Claudio will follow the sailor.";
            var r = LogParser.Message(input);
            Assert.True(r.IsSuccess);
        }
    }

    public class Log
    {
        [Fact]
        public void Multiline()
        {
            var input = "[00:01] <\x000303player1\x000f> foo" + Environment.NewLine + "[00:02] <\x000303player2\x000f> bar" + Environment.NewLine;
            var r = LogParser.Log(input);
            Assert.True(r.IsSuccess);
            Assert.Empty(r.Remainder);
        }
    }

}
