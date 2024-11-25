using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Web_ExclusivedFood
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            // verificar  si existe una cookie de autenticacióo
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                try
                {
                    // Desencriptar el ticket de autenticación
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    if (authTicket != null && !authTicket.Expired) // se valida si no esta expirado o es nulo
                    {

                        var datos = authTicket.UserData.Split(';');
                        string[] roles = datos[0].Split(',');


                        var identity = new GenericIdentity(authTicket.Name);
                        var principal = new GenericPrincipal(identity, roles);

                        // Asignar el usuario autenticado y sus roles al contexto de seguridad
                        Context.User = principal;
                    }
                }
                catch (ArgumentException)
                {
                    FormsAuthentication.SignOut();
                }
            }

        }

    }
}
