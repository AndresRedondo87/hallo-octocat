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
        private Dictionary<string, string> octocatUrls = new Dictionary<string, string>();
        private Dictionary<string, BitmapImage> octocatImages = new Dictionary<string, BitmapImage>();

        private readonly string imageToDisplayUrl = @"https://octodex.github.com/images/baracktocat.jpg";
        private readonly string octodexBaseUrl = @"https://octodex.github.com/";

        public MainViewModel()
        {
            FoundOctocats = 0;
            IsOctoShowRunning = false;
            OctocatNameToDisplay = "baracktocat";
            OctocatToDisplay = new BitmapImage(new Uri(imageToDisplayUrl));
            DetectAllOctocatsInOctodex();
        }

        private RelayCommand startOctoShowCommand;
        private BitmapImage octocatToDisplay;
        private string octocatNameToDisplay;
        private int foundOctocats;
        private bool isOctoShowRunning;

        /// <summary>
        /// Starts showing different Octocats from Octodex. Intervall is 3 seconds.
        /// </summary>
        public ICommand StartOctoShow
        {
            get
            {
                return startOctoShowCommand ?? (startOctoShowCommand = new RelayCommand(() => CyclicalyShowRandomOctocat(), () => !this.IsOctoShowRunning));
            }
        }

        /// <summary>
        /// The currently shown Octocat.
        /// </summary>
        public BitmapImage OctocatToDisplay
        {
            get
            {
                return octocatToDisplay;
            }
            private set
            {
                if (octocatToDisplay != value)
                {
                    octocatToDisplay = value;
                    RaisePropertyChanged("OctocatToDisplay");
                }
            }
        }

        /// <summary>
        /// Provides the Name of the currently shown Octocat
        /// </summary>
        public string OctocatNameToDisplay
        {
            get { return octocatNameToDisplay; }
            private set
            {
                if (octocatNameToDisplay != value)
                {
                    octocatNameToDisplay = value;
                    RaisePropertyChanged("OctocatNameToDisplay");
                }
            }
        }

        /// <summary>
        /// Indicates how many Octocats were found in the Octodex
        /// </summary>
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

        /// <summary>
        /// Indicates whether cyclically a new Octocat from Octodex is shown.
        /// </summary>
        public bool IsOctoShowRunning
        {
            get { return isOctoShowRunning; }
            private set
            {
                if (isOctoShowRunning != value)
                {
                    isOctoShowRunning = value;
                    RaisePropertyChanged("IsOctoShowRunning");
                }
            }
        }

        private async Task CyclicalyShowRandomOctocat()
        {
            IsOctoShowRunning = true;

            while (IsOctoShowRunning)
            {
                KeyValuePair<string, BitmapImage> octocat = RandomlySelectOctocats();
                OctocatToDisplay = octocat.Value;
                OctocatNameToDisplay = octocat.Key;
                await Task.Delay(3000); // <- await with cancellation
            }
        }

        private KeyValuePair<string, BitmapImage> RandomlySelectOctocats()
        {
            KeyValuePair<string, BitmapImage> result;
            Random rand = new Random();
            KeyValuePair<string, string> randomOctocatNameUrl = octocatUrls.ElementAt(rand.Next(0, octocatUrls.Count));

            if (octocatImages.ContainsKey(randomOctocatNameUrl.Key))
            {
                result = new KeyValuePair<string, BitmapImage>(randomOctocatNameUrl.Key, octocatImages[randomOctocatNameUrl.Key]);
            }
            else
            {
                result = new KeyValuePair<string, BitmapImage>(randomOctocatNameUrl.Key, new BitmapImage(new Uri(randomOctocatNameUrl.Value)));
                octocatImages.Add(result.Key, result.Value);
            }

            return result;
        }

        private async void DetectAllOctocatsInOctodex()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(octodexBaseUrl);

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
                        octocatUrls.Add(name.Attributes["name"].Value, octodexBaseUrl + relativeUrl.Attributes["data-src"].Value);
                        FoundOctocats = octocatUrls.Count;
                    }
                }
            }
        }
    }
}
