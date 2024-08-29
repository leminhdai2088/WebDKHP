namespace dev_DKHP.CoreModule.Helper.Procedure
{
    public class StoreParamAttribute: Attribute
    {
        public StoreParamAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
