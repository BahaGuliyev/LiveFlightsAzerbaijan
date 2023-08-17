using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LiveFlightsAzerbaijan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static RestClient client = new RestClient("https://airlabs.co/api/v9/flights?api_key=201271ad-b825-418a-86b4-9ff192dacadc&bbox=38.3929551,44.7633701,41.9502947,51.0090302");

        static RestRequest request = new RestRequest();

        static RestResponse response = client.ExecuteGet(request);

        static String text = response.Content;
        //richTextBox1.Text = text;

        static JObject json = JObject.Parse(text);
       static String text1 = json.SelectToken("response").ToString();
        //richTextBox1.Text = text1;

        DataTable dataTable = (DataTable)JsonConvert.DeserializeObject(text1, (typeof(DataTable)));
        private void Form1_Load(object sender, EventArgs e)
        {
           
            DataView dv = new DataView(dataTable);
            dataGridView1.DataSource = dv;

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted_1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            String lat = dataGridView1.CurrentRow.Cells["lat"].Value.ToString();
            String lon = dataGridView1.CurrentRow.Cells["lng"].Value.ToString();
            String newLat=SexagesimalAngle.FromDouble(Double.Parse(lat)).ToString("NS");
            String newLon = SexagesimalAngle.FromDouble(Double.Parse(lon)).ToString("WE");
            String newSearchStr= "https://www.google.com/maps/place/"+newLat+"+"+newLon+"/"+ "@40.254249,46.5440789,7.39z/";
            webBrowser1.Navigate(newSearchStr);

            //Flag Picture
            pictureBox1.ImageLocation= "http://www.geognos.com/api/en/countries/flag/"+dataGridView1.CurrentRow.Cells["flag"].Value.ToString()+".png";

            //Plane Picture
            WebClient websehife = new WebClient();
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            websehife.Encoding = Encoding.UTF8;
            string source = websehife.DownloadString("https://www.jetphotos.com/photo/keyword/" + dataGridView1.CurrentRow.Cells["reg_number"].Value.ToString());
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load("https://www.jetphotos.com/photo/keyword/" + dataGridView1.CurrentRow.Cells["reg_number"].Value.ToString());

            HtmlNode node = doc.DocumentNode.SelectSingleNode("//*[@id=\"results\"]/div[1]/div[1]/a/img");
            if (node != null)
            {
                String PictureOfPlane = node.GetAttributeValue("src", "");
                pictureBox2.ImageLocation = "https:"+PictureOfPlane;
            }

            //Heading-Flight IATA/ICAO
            label2.Text = dataGridView1.CurrentRow.Cells["flight_iata"].Value.ToString();
            label3.Text = "/" + dataGridView1.CurrentRow.Cells["flight_icao"].Value.ToString();

            //DEP-ARR
            label4.Text = dataGridView1.CurrentRow.Cells["dep_iata"].Value.ToString();
            label5.Text = dataGridView1.CurrentRow.Cells["arr_iata"].Value.ToString();

            //More Info
            label9.Text = dataGridView1.CurrentRow.Cells["reg_number"].Value.ToString();
            label13.Text = dataGridView1.CurrentRow.Cells["squawk"].Value.ToString();
            label12.Text = dataGridView1.CurrentRow.Cells["speed"].Value.ToString();

            //Airplane icon
            if (dataGridView1.CurrentRow.Cells["status"].Value.ToString() == "en-route")
            {
                pictureBox3.Image= Properties.Resources.Plane_icon;
            }
            else if(dataGridView1.CurrentRow.Cells["status"].Value.ToString() == "landed")
            {
                pictureBox3.Image = Properties.Resources.Plane_on_ground;
            }

        }
        public class SexagesimalAngle
        {
            public bool IsNegative { get; set; }
            public int Degrees { get; set; }
            public int Minutes { get; set; }
            public int Seconds { get; set; }
            public int Milliseconds { get; set; }



            public static SexagesimalAngle FromDouble(double angleInDegrees)
            {
                //ensure the value will fall within the primary range [-180.0..+180.0]
                while (angleInDegrees < -180.0)
                    angleInDegrees += 360.0;

                while (angleInDegrees > 180.0)
                    angleInDegrees -= 360.0;

                var result = new SexagesimalAngle();

                //switch the value to positive
                result.IsNegative = angleInDegrees < 0;
                angleInDegrees = Math.Abs(angleInDegrees);

                //gets the degree
                result.Degrees = (int)Math.Floor(angleInDegrees);
                var delta = angleInDegrees - result.Degrees;

                //gets minutes and seconds
                var seconds = (int)Math.Floor(3600.0 * delta);
                result.Seconds = seconds % 60;
                result.Minutes = (int)Math.Floor(seconds / 60.0);
                delta = delta * 3600.0 - seconds;

                //gets fractions
                result.Milliseconds = (int)(1000.0 * delta);

                return result;
            }



            //public override string ToString()
            //{
            //    var degrees = this.IsNegative
            //        ? -this.Degrees
            //        : this.Degrees;

            //    return string.Format(
            //        "{0}° {1:00}' {2:00}\"",
            //        degrees,
            //        this.Minutes,
            //        this.Seconds);
            //}



            public string ToString(string format)
            {
                switch (format)
                {
                    case "NS":
                        return string.Format(
                            "{0}°{1:00}'{2:00}.{3:000}\"{4}",
                            this.Degrees,
                            this.Minutes,
                            this.Seconds,
                            this.Milliseconds,
                            this.IsNegative ? 'S' : 'N');

                    case "WE":
                        return string.Format(
                            "{0}°{1:00}'{2:00}.{3:000}\"{4}",
                            this.Degrees,
                            this.Minutes,
                            this.Seconds,
                            this.Milliseconds,
                            this.IsNegative ? 'W' : 'E');

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.flightradar24.com/data/flights/" + dataGridView1.CurrentRow.Cells["flight_iata"].Value.ToString());
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(dataTable);
            dv.RowFilter = "reg_number like '"+textBox2.Text+"*'";
            dataGridView1.DataSource = dv;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(dataTable);
            dv.RowFilter = "flight_icao like '" + textBox1.Text + "*'";
            dataGridView1.DataSource = dv;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(dataTable);
            dv.RowFilter = "dep_iata like '" + textBox3.Text + "*'";
            dataGridView1.DataSource = dv;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(dataTable);
            dv.RowFilter = "arr_iata like '" + textBox4.Text + "*'";
            dataGridView1.DataSource = dv;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(dataTable);
            dv.RowFilter = "squawk like '" + textBox6.Text + "*'";
            dataGridView1.DataSource = dv;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            DataView dv = new DataView(dataTable);
            dv.RowFilter = "flight_iata like '" + textBox5.Text + "*'";
            dataGridView1.DataSource = dv;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RestClient client = new RestClient("https://airlabs.co/api/v9/flights?api_key=201271ad-b825-418a-86b4-9ff192dacadc&bbox=38.3929551,44.7633701,41.9502947,51.0090302");

            RestRequest request = new RestRequest();

            RestResponse response = client.ExecuteGet(request);

            String text = response.Content;
            //richTextBox1.Text = text;

            JObject json = JObject.Parse(text);
            String text1 = json.SelectToken("response").ToString();
            //richTextBox1.Text = text1;

            DataTable dataTable = (DataTable)JsonConvert.DeserializeObject(text1, (typeof(DataTable)));

            DataView dv = new DataView(dataTable);
            dataGridView1.DataSource = dv;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
