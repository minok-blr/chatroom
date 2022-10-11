using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.IO;

namespace WindowsFormsApp2CHATCLIENT
{

   



    public partial class Form1 : Form
    {
        NetworkStream networkStream;
        TcpClient client = new TcpClient();


        public string getEncrypted(NetworkStream strm)
        {
            MemoryStream memstrm = new MemoryStream();

            byte[] Key = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                    0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};

            byte[] IV = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                    0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            CryptoStream csw = new CryptoStream(memstrm, tdes.CreateDecryptor(Key, IV),
                CryptoStreamMode.Write);

            byte[] data = new byte[1024];
            int recv = strm.Read(data, 0, 4);
            int size = BitConverter.ToInt32(data, 0);
            int offset = 0;
            while (size > 0)
            {
                recv = strm.Read(data, 0, size);
                csw.Write(data, offset, recv);
                offset += recv;
                size -= recv;
            }
            csw.FlushFinalBlock();
            memstrm.Position = 0;
            byte[] info = memstrm.GetBuffer();
            int infosize = (int)memstrm.Length;
            csw.Close();
            memstrm.Close();
            return Encoding.ASCII.GetString(info, 0, infosize);
        }

        public void sendEncrypted(NetworkStream strm, string data)
        {
            MemoryStream memstrm = new MemoryStream();

            byte[] Key = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                    0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};

            byte[] IV = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                   0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16};

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            CryptoStream csw = new CryptoStream(memstrm, tdes.CreateEncryptor(Key, IV),
                      CryptoStreamMode.Write);

            csw.Write(Encoding.ASCII.GetBytes(data), 0, data.Length);
            csw.FlushFinalBlock();

            byte[] cryptdata = memstrm.GetBuffer();
            int size = (int)memstrm.Length;
            byte[] bytesize = BitConverter.GetBytes(size);
            strm.Write(bytesize, 0, 4);
            strm.Write(cryptdata, 0, size);
            strm.Flush();
            csw.Close();
            memstrm.Close();
        }

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Button1_ClickConnect(object sender, EventArgs e)
        {

            try
            {
                client.Connect(IPAddress.Parse("127.0.0.1"), 2222);
                networkStream = client.GetStream();
                chatbox.Items.Add("Connected to chatroom!");

                sendEncrypted(networkStream, textBox1.Text + ">$e<");

                textBox1.Clear();
                textBox1.Enabled = false;
                button1.Enabled = false;

                Thread ctThread = new Thread(getMessage);
                ctThread.Start();

            } catch(SocketException x )
            {
                MessageBox.Show("Server not available!");
                
            }
            

        }

        private void getMessage()
        {
            while (true)
            {

                networkStream = client.GetStream();
                string rec = getEncrypted(networkStream);
                //byte[] inStream = new byte[128];
                //networkStream.Read(inStream, 0, inStream.Length);
                //string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                rec = rec.Substring(0, rec.IndexOf(">$e<"));
                chatbox.Items.Add(rec);
                
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            
                
        }

        private void button2_Click(object sender, EventArgs e)
        {

            sendEncrypted(networkStream, textBox2.Text + ">$e<");
            byte[] ToSend = Encoding.ASCII.GetBytes(textBox2.Text + ">$e<");
            textBox2.Clear();
          
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
        }

        private void chatbox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textbox2_keyEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button2_Click(this, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                Button1_ClickConnect(this, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            networkStream.Close();
            client.Client.Disconnect(true);
        }
    }
}

/*   private void receivedData()
        {
            int recv;
            string stringData;
            while (true)
            {
                clien
                recv = client.Receive(data);
                stringData = Encoding.ASCII.GetString(data, 0, recv);
                if (stringData == "bye")
                    break;
                results.Items.Add(stringData);
            }
            stringData = "bye";
            byte[] message = Encoding.ASCII.GetBytes(stringData);
            client.Send(message);
            client.Close();
            results.Items.Add("Connection stopped");
            return;
        }*/

//DateTime.Now + " " + textBox1.Text;
//listBox1.Items.Add(toSendBuff);
