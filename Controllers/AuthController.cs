using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using WebService;
using System.Net;
using UmbracoLogin.Models;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace UmbracoLogin.Controllers
{
	public class AuthController : SurfaceController
	{
		public AuthController(
		IUmbracoContextAccessor umbracoContextAccessor,
		IUmbracoDatabaseFactory databaseFactory,
		ServiceContext services,
		AppCaches appCaches,
		IProfilingLogger profilingLogger,
		IPublishedUrlProvider publishedUrlProvider)
		: base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
		{
		}

		[HttpPost]
		public async Task<IActionResult> HandleSignIn(SignInFormModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.ToJsonErrors());

			try
			{
				using (var client = new ICUTechClient())
				{
					string ip = Request.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? IPAddress.None.ToString();
					LoginResponse response = await client.LoginAsync(model.Email, model.Password, ip);


					var customerData = JsonSerializer.Deserialize<CustomerData>(response.@return);

					if (customerData != null && customerData.EntityId != 0)
					{
						var jsonPretty = JToken.Parse(response.@return).ToString();
						TempData["Success"] = true;
						TempData["Message"] = $"Login successful!\n{jsonPretty}";
					}
					else
					{
						var resultData = JsonSerializer.Deserialize<ResultData>(response.@return);

						TempData["Success"] = false;	
						TempData["Message"] = $"Login failed. Please check your credentials\n(Not production!) Code: {resultData?.ResultCode}\nMessage: {resultData?.ResultMessage}";
					}
				}
			}
			catch (Exception ex)
			{
				TempData["Success"] = false;
				TempData["Message"] = "An error occurred: " + ex.Message;
			}

			// Redirect back to the login page
			return RedirectToCurrentUmbracoPage();
		}


		[HttpPost]
		public async Task<IActionResult> HandleSignUp(SignupFormModel model)
		{
			if (ModelState.IsValid)
			{
				// You can get the IP address like this
				model.SignupIP = Request.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? IPAddress.None.ToString();
				model.aID = 01300;

				using (var client = new ICUTechClient())
				{
					RegisterNewCustomerResponse response = await client.RegisterNewCustomerAsync(
					model.Email,
					model.Password,
					model.FirstName,
					model.LastName,
					model.Mobile,
					model.CountryID,
					model.aID,
					model.SignupIP
					);


					var responseObject = JsonSerializer.Deserialize<RegisterNewCustomer>(response.@return);

					if (responseObject != null && responseObject.ResultCode == 1 && responseObject.EntityId != -1)
					{
						return Redirect("SignIn");
					}
					else
					{
						// Handle the case where the registration failed
						ModelState.AddModelError("", $"Error: {responseObject?.ResultMessage}");
						return BadRequest(ModelState.ToJsonErrors());
					}

				}
			}
			return BadRequest(ModelState);

		}
	}
}

public class RegisterNewCustomer
{
	public int ResultCode { get; set; }
	public string ResultMessage { get; set; }
	public int EntityId { get; set; }
	public int AffiliateResultCode { get; set; }
	public string AffiliateResultMessage { get; set; }
}


public class ResultData
{
	public int ResultCode { get; set; }
	public string ResultMessage { get; set; }
}

public class CustomerData
{
	public int EntityId { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Company { get; set; }
	public string Address { get; set; }
	public string City { get; set; }
	public string Country { get; set; }
	public string Zip { get; set; }
	public string Phone { get; set; }
	public string Mobile { get; set; }
	public string Email { get; set; }
	public int EmailConfirm { get; set; }
	public int MobileConfirm { get; set; }
	public int CountryID { get; set; }
	public int Status { get; set; }
	public string lid { get; set; }
	public string FTPHost { get; set; }
	public int FTPPort { get; set; }
}
