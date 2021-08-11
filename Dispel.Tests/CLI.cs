using Dispel.CommandLine;
using System;
using System.IO;
using System.Threading.Tasks;
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
    public async Task SpecifySingleFile()
    {
        using (var writer = File.CreateText("cliTests/foo.log"))
        {
            writer.Write(singleSession);
        }

        await Program.Main(new[] { "-q", "-s", "-o", "page", "cliTests/foo.log" });

        Assert.True(File.Exists("cliTests/foo.html"));
    }

    [Fact]
    public async Task SpecifyMultipleFiles()
    {
        using (var writer = File.CreateText("cliTests/1.log")) writer.Write(singleSession);
        using (var writer = File.CreateText("cliTests/2.log")) writer.Write(singleSession);

        await Program.Main(new[] { "-q", "-s", "-o", "page", "cliTests/1.log", "cliTests/2.log" });

        Assert.True(File.Exists("cliTests/1.html"));
        Assert.True(File.Exists("cliTests/2.html"));
    }

    [Fact]
    public async Task OmitFile()
    {
        using (var writer = File.CreateText("cliTests/1.log")) writer.Write(singleSession);
        using (var writer = File.CreateText("cliTests/2.log")) writer.Write(singleSession);

        await Program.Main(new[] { "-q", "-s", "-o", "page", "cliTests/1.log" });

        Assert.True(File.Exists("cliTests/1.html"));
        Assert.False(File.Exists("cliTests/2.html"));
    }

    [Fact]
    public async Task TolerateMissingFile()
    {
        using (var writer = File.CreateText("cliTests/1.log")) writer.Write(singleSession);

        await Program.Main(new[] { "-q", "-s", "-o", "page", "cliTests/1.log", "cliTests/2.log" });

        Assert.True(File.Exists("cliTests/1.html"));
    }

    [Fact]
    public async Task AllLogFilesInCurrentDirectory()
    {
        using (var writer = File.CreateText("cliTests/1.log")) writer.Write(singleSession);
        using (var writer = File.CreateText("cliTests/2.frog")) writer.Write(singleSession);
        using (var writer = File.CreateText("cliTests/3.log")) writer.Write(singleSession);

        Environment.CurrentDirectory = Path.Combine(Path.GetTempPath(), "cliTests");

        await Program.Main(new[] { "-q", "-s", "-o", "page" });

        Environment.CurrentDirectory = Path.Combine(Path.GetTempPath());

        Assert.True(File.Exists("cliTests/1.html"));
        Assert.False(File.Exists("cliTests/2.html"));
        Assert.True(File.Exists("cliTests/3.html"));
    }

    [Fact]
    public async Task NoLogFilesInCurrentDirectory()
    {
        Environment.CurrentDirectory = Path.Combine(Path.GetTempPath(), "cliTests");

        await Program.Main(new[] { "-q" });

        Environment.CurrentDirectory = Path.Combine(Path.GetTempPath());

        Assert.Empty(Directory.EnumerateFiles("cliTests/"));
    }

    [Fact]
    public async Task FormatHTMLExplicitly()
    {
        using (var writer = File.CreateText("cliTests/foo.log"))
        {
            writer.Write(singleSession);
        }

        await Program.Main(new[] { "-q", "-s", "-o", "page", "cliTests/foo.log" });

        Assert.True(File.Exists("cliTests/foo.html"));
    }

    [Fact]
    public async Task FormatTextExplicitly()
    {
        using (var writer = File.CreateText("cliTests/foo.log"))
        {
            writer.Write(singleSession);
        }

        await Program.Main(new[] { "-q", "-s", "-o", "text", "cliTests/foo.log" });

        Assert.True(File.Exists("cliTests/foo.txt"));
    }

    [Fact]
    public async Task IgnoreUnknownFormat()
    {
        using (var writer = File.CreateText("cliTests/foo.log"))
        {
            writer.Write(singleSession);
        }

        await Program.Main(new[] { "-q", "-o", "what", "cliTests/foo.log" });

        Assert.Single(Directory.EnumerateFiles("cliTests/"));
    }
}
