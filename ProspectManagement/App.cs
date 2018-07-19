using MvvmCross.ViewModels;
using MvvmCross.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProspectManagement.Core.ViewModels;

namespace ProspectManagement.Core
{
    public class App: MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Repository")
                .AsInterfaces()
                .RegisterAsLazySingleton();
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

			//RegisterCustomAppStart<AppStart>();
			RegisterAppStart<RootViewModel>();
        }
    }
}
