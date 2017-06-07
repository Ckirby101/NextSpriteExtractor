using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NextSpriteExtractor
{



	public partial class Form1 : Form
	{
		public int spriteindex = 0;
		public int spriteframeindex = 0;
		public List<SpriteData> spriteData = new List<SpriteData>();


		/// -------------------------------------------------------------------------------------------------
		/// <summary>   Default constructor. </summary>
		/// -------------------------------------------------------------------------------------------------
		public Form1()
		{
			InitializeComponent();
		}

        /// -------------------------------------------------------------------------------------------------
        /// <summary>
        /// Event handler. Called by loadControlFileToolStripMenuItem for click events.
        /// </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        /// -------------------------------------------------------------------------------------------------
		private void loadControlFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int size = -1;
			DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
			if (result == DialogResult.OK) // Test result.
			{
				string file = openFileDialog1.FileName;
				try
				{
					string text = File.ReadAllText(file);
					size = text.Length;

					ini.inifile = file;


					ini.ParseSettingsData(text);

				}
				catch (IOException)
				{
				}
			}


	        ParseIniData();

			Console.WriteLine(size); // <-- Shows file size in debugging mode.
			Console.WriteLine(result); // <-- For debugging use.
		}

        /// -------------------------------------------------------------------------------------------------
        /// <summary>   Event handler. Called by testToolStripMenuItem for click events. </summary>
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        /// -------------------------------------------------------------------------------------------------
		private void testToolStripMenuItem_Click(object sender, EventArgs e)
		{



		}

        /// -------------------------------------------------------------------------------------------------
        /// <summary>   Extracts the image. </summary>
        /// <param name="sheet">    [in,out] The sheet. </param>
        /// <param name="store">    [in,out] The store. </param>
        /// <param name="xo">       The xo. </param>
        /// <param name="yo">       The yo. </param>
        /// <param name="display">  true to display. </param>
        /// -------------------------------------------------------------------------------------------------
		public void ExtractImage(ref Bitmap sheet, ref byte[] store, int xo, int yo,ref Bitmap minibitmap,int tempoffx,int tempoffy)
		{



			List<Color> palette = sheet.Palette.Entries.ToList();

			int index = 0;
			for (int x = 0; x < 16; x++)
			{
				for (int y = 0; y < 16; y++)
				{
					int cindex = 227;
					Color c =  System.Drawing.Color.Transparent;
					try
					{
						c = sheet.GetPixel(x + xo, y + yo);
						cindex = palette.IndexOf(c);

					}
					catch (Exception)
					{
					}


					if (cindex == 227) c = System.Drawing.Color.Transparent;

					minibitmap.SetPixel(x+ tempoffx, y+ tempoffy, c);




					store[index] = (byte)cindex;
					index++;

				}

			}


		}

        /// -------------------------------------------------------------------------------------------------
        /// <summary>   Parse initialise data. </summary>
        /// -------------------------------------------------------------------------------------------------
		public void ParseIniData()
        {
	        spriteframeindex = 0;
	        spriteindex = 0;
			spriteData.Clear();


			string exportfile = ini.GetSettingsString("export", "");
			string file = ini.GetSettingsString("file", "");
			if (string.IsNullOrEmpty(file)) return;
			if (string.IsNullOrEmpty(exportfile)) return;

			//load sheet
			Bitmap myBitmap = new Bitmap(file);

			//make byte data store
			byte[] current = new byte[256];

			string code = ";exported sprite data\n";

			foreach (string name in ini.names)
			{

				SpriteData sData = new SpriteData();
				sData.name = name;

				int offsetx = ini.GetSettingsInt(name + "offsetx", 0);
				int offsety = ini.GetSettingsInt(name + "offsety", 0);
				int	modx = ini.GetSettingsInt(name + "modx", 0);
				int mody = ini.GetSettingsInt(name + "mody", 0);
				int countx = ini.GetSettingsInt(name + "countx", 1);
				int county = ini.GetSettingsInt(name + "county", 1);

				int sizex = ini.GetSettingsInt(name + "sizex", 16);
				int sizey = ini.GetSettingsInt(name + "sizey", 16);

				int currectoffsety = offsety;
				int count = 0;
				for (int cy = 0; cy < county; cy++)
				{
					int currectoffsetx = offsetx;
					for (int cx = 0; cx < countx; cx++)
					{
						code = code + "; extracted from " + currectoffsetx + "," + currectoffsety + " of sheet\n";

						Bitmap temp = new Bitmap(sizex, sizey);


						if (sizex <= 16 && sizey <= 16)
						{
							ExtractImage(ref myBitmap, ref current, currectoffsetx, currectoffsety, ref temp,0,0);
							BuildCode(ref code, ref current, name, count);
						}
						else
						{
							int xlumps = sizex / 16;
							int ylumps = sizey / 16;

							for (int ys = 0; ys < ylumps; ys++)
							{
								for (int xs = 0; xs < xlumps; xs++)
								{
									ExtractImage(ref myBitmap, ref current, currectoffsetx +(xs*16), currectoffsety+(ys*16), ref temp,xs*16,ys*16);
									BuildCode(ref code, ref current, name+"x"+ys+"y"+xs+"_", count);


								}

							}

						}




						sData.sprites.Add(temp);
						pictureBox1.Image = temp;



						currectoffsetx += modx;
						count++;
					}


					currectoffsety += mody;
				}


				spriteData.Add(sData);

			}

			System.IO.File.WriteAllText(exportfile,code);


		}

        /// -------------------------------------------------------------------------------------------------
        /// <summary>   Builds a code. </summary>
        /// <param name="code">     [in,out] The code. </param>
        /// <param name="current">  [in,out] The current. </param>
        /// <param name="name">     The name. </param>
        /// <param name="count">    Number of. </param>
        /// -------------------------------------------------------------------------------------------------
		public void BuildCode(ref string code, ref byte[] current, string name, int count)
        {
			string HexAlphabet = "0123456789ABCDEF";

			if (count == 0)
				code = code + "\n"+name + ":\n";


			code = code + "" + name + "_"+count.ToString()+":\n";

	        int index = 0;
	        for (int y = 0; y < 16; y++)
	        {
		        code = code + "db ";

				for (int x = 0; x < 16; x++)
				{
					int B = current[index];

					code = code + "0x"+HexAlphabet[(int)(B >> 4)] +""+ HexAlphabet[(int)(B & 0xF)];
					if (x < 15)
						code = code + ",";
					else
						code = code + "\n";

					index++;

				}

			}


		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (spriteData.Count <= 0) return;
			spriteframeindex++;
			if (spriteframeindex >= spriteData[spriteindex].sprites.Count) spriteframeindex = 0;


			SetSprite(spriteData[spriteindex], spriteframeindex);
		}



		public void SetSprite(SpriteData data, int frame)
		{
			pictureBox1.Image = data.sprites[frame];
			pictureBox2.Image = data.sprites[frame];
			label1.Text = data.name + " " + (frame + 1) + "/" + data.sprites.Count;

		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (spriteData.Count <= 0) return;

			spriteframeindex--;
			if (spriteframeindex < 0) spriteframeindex = (spriteData[spriteindex].sprites.Count - 1);


			SetSprite(spriteData[spriteindex], spriteframeindex);

		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (spriteData.Count <= 0) return;

			spriteframeindex = 0;
			spriteindex --;
			if (spriteindex < 0) spriteindex = spriteData.Count - 1;
			SetSprite(spriteData[spriteindex], spriteframeindex);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (spriteData.Count <= 0) return;

			spriteframeindex = 0;
			spriteindex++;
			if (spriteindex >= spriteData.Count) spriteindex = 0;
			SetSprite(spriteData[spriteindex], spriteframeindex);

		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			about ab = new about();
			ab.ShowDialog();
		}
	}
}
