namespace WebTreeApp.Models
{
    public class TreeNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public TreeNode? Parent { get; set; }
        public ICollection<TreeNode> Children { get; set; } = new List<TreeNode>();
        public int Order { get; set; }
    }
}
