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
            var r = LineParser.AttributedText(text);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void AttributedRun()
        {
            var text = "\x0002foo";
            var r = LineParser.AttributedText(text);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void MultipleAttributes()
        {
            var text = "\x0002\x001Dfoo";
            var r = LineParser.AttributedText(text);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void MultipleRuns()
        {
            var text = "foo\x0002bar";
            var r = LineParser.AttributedText(text);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void FinalAttribute()
        {
            var text = "foo\x0002";
            var r = LineParser.AttributedText(text);
            Assert.True(r.IsSuccess);
            Assert.Empty(r.Remainder);
        }

        [Fact]
        public void FinalAttributeWithoutRuns()
        {
            var text = "\x0002";
            var r = LineParser.AttributedText(text);
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
            var r = LineParser.SetColor(text);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void DoubleDigit()
        {
            var text = "\x000301";
            var r = LineParser.SetColor(text);
            Assert.True(r.IsSuccess);
        }

        [Theory, InlineData("1,1"), InlineData("01,1"), InlineData("1,01"), InlineData("01,01")]
        public void WithBackground(string input)
        {
            input = "\x0003" + input;
            var r = LineParser.SetColor(input);
            Assert.True(r.IsSuccess);
            Assert.Empty(r.Remainder);
        }
    }

    public class Username
    {
        [Fact]
        public void DumbCharacters()
        {
            var line = "<\x000303pipe|[]\x000f>";
            var r = LineParser.Username(line);
            Assert.True(r.IsSuccess);
        }


        [Fact]
        public void Chanop()
        {
            var line = "<\x000303@player\x000f> ";
            var r = LineParser.Username(line);
            Assert.True(r.IsSuccess);
        }
    }

    public class Line
    {
        [Fact]
        public void Basic()
        {
            var line = "[09:41] <\x000303player\x000f> ";
            var r = LineParser.Line(line);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void WithReset()
        {
            var line = "\x000301[09:20] <\x000303gm\x000f> ";
            var r = LineParser.Line(line);
            Assert.True(r.IsSuccess);
        }

        [Fact]
        public void Control()
        {
            var input = "\x000303[20:08] * Now talking in #aurora";
            var r = LineParser.Line(input);
            Assert.True(r.IsSuccess);
            Assert.Empty(r.Remainder);
        }

        [Fact]
        public void NarrativeMessage()
        {
            var input = @"[13:02] <03Quaker> 1“Please, lead the way.” 13Claudio will follow the sailor.";
            var r = LineParser.Line(input);
            Assert.True(r.IsSuccess);
            Assert.Empty(r.Remainder);
        }

        [Fact]
        public void InitialConnectPragma()
        {
            var text = "[12:26] #aurora created on Fri Jan 03 22:56:14 2020";
            var r = LineParser.Pragma(text);
            Assert.True(r.IsSuccess);
        }
    }

//    public class Log
//    {
//        [Fact]
//        public void CrossMidnight()
//        {
//            var input = @"
//Session Start: Tue Feb 07 22:10:15 2017
//Session Ident: #SonsAndDoctors
//03[22:10] * Now talking in #SonsAndDoctors
//03[22:10] * Topic is 'Exalted: Imperfect Circle (THU 8PM EDT) | Exalted: There Can Be No End (TUE 8PM EDT) | Shadowrun: The Looking-Glass War (MON 8PM EDT) | http://i.imgur.com/0XkYldr.png'
//03[22:10] * Set by VoxPVoxD!VoxPVoxD@sorcery-q06iee.res.rr.com on Tue Oct 25 10:19:06 2016
//Session Time: Wed Feb 08 00:00:00 2017
//03[01:55] * banana is now known as banana|sleep
//02[01:59] * Disconnected
//Session Close: Wed Feb 08 01:59:11 2017";

//            var r = LineParser.Log(input);
//            Assert.True(r.IsSuccess);
//            Assert.Empty(r.Remainder);
//        }

//        [Fact]
//        public void EarlyClose()
//        {
//            var input = @"
//Session Start: Tue Feb 07 20:48:15 2017
//Session Ident: #SonsAndDoctors
//03[20:48] * Rejoined channel #SonsAndDoctors
//03[20:48] * Topic is 'Exalted: Imperfect Circle (THU 8PM EDT) | Exalted: There Can Be No End (TUE 8PM EDT) | Shadowrun: The Looking-Glass War (MON 8PM EDT) | http://i.imgur.com/0XkYldr.png'
//03[20:48] * Set by VoxPVoxD!VoxPVoxD@sorcery-q06iee.res.rr.com on Tue Oct 25 10:19:06 2016
//";

//            var r = LineParser.Log(input);
//            Assert.True(r.IsSuccess);
//            Assert.Empty(r.Remainder);
//        }

//        [Fact]
//        public void MultiSession()
//        {
//            var input = @"
//Session Start: Tue Feb 07 22:10:15 2017
//Session Ident: #SonsAndDoctors
//03[22:10] * Now talking in #SonsAndDoctors
//03[22:10] * Topic is 'Exalted: Imperfect Circle (THU 8PM EDT) | Exalted: There Can Be No End (TUE 8PM EDT) | Shadowrun: The Looking-Glass War (MON 8PM EDT) | http://i.imgur.com/0XkYldr.png'
//03[22:10] * Set by VoxPVoxD!VoxPVoxD@sorcery-q06iee.res.rr.com on Tue Oct 25 10:19:06 2016
//03[01:55] * banana is now known as banana|sleep
//02[01:59] * Disconnected
//Session Close: Wed Feb 08 01:59:11 2017

//Session Start: Tue Feb 07 22:10:15 2017
//Session Ident: #SonsAndDoctors
//03[22:10] * Now talking in #SonsAndDoctors
//03[22:10] * Topic is 'Exalted: Imperfect Circle (THU 8PM EDT) | Exalted: There Can Be No End (TUE 8PM EDT) | Shadowrun: The Looking-Glass War (MON 8PM EDT) | http://i.imgur.com/0XkYldr.png'
//03[22:10] * Set by VoxPVoxD!VoxPVoxD@sorcery-q06iee.res.rr.com on Tue Oct 25 10:19:06 2016
//03[01:55] * banana is now known as banana|sleep
//02[01:59] * Disconnected
//Session Close: Wed Feb 08 01:59:11 2017";

//            var r = LineParser.Log(input);
//            Assert.True(r.IsSuccess);
//            Assert.Empty(r.Remainder);
//        }

//        [Fact]
//        public void UnclosedSession()
//        {
//            var input = @"
//Session Start: Tue Feb 07 20:48:15 2017
//Session Ident: #SonsAndDoctors
//03[20:48] * Rejoined channel #SonsAndDoctors
//03[20:48] * Topic is 'Exalted: Imperfect Circle (THU 8PM EDT) | Exalted: There Can Be No End (TUE 8PM EDT) | Shadowrun: The Looking-Glass War (MON 8PM EDT) | http://i.imgur.com/0XkYldr.png'
//03[20:48] * Set by VoxPVoxD!VoxPVoxD@sorcery-q06iee.res.rr.com on Tue Oct 25 10:19:06 2016

//Session Start: Tue Feb 07 22:10:15 2017
//Session Ident: #SonsAndDoctors
//03[22:10] * Now talking in #SonsAndDoctors
//03[22:10] * Topic is 'Exalted: Imperfect Circle (THU 8PM EDT) | Exalted: There Can Be No End (TUE 8PM EDT) | Shadowrun: The Looking-Glass War (MON 8PM EDT) | http://i.imgur.com/0XkYldr.png'
//03[22:10] * Set by VoxPVoxD!VoxPVoxD@sorcery-q06iee.res.rr.com on Tue Oct 25 10:19:06 2016
//Session Time: Wed Feb 08 00:00:00 2017
//03[01:55] * banana is now known as banana|sleep
//02[01:59] * Disconnected
//Session Close: Wed Feb 08 01:59:11 2017";

//            var r = LineParser.Log(input);
//            Assert.True(r.IsSuccess);
//            Assert.Empty(r.Remainder);
//        }
//    }
}
