namespace ApiShared.Core.Data.BaseInterface
{
    /// <summary>
    /// موجودیت دارای شرکت هست: CompanyId
    /// </summary>
    public interface IEntityHasCompany 
    {
        public int CompanyId { get; set; }
    }
}
