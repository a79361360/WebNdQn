using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FJSZ.OA.Common.Helper
{
    public class TreeNode
    {
        public TreeNode() { }
        public string NodeId { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public IEnumerable<TreeNode> Children { get; set; }
    }

}
