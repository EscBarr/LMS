namespace LMS.StartupPrep
{
    public static partial class MiddlewareInitializer
    {
        public static WebApplication ConfigureMiddleware(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            ///ВИДИМО ПОСТОЯННОЕ ХРАНЕНИЕ JWT-ТОКЕНА
            app.Use(async (context, next) =>
            {
                var token = context.Session.GetString("Token");
                if (!String.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + token);
                }
                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMvc(routes => RouteConfig.RegisterRoutes(routes));
            //app.UseSwagger().UseSwaggerUI();

            return app;
        }
    }
}