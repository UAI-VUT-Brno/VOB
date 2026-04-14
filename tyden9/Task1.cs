var packages = new List<Package>
{
new Package { Id = 1, Dimensions = "10x20x30", Weight = 12.5 },
new Package { Id = 2, Dimensions = "15x25x35", Weight = 7.8 },
new Package { Id = 3, Dimensions = "8x18x28", Weight = 19.2 },
new Package { Id = 4, Dimensions = "12x22x32", Weight = 14.1},
new Package { Id = 5, Dimensions = "10x20x30", Weight = 1.9 },
new Package { Id = 6, Dimensions = "20x25x35", Weight = 1.8 },
new Package { Id = 7, Dimensions = "8x18x28", Weight = 10.0 },
new Package { Id = 8, Dimensions = "12x22x32", Weight = 3.7 }
};

Console.WriteLine("Packages report");
Console.WriteLine("???????????????");   


public class Package
{
public int Id { get; set; }
public string Dimensions { get; set; } = "";
public double Weight { get; set; }
}
