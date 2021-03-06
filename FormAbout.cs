using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PlotReader
{
	/// <summary>
	/// Summary description for FormAbout.
	/// </summary>
	public class FormAbout : System.Windows.Forms.Form
    {
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Label labelCopyright;
		private System.Windows.Forms.Label labelRestrictedUse;
		private System.Windows.Forms.Label labelURL;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormAbout()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.labelVersion.Text = "PlotReader "+this.ProductVersion;
			this.labelURL.Text = Info.URL;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
            this.labelVersion = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelRestrictedUse = new System.Windows.Forms.Label();
            this.labelURL = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelVersion
            // 
            this.labelVersion.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelVersion.Location = new System.Drawing.Point(8, 8);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(264, 16);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "PlotReader";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonOK.Location = new System.Drawing.Point(304, 69);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            // 
            // labelCopyright
            // 
            this.labelCopyright.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelCopyright.Location = new System.Drawing.Point(8, 24);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(264, 16);
            this.labelCopyright.TabIndex = 0;
            this.labelCopyright.Text = "Copyright © 2004-2021, Jorn Bruggeman";
            // 
            // labelRestrictedUse
            // 
            this.labelRestrictedUse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelRestrictedUse.Location = new System.Drawing.Point(8, 54);
            this.labelRestrictedUse.Name = "labelRestrictedUse";
            this.labelRestrictedUse.Size = new System.Drawing.Size(368, 20);
            this.labelRestrictedUse.TabIndex = 3;
            this.labelRestrictedUse.Text = "Homepage:";
            // 
            // labelURL
            // 
            this.labelURL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelURL.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelURL.ForeColor = System.Drawing.Color.Blue;
            this.labelURL.Location = new System.Drawing.Point(8, 70);
            this.labelURL.Name = "labelURL";
            this.labelURL.Size = new System.Drawing.Size(200, 58);
            this.labelURL.TabIndex = 4;
            this.labelURL.Text = "http://www.xs4all.nl/~jornbr/plotreader/";
            this.labelURL.Click += new System.EventHandler(this.labelURL_Click);
            // 
            // FormAbout
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(386, 101);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelURL);
            this.Controls.Add(this.labelRestrictedUse);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelCopyright);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAbout";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About PlotReader";
            this.ResumeLayout(false);

		}
		#endregion

		private void labelURL_Click(object sender, System.EventArgs e)
		{
			System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(Info.URL);
			psi.ErrorDialog = true;
			psi.ErrorDialogParentHandle = this.Handle;
			try 
			{
				System.Diagnostics.Process.Start(psi);
			}
			catch {}
		}
	}
}
