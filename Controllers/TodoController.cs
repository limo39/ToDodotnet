using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace TodoApp.Controllers
{
    public class TodoController : Controller
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }


        public IActionResult Index(string searchQuery)
        {
            var tasks = _context.TodoItems
                                .Where(x => !x.IsDeleted)
                                .AsQueryable();

            // Apply search filtering if searchQuery is provided
            if (!string.IsNullOrEmpty(searchQuery))
            {
                tasks = tasks.Where(x => x.Title.ToLower().Contains(searchQuery.ToLower()));
            }

            var taskList = tasks.OrderBy(x => x.IsCompleted).ToList();
            return View(taskList);
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TodoItem todoItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(todoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todoItem);
        }

        public async Task<IActionResult> MarkComplete(int id)
        {
            var todoItem = _context.TodoItems.FirstOrDefault(x => x.Id == id);
            if (todoItem != null)
            {
                todoItem.IsCompleted = true;
                todoItem.DateCompleted = DateTime.Now;  // Set the date of completion
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // Soft delete task
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var todoItem = _context.TodoItems.FirstOrDefault(x => x.Id == id);
            if (todoItem != null)
            {
                todoItem.IsDeleted = true;  // Soft delete
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // Edit task (GET to load the task in an edit form)
        public IActionResult Edit(int id)
        {
            var todoItem = _context.TodoItems.FirstOrDefault(x => x.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // Save changes after editing (POST)
        [HttpPost]
        public IActionResult Edit(TodoItem updatedItem)
        {
            var todoItem = _context.TodoItems.FirstOrDefault(x => x.Id == updatedItem.Id);
            if (todoItem != null)
            {
                todoItem.Title = updatedItem.Title;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult CompletedTasksByDay()
        {
            // Get all completed tasks, and group them by the date of completion
            var completedTasks = _context.TodoItems
                .Where(x => x.IsCompleted && x.DateCompleted.HasValue)
                .ToList() // Fetch the data into memory
                .GroupBy(x => x.DateCompleted.Value.Date)
                .OrderByDescending(g => g.Key)
                .ToList();



            return View(completedTasks);
        }

    }
}
