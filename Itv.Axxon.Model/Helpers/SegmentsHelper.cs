using System;
using Itv.Axxon.Model.Figures;

namespace Itv.Axxon.Model.Helpers
{
    /// <summary>
    /// Функции для работы с отрезками
    /// </summary>
    public static class SegmentsHelper
    {
        /// <summary>
        /// Определяет пересекаются ли отрезки (<paramref name="p1"/>, <paramref name="p2"/>) и (<paramref name="p3"/>, <paramref name="p4"/>).
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        public static bool IsSegmentsIntersecting(Point p1, Point p2, Point p3, Point p4)
        {
            // алгоритм из Главы 33.1 книги Кормен Т. - Алгоритмы: построение и анализ.  ISBN 978-5-8459-1794-2

            // если точки p1 и p2 лежат в разных полуплоскостях отрезка (p3, p4) 
            // и точки p3 и p4 лежат в разных полуплоскостях отрезка (p1, p2),
            // значит отрезки (p1,p2) и (p3, p4) пересекаются.

            var d1 = Direction(p3, p4, p1);
            var d2 = Direction(p3, p4, p2);
            var d3 = Direction(p1, p2, p3);
            var d4 = Direction(p1, p2, p4);

            if ((d1 > 0 && d2 < 0 || d1 < 0 && d2 > 0)
                && (d3 > 0 && d4 < 0 || d3 < 0 && d4 > 0))
            {
                return true;
            }

            if (Math.Abs(d1) < Constants.Epsilon && OnSegment(p3, p4, p1) ||
                Math.Abs(d2) < Constants.Epsilon && OnSegment(p3, p4, p2) ||
                Math.Abs(d3) < Constants.Epsilon && OnSegment(p1, p2, p3) ||
                Math.Abs(d4) < Constants.Epsilon && OnSegment(p1, p2, p4))
            {
                return true;
            }
                
            return false;
        }

        /// <summary>
        /// Определяет направление вектора (a, p) относительно вектора (a, b), 
        /// т.е. находится ли точка p в левой (против часовой стрелки) полуплоскости отрезка (a, b)
        /// или в правой (по часовой стрелке). Если равно нулю, точки a, b и p коллинеарны.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double Direction(Point a, Point b, Point p)
        {
            // векторное произведение (a, p) * (a, b)
            return (p.X - a.X) * (b.Y - a.Y) - (b.X - a.X) * (p.Y - a.Y);
        }

        private static bool OnSegment(Point a, Point b, Point p)
        {
            if (Math.Min(a.X, b.X) <= p.X && p.X <= Math.Max(a.X, b.X) && 
                Math.Min(a.Y, b.Y) <= p.Y && p.Y <= Math.Max(a.Y, b.Y))
            {
                return true;
            }

            return false;
        }
    }
}