using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.IoC
{
    public interface ICoreModule
    {
        //DependencyResolvers yani bağımlılıkları yükleyecek metot
        void Load(IServiceCollection serviceCollection);
    }
}
