﻿using Microsoft.Win32;
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
using ViewModel;

namespace View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
		}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog fileDialog = new OpenFileDialog();
        //    fileDialog.Filter = "Exe file (*.exe)|*.exe";
        //    if (fileDialog.ShowDialog() == true)
        //    {
        //        lll.Text = fileDialog.InitialDirectory + fileDialog.FileName;
        //    }
        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    Close();
        //}
    }
}
