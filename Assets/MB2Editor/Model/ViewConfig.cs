namespace MB2Editor.Model
{
    public interface DataSetUpdate
    {
        /// <summary>
        /// the call back interface when the version of serilized data for the element doesn't match the current version. 
        /// </summary>
        /// <param name="element">the name of element need to be updated</param>
        /// <param name="serilizedData">the serilized data</param>
        /// <param name="currentVersion">the version of serilized data</param>
        /// <param name="updatedData">updated data</param>
        /// <returns>update successfully or not</returns>
        bool Update(string element, string serilizedData, int currentVersion, out string updatedData);
    }

    public class DataSetConfig
    {
        public string NameSpace;
        public ElementConfig[] elements;
        public ElementConfig[] Datasets;
        public int version = 0x00010000;
        public DataSetUpdate DataUpdate;
    }

    public class ViewConfig
    {
        public string Name;
        public string RenderName;
        public bool Optional;

        public ViewConfig() { }
        public ViewConfig(string name, string renderName, bool optional) => (Name, RenderName, Optional) = (name, renderName, optional);
    }

    public class ElementConfig : ViewConfig
    {
        public DataSetConfig DataSetConfig;
        public AttributeConfig[] Attributes;
        public ElementConfig[] NestedElements;
        public bool IsContainer;

        public ElementConfig() { }
        public ElementConfig(string name, string renderName, bool optional,
            AttributeConfig[] attributes, ElementConfig[] nestedElements, bool isContainer, DataSetConfig dataSetConfig) : base(name, renderName, optional)
            => (Attributes, NestedElements, IsContainer, DataSetConfig) = (attributes, nestedElements, isContainer, dataSetConfig);
    }

    public class AttributeConfig : ViewConfig
    {
        public string Type;
        public string Extra;

        public AttributeConfig() { }
        public AttributeConfig(string name, string renderName, bool optional, string type, string extra) : base(name, renderName, optional)
            => (Type, Extra) = (type, extra);
    }
}
