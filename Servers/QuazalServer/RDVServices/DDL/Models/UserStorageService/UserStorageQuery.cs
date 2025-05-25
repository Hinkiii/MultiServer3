namespace QuazalServer.RDVServices.DDL.Models.UserStorageService
{
    public enum VariantType : byte
    {
        None = 0,
        Sint64 = 1,
        Double = 2,
        Bool = 3,
        String = 4,
        DateTime = 5,
        Uint64 = 6
    }

    public class Variant
    {
        public VariantType Type { get; }
        public object Value { get; }

        public Variant()
        {
            Type = VariantType.None;
            Value = null;
        }

        public Variant(long v)
        {
            Type = VariantType.Sint64;
            Value = v;
        }

        public Variant(ulong v)
        {
            Type = VariantType.Uint64;
            Value = v;
        }

        public Variant(double v)
        {
            Type = VariantType.Double;
            Value = v;
        }

        public Variant(bool v)
        {
            Type = VariantType.Bool;
            Value = v;
        }

        public Variant(string v)
        {
            Type = VariantType.String;
            Value = v;
        }

        public Variant(DateTime v)
        {
            Type = VariantType.DateTime;
            Value = v;
        }
    }

    public struct ContentProperty
    {
        public uint Id { get; set; }
        public Variant Value { get; set; }
    }

    public struct UserContentKey
    {
        public uint TypeId { get; set; }
        public ulong ContentId { get; set; }
    }

    public class SearchResults
    {
        public UserContentKey Key { get; set; }
        public uint Pid { get; set; }
        public List<ContentProperty> Properties { get; set; } = new List<ContentProperty>();
    }
    public struct ResultRange
    {
        public uint muiOffset;
        public uint muiSize;

        public ResultRange(uint offset, uint size)
        {
            muiOffset = offset;
            muiSize = size;
        }
    }
    public class UserStorageQuery
    {
        public uint type_id { get; set; }
        public uint query_id { get; set; }
        public ResultRange result_range { get; set; }
        public ICollection<GameSessionProperty> m_attributes { get; set; }
    }
}
