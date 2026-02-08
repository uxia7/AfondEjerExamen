using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ejercicio1Bol6
{
    public partial class Form1 : Form
    {
        public string ip = "127.0.0.1";
        public int puerto = 5000;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnTiempo_Click(object sender, EventArgs e)
        {
            //EnviarComando("tiempo");
            EnviarComando("time");
            //debe de enviar time no tiempo y asi tambien con todos los de abajo,
            //porque es lo de las especificaciones del ejercicio e igual podria estar mal
        }

        private void btnFecha_Click(object sender, EventArgs e)
        {
            //EnviarComando("fecha");
            EnviarComando("date");

        }

        public void btnTodo_Click(object sender, EventArgs e)
        {
            //EnviarComando("todo");
            EnviarComando("all");
        }
        public void btnCierre_Click(object sender, EventArgs e)
        {
            EnviarComando("close " + txtContraseña.Text);
            //tengo que ponerle un espacio porque si no, el servidor
            //no recibe lo que espera y no me funciona
        }
        
        public void EnviarComando(string comando)
        {
            Thread hilo = new Thread(() =>
            {
                try
                {
                    IPEndPoint ie = new IPEndPoint(IPAddress.Parse(ip), puerto);
                    Socket servidor = new Socket(AddressFamily.InterNetwork,
                                                 SocketType.Stream,
                                                 ProtocolType.Tcp);

                    servidor.Connect(ie);

                    NetworkStream ns = new NetworkStream(servidor);
                    StreamReader sr = new StreamReader(ns);
                    StreamWriter sw = new StreamWriter(ns);
                    //sw.AutoFlush = true;

                    sw.WriteLine(comando);
                    string respuesta = sr.ReadLine();

                    Invoke(new Action(() =>
                    {
                        lblResultado.Text = respuesta;
                    }));

                    servidor.Close();
                }
                catch
                {
                    Invoke(new Action(() =>
                    {
                        lblResultado.Text = "No se pudo conectar";
                    }));
                }
            });

            hilo.Start();
        }
    }
}
