using System;
using Gaming.Core.Entities.Base;

namespace Gaming.Core.Entities;
public class Game : BaseEntity
{
	public string Name { get; set; }
	public decimal Price { get; set; }
	public decimal? DiscountedPrice { get; set; }
	public int CategoryId { get; set; }
	public Category Category { get; set; }
	public string? ImageUrl { get; set; }
}

