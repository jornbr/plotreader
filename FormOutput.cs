using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;

namespace PlotReader
{
	/// <summary>
	/// Summary description for FormOutput.
	/// </summary>
	public class FormOutput : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ComboBox comboCulture;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioDefault;
		private System.Windows.Forms.RadioButton radioEnglish;
		private System.Windows.Forms.RadioButton radioOther;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioScientific;
		private System.Windows.Forms.RadioButton radioFixedPoint;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboSeparator;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericDecimals;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private string m_strRegKey = null;
		public FormOutput(string strRegKey)
		{
			InitializeComponent();

			// Fill combo box with names of all available formatting cultures.
			CultureInfo [] cults = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
			this.comboCulture.DisplayMember = "EnglishName";
			this.comboCulture.Items.AddRange(cults);
			this.comboCulture.SelectedIndex = 0;

			// Select first separator by default.
			this.comboSeparator.SelectedIndex = 0;

			Microsoft.Win32.RegistryKey regSettingsRoot = null;
			if (strRegKey != null) regSettingsRoot = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(strRegKey,false);
			if (regSettingsRoot != null) 
			{
				// NumberStyle=0: scientific, NumberStyle=1: fixed-point
				int iStyle = (int)regSettingsRoot.GetValue("NumberStyle",(int)0);
				if (iStyle==1) this.radioFixedPoint.Checked = true;

				// NumberCulture=0: user default, NumberCulture=1: US English, NumberCulture=2: custom.
				int iCultureType = (int)regSettingsRoot.GetValue("NumberCulture",(int)0);
				switch (iCultureType) 
				{
					case (0):
						this.radioDefault.Checked = true;
						break;
					case (1):
						this.radioEnglish.Checked = true;
						break;
					case (2):
						this.radioOther.Checked = true;
						break;
				}

				// CustomCulture stored as CultureInfo name, e.g. "en-US".
				string strCustomCulture = (string)regSettingsRoot.GetValue("CustomCulture",cults[0].Name);
				int iCurrent = 0;
				foreach (CultureInfo ci in this.comboCulture.Items) 
				{
					if (ci.Name==strCustomCulture) this.comboCulture.SelectedIndex = iCurrent;
					iCurrent++;
				}

				this.numericDecimals.Value = (int)regSettingsRoot.GetValue("NumberOfDecimals",(int)this.numericDecimals.Value);
				this.comboSeparator.Text = (string)regSettingsRoot.GetValue("Separator",this.comboSeparator.Text);
			}
			this.m_strRegKey = strRegKey;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				Microsoft.Win32.RegistryKey regSettingsRoot = null;
				if (this.m_strRegKey != null) regSettingsRoot = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(this.m_strRegKey,true);
				if (regSettingsRoot != null) 
				{
					int iStyle = (this.radioFixedPoint.Checked ? 1 : 0);
					regSettingsRoot.SetValue("NumberStyle",iStyle);

					if      (this.radioDefault.Checked)
						regSettingsRoot.SetValue("NumberCulture",(int)0);
					else if (this.radioEnglish.Checked)
						regSettingsRoot.SetValue("NumberCulture",(int)1);
					else if (this.radioOther.Checked) 
					{
						regSettingsRoot.SetValue("NumberCulture",(int)2);
						regSettingsRoot.SetValue("CustomCulture",((CultureInfo)this.comboCulture.SelectedItem).Name);
					}

					regSettingsRoot.SetValue("NumberOfDecimals",(int)this.numericDecimals.Value);
					regSettingsRoot.SetValue("Separator",this.comboSeparator.Text);
				}

				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormOutput));
			this.comboCulture = new System.Windows.Forms.ComboBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioDefault = new System.Windows.Forms.RadioButton();
			this.radioEnglish = new System.Windows.Forms.RadioButton();
			this.radioOther = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioScientific = new System.Windows.Forms.RadioButton();
			this.radioFixedPoint = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.comboSeparator = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.numericDecimals = new System.Windows.Forms.NumericUpDown();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericDecimals)).BeginInit();
			this.SuspendLayout();
			// 
			// comboCulture
			// 
			this.comboCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCulture.Enabled = false;
			this.comboCulture.Location = new System.Drawing.Point(64, 66);
			this.comboCulture.Name = "comboCulture";
			this.comboCulture.Size = new System.Drawing.Size(192, 21);
			this.comboCulture.Sorted = true;
			this.comboCulture.TabIndex = 1;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(112, 248);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(200, 248);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.radioDefault);
			this.groupBox1.Controls.Add(this.comboCulture);
			this.groupBox1.Controls.Add(this.radioEnglish);
			this.groupBox1.Controls.Add(this.radioOther);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(8, 88);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(264, 96);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Number formatting culture";
			// 
			// radioDefault
			// 
			this.radioDefault.Checked = true;
			this.radioDefault.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioDefault.Location = new System.Drawing.Point(8, 16);
			this.radioDefault.Name = "radioDefault";
			this.radioDefault.Size = new System.Drawing.Size(248, 24);
			this.radioDefault.TabIndex = 2;
			this.radioDefault.TabStop = true;
			this.radioDefault.Text = "User default (configured in control panel)";
			// 
			// radioEnglish
			// 
			this.radioEnglish.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioEnglish.Location = new System.Drawing.Point(8, 40);
			this.radioEnglish.Name = "radioEnglish";
			this.radioEnglish.Size = new System.Drawing.Size(248, 24);
			this.radioEnglish.TabIndex = 2;
			this.radioEnglish.Text = "English (United States)";
			// 
			// radioOther
			// 
			this.radioOther.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioOther.Location = new System.Drawing.Point(8, 64);
			this.radioOther.Name = "radioOther";
			this.radioOther.Size = new System.Drawing.Size(64, 24);
			this.radioOther.TabIndex = 2;
			this.radioOther.Text = "Other:";
			this.radioOther.CheckedChanged += new System.EventHandler(this.radioOther_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioScientific);
			this.groupBox2.Controls.Add(this.radioFixedPoint);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(8, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(264, 72);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Number style";
			// 
			// radioScientific
			// 
			this.radioScientific.Checked = true;
			this.radioScientific.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioScientific.Location = new System.Drawing.Point(8, 16);
			this.radioScientific.Name = "radioScientific";
			this.radioScientific.Size = new System.Drawing.Size(248, 24);
			this.radioScientific.TabIndex = 0;
			this.radioScientific.TabStop = true;
			this.radioScientific.Text = "Scientific (1.2134123e+001)";
			// 
			// radioFixedPoint
			// 
			this.radioFixedPoint.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioFixedPoint.Location = new System.Drawing.Point(8, 40);
			this.radioFixedPoint.Name = "radioFixedPoint";
			this.radioFixedPoint.Size = new System.Drawing.Size(248, 24);
			this.radioFixedPoint.TabIndex = 0;
			this.radioFixedPoint.Text = "Fixed-point (12.134123)";
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Location = new System.Drawing.Point(8, 218);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Number separator:";
			// 
			// comboSeparator
			// 
			this.comboSeparator.Items.AddRange(new object[] {
																												"tab",
																												"semi-colon",
																												"comma",
																												"space"});
			this.comboSeparator.Location = new System.Drawing.Point(112, 216);
			this.comboSeparator.Name = "comboSeparator";
			this.comboSeparator.Size = new System.Drawing.Size(88, 21);
			this.comboSeparator.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Location = new System.Drawing.Point(8, 194);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Number of decimals:";
			// 
			// numericDecimals
			// 
			this.numericDecimals.Location = new System.Drawing.Point(112, 192);
			this.numericDecimals.Maximum = new System.Decimal(new int[] {
																																		16,
																																		0,
																																		0,
																																		0});
			this.numericDecimals.Name = "numericDecimals";
			this.numericDecimals.Size = new System.Drawing.Size(40, 20);
			this.numericDecimals.TabIndex = 7;
			this.numericDecimals.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericDecimals.Value = new System.Decimal(new int[] {
																																	8,
																																	0,
																																	0,
																																	0});
			// 
			// FormOutput
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(282, 280);
			this.Controls.Add(this.numericDecimals);
			this.Controls.Add(this.comboSeparator);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormOutput";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Text output settings";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericDecimals)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	
		private void radioOther_CheckedChanged(object sender, System.EventArgs e)
		{
			this.comboCulture.Enabled = (this.radioOther.Checked);
		}
	
		public NumberFormatInfo Culture 
		{
			get 
			{
				if (this.radioDefault.Checked) 
					return NumberFormatInfo.CurrentInfo;
				else if (this.radioEnglish.Checked)
					return new CultureInfo("en-US", false).NumberFormat;
				else
					return ((CultureInfo)(this.comboCulture.SelectedItem)).NumberFormat;
			}
		}

		public string Format 
		{
			get 
			{
				string strFormat;

				// Get the number style.
				if (this.radioFixedPoint.Checked) 
					strFormat = "f";
				else
					strFormat = "e";

				// Get the number of decimals
				strFormat += this.numericDecimals.Value;

				return strFormat;
			}
		}

		public string Separator 
		{
			get
			{
				switch (this.comboSeparator.Text) 
				{
					case "tab":
						return "\t";
					case "semi-colon":
						return ";";
					case "comma":
						return ",";
					case "space":
						return " ";
					default:
						return this.comboSeparator.Text;
				}
			}
		}
	}
}
