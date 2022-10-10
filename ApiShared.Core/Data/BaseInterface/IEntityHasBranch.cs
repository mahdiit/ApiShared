namespace ApiShared.Core.Data.BaseInterface
{
    /// <summary>
    /// موجودیت دارای شعبه هست: BranchId
    /// </summary>
    public interface IEntityHasBranch 
    {
        public int BranchId { get; set; }
    }
}
