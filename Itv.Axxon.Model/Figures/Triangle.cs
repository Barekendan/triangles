using System;
using System.Collections.Generic;
using Itv.Axxon.Model.BoundaryCompare;

namespace Itv.Axxon.Model.Figures
{
    public class Triangle : IBoundaryContainingComparable<Triangle>, IBoundaryContainingComparable<Point>
    {
        public Triangle(Point a, Point b, Point c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Point A { get; }

        public Point B { get; }

        public Point C { get; }

        public BoundaryContainingResult CompareTo(Triangle other)
        {
            var a = this.CompareTo(other.A);
            var b = this.CompareTo(other.B);
            var c = this.CompareTo(other.C);

            if (a == BoundaryContainingResult.Inside && 
                b == BoundaryContainingResult.Inside &&
                c == BoundaryContainingResult.Inside)
            {
                return BoundaryContainingResult.Inside;
            }

            if (a == BoundaryContainingResult.Outside &&
                b == BoundaryContainingResult.Outside &&
                c == BoundaryContainingResult.Outside)
            {
                return BoundaryContainingResult.Outside;
            }

            return BoundaryContainingResult.Intersects;
        }

        public BoundaryContainingResult CompareTo(Point p)
        {
            var a = (A.X - p.X) * (B.Y - A.Y) - (B.X - A.X) * (A.Y - p.Y);
            var b = (B.X - p.X) * (C.Y - B.Y) - (C.X - B.X) * (B.Y - p.Y);
            var c = (C.X - p.X) * (A.Y - C.Y) - (A.X - C.X) * (C.Y - p.Y);

            if (a < 0 && b < 0 && c < 0 || a > 0 && b > 0 && c > 0)
            {
                return BoundaryContainingResult.Inside;
            }

            if (Math.Abs(a) < Constants.Epsilon && (b < 0 && c < 0 || b > 0 && c > 0)
                || Math.Abs(b) < Constants.Epsilon && (a < 0 && c < 0 || a > 0 && c > 0)
                || Math.Abs(c) < Constants.Epsilon && (a < 0 && b < 0 || a > 0 && b > 0))
            {
                return BoundaryContainingResult.Intersects;
            }

            return BoundaryContainingResult.Outside;
        }
    }
}
