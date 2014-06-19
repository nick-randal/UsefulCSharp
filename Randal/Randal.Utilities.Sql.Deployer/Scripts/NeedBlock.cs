using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace Randal.Utilities.Sql.Deployer.Scripts
{
	public sealed class NeedBlock : CsvParameterBlock
	{
		public NeedBlock(string text) : base("need", text)
		{
			_files = new List<string>();
		}

		public IReadOnlyList<string> Files { get { return _files; } } 

		public override IReadOnlyList<string> Parse()
		{
			var messages = new List<string>();

			foreach(var item in base.Parse())
			{
				var temp = item.Trim();

				

				if (InvalidFileName.IsMatch(temp))
				{
					messages.Add("Invalid file name '" + temp + "' provided as a 'need'.");
					continue;
				}

				if (temp.EndsWith(SqlExtension, StringComparison.InvariantCultureIgnoreCase))
					_files.Add(temp);
				else
					_files.Add(temp + ".sql");
			}

			IsValid = messages.Count == 0;

			return messages;
		}

		private const string SqlExtension = ".sql";

		private static readonly Regex InvalidFileName =
			new Regex("[" + Regex.Escape(string.Join(string.Empty, Path.GetInvalidFileNameChars())) + "]",
				RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
		private readonly List<string> _files;
	}
}
