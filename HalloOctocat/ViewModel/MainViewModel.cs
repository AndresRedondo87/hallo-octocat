using HalloOctocat.ViewModel.MvvmFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HalloOctocat.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {

        }

        /// <summary>
        /// Starts showing different Octocats from Octodex. Intervall is 3 seconds.
        /// </summary>
        public ICommand StartOctoShow
        {
            get { throw new NotImplementedException(); }
        }

        public string ImagePathToDisplay
        {
            get { return @"pack://application:,,,/HalloOctocat;component/Images/baracktocat.jpg"; }
        }
    }
}
