using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FlatPackCommandCreator
{
	public class MainWindowPresenter : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, args);

			HandleDependentProperties(args);
		}

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(storage, value))
				return false;

			storage = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		private void HandleDependentProperties(PropertyChangedEventArgs args)
		{
			switch (args.PropertyName)
			{
				case "InputText":
				case "LeaveInitialCommandBlock":
				case "OutputFormatter":
				case "OutputDirection":
					{
						OutputText = OutputFormatter.Format(InputText);
					}
					break;
				case "OutputText":
					{
						IsInvalid = OutputText.Length > 32767;
					}
					break;
				case "SelectedTab":
					{
						TabItem obj = SelectedTab as TabItem;
						if (obj != null)
						{
							switch(obj.Header.ToString())
							{
								case "Single Pillar":
								default:
									OutputFormatter = new SinglePillarOutputFormatter(this);
									break;
								case "Dual Pillar":
									OutputFormatter = new DualPillarOutputFormatter(this);
									break;
							}
						}
					}
					break;
			}
		}

		public MainWindowPresenter()
		{
			OutputFormatter = new SinglePillarOutputFormatter(this);
		}

		private string _inputText = string.Empty;
		public string InputText
		{
			get { return _inputText; }
			set
			{
				SetProperty(ref _inputText, value);
			}
		}

		private string _outputText = string.Empty;
		public string OutputText
		{
			get { return _outputText; }
			set
			{
				SetProperty(ref _outputText, value);
			}
		}

		private bool _isInvalid = false;
		public bool IsInvalid
		{
			get { return _isInvalid; }
			set
			{
				SetProperty(ref _isInvalid, value);
			}
		}

		private bool _leaveInitialCommandBlock = false;
		public bool LeaveInitialCommandBlock
		{
			get { return _leaveInitialCommandBlock; }
			set
			{
				SetProperty(ref _leaveInitialCommandBlock, value);
			}
		}

		private object _selectedTab;
		public object SelectedTab
		{
			get { return _selectedTab; }
			set
			{
				SetProperty(ref _selectedTab, value);
			}
		}

		private IOutputFormatter _outputFormatter = null;
		public IOutputFormatter OutputFormatter
		{
			get { return _outputFormatter; }
			set
			{
				SetProperty(ref _outputFormatter, value);
			}
		}

		private string _outputDirection = "North";
		public string OutputDirection
		{
			get { return _outputDirection; }
			set
			{
				SetProperty(ref _outputDirection, value);
			}
		}

		private void SetOutputTextDualPillar(string inboundText)
		{
			OutputText = string.Empty;
		}
	}
}
