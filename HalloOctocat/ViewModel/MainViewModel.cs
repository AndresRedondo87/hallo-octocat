using HalloOctocat.ViewModel.MvvmFoundation;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HalloOctocat.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private Dictionary<string, string> octocats = new Dictionary<string, string>();
        private string imageToDisplayUrl = @"https://octodex.github.com/images/baracktocat.jpg";

        public MainViewModel()
        {
            FoundOctocats = 0;
        }

        /// <summary>
        /// Starts showing different Octocats from Octodex. Intervall is 3 seconds.
        /// </summary>
        public ICommand StartOctoShow
        {
            get
            {
                return startOctoShowCommand ?? (startOctoShowCommand = new RelayCommand(() => DetectAllOctocatsInOctodex()));
            }
        }
        private RelayCommand startOctoShowCommand;

        public BitmapImage ImageToDisplay
        {
            get
            {
                return new BitmapImage(new Uri(imageToDisplayUrl));
            }
        }

        private int foundOctocats;
        public int FoundOctocats
        {
            get { return foundOctocats; }
            private set
            {
                if (foundOctocats != value)
                {
                    foundOctocats = value;
                    RaisePropertyChanged("FoundOctocats");
                }
            }
        }

        private async void DetectAllOctocatsInOctodex()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(@"https://octodex.github.com/");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var html = new HtmlDocument();
                    html.LoadHtml(content);
                    var root = html.DocumentNode;
                    var images = root.Descendants().Where(n => n.GetAttributeValue("class", "").Equals("item-shell"));
                    foreach (var octocat in images)
                    {
                        var name = octocat.Descendants("a").First();
                        var relativeUrl = octocat.Descendants("img").First();
                        octocats.Add(name.Attributes["name"].Value, relativeUrl.Attributes["data-src"].Value);
                        FoundOctocats = octocats.Count;
                    }
                }
            }
        }
    }
}
