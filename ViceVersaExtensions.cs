﻿using Xmu.Crms.Services.ViceVersa;
<<<<<<< HEAD
using Xmu.Crms.Shared.Models;
=======
using Xmu.Crms.Services.ViceVersa.Daos;
using Xmu.Crms.Services.ViceVersa.Services;
>>>>>>> master
using Xmu.Crms.Shared.Service;

namespace Microsoft.Extensions.DependencyInjection
{ 
    public static class ViceVersaExtensions
    {
        // 为每一个你写的Service写一个这样的函数，把 UserService 替换为你实现的 Service
        public static IServiceCollection AddViceVersaClassService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<IClassService, ClassService>();
        }

        public static IServiceCollection AddViceVersaClassDao(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<IClassDao,ClassDao>();
        }

        public static IServiceCollection AddViceVersaGradeService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<IGradeService, GradeService>();
        }


        // 为每一个你写的Dao写一个这样的函数，把 UserDao 替换为你实现的 Dao
        public static IServiceCollection AddViceVersaGradeDao(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<IGradeDao, GradeDao>();
        }
    }
}
