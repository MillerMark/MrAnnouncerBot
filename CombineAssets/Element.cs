using System.Collections.Generic;

namespace CombineAssets
{
    public abstract class Element
    {
        protected Element(string path)
        {
            ElementPath = path;
            _children = new List<Element>();
        }

        public string ElementPath { get; }
        public string Name { get; set; }
        protected List<Element> _children;
        public IReadOnlyList<Element> Children => _children;
        public string Location { get; set; }
    }
}
