using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TPCASTWindows
{
	internal class WaterTextBox : TextBox
	{
		private readonly Label lblwaterText = new Label();

		[Category("扩展属性"), Description("显示的提示信息")]
		public string WaterText
		{
			get
			{
				return this.lblwaterText.Text;
			}
			set
			{
				this.lblwaterText.Text = value;
			}
		}

		[Category("扩展属性"), Description("提示信息的颜色")]
		public Color WaterTextColor
		{
			get
			{
				return this.lblwaterText.ForeColor;
			}
			set
			{
				this.lblwaterText.ForeColor = value;
			}
		}

		[Category("扩展属性"), Description("提示信息的字体")]
		public Font WaterTextFont
		{
			get
			{
				return this.lblwaterText.Font;
			}
			set
			{
				this.lblwaterText.Font = value;
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
				this.lblwaterText.Visible = (value == string.Empty);
				base.Text = value;
			}
		}

		public WaterTextBox()
		{
			this.AutoSize = false;
			this.lblwaterText.BorderStyle = BorderStyle.None;
			this.lblwaterText.Enabled = true;
			this.lblwaterText.BackColor = Color.White;
			this.lblwaterText.AutoSize = false;
			this.lblwaterText.Top = 3;
			this.lblwaterText.Left = 2;
			this.lblwaterText.FlatStyle = FlatStyle.System;
			this.lblwaterText.Click += new EventHandler(this.Onclick);
			this.lblwaterText.Font = this.WaterTextFont;
			this.lblwaterText.TextAlign = ContentAlignment.MiddleLeft;
			base.Controls.Add(this.lblwaterText);
		}

		private void Onclick(object sender, EventArgs e)
		{
			base.Focus();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			if (this.Multiline && (base.ScrollBars == ScrollBars.Vertical || base.ScrollBars == ScrollBars.Both))
			{
				this.lblwaterText.Width = base.Width - 20;
			}
			else
			{
				this.lblwaterText.Width = base.Width;
			}
			this.lblwaterText.Height = base.Height - 2;
			base.OnSizeChanged(e);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			this.lblwaterText.Visible = (base.Text == string.Empty);
			base.OnTextChanged(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.lblwaterText.Visible = false;
			base.OnMouseDown(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			this.lblwaterText.Visible = (base.Text == string.Empty);
			base.OnMouseLeave(e);
		}
	}
}
