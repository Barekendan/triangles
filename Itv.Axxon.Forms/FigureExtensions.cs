using System.Drawing;

namespace Itv.Axxon.Forms
{
    public static class FigureExtensions
    {
        public static PointF ToPointF(this Model.Figures.Point point)
        {
            return new PointF((float) point.X, (float) point.Y);
        }
    }
}