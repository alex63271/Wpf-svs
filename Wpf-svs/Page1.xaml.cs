using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Text.Json;


namespace Wpf_svs
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    ///   
    public class Class1
    {
        public object Message { get; set; }
    public bool Result { get; set; }
    public string SignerCertificate { get; set; }
    public Signercertificateinfo SignerCertificateInfo { get; set; }
    public Signatureinfo SignatureInfo { get; set; }
    public object AdditionalCertificateResult { get; set; }
}

public class Signercertificateinfo
{
    public string SubjectName { get; set; }
    public string IssuerName { get; set; }
    public DateTime NotBefore { get; set; }
    public DateTime NotAfter { get; set; }
    public string SerialNumber { get; set; }
    public string Thumbprint { get; set; }
}

public class Signatureinfo
{
    public string CAdESType { get; set; }
}

public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }

        private string IOFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog(); 
               
                file.Text = openFileDialog.FileName;
          
                using (FileStream fstream = File.OpenRead(openFileDialog.FileName))
                    { 
                        // преобразуем документ в байты
                        byte[] document = new byte[fstream.Length];
                        // считываем данные
                        fstream.Read(document, 0, document.Length);

                string documentstr = Convert.ToBase64String(document);
                //Console.WriteLine(c);
                return documentstr;
            }

            }


       

        private string IOSign(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

            sig.Text = openFileDialog.FileName;

            using (FileStream fstream1 = File.OpenRead(openFileDialog.FileName))
            {  // преобразуем строку в байты
                byte[] signature = new byte[fstream1.Length];
                // считываем данные
                fstream1.Read(signature, 0, signature.Length);

                string signaturestr = Convert.ToBase64String(signature);
                //Console.WriteLine(c);
                return signaturestr;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e, string signaturestr, string documentstr)
        {
            NavigationService.Navigate(new Page2());



            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://10.251.85.74/SVS/rest/api/signatures");
            request.Method = "POST";
            request.ContentType = "application/json";
            string data = "{\"SignatureType\": \"2\"," +
                "    \"Content\": \""+ documentstr + "\"," +
                "    \"Source\": \""+ signaturestr + "\"}";

            byte[] Array = Encoding.UTF8.GetBytes(data);

            request.ContentLength = Array.Length;


            using (Stream writer = request.GetRequestStream())
            {

                writer.Write(Array, 0, Array.Length);

            }
            Console.WriteLine("Запрос создан");



            using (WebResponse response = request.GetResponse())
            {


                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {

                    string q = reader.ReadToEnd();
                    Console.WriteLine(q);

                    Console.WriteLine("-----------------------");
                    Class1[] otvet = JsonSerializer.Deserialize<Class1[]>(q);

                    Console.WriteLine(otvet[0].SignerCertificateInfo.SubjectName);
                    Console.WriteLine("-----------------------");
                    Console.WriteLine(otvet[0].SignerCertificateInfo.Thumbprint);
                }




            }

           
        }

    }
    
}

