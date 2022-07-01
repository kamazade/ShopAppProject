using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using shopapp.business.Abstract;
using shopapp.business.Concrete;
using shopapp.data.Abstract;
using shopapp.data.Concrete.EfCore;
using shopapp.webui.EmailServices;
using shopapp.webui.Identity;

namespace shopapp.webui
{
    public class Startup
    {   
        private IConfiguration _configuration; 
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options=> options.UseSqlite("Data Source = shopDb"));
            services
                .AddIdentity<User,IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();
            //Identity Settings
            services.Configure<IdentityOptions>(options=> {
                //password
                options.Password.RequireDigit = false; // Password must contain a numeric char.
                options.Password.RequireLowercase = false; // Password must contain a lowercase char 
                options.Password.RequireUppercase = false; // Password must contain a Uppercase char
                options.Password.RequiredLength = 5; // Min 5 chars
                options.Password.RequireNonAlphanumeric= false; // No need to contain alphanumeric chars

                // Lockout
                options.Lockout.AllowedForNewUsers = true; // activate lockout
                options.Lockout.MaxFailedAccessAttempts = 5; // lock the account after 5 failed access attempts
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // user can access the account after 5 minutes 

                // User
                // options.User.AllowedUserNameCharacters="";
                options.User.RequireUniqueEmail =true;

                // SignIn
                options.SignIn.RequireConfirmedEmail = true; // Confirm account with email
                options.SignIn.RequireConfirmedPhoneNumber =false; // Confirm account with phone
            });    

            // Cookies
            services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true; // if false ; Default cookie lifetime is 20 mins .After 20 mins system authomatically logs out . if true ; in 20 mins ,if user make a request 20 mins resets and starts form 0 again.
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // changing default 20 mins value to 30 mins 

                options.Cookie = new CookieBuilder() {
                    HttpOnly =true,
                    Name = ".Shopapp.Security.Cookie",
                    SameSite = SameSiteMode.Strict //if someone stole users cookie and try to reach users account from an other device this is going to prevent it  


                };

            });



            // Burada IProductService i çağırınca  ProductManager gelecek ProductManager da kendi içinde IProductRepository i çağıracak IProductRepository de EfCoreProductRepository i çağıracak.
            services.AddScoped<IProductService,ProductManager>(); 
            services.AddScoped<IProductRepository,EfCoreProductRepository>(); 
            
            services.AddScoped<ICategoryService,CategoryManager>();
            services.AddScoped<ICategoryRepository,EfCoreCategoryRepository>(); 
            
            services.AddScoped<ICartService,CartManeger>();
            services.AddScoped<ICartRepository,EfCoreCartRepository>();

            services.AddScoped<IOrderService,OrderManager>();
            services.AddScoped<IOrderRepository,EfCoreOrderRepository>();


            services.AddScoped<IAdressService,AdressManager>();
            services.AddScoped<IAdressRepository,EfCoreAdressRepository>();


            services.AddScoped<IEmailSender,SmtpEmailSender>(i=>
                new SmtpEmailSender(
                    _configuration["EmailSender:Host"],
                    _configuration.GetValue<int>("EmailSender:Port"),
                    _configuration.GetValue<bool>("EmailSender:EnableSSL"),
                    _configuration["EmailSender:UserName"],
                    _configuration["EmailSender:Password"]
                )
            );

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IConfiguration configuration,UserManager<User> userManager,RoleManager<IdentityRole> roleManager)
        {
            app.UseStaticFiles(); // wwwroot altındaki klasorleri açtık
            app.UseStaticFiles( // Bootstrap için node_modules klasörünü açıyoruz
                new StaticFileOptions(){
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(Directory.GetCurrentDirectory(),"node_modules")
                    ),
                    RequestPath ="/modules"
                }
            );
            if (env.IsDevelopment())
            {
                SeedData.Seed(); // DataSeeding yapıyoruz
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication(); // for Identity . Then create a migration and update database

            app.UseRouting();

            app.UseAuthorization(); // for authorization .Must be right here  
       
            app.UseEndpoints(endpoints =>
            {   
                //Cart Pages
                endpoints.MapControllerRoute(
                    name:"Checkout",
                    pattern:"checkout",
                    defaults: new {controller = "Cart",action ="Checkout"}
                    
                );
                endpoints.MapControllerRoute(
                    name:"CartIndex",
                    pattern:"cart",
                    defaults: new {controller = "Cart",action ="Index"}
                    
                );
                
                //account pages
                endpoints.MapControllerRoute(
                    name:"AccessDenied",
                    pattern:"admin/accessdenied/{ReturnUrl?}",
                    defaults: new {controller = "Admin",action ="AccessDenied"}
                    
                );
                //admin sayfaları
                //Admin Users
                endpoints.MapControllerRoute(
                    name:"adminUserDelete",
                    pattern:"admin/orders",
                    defaults: new {controller = "Admin",action ="OrderList"}
                    
                );
                //Admin Users
                endpoints.MapControllerRoute(
                    name:"adminUserDelete",
                    pattern:"admin/users",
                    defaults: new {controller = "Admin",action ="UserList"}
                    
                );
                endpoints.MapControllerRoute(
                    name:"adminUserDelete",
                    pattern:"admin/user/{id}",
                    defaults: new {controller = "Admin",action ="EditUser"}
                    
                );

                //Admin Roles
                endpoints.MapControllerRoute(
                    name:"adminroleEdit",
                    pattern:"admin/role/{id}",
                    defaults: new {controller = "Admin",action ="RoleEdit"}
                    
                );
                
                endpoints.MapControllerRoute(
                    name:"adminroles",
                    pattern:"admin/role/list",
                    defaults: new {controller = "Admin",action ="RoleList"}
                    
                );
                endpoints.MapControllerRoute(
                    name:"adminroleCreate",
                    pattern:"admin/role/create",
                    defaults: new {controller = "Admin",action ="CreateRole"}
                    
                );
                // Admin Categories
                endpoints.MapControllerRoute(
                    name:"admincategories",
                    pattern:"admin/categories",
                    defaults: new {controller = "Admin",action ="CategoryList"}
                    
                );
                endpoints.MapControllerRoute(
                    name:"admincategorycreate",
                    pattern:"admin/categories/create",
                    defaults: new {controller = "Admin",action ="CreateCategory"}
                    
                );       
                endpoints.MapControllerRoute(
                    name:"admincategoryedit",
                    pattern:"admin/categories/{id?}",
                    defaults: new {controller = "Admin",action ="EditCategory"}
                    
                );       
                //Admin Products   
                endpoints.MapControllerRoute(
                    name:"adminproducts",
                    pattern:"admin/products",
                    defaults: new {controller = "Admin",action ="ProductList"}
                    
                );
                endpoints.MapControllerRoute(
                    name:"adminproductcreate",
                    pattern:"admin/products/create",
                    defaults: new {controller = "Admin",action ="CreateProduct"}
                    
                );     
                endpoints.MapControllerRoute(
                    name:"adminedit",
                    pattern:"admin/products/{id?}",
                    defaults: new {controller = "Admin",action ="EditProduct"}
                    
                );
                
                
                //kullanıcı sayfaları    
                endpoints.MapControllerRoute(
                    name:"search",
                    pattern:"search",
                    defaults: new {controller = "Shop",action ="search"}
                    
                );
                    
                endpoints.MapControllerRoute(
                    name:"productdetails",
                    pattern:"details/{url}",
                    defaults: new {controller = "Shop",action ="details"}
                    
                );
               
                endpoints.MapControllerRoute(
                    name:"products",
                    pattern:"products/{category?}",
                    defaults: new {controller = "Shop",action ="list"}
                    
                );

                endpoints.MapControllerRoute(
                    name:"home",
                    pattern:"home",
                    defaults: new {controller = "home",action ="index"}
                    
                );
                //Default Route
                endpoints.MapControllerRoute(
                    name:"default",
                    pattern:"{controller=home}/{action=index}/{id?}"
                );
            });
        
            SeedIdentity.Seed(userManager,roleManager,configuration).Wait();

        }
    }
}
