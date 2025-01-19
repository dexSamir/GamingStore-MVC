using Gaming.BL.Extension;
using Gaming.BL.VM.Games;
using Gaming.Core.Entities;
using Gaming.DAL.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace Gaming.MVC.Areas.Admin.Controllers;
[Area("Admin")]
public class GameController : Controller
{
    readonly AppDbContext _context;
    readonly IWebHostEnvironment _env;
    public GameController(AppDbContext context, IWebHostEnvironment env)
    {
        _env = env; 
        _context = context; 
    }
    private async Task PopulateCategoriesAsync()
        => ViewBag.Categories =  await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();

    private async Task<IActionResult> HandleAddModelErrorAsync(GameCreateVM vm, string msg, string key = "")
    {
        if (!string.IsNullOrWhiteSpace(key))
            ModelState.AddModelError(key, msg);
        else
            ModelState.AddModelError(key, msg);

        await PopulateCategoriesAsync();
        return View(vm); 
    }

    private async Task<IActionResult> HandleAddModelErrorAsync(GameUpdateVM vm, string msg, string key = "")
    {
        if (!string.IsNullOrWhiteSpace(key))
            ModelState.AddModelError(key, msg);
        else
            ModelState.AddModelError(key, msg);

        await PopulateCategoriesAsync();
        return View(vm);
    }

    // GET: GameController
    public async Task<IActionResult> Index()
    {
        return View(await _context.Games.Include(x=> x.Category).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesAsync(); 
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(GameCreateVM vm)
    {
        if (!ModelState.IsValid)
            return await HandleAddModelErrorAsync(vm, "Model is not valid!");

        if (await _context.Games.AnyAsync(x => x.Name == vm.Name))
            return await HandleAddModelErrorAsync(vm, "A Game with that name is already exists");


        if (vm.Image == null || vm.Image.Length == 0)
            return await HandleAddModelErrorAsync(vm, "File is required", "Image");

        if (!vm.Image.IsValidType("image"))
            return await HandleAddModelErrorAsync(vm, "File type must be an Image", "Image");

        if (!vm.Image.IsValidSize(5))
            return await HandleAddModelErrorAsync(vm, "Image size must be less than 5MB", "Image");

        Game game = new Game
        {
            Name = vm.Name,
            CategoryId = vm.CategoryId,
            Price = vm.Price,
            DiscountedPrice = vm?.DiscountedPrice ?? null,
            CreatedTime = DateTime.UtcNow,
            ImageUrl = await vm.Image.UploadAsync(_env.WebRootPath, "imgs", "games")
        };
        await _context.AddAsync(game);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index)); 
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (!id.HasValue) return BadRequest(); 
        await PopulateCategoriesAsync();

        var data = await _context.Games
            .Where(x => x.Id == id)
            .Select(x => new GameUpdateVM
            {
                Name = x.Name,
                Price = x.Price,
                DiscountedPrice = x.DiscountedPrice ?? null,
                CategoryId = x.CategoryId,
                ExistingImageUrl = x.ImageUrl, 
            }).FirstOrDefaultAsync();

        return View(data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(int? id, GameUpdateVM vm)
    {
        if (!id.HasValue) BadRequest();
        var data = await _context.Games.FindAsync(id);
        if (data == null) return NotFound();

        await PopulateCategoriesAsync();

        if (vm.Image != null)
        {
            if (!vm.Image.IsValidType("image"))
                return await HandleAddModelErrorAsync(vm, "File type must be an Image", "Image");

            if (!vm.Image.IsValidSize(5))
                return await HandleAddModelErrorAsync(vm, "Image size must be less than 5MB", "Image");

            string fileName = Path.Combine(_env.WebRootPath, "imgs", "games", data.ImageUrl);

            if (System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);

            string newFileName = await vm.Image.UploadAsync(_env.WebRootPath, "imgs", "games");
            data.ImageUrl = newFileName; 
        }

        data.Name = vm.Name;
        data.Price = vm.Price;
        data.CategoryId = vm.CategoryId;
        data.DiscountedPrice = vm.DiscountedPrice;
        data.UpdatedTime = DateTime.UtcNow; 

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ToggleGameVisibility(int? id, bool visible)
    {
        if (!id.HasValue) BadRequest();
        var data = await _context.Games.FindAsync(id);
        if (data == null) return NotFound();

        data.IsDeleted = !visible;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index)); 
    }
    public async Task<IActionResult> Show(int? id)
        => await ToggleGameVisibility(id, true);

    public async Task<IActionResult> Hide(int? id)
        => await ToggleGameVisibility(id, false);


    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue) BadRequest();
        var data = await _context.Games.FindAsync(id);
        if (data == null) return NotFound();

        string filePath = Path.Combine(_env.WebRootPath, "imgs", "games", data.ImageUrl);

        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath); 

        _context.Games.Remove(data); 
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

}
