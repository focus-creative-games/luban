namespace Luban.Job.Db.TypeVisitors
{
    class DbCsWriteBlobVisitor
    {
        public static DbCsCompatibleSerializeVisitor Ins { get; } = new();

    }
}
