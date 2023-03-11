using Cripto_App.Models;
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
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Crypto_App
{

    public class MainWindowModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;

        private List<string> _strAssetsList;

        public List<string> StrAssetsList
        {
            get { return _strAssetsList; }
            set
            {
                _strAssetsList = value;
                OnPropertyChanged("StrAssetsList");
            }
        }

        private TextToSearchClass _textToSearch;

        public TextToSearchClass TextToSearch
        {
            get { return _textToSearch; }
            set
            {
                _textToSearch = value;
                OnPropertyChanged("TextToSearch");
            }
        }



        public MainWindowModel()
        {
            _strAssetsList = new List<string> { };

            try
            {
                _httpClient = new HttpClient();
                _httpClient.BaseAddress = new Uri(@"https://cryptingup.com/api/");


                var response = _httpClient.GetAsync("assets").Result;

                string content = response.Content.ReadAsStringAsync().Result;

                JObject assetsSearch = JObject.Parse(content);

                // get JSON result objects into a list
                List<JToken> results = assetsSearch["assets"].Children().ToList();

                // serialize JSON results into .NET objects
                List<AssetData> assets = new List<AssetData>();
                foreach (JToken result in results)
                {
                    // JToken.ToObject is a helper method that uses JsonSerializer internally
                    AssetData searchResult = result.ToObject<AssetData>();
                    assets.Add(searchResult);
                }
                foreach (var asset in assets)
                {
                    _strAssetsList.Add(asset.asset_id);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("No assets found");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
