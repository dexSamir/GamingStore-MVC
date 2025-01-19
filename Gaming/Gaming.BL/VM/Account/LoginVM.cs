using System;
using System.ComponentModel.DataAnnotations;

namespace Gaming.BL.VM.Account;
public class LoginVM
{
	[Required, MaxLength(128)]
	public string EmailOrUsername { get; set; }

	[Required, DataType(DataType.Password)]
	public string Password { get; set; }

	public bool RememberMe { get; set; }
}

