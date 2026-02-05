using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ejercicio1Bol6
{
    internal class ServidorFechaHora
    {
        public bool servidorActivo = true;
        public int PUERTO = 5000;

        static void Main(string[] args)
        {
            ServidorFechaHora sfh = new ServidorFechaHora();
            IPEndPoint ie = new IPEndPoint(IPAddress.Any, sfh.PUERTO);

            using (Socket servidor = new Socket(AddressFamily.InterNetwork,
                                                SocketType.Stream,
                                                ProtocolType.Tcp))
            {
                servidor.Bind(ie);
                servidor.Listen(10);

                Console.WriteLine("El servidor se ha iniciado en el siguiente puerto: " + sfh.PUERTO);

                while (sfh.servidorActivo)
                {
                    Socket cliente = servidor.Accept();
                    //Thread hilo = new Thread(() => AtenderCliente(cliente));
                    //tengo que cambiar la forma de llamarlo porque si no en cada vuelta estoy creabndo
                    //un servidor para cada hilo y eso no puede ser porque quedarian hilos abiertos,
                    //entonces, si uso el servidor actual unicamente ya no pasaria y para ello tengo
                    //que pasarlo como parametro
                    Thread hilo = new Thread(() => AtenderCliente(cliente, sfh));
                    hilo.Start();
                }
            }

            Console.WriteLine("Se cerro el servidor");
        }

        static void AtenderCliente(Socket socketCliente, ServidorFechaHora sfh)
        {
            //ServidorFechaHora sfh2 = new ServidorFechaHora();
            //y ahora elimino esta linea porque ya no seria necesaria por pasarle el 
            //servidor actual que ya se inicializo mas arriba
            using (socketCliente)
            {
                try
                {
                    NetworkStream ns = new NetworkStream(socketCliente);
                    StreamReader sr = new StreamReader(ns);
                    StreamWriter sw = new StreamWriter(ns);
                    sw.AutoFlush = true;

                    string comando = sr.ReadLine();

                    if (comando == null)
                        return;

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
                        string[] partes = comando.Split(' ');

                        if (partes.Length < 2)
                        {
                            sw.WriteLine("ERROR: No se ha enviado contraseña");
                        }
                        else
                        {
                            string passwordCliente = partes[1];
                            string passwordServidor = LeerPassword();

                            if (passwordCliente == passwordServidor)
                            {
                                sw.WriteLine("Servidor apagado correctamente");
                                sfh.servidorActivo = false;
                                //y aqui es donde cerramos el servidor actual poniendolo a false
                            }
                            else
                            {
                                sw.WriteLine("ERROR: Contraseña incorrecta");
                            }
                        }
                    }
                    else
                    {
                        sw.WriteLine("ERROR: Comando no válido");
                    }
                }
                catch
                {
                    // Error de cliente, se ignora
                }
            }
        }

        static string LeerPassword()
        {
            string ruta = Environment.GetFolderPath(
                              Environment.SpecialFolder.CommonApplicationData)
                              + "\\password.txt";

            try
            {
                using (StreamReader sr = new StreamReader(ruta))
                {
                    return sr.ReadLine();
                }
            }
            catch
            {
                return "";
            }
        }
    }

}


