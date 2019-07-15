using System;
using System.Collections.Generic;
using System.IO;
using Itv.Axxon.Model.Figures;

namespace Itv.Axxon.Model.Utils
{
    public static class TriangleParser
    {
        public static IEnumerable<Triangle> ParseTriangles(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var countStr = streamReader.ReadLine();

                if (int.TryParse(countStr, out var count))
                {
                    for (int i = 0; i < count; i++)
                    {
                        var triangleStr = streamReader.ReadLine();

                        yield return ParseTriangle(triangleStr);
                    }
                }
                else
                {
                    throw new FormatException($"Не удалось преобразовать строку \"{countStr}\" в число (количество треугольников).");
                }
            }
        }

        private static Triangle ParseTriangle(string str)
        {
            string[] coordinates;
            try
            {
                coordinates = str.Split(new[] {' ', '\t'}, 6, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (ArgumentException e)
            {
                throw new FormatException($"Не удалось преобразовать строку \"{str}\" в координаты треугольника", e);
            }

            var a = ParsePoint(coordinates[0], coordinates[1]);
            var b = ParsePoint(coordinates[2], coordinates[3]);
            var c = ParsePoint(coordinates[4], coordinates[5]);

            return new Triangle(a, b, c);
        }

        private static Point ParsePoint(string xStr, string yStr)
        {
            if (double.TryParse(xStr, out var x) && double.TryParse(yStr, out var y))
            {
                return new Point(x, y);
            }

            throw new FormatException($"Не удалось преобразовать \"{xStr} {yStr}\" в точку");
        }

    }
}
