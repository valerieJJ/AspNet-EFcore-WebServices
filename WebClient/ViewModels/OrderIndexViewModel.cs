using System;
using Neverland.Domain;

namespace Neverland.Web.ViewModels
{
	public class OrderIndexViewModel
	{
		public IEnumerable<Order> Orders { get; set; }
	}
}

