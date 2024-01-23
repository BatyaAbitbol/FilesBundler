
var path = "C:\\Users\\win\\PraktiCode\\FilesBundler";
var invalidDirs = new String[] { "Debug", "bin", "node_modules", ".git", ".vs", "publish" };

var files = Directory.EnumerateFiles(path);
var dirs = Directory.EnumerateDirectories(path);
dirs = dirs.Where(_dir => !invalidDirs.Any(d => _dir.ToLower().EndsWith(d.ToLower())));

foreach (var dir in dirs)
{
    CollectFiles(dir, ref files);
}
Console.WriteLine("FILES");
foreach (var file in files)
{
    Console.WriteLine(file);
}
void CollectFiles(string path, ref IEnumerable<string> files)
{
    Console.WriteLine(path);
    
    var _files = Directory.EnumerateFiles(path);
    var _dirs = Directory.EnumerateDirectories(path);

    foreach (var file in _files)
    {
        files = files.Append(file);
    }
    Console.WriteLine("FILES");
    foreach (var file in files)
    {
        Console.WriteLine(file);
    }

    if (!_dirs.Any())
        return;
    _dirs = _dirs.Where(_dir => !invalidDirs.Any(d => _dir.EndsWith(d)));

    foreach (var dir in _dirs)
    {
        CollectFiles(dir, ref files);
    }
}