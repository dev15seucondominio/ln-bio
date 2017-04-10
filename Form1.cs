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
namespace WindowsFormsApp1
{
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
                    using (BinaryWriter binWriter = new BinaryWriter(File.Open("MyFile.anv", FileMode.Create)))
                    {
                        byte[] gpFeatureBuf1 = new byte[338];
                        byte[] gpFeatureA = new byte[169];
                        byte[] gpFeatureB = new byte[169];
                        gpFeatureA = SliceMe(Win32.gpFeatureA, 169);
                        gpFeatureB = SliceMe(Win32.gpFeatureB, 169);
                        gpFeatureBuf1 = Combine(gpFeatureA, gpFeatureB);
                        binWriter.Write(gpFeatureBuf1);    
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

        public void PrintByteArray(byte[] bytes)
        {
            var sb = new StringBuilder("new byte[] { ");
            foreach (var b in bytes)
            {
                sb.Append(b + ", ");
            }
            sb.Append("}");
            Console.WriteLine(sb.ToString());
        }

        public static byte[] UnsignedBytesFromSignedBytes(byte[] signed)
        {
            var unsigned = new byte[signed.Length];
            Buffer.BlockCopy(signed, 0, unsigned, 0, signed.Length);
            return unsigned;
        }

        static byte[] SliceMe(byte[] source, int length)
        {
            byte[] destfoo = new byte[length];
            Array.Copy(source, 0, destfoo, 0, length);
            return destfoo;
        }

        public static T[] Combine<T>(T[] first, T[] second)
        {
            T[] ret = new T[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
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

        public void MyWebRequest(string url)
        {
            // Create a request using a URL that can receive a post.

            request = WebRequest.Create(url);
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

        public void MyWebRequest(string url, string data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers["cliente"] = textBox1.Text;
            request.Headers["equipamento"] = textBox2.Text;
            request.Headers["usuario"] = textBox3.Text;
            request.Headers["digital"] = data;


            byte[] _byteVersion = Encoding.ASCII.GetBytes(string.Concat("equipamento=", data));

            request.ContentLength = _byteVersion.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
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


        public void MyWebRequest(string url, string method, string data)

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

            UInt16 Ret = 0;
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
                MyWebRequest("http://seucondominio.ddns.net:3001/guaritas/buffer_digital", buffer);
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
    }
}
