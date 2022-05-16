using System;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Controllers
{

	[ApiController]
	[Route("[controller]/[action]")]
	public class GatewayController: ControllerBase
	{
		public GatewayController()
		{
		}

		[HttpGet]
		public string Get()
        {
			return "gateway running";
        }

	}
}

