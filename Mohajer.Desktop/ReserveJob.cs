using FluentScheduler;
using Mohajer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mohajer.Desktop
{
    class ReserveJob : IJob
    {
        public async void Execute()
        {
            var reserver = Caliburn.Micro.IoC.Get<IReserver>();
            await reserver.Run();
        }
    }
}
