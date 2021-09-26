namespace Luban.Job.Db.RawDefs
{
    public class Table
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }

        public string KeyType { get; set; }

        public string ValueType { get; set; }

        public bool IsPersistent { get; set; }

        public string Comment { get; set; }
    }
}
