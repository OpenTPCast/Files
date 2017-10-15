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
					SizeF sizeF = this.gcs.MeasureString(new string('测', 20), ft);
					this.searchPos = (int)((float)(iWidth * 20) / sizeF.Width);
				}
				this.nrLine = value.Split(new string[]
				{
					"/r/n"
				}, StringSplitOptions.RemoveEmptyEntries);
				this.section = this.nrLine.Length;
				this.nrLinePos = new string[this.section];
				for (int i = 0; i < this.section; i++)
				{
					int num = 0;
					int arg_A4_0 = this.searchPos;
					if (this.searchPos >= this.nrLine[i].Length)
					{
						num += this.nrLine[i].Length;
						string[] var_8_DB_cp_0 = this.nrLinePos;
						int var_8_DB_cp_1 = i;
						var_8_DB_cp_0[var_8_DB_cp_1] = var_8_DB_cp_0[var_8_DB_cp_1] + "," + num.ToString();
						this.iHeight++;
					}
					else
					{
						string text = this.nrLine[i];
						this.nrLinePos[i] = "";
						while (text.Length > this.searchPos)
						{
							bool flag = false;
							for (int j = this.searchPos; j < text.Length; j++)
							{
								string text2 = text.Substring(0, j);
								string text3 = text.Substring(0, j + 1);
								SizeF sizeF2 = this.gcs.MeasureString(text2, ft);
								SizeF sizeF3 = this.gcs.MeasureString(text3, ft);
								if (sizeF2.Width < (float)iWidth && sizeF3.Width > (float)iWidth)
								{
									this.iHeight++;
									num += j;
									string[] var_8_1A1_cp_0 = this.nrLinePos;
									int var_8_1A1_cp_1 = i;
									var_8_1A1_cp_0[var_8_1A1_cp_1] = var_8_1A1_cp_0[var_8_1A1_cp_1] + "," + num.ToString();
									flag = true;
									text = text.Substring(j);
									break;
								}
							}
							if (!flag)
							{
								break;
							}
						}
						num += text.Length;
						string[] var_8_20D_cp_0 = this.nrLinePos;
						int var_8_20D_cp_1 = i;
						var_8_20D_cp_0[var_8_20D_cp_1] = var_8_20D_cp_0[var_8_20D_cp_1] + "," + num.ToString();
						this.iHeight++;
					}
				}
			}
			this.Height = this.iHeight * (ft.Height + this.lineDistance);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics arg_2C_0 = e.Graphics;
			string text = this.Text;
			Font font = this.Font;
			SolidBrush brush = new SolidBrush(this.ForeColor);
			Convert.ToInt16(arg_2C_0.MeasureString(this.Text, this.Font).Width / (float)base.Width);
			int num = this.Font.Height;
			int num2 = 0;
			this.AutoSize = false;
			float x = 0f;
			StringFormat format = new StringFormat();
			int arg_72_0 = text.Length;
			string text2 = "";
			for (int i = 0; i < this.section; i++)
			{
				int num3 = 0;
				string text3 = this.nrLine[i];
				if (this.nrLinePos[i] != null)
				{
					text2 = this.nrLinePos[i].TrimStart(new char[]
					{
						','
					});
				}
				string s = text3.Substring(num3);
				if (text2 != "")
				{
					string[] array = text2.Split(new char[]
					{
						','
					});
					for (int j = 0; j < array.Length; j++)
					{
						int num4 = int.Parse(array[j]);
						s = text3.Substring(num3, num4 - num3);
						e.Graphics.DrawString(s, font, brush, x, (float)Convert.ToInt16(num2), format);
						num2 += num + this.lineDistance;
						num3 = num4;
					}
				}
			}
		}
	}
}
