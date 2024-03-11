using System.Runtime.InteropServices;

// var friends = new List<Friend>();
//
// {
//     var friend1 = new Friend()
//     {
//         Id = 0,
//         Name = "Vadik",
//     };
//
//     var friend2 = new Friend()
//     {
//         Id = 0,
//         Name = "Sergey",
//     };
//     
//     friend1.MyFriend = friend2;
//     friend2.MyFriend = friend1;
//
//     friends.Add(friend1);
//     friends.Add(friend2);
// }
//
// friends[0].MyFriend!.Name = "Peter";
// friends[0].MyFriend = null;
//
// Console.WriteLine(friends[0].Name);
// Console.WriteLine(friends[1].Name);
// Console.WriteLine(friends[1].MyFriend!.Name);
//
// IntPtr ptr = Marshal.AllocHGlobal(sizeof(int) * 12);
//
// unsafe
// {
//     int* arr = (int*)ptr;
//
//     arr[0] = 42;
//     arr[1] = 13;
//
//     for (int i = 0; i < 12; i++)
//     {
//         Console.WriteLine(arr[i]);
//     }
// }



{
    while (true)
    {
        var obj = new Obj();
        Thread.Sleep(10);
    }
}

GC.Collect();

class Obj
{
    public Obj()
    {
        Id++;
        Console.WriteLine($"Created! {Id}");
    }
    ~Obj()
    {
        Console.WriteLine($"Finalized! {Id}");
    }

    public static int Id { get; set; }
}

class Friend
{
    public int Id { get; set; }
    public required string Name { get; set; } = null!;
    public Friend? MyFriend { get; set; }
}


