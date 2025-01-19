using System;
using Gaming.Core.Entities.Base;

namespace Gaming.Core.Entities;
public class Category : BaseEntity 
{
	public string Name { get; set; }
	public ICollection<Game>? Games { get; set; } = new HashSet<Game>(); 
}

