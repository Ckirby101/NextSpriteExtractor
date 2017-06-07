using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextSpriteExtractor
{

	public class SpriteData
	{
		public string name;
		public List<Bitmap> sprites = new List<Bitmap>();
	}

	class ini
	{

		public delegate void LoadEvent();


		public static event LoadEvent loadEvent;

		public static Dictionary<string, string> dicdata = new Dictionary<string, string>();
		public static List<string> names = new List<string>(); 

		public static string inifile = string.Empty;


		public class Vector2
		{
			public float x, y;
		}
		public class Vector3
		{
			public float x, y, z;
		}

		public class uColor
		{
			public float r, g, b, a;





			public Color GetWindowsColor()
			{
				return Color.FromArgb((int)(a * 255.0f), (int)(r * 255.0f), (int)(g * 255.0f), (int)(b * 255.0f));
			}

		}




		public static bool ParseSettingsData(string datafile)
		{
			dicdata.Clear();

			//get a array of lines, remove mac end of line characters
			string[] data = datafile.Replace("\r", "").Split('\n');

			string currentname = "";
			foreach (var line in data)
			{
				string newline = line;
				if (newline.Trim() == "")
				{
					continue;
				}
				if (newline.StartsWith("//"))
				{
					dicdata.Add(newline, "");
					continue;

				}

				if (newline.StartsWith("["))
				{
					string name = newline.Replace("[", "").Replace("]", "");
					names.Add(name);
					currentname = name;
					continue;

				}



				int index = newline.IndexOf("//");
				if (index > 0)
					newline = newline.Substring(0, index);

				string[] splitLine = newline.Split('=');

				//cannot have more than 1 = symbol!
				if (splitLine.Length != 2) continue;

				//remove spaces
				splitLine[0] = splitLine[0].Replace(" ", "");
				splitLine[1] = splitLine[1].Trim();

				//add data to dictionary
				dicdata.Add(currentname+splitLine[0], splitLine[1]);
			}


			Console.WriteLine("ParseSettingsData " + dicdata.Count);


			if (loadEvent != null)
				loadEvent();


			return true;
		}


		public static string GetRawData(string name)
		{
			if (dicdata.ContainsKey(name))
			{
				return dicdata[name];
			}

			return "...";
		}



		/// -------------------------------------------------------------------------------------------------
		/// <summary>Gets settings float.</summary>
		///
		/// <remarks>Chris Kirby, 02/11/2015.</remarks>
		///
		/// <param name="name">        Setting Name.</param>
		/// <param name="sheet">       Sheet Name.</param>
		/// <param name="defaultValue">default value to return if not found.</param>
		///
		/// <returns>The settings float value or default value.</returns>
		/// -------------------------------------------------------------------------------------------------
		public static float GetSettingsFloat(string name, float defaultValue = 0)
		{
#if UNITY_EDITOR
			if (!AllReady())
				Debug.LogWarning("Simple Settings not ready!");
#endif


			float v = defaultValue;

			//foreach (SimpleSettingsSheet pd in Sheets)
			{
				//if (sheet == pd.sheetname)
				{
					//if (!pd.Ready) return defaultValue;
					string data = GetRawData(name);

					try
					{
						v = float.Parse(data);
					}
					catch
					{
						//Debug.Log(name + " not found!");
						return defaultValue;
					}

				}
			}

			return v;
		}


		/// -------------------------------------------------------------------------------------------------
		/// <summary>Gets settings int.</summary>
		///
		/// <remarks>Chris Kirby, 02/11/2015.</remarks>
		///
		/// <param name="name">        The name.</param>
		/// <param name="sheet">       The sheet.</param>
		/// <param name="defaultValue">true to default value.</param>
		///
		/// <returns>The settings int.</returns>
		/// -------------------------------------------------------------------------------------------------
		public static int GetSettingsInt(string name, int defaultValue = 0)
		{

			int v = defaultValue;

			//foreach (SimpleSettingsSheet pd in Sheets)
			{
				//if (sheet == pd.sheetname)
				{
					//if (!pd.Ready) return defaultValue;
					string data = GetRawData(name);

					try
					{
						v = int.Parse(data);
					}
					catch
					{
						return defaultValue;
					}

				}
			}

			return v;
		}

		/// -------------------------------------------------------------------------------------------------
		/// <summary>Gets settings string.</summary>
		///
		/// <remarks>Chris Kirby, 02/11/2015.</remarks>
		///
		/// <param name="name">        The name.</param>
		/// <param name="sheet">       The sheet.</param>
		/// <param name="defaultValue">true to default value.</param>
		///
		/// <returns>The settings string.</returns>
		/// -------------------------------------------------------------------------------------------------
		public static string GetSettingsString(string name, string defaultValue = "")
		{


			string v = defaultValue;

			//foreach (SimpleSettingsSheet pd in Sheets)
			{
				//if (sheet == pd.sheetname)
				{
					//if (!pd.Ready) return defaultValue;
					string data = GetRawData(name);

					return data;
				}
			}

			return v;
		}


		/// -------------------------------------------------------------------------------------------------
		/// <summary>Gets settings bool.</summary>
		///
		/// <remarks>Chris Kirby, 02/11/2015.</remarks>
		///
		/// <param name="name">        The name.</param>
		/// <param name="sheet">       The sheet.</param>
		/// <param name="defaultValue">true to default value.</param>
		///
		/// <returns>true if it succeeds, false if it fails.</returns>
		/// -------------------------------------------------------------------------------------------------
		public static bool GetSettingsBool(string name, bool defaultValue = false)
		{
			bool v = defaultValue;

			//foreach (SimpleSettingsSheet pd in Sheets)
			{
				//if (sheet == pd.sheetname)
				{
					//if (!pd.Ready) return defaultValue;
					string data = GetRawData(name);

					string cleaned = data.ToLower();

					if (cleaned.Contains("1")) v = true;
					if (cleaned.Contains("yes")) v = true;
					if (cleaned.Contains("true")) v = true;

					if (cleaned.Contains("0")) v = false;
					if (cleaned.Contains("false")) v = false;
					if (cleaned.Contains("no")) v = false;

					return v;
				}
			}

			return v;
		}

		/// -------------------------------------------------------------------------------------------------
		/// <summary>   Gets settings vector 2. </summary>
		/// <param name="name">         Setting Name. </param>
		/// <param name="defaultValue"> default value to return if not found. </param>
		/// <returns>   The settings vector 2. </returns>
		/// -------------------------------------------------------------------------------------------------
		public static Vector2 GetSettingsVector2(string name, Vector2 defaultValue)
		{
			Vector2 v = defaultValue;

			//foreach (SimpleSettingsSheet pd in Sheets)
			{
				//if (sheet == pd.sheetname)
				{
					//if (!pd.Ready) return defaultValue;
					string data = GetRawData(name);

					string[] splitLine = data.Split(',');

					//not the right number of data
					if (splitLine.Length != 2) return v;


					try
					{
						v.x = float.Parse(splitLine[0]);
						v.y = float.Parse(splitLine[1]);
					}
					catch
					{
						//Debug.Log(name + " not found!");
						return defaultValue;
					}


					return v;
				}
			}

			return v;
		}

		public static Vector3 GetSettingsVector3(string name, Vector3 defaultValue)
		{
			Vector3 v = defaultValue;

			//foreach (SimpleSettingsSheet pd in Sheets)
			{
				//if (sheet == pd.sheetname)
				{
					//if (!pd.Ready) return defaultValue;
					string data = GetRawData(name);

					string[] splitLine = data.Split(',');

					//not the right number of data
					if (splitLine.Length != 3) return v;


					try
					{
						v.x = float.Parse(splitLine[0]);
						v.y = float.Parse(splitLine[1]);
						v.z = float.Parse(splitLine[2]);
					}
					catch
					{
						//Debug.Log(name + " not found!");
						return defaultValue;
					}


					return v;
				}
			}

			return v;
		}

		/// -------------------------------------------------------------------------------------------------
		/// <summary>   Gets settings color. </summary>
		/// <param name="name">         Setting Name. </param>
		/// <param name="defaultValue"> default value to return if not found. </param>
		/// <returns>   The settings color. </returns>
		/// -------------------------------------------------------------------------------------------------
		public static uColor GetSettingsColor(string name, uColor defaultValue)
		{
			uColor v = defaultValue;

			//foreach (SimpleSettingsSheet pd in Sheets)
			{
				//if (sheet == pd.sheetname)
				{
					//if (!pd.Ready) return defaultValue;
					string data = GetRawData(name);

					string[] splitLine = data.Split(',');

					//not the right number of data
					if (splitLine.Length == 3)
					{
						try
						{
							v.a = 1;
							v.r = float.Parse(splitLine[0]);
							v.g = float.Parse(splitLine[1]);
							v.b = float.Parse(splitLine[2]);
						}
						catch
						{
							//Debug.Log(name + " not found!");
							return defaultValue;
						}
					}

					if (splitLine.Length == 4)
					{
						try
						{
							v.r = float.Parse(splitLine[0]);
							v.g = float.Parse(splitLine[1]);
							v.b = float.Parse(splitLine[2]);
							v.a = float.Parse(splitLine[3]);
						}
						catch
						{
							//Debug.Log(name + " not found!");
							return defaultValue;
						}
					}



					return v;
				}
			}

			return v;
		}

		/// -------------------------------------------------------------------------------------------------
		/// <summary>   Gets settings raw. </summary>
		/// <param name="name"> The name. </param>
		/// <returns>   The settings raw. </returns>
		/// -------------------------------------------------------------------------------------------------
		public static string GetSettingsRaw(string name)
		{
			return (GetRawData(name));
		}


		public static void SetSetting(string name, int v)
		{
			if (!dicdata.ContainsKey(name)) dicdata.Add(name, "");

			dicdata[name] = v.ToString();
		}

		public static void SetSetting(string name, float v)
		{
			if (!dicdata.ContainsKey(name)) dicdata.Add(name, "");

			dicdata[name] = v.ToString();
		}

		public static void SetSetting(string name, String v)
		{
			if (!dicdata.ContainsKey(name)) dicdata.Add(name, "");

			dicdata[name] = v;
		}


		public static void SetSetting(string name, bool v)
		{
			if (!dicdata.ContainsKey(name)) dicdata.Add(name, "");

			if (v)
				dicdata[name] = "true";
			else
				dicdata[name] = "false";
		}



		public static void SetSetting(string name, uColor v)
		{
			if (!dicdata.ContainsKey(name)) dicdata.Add(name, "");

			dicdata[name] = v.r.ToString() + "," + v.g.ToString() + "," + v.b.ToString() + "," + v.a.ToString();
		}


		public static void SetSetting(string name, Color v)
		{
			if (!dicdata.ContainsKey(name)) dicdata.Add(name, "");

			float r = ((float)v.R / 255.0f);
			float g = ((float)v.G / 255.0f);
			float b = ((float)v.B / 255.0f);
			float a = ((float)v.A / 255.0f);

			dicdata[name] = r.ToString() + "," + g.ToString() + "," + b.ToString() + "," + a.ToString();
		}


		public static void SetSetting(string name, Vector2 v)
		{
			if (!dicdata.ContainsKey(name)) dicdata.Add(name, "");

			dicdata[name] = v.x.ToString() + "," + v.y.ToString();
		}

		public static void SetSetting(string name, Vector3 v)
		{
			if (!dicdata.ContainsKey(name)) dicdata.Add(name, "");

			dicdata[name] = v.x.ToString() + "," + v.y.ToString() + "," + v.z.ToString();
		}

		public static void SetSettingV3(string name, string x, string y, string z)
		{
			if (!dicdata.ContainsKey(name)) dicdata.Add(name, "");

			if (IsNumber(x) && IsNumber(y) && IsNumber(z))
			{
				dicdata[name] = x + "," + y + "," + z;
			}
		}


		static bool IsNumber(string s)
		{
			int i;
			return int.TryParse(s, out i);
		}

		public static void SetSettingsPath(string name, string v)
		{

			//string file = Path.GetFileName(v);
			if (!dicdata.ContainsKey(name)) dicdata.Add(name, "");


			try
			{

				string path = Path.GetDirectoryName(inifile);

				string rp = v.Replace(path, ".");
				dicdata[name] = rp;
			}
			catch
			{

			}

		}


		public static string GetIniFile()
		{
			string s = "//Generated ini file\n\n";

			foreach (KeyValuePair<string, string> entry in dicdata)
			{

				if (entry.Key.StartsWith("//"))
				{
					s = s + "\n" + entry.Key + "\n";

				}
				else
				{
					s = s + entry.Key + "=" + entry.Value + "\n";

				}

			}

			s = s.Replace("\n", Environment.NewLine);

			return s;
		}




	}
}
