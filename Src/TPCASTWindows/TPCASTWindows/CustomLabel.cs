using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TPCASTWindows
{
	internal class CustomLabel : Label
	{
		private int lineDistance = 3;

		private Graphics gcs;

		private int iHeight;

		private int height = 200;

		private string[] nrLine;

		private string[] nrLinePos;

		private int searchPos;

		private int section = 1;

		public int LineDistance
		{
			get
			{
				return this.lineDistance;
			}
			set
			{
				this.lineDistance = value;
				this.Changed(this.Font, base.Width, this.Text);
			}
		}

		public int FHeight
		{
			get
			{
				return this.Font.Height;
			}
		}

		protected new int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
				base.Height = value;
			}
		}

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				this.Changed(this.Font, base.Width, value);
			}
		}

		public CustomLabel()
		{
			base.SizeChanged += new EventHandler(this.LabelTx_SizeChanged);
			base.FontChanged += new EventHandler(this.LabelTx_FontChanged);
		}

		private void LabelTx_TextChanged(object sender, EventArgs e)
		{
			this.Changed(this.Font, base.Width, this.Text);
		}

		private void LabelTx_FontChanged(object sender, EventArgs e)
		{
			this.Changed(this.Font, base.Width, this.Text);
		}

		private void LabelTx_SizeChanged(object sender, EventArgs e)
		{
			this.Changed(this.Font, base.Width, this.Text);
		}

		public CustomLabel(IContainer container)
		{
			container.Add(this);
		}

		protected void Changed(Font ft, int iWidth, string value)
		{
			this.iHeight = 0;
			if (value != "")
			{
				if (this.gcs == null)
				{
					this.gcs = base.CreateGraphics();
					SizeF sf0 = this.gcs.MeasureString(new string('测', 20), ft);
					this.searchPos = (int)((float)(iWidth * 20) / sf0.Width);
				}
				this.nrLine = value.Split(new string[]
				{
					Environment.NewLine
				}, StringSplitOptions.RemoveEmptyEntries);
				this.section = this.nrLine.Length;
				this.nrLinePos = new string[this.section];
				for (int i = 0; i < this.section; i++)
				{
					int ipos = 0;
					int arg_A4_0 = this.searchPos;
					if (this.searchPos >= this.nrLine[i].Length)
					{
						ipos += this.nrLine[i].Length;
						string[] var_8_DB_cp_0 = this.nrLinePos;
						int var_8_DB_cp_1 = i;
						var_8_DB_cp_0[var_8_DB_cp_1] = var_8_DB_cp_0[var_8_DB_cp_1] + "," + ipos.ToString();
						this.iHeight++;
					}
					else
					{
						string drawstring = this.nrLine[i];
						this.nrLinePos[i] = "";
						while (drawstring.Length > this.searchPos)
						{
							bool isfind = false;
							for (int j = this.searchPos; j < drawstring.Length; j++)
							{
								string temps = drawstring.Substring(0, j);
								string tempt = drawstring.Substring(0, j + 1);
								SizeF sf = this.gcs.MeasureString(temps, ft);
								SizeF sf2 = this.gcs.MeasureString(tempt, ft);
								if (sf.Width < (float)iWidth && sf2.Width > (float)iWidth)
								{
									this.iHeight++;
									ipos += j;
									string[] var_8_1A1_cp_0 = this.nrLinePos;
									int var_8_1A1_cp_1 = i;
									var_8_1A1_cp_0[var_8_1A1_cp_1] = var_8_1A1_cp_0[var_8_1A1_cp_1] + "," + ipos.ToString();
									isfind = true;
									drawstring = drawstring.Substring(j);
									break;
								}
							}
							if (!isfind)
							{
								break;
							}
						}
						ipos += drawstring.Length;
						string[] var_8_20D_cp_0 = this.nrLinePos;
						int var_8_20D_cp_1 = i;
						var_8_20D_cp_0[var_8_20D_cp_1] = var_8_20D_cp_0[var_8_20D_cp_1] + "," + ipos.ToString();
						this.iHeight++;
					}
				}
			}
			this.Height = this.iHeight * (ft.Height + this.lineDistance);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics arg_2C_0 = e.Graphics;
			string drawString = this.Text;
			Font drawFont = this.Font;
			SolidBrush drawBrush = new SolidBrush(this.ForeColor);
			Convert.ToInt16(arg_2C_0.MeasureString(this.Text, this.Font).Width / (float)base.Width);
			int fHeight = this.Font.Height;
			int htHeight = 0;
			this.AutoSize = false;
			float x = 0f;
			StringFormat drawFormat = new StringFormat();
			int arg_72_0 = drawString.Length;
			string tmpStr = "";
			for (int i = 0; i < this.section; i++)
			{
				int first = 0;
				string subStr = this.nrLine[i];
				if (this.nrLinePos[i] != null)
				{
					tmpStr = this.nrLinePos[i].TrimStart(new char[]
					{
						','
					});
				}
				string midStr = subStr.Substring(first);
				if (tmpStr != "")
				{
					string[] idxs = tmpStr.Split(new char[]
					{
						','
					});
					for (int j = 0; j < idxs.Length; j++)
					{
						int idx = int.Parse(idxs[j]);
						midStr = subStr.Substring(first, idx - first);
						e.Graphics.DrawString(midStr, drawFont, drawBrush, x, (float)Convert.ToInt16(htHeight), drawFormat);
						htHeight += fHeight + this.lineDistance;
						first = idx;
					}
				}
			}
		}
	}
}
