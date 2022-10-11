using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{



    class Program
    {

        static List<string> users = new List<string>();
        static int counter = 0;


        private static void ThreadingClients(object arg)
        {
            string name = users.ElementAt(counter-1);

            TcpClient client = (TcpClient)arg;
            NetworkStream stream = client.GetStream();

            string msg;
            byte[] buff = new byte[64];

            while (true)
            {


                stream.Read(buff, 0, buff.Length);

                string received = Encoding.ASCII.GetString(buff);

                byte[] send = Encoding.ASCII.GetBytes(received);

                Console.WriteLine(name + received + " this works");

                stream.Flush();

                stream.Write(send, 0, send.Length);

                Console.WriteLine(Encoding.ASCII.GetString(send) + " this works too");

            }

            client.Close();

        }





        static void Main(string[] args)
        {

            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 2222); // initialize the tcp listener
            listener.Start(); //start listening
            Console.WriteLine("|--- ChatServer started ---|");

            //infinite loop for multi clients

            while (true)
            {

                Console.WriteLine("Awaiting new connection...");

                    TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine("New connection established");

                    string received = RetName(client);

                if (!users.Contains(received))
                {

                    users.Add(received);
                    counter++;
                    Console.WriteLine(counter);
                    //create new thread for every new client

                    Thread newClient = new Thread(ThreadingClients);
                    newClient.Start(client);

                    Console.WriteLine(received + "Joined");
                    byte[] err = new byte[256];
                    err = Encoding.ASCII.GetBytes(received + "Joined");
                    // userStream.Write(buff, 0, buff.Length);
                }

                else
                {
                    byte[] err = new byte[256];
                    err = Encoding.ASCII.GetBytes("Username already taken!");
                    // userStream.Write(buff, 0, buff.Length);
                    Console.WriteLine("Username already taken!");
                }


            }//end while loop

        }

        private static string RetName(TcpClient check)
        {

            NetworkStream userStream = check.GetStream();
            byte[] buff = new byte[24];
            userStream.Read(buff, 0, buff.Length);
            string received = Encoding.ASCII.GetString(buff);

            return received;


        }

    }
}
