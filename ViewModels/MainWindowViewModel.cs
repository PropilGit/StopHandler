using System;
using System.Collections.Generic;
using System.Text;
using StopHandler.ViewModels.Base;

namespace StopHandler.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        #region Port
        private string _Port = "48654";

        public string Port { get => _Port; set => Set(ref _Port, value); }
        #endregion
    }
}
