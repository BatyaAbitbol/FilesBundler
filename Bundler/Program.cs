using Bundler;
using System.CommandLine;
using System.IO;

var rootCommand = new RootCommand("Root command for files bundler CLI");

var bundleCommand = new Command("bundle", "Bundle code files into a single code file");

// define options to the bundle command
var languageOption = new Option<string>("--language", "List of coding languages");
var outputOption = new Option<FileInfo>("--output", "Output file path and name");
var noteOption = new Option<bool>("--note", "Write the source code in a comment in the bundle file");
var sortOption = new Option<string>("--sort", "Order of copying the files, according to alphabet of file name or type of code");
var removeOption = new Option<bool>("--remove-empty-lines", "Remove empty lines from the source code files");
var authorOption = new Option<string>("--author", "Author name of the code file in comment in the bundle file");

// define aliases to the options
languageOption.AddAlias("-l");
outputOption.AddAlias("-o");
noteOption.AddAlias("-n");
sortOption.AddAlias("-s");
removeOption.AddAlias("-r");
authorOption.AddAlias("-a");

// language option is required
languageOption.IsRequired = true;

// define default values
outputOption.SetDefaultValue(new FileInfo("bundleFile"));
noteOption.SetDefaultValue(false);
sortOption.SetDefaultValue("name");
removeOption.SetDefaultValue(false);

bundleCommand.AddOption(languageOption);
bundleCommand.AddOption(outputOption);
bundleCommand.AddOption(noteOption);
bundleCommand.AddOption(sortOption);
bundleCommand.AddOption(removeOption);
bundleCommand.AddOption(authorOption);

bundleCommand.SetHandler((languages, output, note, sort, remove, author) =>
{
    try
    {
        // validate options and arguments of the command
        List<string> languagesList = new List<string>();
        if (!languages.Contains("all"))
        {
            languagesList = languages.Split(',').ToList();
        }

        var _output = output ?? new FileInfo("bundle");
        var _sort = SortType.Name;
        if (sort != "name")
        {
            if (sort == "type")
            {
                _sort = SortType.Type;
            }
            else throw new Exception("Sort type is invalid");
        }

        // bundle the directory
        Bundler.Bundler.Bundle(languagesList, _output, note, _sort, remove, author);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}, languageOption, outputOption, noteOption, sortOption, removeOption, authorOption);



var createRspCommand = new Command("create-rsp", "Create response file");

createRspCommand.SetHandler(() =>
{
    Console.Write("Languages: [list of languages names separated by comma (,) OR all] ");
    var languages = Console.ReadLine()?.Trim().ToLower();

    Console.Write("Output: [path to bundled file] ");
    var output = Console.ReadLine()?.Trim();

    var note = "";
    bool toNote = false;
    while (note != "Y" && note != "N")
    {
        Console.Write("Note source code: [Y/N] ");
        note = Console.ReadLine()?.Trim().ToUpper();
        if (String.IsNullOrWhiteSpace(note)) { note = "N"; }
    }
    if (note == "Y")
        toNote = true;

    var sort = "";
    Console.Write("Sort: [name / type] ");
    sort = Console.ReadLine()?.Trim();
    while (sort != "name" && sort != "type")
    {
        if (string.IsNullOrEmpty(sort))
        {
            sort = "name";
            break;
        }
        Console.Write("Sort: [name / type] ");
        sort = Console.ReadLine()?.Trim();
        if (String.IsNullOrWhiteSpace(sort)) { sort = "name"; }
    }

    var remove = "";
    bool toRemove = false;
    while (remove != "Y" && remove != "N")
    {
        Console.Write("Remove empty lines: [Y/N] ");
        remove = Console.ReadLine()?.Trim().ToUpper();
        if (String.IsNullOrWhiteSpace(remove)) { remove = "N"; }
    }
    if (remove == "Y")
        toRemove = true;

    Console.Write("Author: [name of author] ");
    var author = Console.ReadLine();

    var rsp = $"bundle\n" +
              $"--language {languages}\n";
    
    if (!string.IsNullOrEmpty(output)) { rsp += $"--output {output}\n"; }
    if (toNote) { rsp += "--note\n"; }
    if (sort == "type") { rsp += "--sort type\n"; }
    if (toRemove) { rsp += "--remove-empty-lines\n"; }
    if (!string.IsNullOrEmpty(author)) { rsp += $"--author {author}\n"; }

    try
    {
        File.WriteAllText("bundle.rsp", rsp);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message); ;
    }
});

rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(createRspCommand);

await rootCommand.InvokeAsync(args);