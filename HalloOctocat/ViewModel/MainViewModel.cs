using HalloOctocat.ViewModel.MvvmFoundation;
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

        private string imagePathOnServer;

        public MainViewModel()
        {
            LoadImageFromServer();
        }

        /// <summary>
        /// Starts showing different Octocats from Octodex. Intervall is 3 seconds.
        /// </summary>
        public ICommand StartOctoShow
        {
            get { throw new NotImplementedException(); }
        }

        public BitmapImage ImageToDisplay
        {
            get
            {
                return new BitmapImage(new Uri(@"https://octodex.github.com/images/baracktocat.jpg"));
            }
        }

        private async void LoadImageFromServer()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(@"https://octodex.github.com/");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
