using Gaming.BL.VM.Categories;
using Gaming.Core.Entities;
using Gaming.DAL.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace Gaming.MVC.Areas.Admin.Controllers;
[Area("Admin")]
public class CategoryController : Controller
{
    readonly AppDbContext _context;
    public CategoryController(AppDbContext context)
    {
        _context = context; 
    }

    // GET: CategoryController
    public async Task<IActionResult> Index()
    {
        return View(await _context.Categories.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateVM vm)
    {
        if (!ModelState.IsValid) return View();
        var data = await _context.Categories.FirstOrDefaultAsync(x => x.Name == vm.Name);
        if(data != null)
        {
            ModelState.AddModelError("", "A Category with this name is already exists!");
            return View(); 
        }
        Category category = new Category
        {
            Name = vm.Name,
            CreatedTime = DateTime.UtcNow
        };
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index)); 
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var data = await _context.Categories
            .Where(x => x.Id == id)
            .Select(x => new CategoryUpdateVM
            {
                Name = x.Name
            }).FirstOrDefaultAsync();
        return View(data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(int? id, CategoryUpdateVM vm)
    {

        if (!id.HasValue) return BadRequest();
        var data = await _context.Categories.FindAsync(id);
        if (data == null) return NotFound();

        if (!ModelState.IsValid) return View();

        data.Name = vm.Name;
        data.UpdatedTime = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> ToggleCategoryVisibility(int? id, bool visible)
    {
        if (!id.HasValue) return BadRequest();
        var data = await _context.Categories.FindAsync(id);
        if (data == null) return NotFound();

        data.IsDeleted = !visible;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Show(int? id)
        => await ToggleCategoryVisibility(id, true);

    public async Task<IActionResult> Hide(int? id)
        => await ToggleCategoryVisibility(id, false);

    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var data = await _context.Categories.FindAsync(id);
        if (data == null) return NotFound();

        _context.Remove(data); 
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
