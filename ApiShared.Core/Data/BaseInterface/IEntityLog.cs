namespace ApiShared.Core.Data.BaseInterface
{
    /// <summary>
    /// موجودیت دارای تاریخ و تغییر دهنده است: Creator,CreationTime,HostName,Modifier,ModificationTime
    /// </summary>
    public interface IEntityLog
    {
        public string Creator { get; set; }
        public string? Modifier { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? ModificationTime { get; set; }
        public string? HostName { get; set; }
    }
}
