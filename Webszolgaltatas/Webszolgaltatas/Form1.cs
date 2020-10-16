using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using Webszolgaltatas.Entities;
using Webszolgaltatas.MnbServiceReference;

namespace Webszolgaltatas
{
    public partial class Form1 : Form
    {
        BindingList<Entities.RateData> Rates = new BindingList<Entities.RateData>();
        BindingList<string> Currencies = new BindingList<string>();
        public string pst;
        public Form1()
        {
            InitializeComponent();
          
            var mnbService = new MNBArfolyamServiceSoapClient();

            var request = new GetCurrenciesRequestBody()
            {

            };

            var response = mnbService.GetCurrencies(request);
            var result = response.GetCurrenciesResult;
            var newxml = new XmlDocument();
            newxml.LoadXml(result);
            foreach (XmlElement element in newxml.DocumentElement)
            {
                for (int i = 0; i < element.ChildNodes.Count; i++)
                {
                    var childElement = (XmlElement)element.ChildNodes[i];
                    Currencies.Add(childElement.InnerText);
                }
            }
            RefreshData();
        }

        private void RefreshData()
        {
            Rates.Clear();
            dataGridView1.DataSource = Rates;
            comboBox1.DataSource = Currencies;
            GetExchangeRatesFunction();
            XMLProcess();
            Diagram();
        }

        private void GetExchangeRatesFunction()
        {
            var mnbService = new MNBArfolyamServiceSoapClient();

            var request = new GetExchangeRatesRequestBody()
            {
                currencyNames = comboBox1.SelectedItem.ToString(),
                startDate = dateTimePicker1.Value.ToString(),
                endDate = dateTimePicker2.Value.ToString()
            };

            var response = mnbService.GetExchangeRates(request);

            var result = response.GetExchangeRatesResult;
            pst = result;
        }

        private void XMLProcess()
        {
            var xml = new XmlDocument();
            xml.LoadXml(pst);

            foreach (XmlElement element in xml.DocumentElement)
            {
                var rate = new Entities.RateData();
                Rates.Add(rate);

                rate.Date = DateTime.Parse(element.GetAttribute("date"));

                var childElement = (XmlElement)element.ChildNodes[0];
                if (childElement == null)
                    continue;
                rate.Currency = childElement.GetAttribute("curr");

                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0)
                    rate.Value = value / unit;
            }
        }

        private void Diagram()
        {
            chartRateData.DataSource = Rates;

            var series = chartRateData.Series[0];
            series.ChartType = SeriesChartType.Line;
            series.XValueMember = "Date";
            series.YValueMembers = "Value";
            series.BorderWidth = 2;

            var legend = chartRateData.Legends[0];
            legend.Enabled = false;

            var chartArea = chartRateData.ChartAreas[0];
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisY.IsStartedFromZero = false;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
