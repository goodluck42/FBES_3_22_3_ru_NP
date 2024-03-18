// See https://aka.ms/new-console-template for more information


List<string> names = new List<string>(["John", "Josh", "Joe", "Jack", "Jane"]);

int count;

names.MyTryGetNonEnumeratedCount(out count);

Console.WriteLine(count);


var game = new Game
{
    Id = 1,
    Name = "GTA",
    Tags = ["Open world", "Shooter", "3rd person"],
    Size = 100
};

var game2 = new Game
{
    Id = 1,
    Name = "GTA V",
    Tags = ["Open world", "Shooter", "3rd person"],
    Size = 100
};
///////////////////////
// if (game is { Tags.Count: > 0, Size: > 0 and < 100, Size: var size })
// {
//     Increment(ref size);
//
//     game.Size = size;
// }

// switch (game)
// {
//     case { Tags.Count: > 0, Size: > 0 and < 100, Size: var size }:
//     {
//         Increment(ref size);
//         
//         game.Size = size;
//         
//         break;
//     }
// }

// if (game.Tags.Count > 0 && game.Size > 0 && game.Size < 100)
// {
//     Console.WriteLine("Game is valid");
// }
////////////////////////

//  GET /folder1/folder2/filename.txt

const string tag = "3rd person";

if (game.Tags.ToList() is [.., var secondElement, tag])
{
    Console.WriteLine($"Collection ends with {tag}");
}

if (game.Equals(game2))
{
    Console.WriteLine("Games have same names");
}

if (game is null)
{
    
}

// if (game.Tags is not null)
// {
//     
// }

void Increment(ref int value)
{
    value++;
}

public static class EnumerableExtensions
{
    public static bool MyTryGetNonEnumeratedCount<T>(this IEnumerable<T> source, out int count)
    {
        count = source switch
        {
            ICollection<T> collection => collection.Count,
            IReadOnlyCollection<T> readOnlyCollection => readOnlyCollection.Count,
            _ => -1
        };

        switch (source)
        {
            case ICollection<T> collection:
                count = collection.Count;
                break;
            case IReadOnlyCollection<T> readOnlyCollection:
                count = readOnlyCollection.Count;
                break;
            default:
                count = -1;
                break;
        }

        return count >= 0;
    }
}


class Game
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int Size { get; set; }
    public IEnumerable<string>? Tags { get; set; }

    public static bool operator==(Game left, Game right)
    {
        return left.Name == right.Name;
    }
    
    public static bool operator!=(Game left, Game right)
    {
        return !(left == right);
    }
}