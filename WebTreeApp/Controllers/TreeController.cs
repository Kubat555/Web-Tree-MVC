using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTreeApp.Models;

namespace WebTreeApp.Controllers
{
    public class TreeController : Controller
    {
        private readonly TreeDbContext _context;

        public TreeController(TreeDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var tree = await _context.TreeNodes.Include(t => t.Children).ToListAsync();
            return View(tree);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNode(string name, int? parentId, int parentOrder = 0)
        {
            var node = new TreeNode { Name = name, ParentId = parentId, Order = parentOrder+1 };
            _context.TreeNodes.Add(node);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFirstNode(string name)
        {
            var node = new TreeNode { Name = name, ParentId = null, Order = 1 };
            _context.TreeNodes.Add(node);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetNodesForMove(int id)
        {
            var node = await _context.TreeNodes
                .FirstOrDefaultAsync(t => t.Id == id);

            if (node == null)
            {
                return NotFound();
            }

            // Получаем всех потомков узла
            List<int> descendantIds = new();
            await GetDescendants(id, descendantIds);

            // Получаем узлы, которые не являются потомками и не являются родителями узла
            var nodes = await _context.TreeNodes
                .Where(n => n.Id != node.ParentId && !descendantIds.Contains(n.Id) && n.Id != node.Id)
                .Select(n => new { n.Id, n.Name })
                .ToListAsync();

            return Json(nodes);
        }

        [HttpPost]
        public async Task<IActionResult> MoveNode(int id, int newParentId)
        {
            var node = await _context.TreeNodes.FindAsync(id);
            var parent = await _context.TreeNodes.FindAsync(newParentId);
            if (node != null && parent != null)
            {
                node.ParentId = newParentId;
                node.Order = parent.Order + 1;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNode(int id)
        {
            var node = await _context.TreeNodes.FindAsync(id);
            if (node != null)
            {
                _context.TreeNodes.Remove(node);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }


        private async Task GetDescendants(int nodeId, List<int> descendants)
        {
            descendants.Add(nodeId);

            var children = await _context.TreeNodes
                                        .Where(node => node.ParentId == nodeId)
                                        .Select(node => node.Id)
                                        .ToListAsync();

            foreach (var childId in children)
            {
                await GetDescendants(childId, descendants);
            }
        }
    }
}
