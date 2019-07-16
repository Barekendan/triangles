namespace Itv.Axxon.Model.Figures
{
    public class Point
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }

        public double Y { get; }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}