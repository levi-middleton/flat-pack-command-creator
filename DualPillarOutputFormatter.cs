using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatPackCommandCreator
{
	public class DualPillarOutputFormatter : IOutputFormatter
	{
		private MainWindowPresenter _presenter;
		public DualPillarOutputFormatter(MainWindowPresenter presenter)
		{
			_presenter = presenter;
		}

		public string Format(string inboundText)
		{
			string[] lines = inboundText.Split('\n');

			//sanitize the lines
			for (int i = 0; i < lines.Length; ++i)
			{
				lines[i] = lines[i].Replace("\r", string.Empty);
				if (lines[i].StartsWith("/"))
					lines[i] = lines[i].Remove(0, 1);
			}

			//start with the redstone filler command
			string output = ID_PREFIX;
			output += string.Format(BLOCK, COMMAND_BLOCK);
			switch(_presenter.OutputDirection)
			{
				case "North": //-Z
					output += string.Format(COMMAND_SUFFIX, string.Format("fill ~ ~3 ~-1 ~ ~{0} ~-1 minecraft:redstone_block", lines.Length + 3));
					break;
				case "South":  //+Z
					output += string.Format(COMMAND_SUFFIX, string.Format("fill ~ ~3 ~1 ~ ~{0} ~1 minecraft:redstone_block", lines.Length + 3));
					break;
				case "East": //+X
					output += string.Format(COMMAND_SUFFIX, string.Format("fill ~1 ~3 ~ ~1 ~{0} ~ minecraft:redstone_block", lines.Length + 3));
					break;
				case "West": //-X
					output += string.Format(COMMAND_SUFFIX, string.Format("fill ~-1 ~3 ~ ~-1 ~{0} ~ minecraft:redstone_block", lines.Length + 3));
					break;
			}

			output = ID_PREFIX + string.Format(BLOCK, REDSTONE_BLOCK) + string.Format(RIDING_SUFFIX, output);
			output = ID_PREFIX + string.Format(BLOCK, IRON_BLOCK) + string.Format(RIDING_SUFFIX, output);

			//put the rest of the lines in
			for (int i = 0; i < lines.Length; ++i)
			{
				output = ID_PREFIX + string.Format(BLOCK, COMMAND_BLOCK) + string.Format(COMMAND_SUFFIX, lines[i]) + string.Format(RIDING_SUFFIX, output);
			}

			//capstone cleanup command block
			{
				string cleanup = string.Empty;
				switch (_presenter.OutputDirection)
				{
					case "North": //-Z
						cleanup = string.Format(COMMAND_SUFFIX, string.Format("fill ~ ~ ~-1 ~ ~-{0} ~ minecraft:air", lines.Length + 3 - (_presenter.LeaveInitialCommandBlock ? 1 : 0)));
						break;
					case "South":  //+Z
						cleanup = string.Format(COMMAND_SUFFIX, string.Format("fill ~ ~ ~1 ~ ~-{0} ~ minecraft:air", lines.Length + 3 - (_presenter.LeaveInitialCommandBlock ? 1 : 0)));
						break;
					case "East": //+X
						cleanup = string.Format(COMMAND_SUFFIX, string.Format("fill ~1 ~ ~ ~ ~-{0} ~ minecraft:air", lines.Length + 3 - (_presenter.LeaveInitialCommandBlock ? 1 : 0)));
						break;
					case "West": //-X
						cleanup = string.Format(COMMAND_SUFFIX, string.Format("fill ~-1 ~ ~ ~ ~-{0} ~ minecraft:air", lines.Length + 3 - (_presenter.LeaveInitialCommandBlock ? 1 : 0)));
						break;
				}
				output = string.Format(STARTING_BLOCK, string.Format(BLOCK, COMMAND_BLOCK) + cleanup + string.Format(RIDING_SUFFIX, output));
			}

			return output;
		}

		private static readonly string STARTING_BLOCK = "summon FallingSand ~ ~3 ~ {{{0}}}";
		private static readonly string BLOCK = "Block:{0},Time:1,DropItem:0";
		private static readonly string ID_PREFIX = "id:FallingSand,";
		private static readonly string COMMAND_SUFFIX = ",TileEntityData:{{Command:\"{0}\"}}";
		private static readonly string RIDING_SUFFIX = ",Riding:{{{0}}}";

		private static readonly string REDSTONE_BLOCK = "redstone_block";
		private static readonly string IRON_BLOCK = "iron_block";
		private static readonly string COMMAND_BLOCK = "command_block";
	}
}
