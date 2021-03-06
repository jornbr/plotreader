using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PlotReader
{
	/// <summary>
	/// Summary description for FormColors.
	/// </summary>
	public class FormColors : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelMarker;
		private System.Windows.Forms.Label labelSelectedMarker;
		private System.Windows.Forms.Label labelXAxis;
		private System.Windows.Forms.Label labelYAxis;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelMarkerColor;
		private System.Windows.Forms.Label labelSelectedMarkerColor;
		private System.Windows.Forms.Label labelXAxisColor;
		private System.Windows.Forms.Label labelYAxisColor;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.Button buttonReset;
		private System.Windows.Forms.Label labelTitle;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormColors()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.m_arrDefaultColors = new Color[4];
			this.m_arrColors = new Color[4];
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
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
			this.labelMarker = new System.Windows.Forms.Label();
			this.labelSelectedMarker = new System.Windows.Forms.Label();
			this.labelXAxis = new System.Windows.Forms.Label();
			this.labelYAxis = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelMarkerColor = new System.Windows.Forms.Label();
			this.labelSelectedMarkerColor = new System.Windows.Forms.Label();
			this.labelXAxisColor = new System.Windows.Forms.Label();
			this.labelYAxisColor = new System.Windows.Forms.Label();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.buttonReset = new System.Windows.Forms.Button();
			this.labelTitle = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelMarker
			// 
			this.labelMarker.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelMarker.Location = new System.Drawing.Point(8, 32);
			this.labelMarker.Name = "labelMarker";
			this.labelMarker.Size = new System.Drawing.Size(48, 16);
			this.labelMarker.TabIndex = 0;
			this.labelMarker.Text = "Marker";
			// 
			// labelSelectedMarker
			// 
			this.labelSelectedMarker.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelSelectedMarker.Location = new System.Drawing.Point(8, 56);
			this.labelSelectedMarker.Name = "labelSelectedMarker";
			this.labelSelectedMarker.Size = new System.Drawing.Size(88, 16);
			this.labelSelectedMarker.TabIndex = 0;
			this.labelSelectedMarker.Text = "Selected marker";
			// 
			// labelXAxis
			// 
			this.labelXAxis.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelXAxis.Location = new System.Drawing.Point(8, 80);
			this.labelXAxis.Name = "labelXAxis";
			this.labelXAxis.Size = new System.Drawing.Size(40, 16);
			this.labelXAxis.TabIndex = 1;
			this.labelXAxis.Text = "X-axis";
			// 
			// labelYAxis
			// 
			this.labelYAxis.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelYAxis.Location = new System.Drawing.Point(8, 104);
			this.labelYAxis.Name = "labelYAxis";
			this.labelYAxis.Size = new System.Drawing.Size(40, 16);
			this.labelYAxis.TabIndex = 1;
			this.labelYAxis.Text = "Y-axis";
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(104, 130);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(184, 130);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			// 
			// labelMarkerColor
			// 
			this.labelMarkerColor.BackColor = System.Drawing.Color.Red;
			this.labelMarkerColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelMarkerColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelMarkerColor.Location = new System.Drawing.Point(104, 32);
			this.labelMarkerColor.Name = "labelMarkerColor";
			this.labelMarkerColor.Size = new System.Drawing.Size(16, 16);
			this.labelMarkerColor.TabIndex = 3;
			this.labelMarkerColor.Click += new System.EventHandler(this.labelColor_Click);
			// 
			// labelSelectedMarkerColor
			// 
			this.labelSelectedMarkerColor.BackColor = System.Drawing.Color.Orange;
			this.labelSelectedMarkerColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelSelectedMarkerColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelSelectedMarkerColor.Location = new System.Drawing.Point(104, 56);
			this.labelSelectedMarkerColor.Name = "labelSelectedMarkerColor";
			this.labelSelectedMarkerColor.Size = new System.Drawing.Size(16, 16);
			this.labelSelectedMarkerColor.TabIndex = 3;
			this.labelSelectedMarkerColor.Click += new System.EventHandler(this.labelColor_Click);
			// 
			// labelXAxisColor
			// 
			this.labelXAxisColor.BackColor = System.Drawing.Color.Green;
			this.labelXAxisColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelXAxisColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelXAxisColor.Location = new System.Drawing.Point(104, 80);
			this.labelXAxisColor.Name = "labelXAxisColor";
			this.labelXAxisColor.Size = new System.Drawing.Size(16, 16);
			this.labelXAxisColor.TabIndex = 3;
			this.labelXAxisColor.Click += new System.EventHandler(this.labelColor_Click);
			// 
			// labelYAxisColor
			// 
			this.labelYAxisColor.BackColor = System.Drawing.Color.Blue;
			this.labelYAxisColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelYAxisColor.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelYAxisColor.Location = new System.Drawing.Point(104, 104);
			this.labelYAxisColor.Name = "labelYAxisColor";
			this.labelYAxisColor.Size = new System.Drawing.Size(16, 16);
			this.labelYAxisColor.TabIndex = 3;
			this.labelYAxisColor.Click += new System.EventHandler(this.labelColor_Click);
			// 
			// buttonReset
			// 
			this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonReset.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonReset.Location = new System.Drawing.Point(8, 130);
			this.buttonReset.Name = "buttonReset";
			this.buttonReset.Size = new System.Drawing.Size(88, 23);
			this.buttonReset.TabIndex = 2;
			this.buttonReset.Text = "&Reset defaults";
			this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
			// 
			// labelTitle
			// 
			this.labelTitle.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelTitle.Location = new System.Drawing.Point(8, 8);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(200, 16);
			this.labelTitle.TabIndex = 4;
			this.labelTitle.Text = "Click a color to change it:";
			// 
			// FormColors
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(264, 160);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.labelMarkerColor);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.labelXAxis);
			this.Controls.Add(this.labelMarker);
			this.Controls.Add(this.labelSelectedMarker);
			this.Controls.Add(this.labelYAxis);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.labelSelectedMarkerColor);
			this.Controls.Add(this.labelXAxisColor);
			this.Controls.Add(this.labelYAxisColor);
			this.Controls.Add(this.buttonReset);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormColors";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Pick colors";
			this.ResumeLayout(false);

		}
		#endregion

		private void labelColor_Click(object sender, System.EventArgs e)
		{
			Label label = (Label)sender;
			this.colorDialog1.Color = label.BackColor;
			if (this.colorDialog1.ShowDialog(this) == DialogResult.OK) 
			{
				label.BackColor = this.colorDialog1.Color;
			}
		}

		private Color [] m_arrDefaultColors;
		private Color [] m_arrColors;
		public Color [] Colors 
		{
			get 
			{
				return m_arrColors;
			}
			set 
			{
				m_arrColors = value;
			}
		}
		public Color [] DefaultColors 
		{
			get 
			{
				return m_arrDefaultColors;
			}
			set 
			{
				m_arrDefaultColors = value;
			}
		}

		private void buttonReset_Click(object sender, System.EventArgs e)
		{
			this.labelMarkerColor.BackColor = this.m_arrDefaultColors[0];
			this.labelSelectedMarkerColor.BackColor = this.m_arrDefaultColors[1];
			this.labelXAxisColor.BackColor = this.m_arrDefaultColors[2];
			this.labelYAxisColor.BackColor = this.m_arrDefaultColors[3];
		}
	
		protected override void OnLoad(EventArgs e)
		{
			this.labelMarkerColor.BackColor = this.m_arrColors[0];
			this.labelSelectedMarkerColor.BackColor = this.m_arrColors[1];
			this.labelXAxisColor.BackColor = this.m_arrColors[2];
			this.labelYAxisColor.BackColor = this.m_arrColors[3];
			base.OnLoad (e);
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			this.m_arrColors[0] = this.labelMarkerColor.BackColor;
			this.m_arrColors[1] = this.labelSelectedMarkerColor.BackColor;
			this.m_arrColors[2] = this.labelXAxisColor.BackColor;
			this.m_arrColors[3] = this.labelYAxisColor.BackColor;
		}
	}
}
