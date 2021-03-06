using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace PlotReader
{
	public interface IThumbnailOwner 
	{
		void OnPaintThumbnail(System.Drawing.Graphics g);
		event EventHandler ImageChanged, ShownAreaChanged;
		SizeF CurrentSize {get;}
	}

	/// <summary>
	/// Summary description for ThumnailBox.
	/// </summary>
	public class ThumnailBox : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ThumnailBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.SetStyle(ControlStyles.DoubleBuffer | 
				ControlStyles.UserPaint | 
				ControlStyles.AllPaintingInWmPaint,
				true);
			this.UpdateStyles();

			// TODO: Add any initialization after the InitializeComponent call

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// ThumnailBox
			// 
			this.Name = "ThumnailBox";
			this.Resize += new System.EventHandler(this.ThumnailBox_Resize);
			this.Enter += new System.EventHandler(this.ThumnailBox_Enter);
			this.Leave += new System.EventHandler(this.ThumnailBox_Leave);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ThumnailBox_MouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ThumnailBox_MouseDown);

		}
		#endregion
	
		protected ScrollableControl m_owner;
		public ScrollableControl Owner 
		{
			get 
			{
				return this.m_owner;
			}
			set 
			{
				if (this.m_owner!=null) 
				{
					((IThumbnailOwner)(this.m_owner)).ImageChanged -= new EventHandler(this.OnOwnerImageChanged);
					((IThumbnailOwner)(this.m_owner)).ShownAreaChanged -= new EventHandler(this.OnOwnerShownAreaChanged);
				}
				this.m_owner = value;
				((IThumbnailOwner)(this.m_owner)).ImageChanged += new EventHandler(this.OnOwnerImageChanged);
				((IThumbnailOwner)(this.m_owner)).ShownAreaChanged += new EventHandler(this.OnOwnerShownAreaChanged);
				this.OnOwnerImageChanged(this, null);
			}
		}
	
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			//base.OnPaint (e);
			if (this.m_owner != null) 
			{
				e.Graphics.ResetTransform();

				if (!this.m_caret.IsEmpty && this.m_bmpOwner!=null) 
				{
					// Draw thumbnail image.
					e.Graphics.TranslateTransform(this.m_offset.X, this.m_offset.Y, System.Drawing.Drawing2D.MatrixOrder.Append);
					e.Graphics.DrawImage(this.m_bmpOwner, 0, 0);

					// Shade region outside caret.
					RectangleF rec = new RectangleF(0, 0, this.m_bmpOwner.Width, this.m_bmpOwner.Height);
					Region reg = new Region(rec);
					reg.Exclude(this.m_caret);
					Color col = Color.FromArgb(64,this.m_owner.BackColor.R,this.m_owner.BackColor.G,this.m_owner.BackColor.B);
					e.Graphics.FillRegion(new System.Drawing.SolidBrush(col), reg);

					// Draw caret rectangle.
					Pen pen = new Pen(this.Focused ? Brushes.Red : Brushes.DarkRed, 2);
					e.Graphics.DrawRectangle(pen, this.m_caret.Left, this.m_caret.Top, this.m_caret.Width, this.m_caret.Height);

					this.Cursor = Cursors.Hand;
				}
				else 
				{
					this.Cursor = Cursors.Default;
				}
			}
		}

		protected Bitmap m_bmpOwner;
		public void OnOwnerImageChanged(object sender, System.EventArgs e) 
		{
			if (!((IThumbnailOwner)(this.m_owner)).CurrentSize.IsEmpty) 
			{
				// Get size of thumbnail area, owner size.
				Size sizeThumbArea = new Size(this.ClientSize.Width-4, this.ClientSize.Height-4);
				SizeF sizeOwner = ((IThumbnailOwner)(this.m_owner)).CurrentSize;

				// Find scaling factor (original to thumbnail).
				float flHScale = ((float)(sizeThumbArea.Width))/sizeOwner.Width;
				float flVScale = ((float)(sizeThumbArea.Height))/sizeOwner.Height;
				float flScale = Math.Min(flHScale, flVScale);

				// Get size of thumbnail image.
				SizeF sizeThumbImage = new SizeF(flScale*sizeOwner.Width, flScale*sizeOwner.Height);

				// Find offset to ensure image is centered in thumbnail control.
				float hoffset = (this.ClientSize.Width -sizeThumbImage.Width )/2F;
				float voffset = (this.ClientSize.Height-sizeThumbImage.Height)/2F;
				this.m_offset = new PointF(hoffset, voffset);

				// Get size of thumbnail image, rounded to integers.
				Size sizeIntThumbImage = Size.Round(sizeThumbImage);

				// Get offscreen bitmap for thumbnail.
				if (this.m_bmpOwner==null || this.m_bmpOwner.Size!=sizeIntThumbImage) 
				{
					// Bitmap didn't exists or was wrong size. Create a new bitmap.
					this.m_bmpOwner = new Bitmap(sizeIntThumbImage.Width, sizeIntThumbImage.Height);
				}

				// Create drawing canvas for in-memory bitmap.
				Graphics g = Graphics.FromImage(this.m_bmpOwner);

				// Scale owner size to bitmap size.
				g.ScaleTransform(flScale, flScale, System.Drawing.Drawing2D.MatrixOrder.Append);

				// Set high-quality drawing options for thumbnail.
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

				// Let owner draw image.
				((IThumbnailOwner)(this.m_owner)).OnPaintThumbnail(g);
			}
			else 
			{
				// Owner has size (0,0), therefore we do not have a thumbnail bitmap.
				this.m_bmpOwner = null;
			}

			// Update caret area.
			this.OnOwnerShownAreaChanged(this, null);

			// Invalidate thumbnail area.
			this.Invalidate();

			this.Update();
		}

		protected PointF m_offset;
		protected RectangleF m_caret;
		public void OnOwnerShownAreaChanged(object sender, System.EventArgs e) 
		{
			RectangleF caret;
			if (!((IThumbnailOwner)(this.m_owner)).CurrentSize.IsEmpty && this.m_bmpOwner!=null) 
			{
				// Find caret rectangle in thumbnail coordinates.
				float flScale = ((float)(this.m_bmpOwner.Width))/((IThumbnailOwner)(this.m_owner)).CurrentSize.Width;
				caret = new RectangleF(-flScale*this.m_owner.AutoScrollPosition.X, -flScale*this.m_owner.AutoScrollPosition.Y, flScale*this.m_owner.ClientSize.Width, flScale*this.m_owner.ClientSize.Height);
				caret.Intersect(new RectangleF(new PointF(0, 0), this.m_bmpOwner.Size));
			}
			else caret = new RectangleF();

			// If caret changed, invalidate thumbnail area.
			if (caret!=this.m_caret) 
			{
				this.m_caret = caret;
				this.Invalidate();
				this.Update();
			}
		}

		private PointF m_ptRelDragStart;
		private void ThumnailBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// If (1) the owner cannot scroll, (2) the owner's thumbnail image is
			// empty, or (3) the owner's current size is (0,0), do nothing.
			if (!this.m_owner.AutoScroll || this.m_bmpOwner==null || ((IThumbnailOwner)(this.m_owner)).CurrentSize.IsEmpty) return;
			
			// Get the cursor position in thumbnail coordinates.
			PointF ptThumbnail = new PointF(e.X-this.m_offset.X, e.Y-this.m_offset.Y);

			// If outside the thumbnail image, do nothing.
			//RectangleF recThumb = new RectangleF(new Point(0,0), this.m_bmpOwner.Size);
			//if (!recThumb.Contains(ptThumbnail)) return;

			// Check whether button was pressed inside or outside the caret.
			if (this.m_caret.Contains(ptThumbnail))
			{
				// Button was pressed inside caret; user want to move caret.
				// Calculate ans save distance between cursor and center of caret.
				this.m_ptRelDragStart = new PointF(ptThumbnail.X-(this.m_caret.Left+this.m_caret.Width/2F), ptThumbnail.Y-(this.m_caret.Top+this.m_caret.Height/2F));
			}
			else 
			{
				// Button was pressed outside caret; user wants instant other caret position.
				this.m_ptRelDragStart = new PointF(0, 0);
				this.CenterOn(ptThumbnail);
			}
		}

		private void ThumnailBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// If no mouse button is pressed, do nothing.
			if (e.Button==MouseButtons.None) return;

			// If (1) the owner cannot scroll, (2) the owner's thumbnail image is
			// empty, or (3) the owner's current size is (0,0), do nothing.
			if (!this.m_owner.AutoScroll || this.m_bmpOwner==null || ((IThumbnailOwner)(this.m_owner)).CurrentSize.IsEmpty) return;

			// Get the cursor position in thumbnail coordinates.
			PointF ptThumbnail = new PointF(e.X-this.m_offset.X, e.Y-this.m_offset.Y);
			
			// Correct targeted center position if caret-dragging started from within caret.
			ptThumbnail.X -= this.m_ptRelDragStart.X;
			ptThumbnail.Y -= this.m_ptRelDragStart.Y;

			// Center on coordinate.
			this.CenterOn(ptThumbnail);
		}

		private void CenterOn(PointF ptThumbnail) 
		{
			// Convert thumbnail coordinate into relative (0-1) coordinate.
			PointF ptRel = new PointF(ptThumbnail.X/this.m_bmpOwner.Width, ptThumbnail.Y/this.m_bmpOwner.Height);

			// Convert relative center position to top-left owner coordinate.
			int iNewX = (int)(ptRel.X*((IThumbnailOwner)(this.m_owner)).CurrentSize.Width-this.m_owner.ClientSize.Width/2F);
			int iNewY = (int)(ptRel.Y*((IThumbnailOwner)(this.m_owner)).CurrentSize.Height-this.m_owner.ClientSize.Height/2F);

			// Scroll the owner.
			this.m_owner.AutoScrollPosition = new Point(iNewX, iNewY);
		}

		private void ThumnailBox_Resize(object sender, System.EventArgs e)
		{
			// Resize changes the thumbnail size, thus invalidating the in-memory
			// bitmap of the owner (which wouldn't fit anymore), the offset, and
			// caret. Update all.
			this.OnOwnerImageChanged(this, null);
		}

		private void ThumnailBox_Enter(object sender, System.EventArgs e)
		{
			// We obtained focus; therefore caret should be drawn differently.
			this.Invalidate();
		}

		private void ThumnailBox_Leave(object sender, System.EventArgs e)
		{
			// We lost focus; therefore caret should be drawn differently.
			this.Invalidate();
		}
	}
}
