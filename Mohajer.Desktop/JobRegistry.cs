using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mohajer.Desktop
{
    class JobRegistry : FluentScheduler.Registry
    {
        public JobRegistry()
        {
            NonReentrantAsDefault();

            Schedule<ReserveJob>().ToRunEvery(1).Days().At(0, 0);
            Schedule<ReserveJob>().ToRunEvery(1).Days().At(1, 0);
            Schedule<ReserveJob>().ToRunEvery(1).Days().At(2, 0);
            Schedule<ReserveJob>().ToRunEvery(1).Days().At(3, 0);
            Schedule<ReserveJob>().ToRunEvery(1).Days().At(4, 0);
            Schedule<ReserveJob>().ToRunEvery(1).Days().At(5, 0);
            Schedule<ReserveJob>().ToRunEvery(1).Days().At(6, 0);

            Schedule<ReserveJob>().ToRunEvery(1).Days().At(15, 0);
            Schedule<ReserveJob>().ToRunEvery(1).Days().At(16, 0);
            Schedule<ReserveJob>().ToRunEvery(1).Days().At(17, 0);

            Schedule<ReserveJob>().ToRunEvery(1).Days().At(21, 0);
            Schedule<ReserveJob>().ToRunEvery(1).Days().At(22, 0);
            Schedule<ReserveJob>().ToRunEvery(1).Days().At(23, 0);
        }
    }
}
