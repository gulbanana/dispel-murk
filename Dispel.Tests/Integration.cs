using Dispel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class Integration
{
    private const string sessionTemplate = @"Session Start: Mon Jan 02 16:07:49 2017
Session Ident: #Foo
03[16:07] * Now talking in #Foo
03[16:07] * Topic is 'foo bar'
03[16:07] * Set by foo!foo@bar on Tue Oct 25 10:19:06 2016
{0}02[18:33] * Disconnected
Session Close: Tue Jan 03 18:33:05 2017
";

    [Fact]
    public async Task Empty_GenerateWeb()
    {
        var emptyLog = string.Format(sessionTemplate, "");

        using var input = new MemoryStream(Encoding.UTF8.GetBytes(emptyLog));
        var engine = new Engine(new());        
        var logs = await engine.ConvertAsync("#foo.log", input, OutputFormat.WebSite);

        Assert.Collection(logs.OrderBy(f => f.Filename),
            f => Assert.Equal("index.html", f.Filename),
            f => Assert.Equal("style.css", f.Filename)            
        );
    }

    [Fact]
    public async Task Empty_GeneratePage()
    {
        var emptyLog = string.Format(sessionTemplate, "");

        using var input = new MemoryStream(Encoding.UTF8.GetBytes(emptyLog));
        var engine = new Engine(new());
        var logs = await engine.ConvertAsync("#foo.log", input, OutputFormat.WebPage);

        Assert.Collection(logs,
            f => Assert.Equal("#foo.html", f.Filename)
        );
    }

    [Fact]
    public async Task Single_GenerateWeb()
    {
        var emptyLog = string.Format(sessionTemplate, "[17:00] <banana> hello world\r\n");

        using var input = new MemoryStream(Encoding.UTF8.GetBytes(emptyLog));
        var engine = new Engine(new());
        var logs = await engine.ConvertAsync("#foo.log", input, OutputFormat.WebSite);

        Assert.Collection(logs.OrderBy(f => f.Filename),
            f => Assert.Equal("#Foo-0.html", f.Filename),
            f => Assert.Equal("index.html", f.Filename),
            f => Assert.Equal("style.css", f.Filename)            
        );
    }
}
