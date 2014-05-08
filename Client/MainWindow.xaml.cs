using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sem3.Contracts;
using System.ServiceModel;

namespace Sem3.Client
{

	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, ICallbackService
	{
		private IService _proxy;

		private void _buttonClick(object sender, RoutedEventArgs e)
		{
			_answerBlock.Text = "";
			string value = _textBox.Text;
			long valueLong = 0;
			if (!long.TryParse(value, out valueLong))
			{
				_answerBlock.Text = "Not a number";
				return;
			}
			try
			{
				_proxy.PrimalityRequest(valueLong, (bool)vipCheckBox.IsChecked);
				_answerBlock.Text = "Waiting for answer...";
				_button.IsEnabled = false;
			}
			catch (EndpointNotFoundException)
			{
				_answerBlock.Text = "Couldn't connect to server";
				_button.IsEnabled = true;
				return;
			}
			
		}

		public void ReturnPrimality(long answer)
		{
			_answerBlock.Text = answer == 0 ? "Number is not prime" : 
					(answer == 1 ? "Number is prime" : "Number is divided by " + answer);
			_button.IsEnabled = true;
		}

		public MainWindow()
		{
			_proxy = DuplexChannelFactory<IService>.CreateChannel(this, new NetTcpBinding(), 
					new EndpointAddress("net.tcp://localhost:9000/MyEndpoint"));
			InitializeComponent();
		}
	}

}
