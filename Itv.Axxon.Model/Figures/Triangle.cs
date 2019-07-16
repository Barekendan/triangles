using System;
using System.Collections.Generic;
using Itv.Axxon.Model.BoundaryCompare;
using Itv.Axxon.Model.Helpers;

namespace Itv.Axxon.Model.Figures
{
    public class Triangle //: IBoundaryContainingComparable<Triangle>, IBoundaryContainingComparable<Point>
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

        public IReadOnlyList<Point> Points => new Point[] {A, B, C};

        public BoundaryContainingResult CompareTo(Triangle other)
        {
            // проверяем находятся ли все вершины сравниваемого треугольника внутри данного
            var a = this.CompareTo(other.A);
            var b = this.CompareTo(other.B);
            var c = this.CompareTo(other.C);

            // если все вершины внутри данного, то и треунольник внутри данного
            if (a == BoundaryContainingResult.Contains && 
                b == BoundaryContainingResult.Contains &&
                c == BoundaryContainingResult.Contains)
            {
                return BoundaryContainingResult.Contains;
            }

            if (a == BoundaryContainingResult.IsNotContains &&
                b == BoundaryContainingResult.IsNotContains &&
                c == BoundaryContainingResult.IsNotContains)
            {
                // если все вершины снаружи, то возможны пересечения сторон треугольников

                // проверяем последовательно пары сторон треугольников
                for (int i = 0; i < Points.Count; i++)
                {
                    var p1 = Points[i];
                    var p2 = Points[(i + 1) % Points.Count];
                    // тут "other.Points.Count - 1" - небольшая оптимизация, т.к. если есть одно пересечение, то есть, как минимум, еще одно, 
                    // т.е. последний отрезок проверять не нужно
                    for (int j = 0; j < other.Points.Count - 1; j++)
                    {
                        var p3 = other.Points[j];
                        var p4 = other.Points[(j + 1) % other.Points.Count];

                        if (SegmentsHelper.IsSegmentsIntersecting(p1, p2, p3, p4))
                            return BoundaryContainingResult.Intersects;
                    }
                }

                return BoundaryContainingResult.IsNotContains;
            }

            return BoundaryContainingResult.Intersects;
        }

        private BoundaryContainingResult CompareTo(Point p)
        {
            // a, b, c - определяют принадлежность точки p левым или правым полуплоскостям отрезков AB, BC и CA, соответственно.
            var a = SegmentsHelper.Direction(A, B, p);// (A.X - p.X) * (B.Y - A.Y) - (B.X - A.X) * (A.Y - p.Y);
            var b = SegmentsHelper.Direction(B, C, p);// (B.X - p.X) * (C.Y - B.Y) - (C.X - B.X) * (B.Y - p.Y);
            var c = SegmentsHelper.Direction(C, A, p);// (C.X - p.X) * (A.Y - C.Y) - (A.X - C.X) * (C.Y - p.Y);

            // если при однонаправленном (по часовой стрелке или против) обходе вершин треугольника 
            // точка p всегда лежит в правой полуплоскости отрезков (AB, BC, CA), 
            // или всегда в правой, то она находится внутри треугольника.
            if (a < 0 && b < 0 && c < 0 || a > 0 && b > 0 && c > 0)
            {
                return BoundaryContainingResult.Contains;
            }

            // в пограничных случаях, определяем находится ли точка непосредственно на одной из сторон треугольника
            if (Math.Abs(a) < Constants.Epsilon && (b < 0 && c < 0 || b > 0 && c > 0)
                || Math.Abs(b) < Constants.Epsilon && (a < 0 && c < 0 || a > 0 && c > 0)
                || Math.Abs(c) < Constants.Epsilon && (a < 0 && b < 0 || a > 0 && b > 0))
            {
                return BoundaryContainingResult.Intersects;
            }

            return BoundaryContainingResult.IsNotContains;
        }

        public override string ToString()
        {
            return $"{{A{A}, B{B}, C{C}}}";
        }
    }
}
