using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Gaming.BL.VM.Games;
public class GameUpdateVM
{
    [Required(ErrorMessage = "Name is required!"), MaxLength(64, ErrorMessage = "Name length must be less than 32 charachters")]
    public string Name { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal? DiscountedPrice { get; set; }

    [Required(ErrorMessage = "Category is required!")]
    public int? CategoryId { get; set; }
    public IFormFile? Image { get; set; }
    public string? ExistingImageUrl { get; set; }
}

