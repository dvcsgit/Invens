using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using PFG.Inventory.Data.Infrastructure;
using PFG.Inventory.Data.Repositories;
using PFG.Inventory.Domain.Services;
using PFG.Inventory.Web.Library.Authentication;
namespace PFG.Inventory.Web.App_Start
{
    public class Bootstrapper
    {
        public static void Run()
        {
            SetAutofacContainer();

        }

        private static void SetAutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterType<DatabaseFactory>().As<IDatabaseFactory>().InstancePerRequest();

            builder.RegisterAssemblyTypes(typeof(UserRepository).Assembly)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();

            //service
            builder.RegisterAssemblyTypes(typeof(UserService).Assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(DefaultFormsAuthentication).Assembly)
                .Where(t => t.Name.EndsWith("Authentication"))
                .AsImplementedInterfaces();

            //   builder.Register(c => new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new SocialGoalEntities())))
            //       .As<UserManager<ApplicationUser>>().InstancePerHttpRequest();

            //   builder.RegisterFilterProvider();
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}