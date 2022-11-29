// See https://aka.ms/new-console-template for more information
using ProtoBuf;

if (args.Length == 0)
{
    System.Console.WriteLine("Please specify save file path");
    return 1;
}

var stream = new MemoryStream();
using (var file = File.OpenRead(args[0]))
{
    file.Seek(4, SeekOrigin.Begin);
    var saveFile = Serializer.Deserialize<SaveFile>(file);
    foreach (var profile in saveFile.Profiles.Where(profile => profile.Present))
    {
        if (profile.data.Skirmishs.Exists(results =>
                results.result == SaveFile.Profile.Data.SkirmishResults.Result.Win))
            profile.data.Skirmishs.First(results => results.result == SaveFile.Profile.Data.SkirmishResults.Result.Win)
                .Count = 100;
        else
        {
            profile.data.Skirmishs.Add(new()
            { result = SaveFile.Profile.Data.SkirmishResults.Result.Win, Count = 100 });
        }
    }

    Serializer.Serialize(stream, saveFile);
}

using var wfile = File.OpenWrite(args[0]);
wfile.Write(BitConverter.GetBytes((int)stream.Length));
stream.CopyTo(wfile);

Console.WriteLine("Hello, World!");
return 0;