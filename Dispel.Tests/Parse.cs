using Dispel;
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
    }

    public class Color
    {
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
            var input = @"[13:02] <03Quaker> 1“Please, lead the way.” 13Claudio will follow the sailor. ";
            var r = LogParser.Line(input);
            Assert.True(r.IsSuccess);
        }
    }
}
