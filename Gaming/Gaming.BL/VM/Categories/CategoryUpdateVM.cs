using System;
using System.ComponentModel.DataAnnotations;

namespace Gaming.BL.VM.Categories;
public class CategoryUpdateVM
{
    [Required(ErrorMessage = "Name is required!"), MaxLength(32, ErrorMessage = "Name length must be less than 32 charachters")]
    public string Name { get; set; }
}

