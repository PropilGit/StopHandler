using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using StopHandler.ViewModels.Base;
using StopHandler.Models.POST;
using StopHandler.Models;

namespace StopHandler.ViewModels
{
    class MainWindowViewModel : ViewModel
    {
        Controller controller;
        public string Log { get => controller.Log; }

        public MainWindowViewModel()
        {
            controller = new Controller();
        }
    }
}


//public string Log { get => controller.Log; set => Set(ref controller.Log, value); }