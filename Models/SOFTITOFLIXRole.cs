using System;
using Microsoft.AspNetCore.Identity;
namespace SOFTITOFLIX.Models
{
	public class SOFTITOFLIXRole : IdentityRole<long>
	{
		public SOFTITOFLIXRole(string roleName):base(roleName)
		{ }
		public SOFTITOFLIXRole()
		{ }
	}
}

