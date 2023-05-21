namespace LMS.StartupPrep
{
	public static partial class EndpointMapper
	{
		public static WebApplication RegisterEndpoints(this WebApplication app)
		{
			//app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
			//app.MapControllers();
			//app.MapRazorPages();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

				endpoints.MapRazorPages();
				endpoints.MapControllers();
			});
			return app;
		}
	}
}