using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
					{
						SetOutputText(InputText);
					}
					break;
				case "OutputText":
					{
						IsInvalid = OutputText.Length > 32767;
					}
					break;
			}
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

		private void SetOutputText(string inboundText)
		{
			string[] lines = inboundText.Split('\n');

			//sanitize the lines
			for (int i = 0; i < lines.Length; ++i)
			{
				lines[i] = lines[i].Replace("\r", string.Empty);
				if (lines[i].StartsWith("/"))
					lines[i] = lines[i].Remove(0, 1);
			}

			//start with the first line
			string output = ID_PREFIX;
			output += string.Format(BLOCK, COMMAND_BLOCK);
			output += string.Format(COMMAND_SUFFIX, lines[0]);
			output = ID_PREFIX + string.Format(BLOCK, REDSTONE_BLOCK) + string.Format(RIDING_SUFFIX, output);

			//put the rest of the lines in, interspersing redstone blocks
			for (int i = 1; i < lines.Length; ++i)
			{
				output = ID_PREFIX + string.Format(BLOCK, COMMAND_BLOCK) + string.Format(COMMAND_SUFFIX, lines[i]) + string.Format(RIDING_SUFFIX, output);
				if (i % 2 == 0)
				{
					output = ID_PREFIX + string.Format(BLOCK, REDSTONE_BLOCK) + string.Format(RIDING_SUFFIX, output);
				}
			}

			//add an iron block if the top block is redstone
			if (lines.Length % 2 == 1)
			{
				output = ID_PREFIX + string.Format(BLOCK, IRON_BLOCK) + string.Format(RIDING_SUFFIX, output);
			}

			//add cleanup code
			{
				int numberOfBlocksUnder = ((lines.Length - 1) / 2) * 3 + 4; //5 6 7 8
				output = ID_PREFIX + string.Format(BLOCK, COMMAND_BLOCK) + string.Format(COMMAND_SUFFIX, string.Format(CLEANUP_COMMAND, numberOfBlocksUnder - (LeaveInitialCommandBlock ? 1 : 0))) + string.Format(RIDING_SUFFIX, output);
			}

			//capstone redstone block
			output = string.Format(STARTING_BLOCK, string.Format(BLOCK, REDSTONE_BLOCK) + string.Format(RIDING_SUFFIX, output));

			OutputText = output;
		}

		private static readonly string STARTING_BLOCK = "summon FallingSand ~ ~3 ~ {{{0}}}";
		private static readonly string BLOCK = "Block:{0},Time:1,DropItem:0";
		private static readonly string ID_PREFIX = "id:FallingSand,";
		private static readonly string COMMAND_SUFFIX = ",TileEntityData:{{Command:\"{0}\"}}";
		private static readonly string RIDING_SUFFIX = ",Riding:{{{0}}}";

		private static readonly string REDSTONE_BLOCK = "redstone_block";
		private static readonly string IRON_BLOCK = "iron_block";
		private static readonly string COMMAND_BLOCK = "command_block";

		private static readonly string CLEANUP_COMMAND = "fill ~ ~1 ~ ~ ~-{0} ~ air";
	}
}
