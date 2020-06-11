using System.Collections.Generic;

namespace Sharp16
{
	internal class Config
	{
		internal Dictionary<string, string> Flags = new Dictionary<string, string>();
		internal List<string> Files = new List<string>();

		internal bool IsSet(string flag) => Flags.ContainsKey(flag);

		internal string this[string flag]
		{
			get => Flags[flag];
			set => Flags[flag] = value;
		}

		internal Config(string[] args)
		{
			if (args.Length > 0)
			{
				foreach (string a in args)
				{
					if (a[0] == '/')
					{
						int ind = a.IndexOf(':');
						string key = a;
						string val = null;
						if (ind > -1)
						{
							key = a.Substring(0, ind);
							val = a.Substring(ind + 1);
						}
						Flags[key.Substring(1)] = val;
					}
					else { Files.Add(a); }
				}
			}
		}
	}
}
