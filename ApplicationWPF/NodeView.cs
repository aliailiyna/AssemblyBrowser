using System.Collections.Generic;

namespace ApplicationWPF
{
    internal class NodeView
    {
        public string TextRepresentation
        { get; set; }

        public List<NodeView> Nodes
        { get; set; }
    }
}
