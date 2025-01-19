using System.Diagnostics;
using Gaming.DAL.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gaming.MVC.Controllers;

public class HomeController : Controller
{
    readonly AppDbContext _context;
    public HomeController(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        return View(await _context.Games.Where(x=> !x.IsDeleted).ToListAsync());
    }
}

