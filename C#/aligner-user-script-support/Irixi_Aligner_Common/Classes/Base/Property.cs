namespace Irixi_Aligner_Common.Classes.Base
{
    public class Property
    {
        public Property() { }

        public Property(string Name)
        {
            this.Name = Name;
        }

        public string Name { get; set; }
        public string CollectionName { get; set; }
    }
}
