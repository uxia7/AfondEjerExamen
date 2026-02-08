using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ejercicio1Bol6
{
    internal class ServidorFH
    {
        //necesitamos dos variables las cuales serían una para
        //saber si el servidor esta activo o no siendo una boolean
        public bool servidorActivo = true;
        //esta es para saber el puerto, que por defecto la inicializo
        //a 5000 para tener un valor por defecto
        public int puertoDefec = 5000;

        //creamos una varibale Socket
        private Socket sEscucha;

        //static void Main(string[] args)
        //{
        //    ServidorFechaHora sfh = new ServidorFechaHora();
        //    IPEndPoint ie = new IPEndPoint(IPAddress.Any, sfh.PUERTO);

        //    using (Socket servidor = new Socket(AddressFamily.InterNetwork,
        //                                        SocketType.Stream,
        //                                        ProtocolType.Tcp))
        //    {
        //        servidor.Bind(ie);
        //        servidor.Listen(10);

        //        Console.WriteLine("Servidor iniciado en el puerto: " + sfh.PUERTO);

        //        while (sfh.servidorActivo)
        //        {
        //            Socket cliente = servidor.Accept();
        //            //Thread hilo = new Thread(() => AtenderCliente(cliente));
        //            //tengo que cambiar la forma de llamarlo porque si no en cada vuelta estoy creabndo
        //            //un servidor para cada hilo y eso no puede ser porque quedarian hilos abiertos,
        //            //entonces, si uso el servidor actual unicamente ya no pasaria y para ello tengo
        //            //que pasarlo como parametro
        //            Thread hilo = new Thread(() => AtenderCliente(cliente, sfh));
        //            hilo.Start();
        //        }
        //    }

        //    Console.WriteLine("Servidor cerrado");

        //    //lo de arriba es lo que pertenece al ejercicio del servidor
        //    //lo de aqui abajo es correspondiente al servicio
        //    //es para que cuando se ejecute el programa vendrá al Main y podrá inciar el servicio
        //    //ServiceBase[] ServicesToRun = new ServiceBase[]
        //    //{
        //    //    new ServicioFechaHora()
        //    //};
        //    //ServiceBase.Run(ServicesToRun);
        //    //me da error si pongo esto aqui, es que este program tiene que ser sin Main porque
        //    //ahora tiene que funcionar como servicio no con el servidor, al final
        //    //este es el servidor de Fecha y Hora y debería de quedar y pasar a una clase
        //    //lo que tengo que hacer es crear una nueva clase con un unico Main allí y
        //    //ese correspondera a la ejecucion del servicio
        //}

        //este main que pertenecia al ejercicio de servidores, pase a ser anulado debido al 
        //nuevo main que tiene que tener el servicio, ya que hay que ejecutar solo el servicio 
        //que es el que dará paso a ejecutar todo lo demás

        static void AtenderCliente(Socket socketCliente) //, ServidorFechaHora sfh
        {
            //cuando ya lo usamos como servidor no hace falta poner
            //ServidorFechaHora sfh2 = new ServidorFechaHora();
            //y ahora elimino esta linea porque ya no seria necesaria por pasarle el 
            //servidor actual que ya se inicializo mas arriba
            using (socketCliente)
            {
                //try
                //{
                //esto es lo que añadimos en esta clase para el tema del servicio que es lo que tiene
                //que ver con lo de la IP que es lo que se pedia, teniendo que ajustar esta funcion
                IPEndPoint remoto = (IPEndPoint)socketCliente.RemoteEndPoint; 
                string ip = remoto.Address.ToString(); 
                int puerto = remoto.Port;

                NetworkStream ns = new NetworkStream(socketCliente);
                StreamReader sr = new StreamReader(ns);
                StreamWriter sw = new StreamWriter(ns);
                //sw.AutoFlush = true;
                //no es realmente necesario y no estoy segura si se puede usar

                string comando = sr.ReadLine();

                if (comando == null)
                {
                    return;
                    //no se como ponerlo de otra manera pero es que cuando el comando es null
                    //no quiero que haga nada ni que continue la funcion asi que esto va asi
                }

                EscribirLog($"[@{ip}:{puerto}] {comando}");

                if (comando == "time")
                {
                    DateTime ahora = DateTime.Now;
                    sw.WriteLine(ahora.ToString("HH:mm:ss"));
                }
                else if (comando == "date")
                {
                    DateTime hoy = DateTime.Now;
                    sw.WriteLine(hoy.ToString("dd/MM/yyyy"));
                }
                else if (comando == "all")
                {
                    DateTime ahora = DateTime.Now;
                    sw.WriteLine(ahora.ToString("dd/MM/yyyy HH:mm:ss"));
                }
                else if (comando.StartsWith("close"))
                {
                    //string[] partes = comando.Split(' ');

                    //if (partes.Length < 2)
                    //{
                    //    sw.WriteLine("No se envió una contrasena valida");
                    //}
                    //else
                    //{
                    //    string conCliente = partes[1];
                    //    string conServidor = LeerPassword();

                    //    if (conCliente.Equals(conServidor))
                    //    {
                    //        sw.WriteLine("El servidor se puedo cerrar correctamente, las contraseñas coindcidieron");
                    //        sfh.servidorActivo = false;
                    //        //y aqui es donde cerramos el servidor actual poniendolo a false
                    //    }
                    //    else
                    //    {
                    //        sw.WriteLine("No, la constraseña fue incorrecta el servidor no se ha podido cerrar");
                    //    }
                    //}

                    sw.WriteLine("El comando no está dentro de lo permitido");
                    EscribirEvento("El comando no fue valido: " + comando, EventLogEntryType.Warning);
                }
                else
                {
                    sw.WriteLine("Tu comando no es valido");
                    EscribirEvento("El comando no fue valido: " + comando, EventLogEntryType.Warning);
                }
            //}
            //    catch
            //    {
                //si surge un error en el cliente se ignora
                //por eso el catch vacio
            //}
        }
        }

        //static string contrasena()
        //{
        //    string ruta = Environment.GetFolderPath(
        //                      Environment.SpecialFolder.CommonApplicationData)
        //                      + "\\password.txt";

        //    try
        //    {
        //        using (StreamReader sr = new StreamReader(ruta))
        //        {
        //            return sr.ReadLine();
        //        }
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}


        public void empezarServicio()
        {
            int puerto = lecturaPuertoConfig();
            int puertoFinal = escucharPuertos(puerto, puertoDefec);
            if (puertoFinal == -1)
            {
                EscribirEvento("No se ha podido abrir ningún puerto. Finalizando servicio.", EventLogEntryType.Error);
                return;
            }
            EscribirEvento($"Servidor escuchando en puerto {puertoFinal}", EventLogEntryType.Information);
            IPEndPoint ie = new IPEndPoint(IPAddress.Any, puertoFinal);
            sEscucha = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sEscucha.Bind(ie);
                sEscucha.Listen(10);
                while (servidorActivo)
                {
                    Socket cliente = sEscucha.Accept();
                    Thread hilo = new Thread(() => AtenderCliente(cliente));
                    hilo.Start();
                }
            }
            catch (Exception ex)
            {
                EscribirEvento("Error en bucle de escucha: " + ex.Message, EventLogEntryType.Error);
            }
        }
        public void CerrarSocketEscucha()
        {
            try
            {
                //if(sEscucha != null)
                //{
                //    sEscucha.Close();
                //}
                //se puede hacer de las dos maneras, voy a dejar la segunda
                //para irlo aprendiendo y practicando
                //en base en si es lo mismo, en caso de que sEscucha no sea null
                //entonces ejecuta el Close()
                sEscucha?.Close();
            }
            catch
            {
            }
        }



        public int lecturaPuertoConfig()
        {
            string ruta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "puerto_servidor.txt");

            try
            {
                if (!File.Exists(ruta))
                    throw new FileNotFoundException();

                string contenido = File.ReadAllText(ruta).Trim();
                if (int.TryParse(contenido, out int puerto))
                    return puerto;

                throw new FormatException();
            }
            catch (Exception ex)
            {
                EscribirEvento("Error al leer archivo de configuración de puerto: " + ex.Message,
                               EventLogEntryType.Error);
                return puertoDefec;
            }
        }

        private int escucharPuertos(int puertoConfig, int puertoDefecto)
        {
            foreach (int p in new int[] { puertoConfig, puertoDefecto })
            {
                try
                {
                    IPEndPoint ie = new IPEndPoint(IPAddress.Any, p);
                    using (Socket prueba = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        prueba.Bind(ie);
                    }
                    return p;
                }
                catch (SocketException)
                {
                    // puerto ocupado, probamos el siguiente
                }
            }
            return -1;
        }




        private const string fuente = "ServicioFechaHora";
        private const string destino = "Application";

        public static void EscribirEvento(string mensaje, EventLogEntryType tipo)
        {
            try
            {
                EventLog.WriteEntry(fuente, mensaje, tipo);
            }
            catch
            {
                //EscribirLog("[ERROR] " + mensaje);
                //return "El error fue: " + mensaje;
            }
        }


        public string RutaLog => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                 "servidor_fecha_hora.log");
        //no se si usando lambda puedo poner la variable como privada porque
        //no sabria como inicializarla con los set y gets

        public static void EscribirLog(string mensaje)
        {
            ServidorFH sfh = new ServidorFH();
            string linea = $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] {mensaje}";
            try
            {
                File.AppendAllText(sfh.RutaLog, linea + Environment.NewLine);
            }
            catch
            {
                // aquí ya no hacemos más, para no entrar en bucles
            }
        }


    }
}


