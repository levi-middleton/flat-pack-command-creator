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

		/// <summary>
		/// Formats the specified inbound text.
		/// </summary>
		/// <param name="inboundText">The inbound text.</param>
		/// <returns></returns>
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

			//start with an iron block just cuz
			string output = ID_PREFIX + string.Format(BLOCK, IRON_BLOCK);
			
			//put in the command lines
			for (int i = 0; i < lines.Length; ++i)
			{
				output = ID_PREFIX + string.Format(BLOCK, COMMAND_BLOCK) + string.Format(COMMAND_SUFFIX, lines[i]) + string.Format(RIDING_SUFFIX, output);
			}

			//cleanup command block
			{
				string generated_command_suffix = string.Empty;
				switch (_presenter.OutputDirection)
				{
					case "North": //-Z
						generated_command_suffix = string.Format(COMMAND_SUFFIX, string.Format("fill ~ ~2 ~-1 ~ ~-{0} ~ minecraft:air", lines.Length + 2 - (_presenter.LeaveInitialCommandBlock ? 1 : 0)));
						break;
					case "South":  //+Z
						generated_command_suffix = string.Format(COMMAND_SUFFIX, string.Format("fill ~ ~2 ~1 ~ ~-{0} ~ minecraft:air", lines.Length + 2 - (_presenter.LeaveInitialCommandBlock ? 1 : 0)));
						break;
					case "East": //+X
						generated_command_suffix = string.Format(COMMAND_SUFFIX, string.Format("fill ~1 ~2 ~ ~ ~-{0} ~ minecraft:air", lines.Length + 2 - (_presenter.LeaveInitialCommandBlock ? 1 : 0)));
						break;
					case "West": //-X
						generated_command_suffix = string.Format(COMMAND_SUFFIX, string.Format("fill ~-1 ~2 ~ ~ ~-{0} ~ minecraft:air", lines.Length + 2 - (_presenter.LeaveInitialCommandBlock ? 1 : 0)));
						break;
				}
				output = ID_PREFIX + string.Format(BLOCK, COMMAND_BLOCK) + generated_command_suffix + string.Format(RIDING_SUFFIX, output);
			}

			//command block for power
			{
				string generated_command_suffix = string.Empty;
				switch (_presenter.OutputDirection)
				{
					case "North": //-Z
						generated_command_suffix = string.Format(COMMAND_SUFFIX, string.Format("fill ~ ~-1 ~-1 ~ ~-{0} ~-1 minecraft:redstone_block", lines.Length + 1));
						break;
					case "South":  //+Z
						generated_command_suffix = string.Format(COMMAND_SUFFIX, string.Format("fill ~ ~-1 ~1 ~ ~-{0} ~1 minecraft:redstone_block", lines.Length + 1));
						break;
					case "East": //+X
						generated_command_suffix = string.Format(COMMAND_SUFFIX, string.Format("fill ~1 ~-1 ~ ~1 ~-{0} ~ minecraft:redstone_block", lines.Length + 1));
						break;
					case "West": //-X
						generated_command_suffix = string.Format(COMMAND_SUFFIX, string.Format("fill ~-1 ~-1 ~ ~-1 ~-{0} ~ minecraft:redstone_block", lines.Length + 1));
						break;
				}
				output = ID_PREFIX + string.Format(BLOCK, COMMAND_BLOCK) + generated_command_suffix + string.Format(RIDING_SUFFIX, output);
			}

			//capstone redstone block
			output = string.Format(STARTING_BLOCK, string.Format(BLOCK, REDSTONE_BLOCK) + string.Format(RIDING_SUFFIX, output));

			return output;
		}

		private static readonly string STARTING_BLOCK = "summon FallingSand ~ ~3 ~ {{{0}}}";
		private static readonly string BLOCK = "Block:{0},Time:1";
		private static readonly string ID_PREFIX = "id:FallingSand,";
		private static readonly string COMMAND_SUFFIX = ",TileEntityData:{{Command:\"{0}\"}}";
		private static readonly string RIDING_SUFFIX = ",Riding:{{{0}}}";

		private static readonly string REDSTONE_BLOCK = "redstone_block";
		private static readonly string IRON_BLOCK = "iron_block";
		private static readonly string COMMAND_BLOCK = "command_block";
	}
}
