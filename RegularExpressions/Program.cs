using System.Text.RegularExpressions;

Console.WriteLine(@"\t");

// ^\(?0?(50|51|55|70|99|10)\)?\s?\d{3}-?\d{2}-?\d{2}$ - phone
// ^#?[0-9a-f]{3}$|^#?[0-9a-f]{6}$ - hex code
// ^(?'mail'[a-z0-9_]+)@(?'domain'\w{1,}\.[a-z]{1,})$ - simple mail

var regex = new Regex(@"^\(0?(?'operator'50|51|55|70|99|10)\)?\s?\d{3}-?\d{2}-?\d{2}$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

// if (regex.IsMatch("(055) 38152x47"))
// {
//     Console.WriteLine("Match found!");
// }
// else
// {
//     Console.WriteLine("Match not found!");
// }

IList<Match> matchCollection = regex.Matches("(055) 3815247");

foreach (var match in matchCollection)
{
    Console.WriteLine($"Index: {match.Index}");
    Console.WriteLine($"Length: {match.Length}");
    Console.WriteLine($"Value: {match.Value}");

    var group = ((IList<Group>)match.Groups).FirstOrDefault(g => g.Name == "operator");

    Console.WriteLine($"GroupValue: {group.Value}");
}




