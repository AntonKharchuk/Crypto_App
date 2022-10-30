using Cripto_App.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
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

namespace Crypto_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;

        MainWindowModel mvm;

        public MainWindow()
        {

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(@"https://cryptingup.com/api/");

            Uri iconUri = new Uri(findMyDir(Environment.CurrentDirectory) + "icon.ico", UriKind.Absolute);
            this.Icon = BitmapFrame.Create(iconUri);

            DirectoryInfo DirToSave = new DirectoryInfo(findMyDir(Environment.CurrentDirectory) + @"Themes");

            FileInfo FileWithThemes = new FileInfo(DirToSave + "\\CurrentTheme.txt");

            if (!FileWithThemes.Exists)
            {
                FileWithThemes.Create().Close();
                using (StreamWriter sw = new StreamWriter(FileWithThemes.FullName))
                {
                    sw.WriteLine("1");
                }
            }

            int ThemeNum;

            using (StreamReader sr = File.OpenText(FileWithThemes.FullName))
            {
                ThemeNum = int.Parse(sr.ReadLine());
            }

            Uri uri = null;

            switch (ThemeNum)
            {
                case 1:

                    uri = new Uri("Themes\\Pink.xaml", UriKind.Relative);

                    break;

                case 2:
                    uri = new Uri("Themes\\Gray.xaml", UriKind.Relative);

                    break;
                case 3:
                    uri = new Uri("Themes\\BlackOrange.xaml", UriKind.Relative);

                    break;
                case 4:
                    uri = new Uri("Themes\\Black.xaml", UriKind.Relative);

                    break;
                case 5:
                    uri = new Uri("Themes\\White.xaml", UriKind.Relative);

                    break;
            }

            if (uri != null)
            {
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                // очищаем коллекцию ресурсов приложения
                Application.Current.Resources.Clear();
                // добавляем загруженный словарь ресурсов
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            }



            InitializeComponent();

            mvm = new MainWindowModel();

            this.DataContext = mvm;

            TextBoxSearch.TextChanged += (s, e) => {
                TextBox textToSearch = (TextBox)s;
                string text = textToSearch.Text;
                text = text.ToUpper();
                List<string> ResLisr = new List<string> { };
                foreach (var item in mvm.StrAssetsList)
                {
                    if (item.Contains(text))
                    {
                        ResLisr.Add(item);
                    }
                }
                foreach (var item in ResLisr)
                {
                    mvm.StrAssetsList.Remove(item);
                }


                mvm.StrAssetsList.InsertRange(0, ResLisr);

                ListsAssets.Items.Refresh();

            };
        }

        private void ButtonOxyStart_Click(object sender, RoutedEventArgs e)
        {
            if (ListsAssets.SelectedItem != null)
            {
                if (RadioHour.IsChecked == true)
                    SearchForAssetData(ListsAssets.SelectedItem.ToString(), "hour");
                else if (RadioDay.IsChecked == true)
                    SearchForAssetData(ListsAssets.SelectedItem.ToString(), "day");
                else if (RadioWeek.IsChecked == true)
                    SearchForAssetData(ListsAssets.SelectedItem.ToString(), "week");
                else
                    SearchForAssetData(ListsAssets.SelectedItem.ToString(), "month");
            }

        }

        private async void SearchForAssetData(string assetId, string timing)
        {
            bool IsShown = true;

            DirectoryInfo DirToSave = new DirectoryInfo(findMyDir(Environment.CurrentDirectory) + @"Resources\" + assetId);

            if (!DirToSave.Exists)
            {
                DirToSave.Create();
                FileInfo FileToSave = new FileInfo(DirToSave.FullName + $"\\WindowParams.txt");
                FileToSave.Create().Close();
                using (StreamWriter sw = new StreamWriter(DirToSave.FullName + $"\\WindowParams.txt"))
                {
                    sw.WriteLine(100);
                    sw.WriteLine(100);
                    sw.WriteLine(800);
                    sw.WriteLine(450);

                }

            }
            AssetInfoWindow dayAssetInfoWindow = new AssetInfoWindow();
            dayAssetInfoWindow.Title = assetId;



            do
            {


                FileInfo FileToSave = new FileInfo(DirToSave.FullName + $"\\{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}.txt");

                Dictionary<DateTime, decimal> DateValuePair;

                if (!FileToSave.Exists)
                {
                    FileToSave.Create().Close();

                    DateValuePair = new Dictionary<DateTime, decimal> { };
                }
                else
                {
                    using (StreamReader sr = File.OpenText(FileToSave.FullName))
                    {
                        string dat = sr.ReadLine();
                        if (dat != null)
                            DateValuePair = JsonConvert.DeserializeObject<Dictionary<DateTime, decimal>>(dat);
                        else
                            DateValuePair = new Dictionary<DateTime, decimal> { };

                    }
                }



                try
                {

                    var response = await _httpClient.GetAsync($"assets/{assetId}");

                    string content = await response.Content.ReadAsStringAsync();

                    JObject assetsSearch = JObject.Parse(content);

                    JToken result = assetsSearch["asset"];

                    AssetData searchResult = result.ToObject<AssetData>();

                    string pr = searchResult.price.Replace('.', ',');

                    bool valueIsThere = false;

                    foreach (var item in DateValuePair)
                    {
                        if (item.Key.Minute == DateTime.Now.Minute)
                        {
                            valueIsThere = true;
                        }
                    }
                    if (!valueIsThere)
                    {
                        DateValuePair.Add(DateTime.Now, decimal.Parse(pr));
                    }

                }
                catch (Exception)
                {
                    DateValuePair.Add(DateTime.Now, -1);
                }

                using (StreamWriter sw = new StreamWriter(FileToSave.FullName))
                {
                    sw.Write(JsonConvert.SerializeObject(DateValuePair));
                }
                dayAssetInfoWindow.Close();

                //---
                dayAssetInfoWindow = new AssetInfoWindow(DirToSave.FullName, timing);
                dayAssetInfoWindow.Title = assetId;



                dayAssetInfoWindow.Closed += (e, s) => {
                    IsShown = false;
                    using (StreamWriter sw = new StreamWriter(DirToSave.FullName + $"\\WindowParams.txt"))
                    {
                        sw.WriteLine(dayAssetInfoWindow.Top);
                        sw.WriteLine(dayAssetInfoWindow.Left);
                        sw.WriteLine(dayAssetInfoWindow.Width);
                        sw.WriteLine(dayAssetInfoWindow.Height);
                    }

                };

                //---

                using (StreamReader sr = File.OpenText(DirToSave.FullName + $"\\WindowParams.txt"))
                {
                    dayAssetInfoWindow.Top = double.Parse(sr.ReadLine());
                    dayAssetInfoWindow.Left = double.Parse(sr.ReadLine());
                    dayAssetInfoWindow.Width = double.Parse(sr.ReadLine());
                    dayAssetInfoWindow.Height = double.Parse(sr.ReadLine());

                }

                dayAssetInfoWindow.Show();
                IsShown = true;

                await Task.Delay(59900);


            } while (IsShown);





        }
        string findMyDir(string path)
        {
            if (path.EndsWith("Crypto_App\\"))
            {
                return path;
            }
            else
                return findMyDir(path.Remove(path.Length - 1));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mvm.StrAssetsList.RemoveAt(70);
        }

        private void ButtonThemeChange_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo DirToSave = new DirectoryInfo(findMyDir(Environment.CurrentDirectory) + @"Themes");

            FileInfo FileWithThemes = new FileInfo(DirToSave + "\\CurrentTheme.txt");

            if (!FileWithThemes.Exists)
            {
                FileWithThemes.Create().Close();
                using (StreamWriter sw = new StreamWriter(FileWithThemes.FullName))
                {
                    sw.WriteLine("1");
                }
            }

            int ThemeNum;

            using (StreamReader sr = File.OpenText(FileWithThemes.FullName))
            {
                ThemeNum = int.Parse(sr.ReadLine());
            }

            Uri uri = null;

            switch (ThemeNum)
            {
                case 1:

                    uri = new Uri("Themes\\Gray.xaml", UriKind.Relative);

                    using (StreamWriter sw = new StreamWriter(FileWithThemes.FullName))
                    {
                        sw.WriteLine("2");
                    }
                    break;

                case 2:
                    uri = new Uri("Themes\\BlackOrange.xaml", UriKind.Relative);

                    using (StreamWriter sw = new StreamWriter(FileWithThemes.FullName))
                    {
                        sw.WriteLine("3");
                    }
                    break;
                case 3:
                    uri = new Uri("Themes\\Black.xaml", UriKind.Relative);

                    using (StreamWriter sw = new StreamWriter(FileWithThemes.FullName))
                    {
                        sw.WriteLine("4");
                    }
                    break;
                case 4:
                    uri = new Uri("Themes\\White.xaml", UriKind.Relative);

                    using (StreamWriter sw = new StreamWriter(FileWithThemes.FullName))
                    {
                        sw.WriteLine("5");
                    }
                    break;
                case 5:
                    uri = new Uri("Themes\\Pink.xaml", UriKind.Relative);

                    using (StreamWriter sw = new StreamWriter(FileWithThemes.FullName))
                    {
                        sw.WriteLine("1");
                    }
                    break;
            }

            if (uri != null)
            {
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                // очищаем коллекцию ресурсов приложения
                Application.Current.Resources.Clear();
                // добавляем загруженный словарь ресурсов
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            }

        }
    }
}
