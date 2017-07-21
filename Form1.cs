using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Runtime.Serialization;
using System.Dynamic;
using System.Web.Script.Serialization;

namespace WindowsFormsApp1
{
    public class MessageChannel
    {
        public string usuario { get; set; }
        public string cliente { get; set; }
        public string equipamento { get; set; }
        public string digital { get; set; }
    }

    public partial class Form1 : Form
    {
        string path = "finger.bmp";
        string encode;


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] pName = new byte[8 * 128];
            int i, iNum;

            //comboBox1.Items.Clear();
            iNum = Win32.AvzFindDevice(pName);

            for (i = 0; i < iNum; i++)
            {
                string encode = Encoding.Default.GetString(pName, i * 128, 128);
            }

            long lRet;

            lRet = Win32.AvzOpenDevice(Convert.ToInt16(encode), 0);

            if (lRet == 0)
            {
                MessageBox.Show("Conectado");
                UInt16 uStatus = 0;
                UInt16 Ret;
                MessageBox.Show("Coloque o Dedo");
                while (uStatus == 0)
                {
                    Win32.AvzGetImage(Convert.ToInt16(0), Win32.gpImage, ref uStatus);
                    Win32.AvzSaveHueBMPFile(path, Win32.gpImage);

                    Image bitmap1 = Image.FromFile(path);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    bitmap1.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    bitmap1.Dispose();
                    Image bitmap2 = Image.FromStream(ms);

                    pictureBox1.Image = bitmap2;
                
                    ms.Close();
                    Win32.AvzProcess(Win32.gpImage, Win32.gpFeatureA, Win32.gpBin, 1, 1, 94);

                }

                while (uStatus == 1)
                {
                    MessageBox.Show("Retire o Dedo");
                    Win32.AvzGetImage(Convert.ToInt16(0), Win32.gpImage, ref uStatus);
                }

                MessageBox.Show("Coloque o Mesmo Dedo Novamente");
                while (uStatus == 0)
                {
                    Win32.AvzGetImage(Convert.ToInt16(0), Win32.gpImage, ref uStatus);
                    Win32.AvzSaveHueBMPFile(path, Win32.gpImage);

                    Image bitmap1 = Image.FromFile(path);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    bitmap1.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    bitmap1.Dispose();
                    Image bitmap2 = Image.FromStream(ms);

                    pictureBox1.Image = bitmap2;

                    ms.Close();
                
                    Win32.AvzProcess(Win32.gpImage, Win32.gpFeatureB, Win32.gpBin, 1, 1, 94);
                }

                Ret = Win32.AvzMatch(Win32.gpFeatureA, Win32.gpFeatureB, 9, 60);
                Win32.AvzCloseLED(Convert.ToInt16(encode));

                if (Ret == 0)
                {
                    MessageBox.Show("Sucesso na Captura");
                    Ret = Win32.AvzPackFeature(Win32.gpFeatureA, Win32.gpFeatureB, Win32.gpFeatureBuf1);
                    using (BinaryWriter binWriter = new BinaryWriter(File.Open("MyFile.anv", FileMode.Create)))
                    {
                        binWriter.Write(Win32.gpFeatureBuf1, 0, Ret);
                    }
                    
                }
                else
                {
                    MessageBox.Show("Erro na Captura");
                }
            }
            else
            {
                //comboBox1.Items.Clear();
                Win32.AvzCloseLED(Convert.ToInt16(encode));
                MessageBox.Show("ERROR - Desconectado");
            }

            Win32.AvzCloseDevice(Convert.ToInt16(encode));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Win32.AvzCloseLED(Convert.ToInt16(encode));

            byte[] pName = new byte[8 * 128];
            int i, iNum;

            //comboBox1.Items.Clear();
            iNum = Win32.AvzFindDevice(pName);

            for (i = 0; i < iNum; i++)
            {
                string encode = Encoding.Default.GetString(pName, i * 128, 128);
            }

            Win32.AvzCloseDevice(Convert.ToInt16(encode));
            Win32.AvzCloseLED(Convert.ToInt16(encode));
            MessageBox.Show("ERROR - Desconectado");
        }
        
        private WebRequest request;
        private Stream dataStream;

        private string status;

        public String Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        public byte[] FileToByteArray(string fileName)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(fileName,
                                       FileMode.Open,
                                       FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }

        public void MyWebRequest(string url, string data, string key)
        {   
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            request.Headers["channel_key"] = key;
            dynamic msg = new ExpandoObject();
            
            msg.digital = data.ToString();
            msg.usuario = textBox3.Text;
            msg.cliente = textBox1.Text;
            msg.equipamento = textBox2.Text;
            string mensagem = new JavaScriptSerializer().Serialize(msg);
                
            string channel = string.Format("portaria_equipamento_biometria_{0}_{1}_{2}", textBox1.Text, textBox2.Text, comboBox1.Text);
            string postData = string.Format("channelName={0}&msg={1}", channel, mensagem);
            Console.Write(url);

            byte[] _byteVersion = Encoding.ASCII.GetBytes(postData);

            request.ContentLength = _byteVersion.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                Console.Write(response);
                Console.WriteLine(reader.ReadToEnd());
                Console.WriteLine(request);
                Console.WriteLine(stream);
                Console.WriteLine(_byteVersion);
                foreach(var i in _byteVersion)
                {
                    Console.Write(i + " ");
                }
                Console.WriteLine(_byteVersion.Length);
            }
        }


        public void MyWebRequest(string url, string method, string data, string key)
            
        {
            request = WebRequest.Create(url);

            // Create POST data and convert it to a byte array.
            string postData = data;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();

            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);

            // Close the Stream object.
            dataStream.Close();

        }

        public string GetResponse()
        {
            // Get the original response.
            WebResponse response = request.GetResponse();

            this.Status = ((HttpWebResponse)response).StatusDescription;

            // Get the stream containing all content returned by the requested server.
            dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);

            // Read the content fully up to the end.
            string responseFromServer = reader.ReadToEnd();

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Enviar Digital");
            
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Feature files|*.anv";
            DialogResult result;

            // Displays the MessageBox.

            result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                byte[] bbyt = FileToByteArray(dlg.FileName);

                string buffer = "";
                foreach (var u in bbyt) buffer += Convert.ToString(u, 16) + ",";

                Console.Write(buffer);
                string url;
                string key;
                switch (this.comboBox1.Text)
                {
                    case "development":
                        url = this.textBox4.Text;
                        key = this.textBox5.Text;
                        MyWebRequest(url, buffer, key);
                        break;
                    case "staging":
                        url = "http://channelio-staging.herokuapp.com/talk_in_channel";
                        key = "1z2x3c4v5b6n7m";
                        MyWebRequest(url, buffer, key);
                        break;
                    case "production":
                        url = "http://channelio.herokuapp.com/talk_in_channel";
                        key = "1z2x3c4v5b6n7m";
                        MyWebRequest(url, buffer, key);
                        break;
                }
            }
        }

        private void init(object sender, EventArgs e)
        {
            //char[,] pName = new char[8, 128];
            byte[] pName = new byte[8 * 128];
            int i, iNum;

            //comboBox1.Items.Clear();
            iNum = Win32.AvzFindDevice(pName);

            for (i = 0; i < iNum; i++)
            {
                encode = Encoding.Default.GetString(pName, i * 128, 128);
            }

            //if (iNum > 0) comboBox1.SelectedIndex = 0;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            switch (this.comboBox1.Text)
            {
                case "development":
                    this.label6.Visible = true;
                    this.textBox5.Visible = true;
                    this.label4.Visible = true;
                    this.textBox4.Visible = true;
                    this.button1.Location = new System.Drawing.Point(25, 231);
                    this.button2.Location = new System.Drawing.Point(25, 260);
                    this.button3.Location = new System.Drawing.Point(25, 289);
                    break;
                case "staging":
                    this.label6.Visible = false;
                    this.textBox5.Visible = false;
                    this.label4.Visible = false;
                    this.textBox4.Visible = false;
                    this.button1.Location = new System.Drawing.Point(25, 143);
                    this.button2.Location = new System.Drawing.Point(25, 180);
                    this.button3.Location = new System.Drawing.Point(26, 222);
                    break;
                case "production":
                    this.label6.Visible = false;
                    this.textBox5.Visible = false;
                    this.label4.Visible = false;
                    this.textBox4.Visible = false;
                    this.button1.Location = new System.Drawing.Point(25, 143);
                    this.button2.Location = new System.Drawing.Point(25, 180);
                    this.button3.Location = new System.Drawing.Point(26, 222);
                    break;
            }
        }
    }
}
