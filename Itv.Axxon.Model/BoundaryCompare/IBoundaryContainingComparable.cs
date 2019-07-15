namespace Itv.Axxon.Model.BoundaryCompare
{
    public interface IBoundaryContainingComparable<in T>
    {
        BoundaryContainingResult CompareTo(T other);
    }
}