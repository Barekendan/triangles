namespace Itv.Axxon.Model.BoundaryCompare
{
    public enum BoundaryContainingResult
    {
        /// <summary>
        /// Заключает внутри границ
        /// </summary>
        Contains,

        /// <summary>
        /// Не заключает внутри границ
        /// </summary>
        IsNotContains,

        /// <summary>
        /// Пересекается
        /// </summary>
        Intersects
    }
}