using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ejercicio1Bol6
{
    internal class Program
    {
        static void Main ()
        {
            ServiceBase[] ServicesToRun = new ServiceBase[] 
            { 
                new ServicioFechaHora() 
            }; 
            ServiceBase.Run(ServicesToRun);
        }
    }
}
