namespace Drafts.Database
{
    public interface IDatabaseItemInternal : IDatabaseItem
    {
        void SetIndex(int index);
    }
    
    public interface IDatabaseItem
    {
        uint Id { get; }
        int Index { get; }
    }
}