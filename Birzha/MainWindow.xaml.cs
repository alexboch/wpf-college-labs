using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Birzha
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        List<Currency> Currencies=new List<Currency>();
        SelectedDatesCollection selectedDates;
        string url = "http://www.cbr.ru/scripts/XML_dynamic.asp?";
        ObservableCollection<ChartPoint> Data = new ObservableCollection<ChartPoint>();
        public MainWindow()
        {
            InitializeComponent();
            CultureInfo ci = new CultureInfo(Thread.CurrentThread.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            ci.DateTimeFormat.DateSeparator = "/";
            Thread.CurrentThread.CurrentCulture = ci;
            chart.DataContext = Data;
            LoadCurrenciesList();
        }

        private void LoadCurrenciesList()
        {
            WebClient wcl = new WebClient();
            string uriString="http://www.cbr.ru/scripts/XML_val.asp?d=0";
            wcl.DownloadStringCompleted += DownloadCurrenciesCompleted;
            wcl.DownloadStringAsync(new Uri(uriString));
        }

        private void DownloadCurrenciesCompleted(object sender,DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XDocument xdoc = XDocument.Parse(e.Result);
                IEnumerable<XElement> items = xdoc.Descendants("Valuta");
                foreach (XElement xe in items.Descendants("Item"))
                {
                    string name = xe.Element("Name").Value;
                    string code = xe.Element("ParentCode").Value;
                    Currency c = new Currency(name, code);
                    Currencies.Add(c);
                }
                currenciesListBox.ItemsSource = Currencies;
                currenciesListBox.DisplayMemberPath = "Name";
                currenciesListBox.SelectedValuePath = "ParentCode";
            }
        }

        private void DownloadQuotesCompleted(object sender,DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Data.Clear();
                XDocument xdoc = XDocument.Parse(e.Result, LoadOptions.None);
                PointCollection points = new PointCollection();
                    IEnumerable<XElement> items = xdoc.Descendants("ValCurs");
                    int i = 0;
                    foreach (XElement xe in items.Descendants("Record"))
                    {
                        int nominal =int.Parse(xe.Element("Nominal").Value);
                        double value = double.Parse(xe.Element("Value").Value);
                        Data.Add(new ChartPoint(selectedDates[i].Date,value));
                        i++;
                    }
                }
         }

        private Uri CreateUri(DateTime firstDate,DateTime lastDate,string currencyCode)
        {
            if(firstDate>lastDate)
            {
                DateTime temp = firstDate;
                firstDate = lastDate;
                lastDate = temp;
            }
            string uriString = url + "date_req1=" + firstDate.ToShortDateString() + "&date_req2=" 
                + lastDate.ToShortDateString()+"&VAL_NM_RQ="+currencyCode;
            return new Uri(uriString);
        }

        private void ShowCourse()
        {
           selectedDates = calendar.SelectedDates;
            if (selectedDates != null&&selectedDates.Count>0)
            {
                if (currenciesListBox.SelectedValue != null)
                {
                    string currencyCode = (string)currenciesListBox.SelectedValue;
                    DateTime firstDate = selectedDates.First();
                    DateTime lastDate = selectedDates.Last();
                    WebClient wc = new WebClient();
                    wc.DownloadStringCompleted += DownloadQuotesCompleted;
                    Uri uri = CreateUri(firstDate, lastDate, currencyCode);
                    wc.DownloadStringAsync(uri);
                }
            }
        }

        private void Calendar_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowCourse();
        }

        private void currenciesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowCourse();
        }

    }
}
