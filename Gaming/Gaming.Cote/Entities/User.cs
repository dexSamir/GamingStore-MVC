using System;
using Microsoft.AspNetCore.Identity;

namespace Gaming.Core.Entities;
public class User : IdentityUser
{
	public string Fullname { get; set; }
}

