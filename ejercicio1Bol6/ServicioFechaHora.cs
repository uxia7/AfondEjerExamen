using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ejercicio1Bol6
{
    partial class ServicioFechaHora : ServiceBase
    {
        private ServidorFH servidor;
        //como la clase servidor con los datos ya la tenemos creada la vamos
        //a usar para crear el objeto correspondiente al servicio y poder usar sus funciones
        //NOTA! no es lo mismo el servidor que es la clase que tenemos que el servicio que vamos a crear
        //no confundirlos

        private Thread hiloServidor;
        //tambien es necesario crear un hilo y se crea con la clase Thread
        public ServicioFechaHora()
        {
            //en el constructor como siempre se inicializan los componentes
            //a los valores que queramos
            InitializeComponent();
            //este viene por defecto, no lo toco

            //en el panel de propiedades de la pestaña de diseño, ya puse los valores
            //que eran necesarios reajustar para los requisitos del ejercicio
            //this.ServiceName = "ServicioFechaHora"; 
            //this.CanStop = true; 
            //this.CanPauseAndContinue = false;
            //this.AutoLog = false;
        }

        //estas dos funciones las de OnStart y OnStop vienen por defecto al crear la clase que hereda
        //de la clase ServiceBase, entonces dentro tenemos que crear el codigo que nos interesa que se ejecute en:
        //OnStop -> cuando se para el servicio
        //OnStart -> al iniciar el servicio
        protected override void OnStart(string[] args)
        {
            // TODO: agregar código aquí para iniciar el servicio.
            servidor = new ServidorFH(); 
            hiloServidor = new Thread(() => servidor.empezarServicio()); 
            hiloServidor.IsBackground = true; 
            hiloServidor.Start();
        }

        protected override void OnStop()
        {
            // TODO: agregar código aquí para realizar cualquier anulación necesaria para detener el servicio.
            if (servidor != null)
            {
                servidor.servidorActivo = false;
                servidor.CerrarSocketEscucha();
            }
        }
    }
}
