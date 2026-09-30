// VOB - Exercise 3, block 3.2 - the same idea, written with a switch
// One class for every shape, and a switch that has to know them all.

namespace Oop0306
{
    public enum ShapeKind
    {
        Circle,
        Rectangle,
        Triangle
    }

    public class Shape
    {
        public ShapeKind Kind;

        public float Radius;          // circle
        public float Width;           // rectangle
        public float Height;          // rectangle, triangle
        public float BaseLength;      // triangle

        public float Area()
        {
            switch (Kind)
            {
                case ShapeKind.Circle:
                    return 3.14159f * Radius * Radius;

                case ShapeKind.Rectangle:
                    return Width * Height;

                case ShapeKind.Triangle:
                    return BaseLength * Height / 2f;

                default:
                    return 0f;
            }
        }
    }

    class Program
    {
        static void Main()
        {
            List<Shape> shapes = new List<Shape>
            {
                new Shape { Kind = ShapeKind.Circle, Radius = 2f },
                new Shape { Kind = ShapeKind.Rectangle, Width = 3f, Height = 4f },
                new Shape { Kind = ShapeKind.Triangle, BaseLength = 6f, Height = 2f },
            };

            float total = 0f;

            foreach (Shape s in shapes)
            {
                Console.WriteLine("area: " + s.Area());
                total = total + s.Area();
            }

            Console.WriteLine("total area: " + total);
        }
    }
}
