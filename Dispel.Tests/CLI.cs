using Dispel.CommandLine;
using System;
using System.IO;
using Xunit;

public class CLI : IDisposable
{
    private const string singleSession = @"Session Start: Mon Jan 02 16:07:49 2017
Session Ident: #Foo
03[16:07] * Now talking in #Foo
03[16:07] * Topic is 'foo bar'
03[16:07] * Set by foo!foo@bar on Tue Oct 25 10:19:06 2016
02[18:33] * Disconnected
Session Close: Tue Jan 03 18:33:05 2017
";

    public CLI()
    {
        Environment.CurrentDirectory = Path.GetTempPath();
        Directory.CreateDirectory("cliTests");
    }

    public void Dispose()
    {
        Directory.Delete("cliTests", true);
    }

    [Fact]
    public void SpecifySingleFile()
    {
        using (var writer = File.CreateText("cliTests/foo.log"))
        {
            writer.Write(singleSession);
        }

        Program.Main(new[] { "cliTests/foo.log" });

        Assert.True(File.Exists("cliTests/foo.html"));
    }

    [Fact]
    public void SpecifyMultipleFiles()
    {
        using (var writer = File.CreateText("cliTests/1.log")) writer.Write(singleSession);
        using (var writer = File.CreateText("cliTests/2.log")) writer.Write(singleSession);

        Program.Main(new[] { "cliTests/1.log", "cliTests/2.log" });

        Assert.True(File.Exists("cliTests/1.html"));
        Assert.True(File.Exists("cliTests/2.html"));
    }

    [Fact]
    public void OmitFile()
    {
        using (var writer = File.CreateText("cliTests/1.log")) writer.Write(singleSession);
        using (var writer = File.CreateText("cliTests/2.log")) writer.Write(singleSession);

        Program.Main(new[] { "cliTests/1.log" });

        Assert.True(File.Exists("cliTests/1.html"));
        Assert.False(File.Exists("cliTests/2.html"));
    }

    [Fact]
    public void TolerateMissingFile()
    {
        using (var writer = File.CreateText("cliTests/1.log")) writer.Write(singleSession);

        Program.Main(new[] { "cliTests/1.log", "cliTests/2.log" });

        Assert.True(File.Exists("cliTests/1.html"));
    }

    [Fact]
    public void AllLogFilesInCurrentDirectory()
    {
        using (var writer = File.CreateText("cliTests/1.log")) writer.Write(singleSession);
        using (var writer = File.CreateText("cliTests/2.frog")) writer.Write(singleSession);
        using (var writer = File.CreateText("cliTests/3.log")) writer.Write(singleSession);

        Environment.CurrentDirectory = Path.Combine(Path.GetTempPath(), "cliTests");

        Program.Main(Array.Empty<string>());

        Environment.CurrentDirectory = Path.Combine(Path.GetTempPath());

        Assert.True(File.Exists("cliTests/1.html"));
        Assert.False(File.Exists("cliTests/2.html"));
        Assert.True(File.Exists("cliTests/3.html"));
    }
}
