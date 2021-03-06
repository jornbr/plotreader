using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace PlotReader
{
    /// <summary>
    /// Summary description for PlotBox.
    /// </summary>
    public class PlotBox : System.Windows.Forms.UserControl, PlotReader.IThumbnailOwner
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        protected class AxisPoint
        {
            public bool HasImageCoordinate, HasValue;
            public PointF Point;
            public double Value, NonLogValue;
            public AxisPoint()
            {
                HasImageCoordinate = false;
                HasValue = false;
            }
        }
        protected AxisPoint[,] m_arrAxesPoints;

        public enum PlotBoxAxisType
        {
            X, Y, None
        }

        public class AxisWarning
        {
            public PlotBoxAxisType Axis;
            public int PointNumber;
            public enum WarningType
            {
                CoordinateNotSet,
                CoordinatesIdentical,
                AxesParallel,
                ValueNotSet,
                ValuesIdentical,
                ValueNonPositive
            }
            public WarningType Warning;
            public AxisWarning(PlotBoxAxisType axis, int iNr, WarningType warning)
            {
                Axis = axis;
                PointNumber = iNr;
                Warning = warning;
            }
        }
        protected ArrayList m_listAxesWarnings;

        public ArrayList AxesWarnings
        {
            get
            {
                return m_listAxesWarnings;
            }
        }

        public PlotBox()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // No image.
            this.m_objImage = null;

            // Set styles.
            this.SetStyle(ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint,
                true);
            this.UpdateStyles();

            // No points (empty list)
            this.m_listPoints = new ArrayList();
            this.m_listPointSel = new ArrayList();

            // No warnings for axes (empty list)
            this.m_listAxesWarnings = new ArrayList();

            // Initialize image-screen transformation matrices (default to identity)
            this.m_matTransform = new System.Drawing.Drawing2D.Matrix();
            this.m_matTransformInverse = new System.Drawing.Drawing2D.Matrix();

            // Default display: 100 % zoom, 0 degrees rotation
            this.m_iZoom = 100;
            this.m_bZoomToFit = false;
            this.m_flAngle = 0;

            // Default marker
            this.m_iMarkerSize = 7;
            this.m_iMarkerType = PlotBox.PlotBoxMarkerType.Dot;

            // No axes
            this.m_arrAxesLogScale = new bool[2];
            this.ClearAxes();

            // Default response to click: none.
            this.m_MouseState = PlotBoxMouseState.Drag;

            // Allocate space for colors.
            this.m_arrColors = new Color[4];

            // Not being dragged by mouse.
            this.m_bMoving = false;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PlotBox
            // 
            this.AutoScroll = true;
            this.Cursor = System.Windows.Forms.Cursors.Cross;
            this.Name = "PlotBox";
            this.Size = new System.Drawing.Size(560, 432);
            this.MouseLeave += new System.EventHandler(this.PlotBox_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PlotBox_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PlotBox_MouseDown);
            this.Resize += new System.EventHandler(this.PlotBox_Resize);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PlotBox_MouseUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PlotBox_KeyDown);
            this.ResumeLayout(false);

        }
        #endregion

        protected void InvalidateMarkerScreen(Point pScreen)
        {
            int iHalf = (m_iMarkerSize - 1) / 2 + 1;
            this.Invalidate(new Rectangle(pScreen.X - iHalf, pScreen.Y - iHalf, m_iMarkerSize + 2, m_iMarkerSize + 2));
        }

        protected int m_iMarkerSize;
        public int MarkerSize
        {
            get
            {
                return m_iMarkerSize;
            }
            set
            {
                m_iMarkerSize = value;
                this.Invalidate();
            }
        }

        protected PlotBoxMarkerType m_iMarkerType;
        public PlotBoxMarkerType MarkerType
        {
            get
            {
                return m_iMarkerType;
            }
            set
            {
                m_iMarkerType = value;
                this.Invalidate();
            }
        }

        protected void InvalidateMarkerImage(PointF pImage)
        {
            this.InvalidateMarkerScreen(Point.Round(this.ImageToScreen(pImage)));
        }

        protected void InvalidateRectangeImage(RectangleF rectImage)
        {
            PointF topleft = this.ImageToScreen(rectImage.Left, rectImage.Top);
            PointF topright = this.ImageToScreen(rectImage.Right, rectImage.Top);
            PointF botleft = this.ImageToScreen(rectImage.Left, rectImage.Bottom);
            PointF botright = this.ImageToScreen(rectImage.Right, rectImage.Bottom);
            int minX = (int)Math.Floor(Math.Min(Math.Min(topleft.X, topright.X), Math.Min(botleft.X, botright.X)));
            int maxX = (int)Math.Ceiling(Math.Max(Math.Max(topleft.X, topright.X), Math.Max(botleft.X, botright.X)));
            int minY = (int)Math.Floor(Math.Min(Math.Min(topleft.Y, topright.Y), Math.Min(botleft.Y, botright.Y)));
            int maxY = (int)Math.Ceiling(Math.Max(Math.Max(topleft.Y, topright.Y), Math.Max(botleft.Y, botright.Y)));
            this.Invalidate(new Rectangle(minX, minY, maxX - minX, maxY - minY));
        }

        protected void DrawMarkerScreen(Graphics g, Point pScreen, Color color, bool bSelected)
        {
            int iHalf = (this.m_iMarkerSize - 1) / 2;
            switch (this.m_iMarkerType)
            {
                case PlotBox.PlotBoxMarkerType.Cross:
                    Pen pen2 = new Pen(color, 1);
                    g.DrawLine(pen2, pScreen.X, pScreen.Y - iHalf, pScreen.X, pScreen.Y + iHalf);
                    g.DrawLine(pen2, pScreen.X - iHalf, pScreen.Y, pScreen.X + iHalf, pScreen.Y);
                    break;
                case PlotBox.PlotBoxMarkerType.FatCross:
                    Pen pen3 = new Pen(color, 2);
                    g.DrawLine(pen3, pScreen.X, pScreen.Y - iHalf, pScreen.X, pScreen.Y + iHalf);
                    g.DrawLine(pen3, pScreen.X - iHalf, pScreen.Y, pScreen.X + iHalf, pScreen.Y);
                    break;
                default:
                    // Dot (default)
                    g.FillEllipse(new SolidBrush(color), pScreen.X - iHalf, pScreen.Y - iHalf, m_iMarkerSize, m_iMarkerSize);
                    g.DrawEllipse(Pens.Black, pScreen.X - iHalf, pScreen.Y - iHalf, m_iMarkerSize, m_iMarkerSize);
                    break;
            }
        }

        protected void DrawMarkerImage(Graphics g, PointF pImage, Color color, bool bSelected)
        {
            this.DrawMarkerScreen(g, Point.Round(this.ImageToScreen(pImage)), color, bSelected);
        }

        protected System.Drawing.Drawing2D.Matrix m_matTransform, m_matTransformInverse;
        protected float m_flAngle;
        protected void UpdateTransform(bool bImageChanged)
        {
            // Save old transformation matrix
            Matrix matOld = this.m_matTransform.Clone();

            // Reset transformation matrix to identity matrix.
            this.m_matTransform.Reset();

            // Variables for x- and y-range.
            float width = 0, height = 0;

            if (this.m_objImage != null)
            {
                width = this.m_objImage.Width;
                height = this.m_objImage.Height;

                // Check if we need to do rotation.
                if (this.m_flAngle != 0F)
                {
                    // Apply requested rotation to transformation matrix.
                    this.m_matTransform.Rotate(this.m_flAngle, MatrixOrder.Append);

                    // Find the transformed position of each image corner.
                    PointF[] corners = {new PointF(0,0),
											new PointF(this.m_objImage.Width,0),
											new PointF(0,this.m_objImage.Height),
											new PointF(this.m_objImage.Width,this.m_objImage.Height)};
                    this.m_matTransform.TransformPoints(corners);

                    // Get new x- and y-range from image corner positions.
                    float xmin = 0, xmax = 0, ymin = 0, ymax = 0;
                    foreach (PointF p in corners)
                    {
                        if (p.X < xmin) xmin = p.X;
                        if (p.X > xmax) xmax = p.X;
                        if (p.Y < ymin) ymin = p.Y;
                        if (p.Y > ymax) ymax = p.Y;
                    }
                    width = xmax - xmin;
                    height = ymax - ymin;

                    // Shift to ensure (0,0) lies at the minimum x and y.
                    this.m_matTransform.Translate(-xmin, -ymin, MatrixOrder.Append);
                }

                // Apply requested scaling to transformation matrix.
                float flZoom = ((float)this.m_iZoom) / 100.0F;
                if (this.m_bZoomToFit)
                {
                    float xscale = (float)(this.ClientSize.Width) / width;
                    float yscale = (float)(this.ClientSize.Height) / height;
                    if (xscale > 0 && yscale > 0)
                    {
                        flZoom = Math.Min(xscale, yscale);
                        this.m_iZoom = (int)Math.Round(100 * flZoom);
                    }
                }
                this.m_matTransform.Scale(flZoom, flZoom, MatrixOrder.Append);
                width *= flZoom;
                height *= flZoom;
            }

            if (!this.m_matTransform.Equals(matOld))
            {
                // Get inverse tranformation matrix (for screen-to-image)
                this.m_matTransformInverse = this.m_matTransform.Clone();
                this.m_matTransformInverse.Invert();
            }

            if (!this.m_matTransform.Equals(matOld) || bImageChanged)
            {
                // Use x- and y-range to set the scrollable area.
                this.m_sizeCurrent = new SizeF(width, height);
                if (!this.m_bZoomToFit) this.AutoScrollMinSize = Size.Round(this.m_sizeCurrent);

                // Update display
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.m_objImage != null)
            {
                // Anti-alias edges of image, and vector drawings.
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw image
                e.Graphics.PageUnit = GraphicsUnit.Pixel;
                e.Graphics.Transform = this.m_matTransform.Clone();
                e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y, System.Drawing.Drawing2D.MatrixOrder.Append);
                Rectangle rectImage = new Rectangle(0, 0, m_objImage.Width, m_objImage.Height);
                e.Graphics.DrawImage(this.m_objImage, rectImage, rectImage, GraphicsUnit.Pixel);
                if (this.m_bmOverlay != null) e.Graphics.DrawImage(this.m_bmOverlay.bitmap, this.m_bmOverlay.offset);
                e.Graphics.ResetTransform();

                if (this.ShownAreaChanged != null) this.ShownAreaChanged(this, null);

                // Plot axes.
                if (this.AxesState > 0)
                {
                    Pen objPenX = new Pen(this.m_arrColors[2], 1);
                    Pen objPenY = new Pen(this.m_arrColors[3], 1);
                    this.DrawAxis(e.Graphics, objPenX, 0);
                    this.DrawAxis(e.Graphics, objPenY, 1);
                }

                // Disable anti-aliasing for markers.
                e.Graphics.SmoothingMode = SmoothingMode.None;

                // Plot specified x-axis points
                if (this.m_arrAxesPoints[0, 0].HasImageCoordinate) this.DrawMarkerImage(e.Graphics, this.m_arrAxesPoints[0, 0].Point, this.m_arrColors[2], false);
                if (this.m_arrAxesPoints[0, 1].HasImageCoordinate) this.DrawMarkerImage(e.Graphics, this.m_arrAxesPoints[0, 1].Point, this.m_arrColors[2], false);

                // Plot specified y-axis points
                if (this.m_arrAxesPoints[1, 0].HasImageCoordinate) this.DrawMarkerImage(e.Graphics, this.m_arrAxesPoints[1, 0].Point, this.m_arrColors[3], false);
                if (this.m_arrAxesPoints[1, 1].HasImageCoordinate) this.DrawMarkerImage(e.Graphics, this.m_arrAxesPoints[1, 1].Point, this.m_arrColors[3], false);

                // Plot data points.
                int iPoint = 0;
                foreach (PointF point in this.m_listPoints)
                {
                    if ((bool)(this.m_listPointSel[iPoint]))
                    {
                        this.DrawMarkerImage(e.Graphics, point, this.m_arrColors[1], true);
                    }
                    else
                    {
                        this.DrawMarkerImage(e.Graphics, point, this.m_arrColors[0], false);
                    }
                    iPoint++;
                }
                if (!this.m_pointTemp.IsEmpty) this.DrawMarkerImage(e.Graphics, this.m_pointTemp, this.m_arrColors[0], false);

                if (this.m_bCaretSelect)
                {
                    Pen p = new Pen(Brushes.White, 1);
                    p.DashPattern = new float[] { 4F, 4F };
                    RectangleF caret = this.GetScreenCaret();
                    e.Graphics.DrawRectangle(Pens.Black, caret.Left, caret.Top, caret.Width, caret.Height);
                    e.Graphics.DrawRectangle(p, caret.Left, caret.Top, caret.Width, caret.Height);
                }
            }
        }

        protected Image m_objImage;
        public Image Image
        {
            get
            {
                return m_objImage;
            }
            set
            {
                this.m_objImage = value;

                // Reset the axes.
                this.ClearAxes();

                // Remove any exiting magic wand object attached to the image.
                this.m_magicWand = null;

                // Remove all current points.
                this.m_listPoints.Clear();
                this.m_listPointSel.Clear();

                // Zoom at 100 %, rotation at 0.
                this.m_iZoom = 100;
                this.m_flAngle = 0F;
                this.m_bZoomToFit = false;
                this.AutoScroll = true;

                // Update the transformation matrix.
                this.UpdateTransform(true);

                // Scroll position at (0,0)
                this.AutoScrollPosition = new Point(0, 0);

                // If we have any thumbnail control coupled, inform it.
                if (this.ImageChanged != null) this.ImageChanged(this, null);
            }
        }

        public void ClearPoints()
        {
            this.m_listPoints.Clear();
            this.m_listPointSel.Clear();
            this.Invalidate();
        }

        protected int m_iZoom;

        protected ArrayList m_listPoints, m_listPointSel;
        public ArrayList Points
        {
            get
            {
                return m_listPoints;
            }
        }

        private void PlotBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.m_bMoving)
            {
                this.m_bMoving = false;
            }
            if (this.m_bCaretSelect && e.Button == MouseButtons.Left)
            {
                this.m_bCaretSelect = false;
                InvalidateCaret();
                RectangleF car = this.GetScreenCaret();
                int i = 0;
                bool bSelectionChanged = false;
                foreach (PointF p in this.m_listPoints)
                {
                    PointF sc = this.ImageToScreen(p);
                    if (car.Contains(sc))
                    {
                        this.InvalidateMarkerImage(p);
                        this.m_listPointSel[i] = !((bool)(this.m_listPointSel[i]));
                        bSelectionChanged = true;
                    }
                    i++;
                }
                if (bSelectionChanged && this.SelectionChanged != null) this.SelectionChanged(this, null);
            }
            if (!this.m_pointTemp.IsEmpty && e.Button == MouseButtons.Left)
            {
                // Add point (not selected by default)
                this.m_listPoints.Add(this.m_pointTemp);
                this.m_listPointSel.Add(false);

                // Tell others interested we have a new point.
                if (this.PointAdded != null) this.PointAdded(this, null);

                // Reset temporary point.
                this.m_pointTemp = new PointF();
            }
            if (this.m_bmOverlay != null && e.Button == MouseButtons.Left)
            {
                Rectangle rectOverlay = new Rectangle(this.m_bmOverlay.offset, this.m_bmOverlay.bitmap.Size);
                this.m_bmOverlay = null;
                this.InvalidateRectangeImage(rectOverlay);
            }
            if (this.StatusTextChanged != null) this.StatusTextChanged(this, new EventArgs());
        }

        protected RectangleF GetScreenCaret()
        {
            PointF[] p = new PointF[] { this.ImageToScreen(this.m_pointCaretStart), this.ImageToScreen(this.m_pointCaretStop) };
            PointF topleft = new PointF(Math.Min(p[0].X, p[1].X), Math.Min(p[0].Y, p[1].Y));
            SizeF size = new SizeF(Math.Abs(p[0].X - p[1].X), Math.Abs(p[0].Y - p[1].Y));
            return new RectangleF(topleft, size);
        }

        protected void InvalidateCaret()
        {
            Rectangle car = Rectangle.Round(this.GetScreenCaret());
            car.Inflate(4, 4);
            car.Offset(-2, -2);
            this.Invalidate(car);
        }

        public event EventHandler PointAdded;
        public event EventHandler StatusTextChanged;
        public event EventHandler SelectionChanged;
        public event EventHandler ZoomChanged;
        public event EventHandler AxisCoordinateSet;

        protected bool m_bMoving;
        protected Point m_pointMoveStart, m_pointScrollPosMoveStart;
        protected AbstractMagicWand.Selection.Overlay m_bmOverlay = null;
        protected PointF m_pointTemp;

        AbstractMagicWand m_magicWand;
        private void PlotBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Find the click position in image coordinates.
            PointF pointImage = this.ScreenToImage(e.X, e.Y);

            if (this.m_iExpCoordinateAxis != 0 && this.m_iExpCoordinateNr != 0 && e.Button == MouseButtons.Left)
            {
                // If we're not inside the image, do nothing.
                if (!this.IsPointInImage(pointImage)) return;

                // Get info on the point we should be dealing with.
                AxisPoint point = this.m_arrAxesPoints[this.m_iExpCoordinateAxis - 1, this.m_iExpCoordinateNr - 1];

                // If we had a previous point, invalidate it so its area will be redrawn.
                if (point.HasImageCoordinate) this.InvalidateMarkerImage(point.Point);

                // Store the new calibration point.
                point.Point = pointImage;
                point.HasImageCoordinate = true;

                // Invalidate the area of the new point, so it will be drawn.
                this.InvalidateMarkerImage(pointImage);

                // Stop waiting for new calibration point (we now got it).
                this.StopExpectAxisCoordinate();

                // Update the axes using our new calibration point.
                this.UpdateAxes();

                // Notify others we have a new axis coordinate
                if (this.AxisCoordinateSet != null) this.AxisCoordinateSet(this, new EventArgs());
            }
            else if ((this.m_MouseState == PlotBoxMouseState.Drag || e.Button == MouseButtons.Right) && !this.m_bZoomToFit)
            {
                this.m_pointMoveStart = new Point(e.X, e.Y);
                this.m_pointScrollPosMoveStart = new Point(-this.AutoScrollPosition.X, -this.AutoScrollPosition.Y);
                this.m_bMoving = true;
            }
            else if (this.m_MouseState == PlotBoxMouseState.Add && e.Button == MouseButtons.Left)
            {
                // If we're not inside the image, do nothing.
                if (!this.IsPointInImage(pointImage)) return;

                if (this.m_bUseMagicWand)
                {
                    if (this.m_magicWand == null) this.m_magicWand = new MagicWandPixel(this.Image);

                    // Use magic wand to select region of similar points.
                    AbstractMagicWand.Selection sel = this.m_magicWand.Select(Point.Round(pointImage), this.m_flMagicWandTolerance);

                    // Replace clicked point by center point in magic wand selected region.
                    pointImage = sel.GetMeanPoint();

                    // Retrieve overlay bitmap that shows magic wand selected region.
                    this.m_bmOverlay = sel.GetOverlay(this.m_arrColors[0], 127);
                    this.InvalidateRectangeImage(new Rectangle(this.m_bmOverlay.offset, this.m_bmOverlay.bitmap.Size));
                }

                this.m_pointTemp = pointImage;

                // Invalidate the area of the new point, so it will be drawn.
                this.InvalidateMarkerImage(pointImage);
            }
            else if (this.m_MouseState == PlotBoxMouseState.Select && e.Button == MouseButtons.Left)
            {
                int iPoint = 0;
                int iPointClicked = -1;
                int iHalf = (m_iMarkerSize - 1) / 2;

                // Find point to select.
                foreach (PointF p in this.m_listPoints)
                {
                    // Get the data point in image and screen coordinates.
                    PointF sc = this.ImageToScreen(p);

                    // Check if the current point has been clicked; stop enumerating if so.
                    if (e.X >= sc.X - iHalf && e.Y >= sc.Y - iHalf && e.X <= sc.X + iHalf && e.Y <= sc.Y + iHalf)
                    {
                        iPointClicked = iPoint;
                        break;
                    }

                    // Increase point index.
                    iPoint++;
                }

                // Update point selection
                iPoint = 0;
                bool bSelectionChanged = false;
                foreach (PointF p in this.m_listPoints)
                {
                    if (iPoint == iPointClicked)
                    {
                        // Point was clicked.
                        if ((Control.ModifierKeys & (Keys.Shift | Keys.Control)) != Keys.None)
                        {
                            // Control and/or shift were pressed.
                            this.m_listPointSel[iPoint] = !((bool)(this.m_listPointSel[iPoint]));
                            this.InvalidateMarkerImage(p);
                            bSelectionChanged = true;
                        }
                        else if (!((bool)(this.m_listPointSel[iPoint])))
                        {
                            // No control/shift keys were pressed, and point was previously unselected.
                            this.m_listPointSel[iPoint] = true;
                            this.InvalidateMarkerImage(p);
                            bSelectionChanged = true;
                        }
                    }
                    else
                    {
                        // Point was not clicked.
                        if (((bool)(this.m_listPointSel[iPoint])) && ((Control.ModifierKeys & Keys.Shift) == Keys.None)
                            && !(iPointClicked != -1 && (Control.ModifierKeys & Keys.Control) != Keys.None))
                        {
                            // Point was not clicked, and
                            // (1) shift was not pressed
                            // (2) control was (a) not pressed, or (b) was pressed without a point being clicked.
                            this.m_listPointSel[iPoint] = false;
                            this.InvalidateMarkerImage(p);
                            bSelectionChanged = true;
                        }
                    }
                    iPoint++;
                }

                // If the selection changed, tell those interested.
                if (bSelectionChanged && this.SelectionChanged != null) this.SelectionChanged(this, null);

                // If we did not find a point to select, start caret-selection mode.
                if (iPointClicked == -1)
                {
                    this.m_bCaretSelect = true;
                    this.m_pointCaretStart = this.ScreenToImage(e.X, e.Y);
                    this.m_pointCaretStop = this.m_pointCaretStart;
                }
            }
            if (this.StatusTextChanged != null) this.StatusTextChanged(this, new EventArgs());
        }

        public int Zoom
        {
            get
            {
                return this.m_iZoom;
            }
            set
            {
                if ((value > 0 && this.m_iZoom != value) || this.m_bZoomToFit)
                {
                    PointF p = this.ScreenToImage(this.ClientSize.Width / 2F, this.ClientSize.Height / 2F);
                    this.m_bZoomToFit = false;
                    this.AutoScroll = true;
                    this.m_iZoom = value;
                    this.UpdateTransform(false);
                    this.CenterOnPoint(p, false);
                }
            }
        }

        protected bool m_bZoomToFit;
        public bool ZoomToFit
        {
            get
            {
                return this.m_bZoomToFit;
            }
            set
            {
                if (this.m_bZoomToFit != value)
                {
                    this.m_bZoomToFit = value;
                    this.AutoScrollPosition = new Point(0, 0);
                    this.AutoScroll = (!this.m_bZoomToFit);
                    this.UpdateTransform(false);
                }
            }
        }

        public float Rotation
        {
            get
            {
                return this.m_flAngle;
            }
            set
            {
                if (this.m_flAngle != value)
                {
                    this.m_flAngle = value;
                    this.UpdateTransform(false);

                    if (this.m_bZoomToFit && this.ZoomChanged != null) this.ZoomChanged(this, null);

                    // If we have any thumbnail control coupled, inform it.
                    if (this.ImageChanged != null) this.ImageChanged(this, null);
                }
            }
        }

        public int SelectedPoint
        {
            get
            {
                ArrayList selpoints = this.SelectedPoints;
                if (selpoints.Count == 0) return -1;
                return (int)(selpoints[0]);
            }
            set
            {
                ArrayList selpoints = new ArrayList();
                if (value != -1) selpoints.Add(value);
                this.SelectedPoints = selpoints;
            }
        }

        public ArrayList SelectedPoints
        {
            get
            {
                ArrayList res = new ArrayList();
                int iPoint = 0;
                foreach (bool bSel in this.m_listPointSel)
                {
                    if (bSel) res.Add(iPoint);
                    iPoint++;
                }
                return res;
            }
            set
            {
                bool bSelectionVisible = false;
                for (int iPoint = 0; iPoint < this.m_listPointSel.Count; iPoint++)
                {
                    // Check whether the point should be selected.
                    bool bVal = false;
                    foreach (int iIndex in value)
                    {
                        if (iIndex == iPoint) bVal = true;
                    }

                    // Get the current point (in image coordinates).
                    PointF point = (PointF)this.m_listPoints[iPoint];

                    if (bVal)
                    {
                        // The point needs to be selected. If it is not yet
                        // selected, select it, and ensure it will be redrawn.
                        if (!(bool)(this.m_listPointSel[iPoint]))
                        {
                            this.m_listPointSel[iPoint] = true;
                            this.InvalidateMarkerImage(point);
                        }

                        if (this.IsVisible(point)) bSelectionVisible = true;
                    }
                    else
                    {
                        // The point should not be selected. If it is currently
                        // selected, deselect it, and ensure it will be redrawn.
                        if ((bool)(this.m_listPointSel[iPoint]))
                        {
                            this.m_listPointSel[iPoint] = false;
                            this.InvalidateMarkerImage((PointF)this.m_listPoints[iPoint]);
                        }
                    }
                }
                if (value.Count != 0 && !bSelectionVisible) this.CenterOnPoint((PointF)(this.m_listPoints[(int)(value[0])]), false);
            }
        }

        public enum PlotBoxMouseState
        {
            None, Drag, Select, Add
        }

        public void ShowPointAt(PointF pImage, PointF pScreen)
        {
            // Get the current screen position of the image point.
            PointF pscr = this.ImageToScreen(pImage);

            // Determine difference between current screen position, and
            // requested screen position.
            Point pOffset = Point.Round(new PointF(pScreen.X - pscr.X, pScreen.Y - pscr.Y));

            // Update scroll position.
            this.AutoScrollPosition = new Point(-this.AutoScrollPosition.X - pOffset.X, -this.AutoScrollPosition.Y - pOffset.Y);
        }

        public void CenterOnPoint(PointF pointImage, bool bOnlyIfNotVisible)
        {
            // Get the current screen position of the image point.
            PointF pscr = this.ImageToScreen(pointImage);

            // Get the visible area (in screen coordinates)
            Rectangle scr = this.ClientRectangle;

            // Update scroll position (center on point), if needed.
            if (!bOnlyIfNotVisible
                || (pscr.X < scr.Left
                    || pscr.X > scr.Right
                    || pscr.Y < scr.Top
                    || pscr.Y > scr.Bottom))
            {
                this.AutoScrollPosition = Point.Round(new PointF(pscr.X - scr.Width / 2F - this.AutoScrollPosition.X, pscr.Y - scr.Height / 2F - this.AutoScrollPosition.Y));
            }
        }

        public bool IsVisible(PointF pointImage)
        {
            // Get the current screen position of the image point.
            PointF pscr = this.ImageToScreen(pointImage);

            // Get the visible area (in screen coordinates)
            Rectangle scr = this.ClientRectangle;

            return (pscr.X >= scr.Left
                    && pscr.X <= scr.Right
                    && pscr.Y >= scr.Top
                    && pscr.Y <= scr.Bottom);
        }

        public enum PlotBoxMarkerType
        {
            Dot, Cross, FatCross
        }
        protected PlotBoxMouseState m_MouseState;
        public PlotBoxMouseState MouseStatus
        {
            get
            {
                return this.m_MouseState;
            }
            set
            {
                this.m_MouseState = value;
                this.SetCursor();
                if (this.StatusTextChanged != null) this.StatusTextChanged(this, new EventArgs());
            }
        }

        public string StatusText
        {
            get
            {
                if (this.Image == null) return "";

                if (this.m_iExpCoordinateAxis != 0)
                {
                    string action = "Left-click";
                    if (!this.m_pointTemp.IsEmpty) action = "Release left mouse button";
                    return action + " to set "
                        + (this.m_iExpCoordinateNr == 1 ? "first" : "second")
                        + " calibration point for "
                        + (this.m_iExpCoordinateAxis == 1 ? "x" : "y")
                        + "-axis, drag with right mouse button to scroll.";
                }
                switch (this.MouseStatus)
                {
                    case PlotBoxMouseState.Add:
                        if (!this.m_pointTemp.IsEmpty) return "Release left mouse button to add data point, press Escape to cancel.";
                        return "Left-click to add data point, drag with right mouse button to scroll.";
                    case PlotBoxMouseState.Drag:
                        return "Drag to scroll image.";
                    case PlotBox.PlotBoxMouseState.Select:
                        if (this.m_bCaretSelect) return "Release left mouse button to select points, press Escape to cancel.";
                        return "Drag or click with left mouse button to select points, drag with right mouse button to scroll.";
                    default:
                        return "";
                }
            }
        }

        protected void SetCursor()
        {
            if (this.m_iExpCoordinateAxis != 0 && this.m_iExpCoordinateNr != 0)
            {
                this.Cursor = Cursors.Cross;
            }
            else
            {
                switch (this.m_MouseState)
                {
                    case (PlotBoxMouseState.Drag):
                        this.Cursor = Cursors.SizeAll;
                        break;
                    case (PlotBoxMouseState.Add):
                        this.Cursor = Cursors.Cross;
                        break;
                    case (PlotBoxMouseState.Select):
                        this.Cursor = Cursors.Arrow;
                        break;
                }
            }
        }

        public PointF ScreenToImage(PointF pointScreen)
        {
            return this.ScreenToImage(pointScreen.X, pointScreen.Y);
        }
        public PointF ScreenToImage(float X, float Y)
        {
            PointF[] points = { new PointF(X, Y) };
            points[0].X -= this.AutoScrollPosition.X;
            points[0].Y -= this.AutoScrollPosition.Y;
            this.m_matTransformInverse.TransformPoints(points);
            return points[0];
        }

        public bool IsPointInImage(PointF point)
        {
            if (this.m_objImage == null) return false;
            return (point.X >= 0 && point.Y >= 0 && point.X <= this.m_objImage.Width && point.Y <= this.m_objImage.Height);
        }

        public PointF ImageToScreen(PointF pointImage)
        {
            return this.ImageToScreen(pointImage.X, pointImage.Y);
        }
        public PointF ImageToScreen(float X, float Y)
        {
            PointF[] points = { new PointF(X, Y) };
            this.m_matTransform.TransformPoints(points);
            points[0].X += this.AutoScrollPosition.X;
            points[0].Y += this.AutoScrollPosition.Y;
            return points[0];
        }

        protected int m_iExpCoordinateAxis, m_iExpCoordinateNr;
        public void ExpectAxisCoordinate(int iAxis, int iNr)
        {
            this.m_iExpCoordinateAxis = iAxis;
            this.m_iExpCoordinateNr = iNr;
            this.SetCursor();
            if (this.StatusTextChanged != null) this.StatusTextChanged(this, new EventArgs());
        }
        public void StopExpectAxisCoordinate()
        {
            this.m_iExpCoordinateAxis = 0;
            this.m_iExpCoordinateNr = 0;
            this.SetCursor();
            if (this.StatusTextChanged != null) this.StatusTextChanged(this, new EventArgs());
        }

        protected int m_iAxesState;
        public void SetAxisTrueCoordinate(PlotBoxAxisType axis, int iNr, bool bValue, double dValue)
        {
            int iAxis = (axis == PlotBoxAxisType.X ? 0 : 1);
            AxisPoint point = this.m_arrAxesPoints[iAxis, iNr - 1];
            point.HasValue = bValue;
            if (this.m_arrAxesLogScale[iAxis])
            {
                point.Value = Math.Log10(dValue);
                point.NonLogValue = dValue;
            }
            else point.Value = dValue;
            this.UpdateAxes();
        }

        protected bool[] m_arrAxesLogScale;
        public void SetAxisLogState(PlotBoxAxisType axis, bool bLog)
        {
            int iAxis = (axis == PlotBoxAxisType.X ? 0 : 1);
            if (this.m_arrAxesLogScale[iAxis] != bLog)
            {
                if (bLog)
                {
                    this.m_arrAxesPoints[iAxis, 0].NonLogValue = this.m_arrAxesPoints[iAxis, 0].Value;
                    if (this.m_arrAxesPoints[iAxis, 0].HasValue) this.m_arrAxesPoints[iAxis, 0].Value = Math.Log10(this.m_arrAxesPoints[iAxis, 0].Value);
                    this.m_arrAxesPoints[iAxis, 1].NonLogValue = this.m_arrAxesPoints[iAxis, 1].Value;
                    if (this.m_arrAxesPoints[iAxis, 1].HasValue) this.m_arrAxesPoints[iAxis, 1].Value = Math.Log10(this.m_arrAxesPoints[iAxis, 1].Value);
                }
                else
                {
                    this.m_arrAxesPoints[iAxis, 0].Value = this.m_arrAxesPoints[iAxis, 0].NonLogValue;
                    this.m_arrAxesPoints[iAxis, 1].Value = this.m_arrAxesPoints[iAxis, 1].NonLogValue;
                }
                this.m_arrAxesLogScale[iAxis] = bLog;
                this.UpdateAxes();
            }
        }

        protected double[] FindLineCross(double x1, double y1, double dx1, double dy1, double x2, double y2, double dx2, double dy2)
        {
            double[] cross = new double[5];

            // Index 4 signifies whether the lines indeed cross. If its value
            // equals 1, the lines are parallel.
            cross[4] = 1;

            double div = (dy1 * dx2 - dx1 * dy2);
            if (div != 0)
            {
                double c1, c2;
                c1 = -(dx2 * (y1 - y2) - dy2 * (x1 - x2)) / div;
                c2 = (dx1 * (y2 - y1) - dy1 * (x2 - x1)) / div;
                cross[0] = x1 + dx1 * c1;
                cross[1] = y1 + dy1 * c1;
                cross[2] = c1;
                cross[3] = c2;
                cross[4] = 0;
            }
            return cross;
        }

        protected double FindAngle(float dx, float dy)
        {
            double side = System.Math.Sqrt(dx * dx + dy * dy);
            if (dx >= 0)
            {
                if (dy >= 0)
                {
                    // First quadrant
                    return Math.Asin(dx / side) * 180 / Math.PI;
                }
                else
                {
                    // Second quadrant
                    return 90 + Math.Asin(-dy / side) * 180 / Math.PI;
                }
            }
            else
            {
                if (dy <= 0)
                {
                    // Third quadrant
                    return 180 + Math.Asin(-dx / side) * 180 / Math.PI;
                }
                else
                {
                    // Fourth quadrant
                    return 270 + Math.Asin(dy / side) * 180 / Math.PI;
                }
            }
        }

        protected PointF[] GetSortedAxisPoints(int iAxis)
        {
            PointF[] res = new PointF[2];
            if (this.m_arrAxesPoints[iAxis, 0].Value < this.m_arrAxesPoints[iAxis, 1].Value)
            {
                res[0] = this.m_arrAxesPoints[iAxis, 0].Point;
                res[1] = this.m_arrAxesPoints[iAxis, 1].Point;
            }
            else
            {
                res[0] = this.m_arrAxesPoints[iAxis, 1].Point;
                res[1] = this.m_arrAxesPoints[iAxis, 0].Point;
            }
            return res;
        }

        public double AxesAngle
        {
            get
            {
                float dx1, dy1, dx2, dy2;
                PointF[] points;

                // X-axis direction
                points = this.GetSortedAxisPoints(0);
                dx1 = points[1].X - points[0].X;
                dy1 = points[0].Y - points[1].Y;

                // Y-axis direction
                points = this.GetSortedAxisPoints(1);
                dx2 = points[1].X - points[0].X;
                dy2 = points[0].Y - points[1].Y;

                // Get angle between axes
                double a1, a2, a;
                a1 = this.FindAngle(dx1, dy1);
                a2 = this.FindAngle(dx2, dy2);
                a = a1 - a2;

                // No negative angles allowed.
                if (a < 0) a += 360;

                return a;
            }
        }

        public void RotateFromAxes()
        {
            float dx1, dy1, dx2, dy2;
            PointF[] points;

            // X-axis direction
            points = this.GetSortedAxisPoints(0);
            dx1 = points[1].X - points[0].X;
            dy1 = points[0].Y - points[1].Y;

            // Y-axis direction
            points = this.GetSortedAxisPoints(1);
            dx2 = points[1].X - points[0].X;
            dy2 = points[0].Y - points[1].Y;

            // Get deviation (x-axis, 90 degrees) and (y-axis, 0 degrees)
            double a1, a2, a;
            a1 = 90 - this.FindAngle(dx1, dy1);
            a2 = -this.FindAngle(dx2, dy2);

            // Deviations must be between -180 and 180 degrees.
            if (a1 < -180) a1 += 360;
            if (a1 > 180) a1 -= 360;
            if (a2 < -180) a2 += 360;
            if (a2 > 180) a2 -= 360;

            // Rotate by average deviation.
            a = (a1 + a2) / 2;
            this.Rotation = (float)a;
        }

        protected void DrawAxis(Graphics g, Pen pen, int iAxis)
        {
            // Get coordinate boundaries (in image units)
            double xmin, ymin, xmax, ymax;
            xmin = 0;
            ymin = 0;
            xmax = this.m_objImage.Width;
            ymax = this.m_objImage.Height;

            // Get axis direction.
            double dx, dy;
            dx = this.m_arrAxesPoints[iAxis, 1].Point.X - this.m_arrAxesPoints[iAxis, 0].Point.X;
            dy = this.m_arrAxesPoints[iAxis, 1].Point.Y - this.m_arrAxesPoints[iAxis, 0].Point.Y;

            // Starting point for the axis.
            double x, y;
            x = this.m_pointCross.X;
            y = this.m_pointCross.Y;

            // Find crossing points of axis with the four sides of image.
            double[] cross1 = this.FindLineCross(x, y, dx, dy, 0, 0, 1, 0);
            double[] cross2 = this.FindLineCross(x, y, dx, dy, 0, m_objImage.Height, 1, 0);
            double[] cross3 = this.FindLineCross(x, y, dx, dy, 0, 0, 0, 1);
            double[] cross4 = this.FindLineCross(x, y, dx, dy, m_objImage.Width, 0, 0, 1);

            // Select crossing points that are (1) valid (axis may be parallel
            // to side), and (2) within display range.
            int iPoint = 0;
            PointF[] arrPoints = new PointF[4];
            if (cross1[4] == 0 && cross1[0] > xmin && cross1[0] < xmax) arrPoints[iPoint++] = new PointF((float)cross1[0], (float)cross1[1]);
            if (cross2[4] == 0 && cross2[0] > xmin && cross2[0] < xmax) arrPoints[iPoint++] = new PointF((float)cross2[0], (float)cross2[1]);
            if (cross3[4] == 0 && cross3[1] > ymin && cross3[1] < ymax) arrPoints[iPoint++] = new PointF((float)cross3[0], (float)cross3[1]);
            if (cross4[4] == 0 && cross4[1] > ymin && cross4[1] < ymax) arrPoints[iPoint++] = new PointF((float)cross4[0], (float)cross4[1]);

            // We should have either 0 or 2 crossing points with the display sides.
            // Either the axis is completely outside the display (0 crossing points),
            // or the axis is within display (2 crossing points).
            // However, numerical trouble could in theory result in 1, 3 or 4
            // crossing points...
            if (iPoint >= 2) g.DrawLine(pen, this.ImageToScreen(arrPoints[0]), this.ImageToScreen(arrPoints[1]));
        }

        public double[] ImageToTrue(PointF pointImage)
        {
            return this.ImageToTrue(pointImage.X, pointImage.Y);
        }

        public double[] ImageToTrue(float x, float y)
        {
            double dx1, dy1, dx2, dy2, xo, yo, dxtrue, dytrue;

            // Get direction of x-axis.
            dx1 = this.m_arrAxesPoints[0, 1].Point.X - m_arrAxesPoints[0, 0].Point.X;
            dy1 = this.m_arrAxesPoints[0, 1].Point.Y - m_arrAxesPoints[0, 0].Point.Y;

            // Get direction of y-axis.
            dx2 = this.m_arrAxesPoints[1, 1].Point.X - m_arrAxesPoints[1, 0].Point.X;
            dy2 = this.m_arrAxesPoints[1, 1].Point.Y - m_arrAxesPoints[1, 0].Point.Y;

            // Get origin.
            xo = this.m_pointOrigin.X;
            yo = this.m_pointOrigin.Y;

            // Get true axis lengths.
            dxtrue = this.m_arrAxesPoints[0, 1].Value - this.m_arrAxesPoints[0, 0].Value;
            dytrue = this.m_arrAxesPoints[1, 1].Value - this.m_arrAxesPoints[1, 0].Value;

            double[] res = new double[2];

            // First map onto x-axis (along y-axis direction)
            double[] xcross = this.FindLineCross(xo, yo, dx1, dy1, x, y, dx2, dy2);
            res[0] = xcross[2] * dxtrue;
            res[1] = -xcross[3] * dytrue;

            // Correct for log-scale, if applicable.
            if (this.m_arrAxesLogScale[0]) res[0] = Math.Pow(10, res[0]);
            if (this.m_arrAxesLogScale[1]) res[1] = Math.Pow(10, res[1]);

            return res;
        }

        protected PointF m_pointCross, m_pointOrigin;
        protected void UpdateAxes()
        {
            int iAxesState = 0;
            this.m_listAxesWarnings.Clear();

            // Check whether axes calibration points have been set.
            bool bCalibrationSet = true;
            if (!this.m_arrAxesPoints[0, 0].HasImageCoordinate)
            {
                bCalibrationSet = false;
                this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.X, 1, AxisWarning.WarningType.CoordinateNotSet));
            }
            if (!this.m_arrAxesPoints[0, 1].HasImageCoordinate)
            {
                bCalibrationSet = false;
                this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.X, 2, AxisWarning.WarningType.CoordinateNotSet));
            }
            if (!this.m_arrAxesPoints[1, 0].HasImageCoordinate)
            {
                bCalibrationSet = false;
                this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.Y, 1, AxisWarning.WarningType.CoordinateNotSet));
            }
            if (!this.m_arrAxesPoints[1, 1].HasImageCoordinate)
            {
                bCalibrationSet = false;
                this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.Y, 2, AxisWarning.WarningType.CoordinateNotSet));
            }

            if (bCalibrationSet)
            {
                bool bCalibrationValid = true;

                // Check whether axes calibration points are valid
                // (the two calibration points for one axis should not be the same)
                if (this.m_arrAxesPoints[0, 0].Point == this.m_arrAxesPoints[0, 1].Point)
                {
                    bCalibrationValid = false;
                    this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.X, 0, AxisWarning.WarningType.CoordinatesIdentical));
                }
                if (this.m_arrAxesPoints[1, 0].Point == this.m_arrAxesPoints[1, 1].Point)
                {
                    bCalibrationValid = false;
                    this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.Y, 0, AxisWarning.WarningType.CoordinatesIdentical));
                }

                if (bCalibrationValid)
                {
                    // Erase current cross point and origin
                    if (!m_pointCross.IsEmpty) this.InvalidateMarkerImage(m_pointCross);
                    this.m_pointCross = new PointF();
                    this.m_pointOrigin = new PointF();

                    // Get axes directions (dx, dy).
                    double c1, c2, dx1, dy1, dx2, dy2;
                    dx1 = this.m_arrAxesPoints[0, 1].Point.X - this.m_arrAxesPoints[0, 0].Point.X;
                    dy1 = this.m_arrAxesPoints[0, 1].Point.Y - this.m_arrAxesPoints[0, 0].Point.Y;
                    dx2 = this.m_arrAxesPoints[1, 1].Point.X - this.m_arrAxesPoints[1, 0].Point.X;
                    dy2 = this.m_arrAxesPoints[1, 1].Point.Y - this.m_arrAxesPoints[1, 0].Point.Y;

                    // Try to find cross point between axes.
                    double[] dPointCross = this.FindLineCross(this.m_arrAxesPoints[0, 0].Point.X, this.m_arrAxesPoints[0, 0].Point.Y,
                        dx1,
                        dy1,
                        this.m_arrAxesPoints[1, 0].Point.X, this.m_arrAxesPoints[1, 0].Point.Y,
                        dx2,
                        dy2);
                    if (dPointCross[4] == 0)
                    {
                        c1 = dPointCross[2];
                        c2 = dPointCross[3];

                        // Save the cross point
                        this.m_pointCross = new PointF((float)dPointCross[0], (float)dPointCross[1]);
                        this.InvalidateMarkerImage(this.m_pointCross);
                        iAxesState = 1;

                        bool bTrueValuesSet = true;

                        // Check whether axes true values have been set.
                        if (!this.m_arrAxesPoints[0, 0].HasValue)
                        {
                            bTrueValuesSet = false;
                            this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.X, 1, AxisWarning.WarningType.ValueNotSet));
                        }
                        if (!this.m_arrAxesPoints[0, 1].HasValue)
                        {
                            bTrueValuesSet = false;
                            this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.X, 2, AxisWarning.WarningType.ValueNotSet));
                        }
                        if (!this.m_arrAxesPoints[1, 0].HasValue)
                        {
                            bTrueValuesSet = false;
                            this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.Y, 1, AxisWarning.WarningType.ValueNotSet));
                        }
                        if (!this.m_arrAxesPoints[1, 1].HasValue)
                        {
                            bTrueValuesSet = false;
                            this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.Y, 2, AxisWarning.WarningType.ValueNotSet));
                        }

                        if (bTrueValuesSet)
                        {
                            bool bTrueValuesValid = true;

                            // Check whether axes true values are valid (not identical)
                            if (this.m_arrAxesPoints[0, 0].Value == this.m_arrAxesPoints[0, 1].Value)
                            {
                                bTrueValuesValid = false;
                                this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.X, 0, AxisWarning.WarningType.ValuesIdentical));
                            }
                            if (this.m_arrAxesPoints[1, 0].Value == this.m_arrAxesPoints[1, 1].Value)
                            {
                                bTrueValuesValid = false;
                                this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.Y, 0, AxisWarning.WarningType.ValuesIdentical));
                            }

                            // If log-scale, check if the values are positive.
                            if (this.m_arrAxesLogScale[0])
                            {
                                if (this.m_arrAxesPoints[0, 0].NonLogValue <= 0)
                                {
                                    bTrueValuesValid = false;
                                    this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.X, 1, AxisWarning.WarningType.ValueNonPositive));
                                }
                                if (this.m_arrAxesPoints[0, 1].NonLogValue <= 0)
                                {
                                    bTrueValuesValid = false;
                                    this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.X, 2, AxisWarning.WarningType.ValueNonPositive));
                                }
                            }
                            if (this.m_arrAxesLogScale[1])
                            {
                                if (this.m_arrAxesPoints[1, 0].NonLogValue <= 0)
                                {
                                    bTrueValuesValid = false;
                                    this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.Y, 1, AxisWarning.WarningType.ValueNonPositive));
                                }
                                if (this.m_arrAxesPoints[1, 1].NonLogValue <= 0)
                                {
                                    bTrueValuesValid = false;
                                    this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.Y, 2, AxisWarning.WarningType.ValueNonPositive));
                                }
                            }

                            if (bTrueValuesValid)
                            {
                                double truedx, truedy;
                                truedx = this.m_arrAxesPoints[0, 1].Value - this.m_arrAxesPoints[0, 0].Value;
                                truedy = this.m_arrAxesPoints[1, 1].Value - this.m_arrAxesPoints[1, 0].Value;
                                double truecrossx = this.m_arrAxesPoints[0, 0].Value + truedx * c1;
                                double truecrossy = this.m_arrAxesPoints[1, 0].Value + truedy * c2;

                                m_pointOrigin = new PointF((float)(m_pointCross.X - dx1 * truecrossx / truedx - dx2 * truecrossy / truedy),
                                    (float)(m_pointCross.Y - dy1 * truecrossx / truedx - dy2 * truecrossy / truedy));

                                iAxesState = 2;
                            }	// Axes true values valid
                        }	// Axes true values set
                    }	// Valid cross point (axes not parallel)
                    else
                    {
                        this.m_listAxesWarnings.Add(new AxisWarning(PlotBoxAxisType.None, 0, AxisWarning.WarningType.AxesParallel));
                        MessageBox.Show("Your X- and Y-axis run parallel", "Invalid axes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }	// Calibration points valid
            }	// Calibration points set
            if (iAxesState > 0 || (iAxesState == 0 && this.m_iAxesState != 0)) this.Invalidate();
            this.m_iAxesState = iAxesState;
        }

        public int AxesState
        {
            get
            {
                return this.m_iAxesState;
            }
        }

        public void ClearAxes()
        {
            this.m_arrAxesPoints = new AxisPoint[2, 2];
            this.m_arrAxesPoints[0, 0] = new AxisPoint();
            this.m_arrAxesPoints[0, 1] = new AxisPoint();
            this.m_arrAxesPoints[1, 0] = new AxisPoint();
            this.m_arrAxesPoints[1, 1] = new AxisPoint();
            this.m_arrAxesLogScale[0] = false;
            this.m_arrAxesLogScale[1] = false;
            this.m_pointOrigin = new PointF();
            this.m_pointCross = new PointF();
            this.UpdateAxes();
        }

        public void DeletePoints(int[] points)
        {
            Array.Sort(points);
            bool bSelectionChanged = false;
            for (int iDelPoint = 0; iDelPoint < points.Length; iDelPoint++)
            {
                int iPoint = points[iDelPoint] - iDelPoint;

                // Invalidate display area of deleted point (forcing redraw).
                this.InvalidateMarkerImage((PointF)(this.m_listPoints[iPoint]));

                // If point was selected, remember we changed the selection.
                if ((bool)(this.m_listPointSel[iPoint])) bSelectionChanged = true;

                // Remove point from lists.
                this.m_listPoints.RemoveAt(iPoint);
                this.m_listPointSel.RemoveAt(iPoint);
            }
            // If the selection changed, notify anyone interested.
            if (bSelectionChanged && this.SelectionChanged != null) this.SelectionChanged(this, null);
        }

        protected Color[] m_arrColors;
        public void SetColor(int iColor, Color color)
        {
            m_arrColors[iColor] = color;
            this.Invalidate();
        }

        public Color GetColor(int iColor)
        {
            return m_arrColors[iColor];
        }

        protected bool m_bCaretSelect;
        protected PointF m_pointCaretStart, m_pointCaretStop;
        private void PlotBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (this.m_bCaretSelect)
            {
                this.InvalidateCaret();
            }
            if (this.m_bMoving)
            {
                Size sizeDif = new Size(e.X - this.m_pointMoveStart.X, e.Y - this.m_pointMoveStart.Y);
                this.AutoScrollPosition = this.m_pointScrollPosMoveStart - sizeDif;
            }
            if (this.m_bCaretSelect)
            {
                this.m_pointCaretStop = this.ScreenToImage(e.X, e.Y);
                this.InvalidateCaret();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                if (e.Delta > 0 && this.m_iZoom <= 1990)
                {
                    //PointF p = this.ScreenToImage(e.X, e.Y);
                    this.Zoom += 10;
                    if (this.ZoomChanged != null) this.ZoomChanged(this, null);
                    //this.ShowPointAt(p, new PointF(e.X, e.Y));
                }
                else if (e.Delta < 0 && this.m_iZoom >= 20)
                {
                    //PointF p = this.ScreenToImage(e.X, e.Y);
                    this.Zoom -= 10;
                    if (this.ZoomChanged != null) this.ZoomChanged(this, null);
                    //this.ShowPointAt(p, new PointF(e.X, e.Y));
                }
            }
            else
            {
                base.OnMouseWheel(e);
            }
        }

        private void PlotBox_Resize(object sender, System.EventArgs e)
        {
            if (this.m_bZoomToFit)
            {
                this.UpdateTransform(false);
                if (this.ZoomChanged != null) this.ZoomChanged(this, null);
            }
            else if (this.ShownAreaChanged != null) this.ShownAreaChanged(this, null);
        }

        public void OnPaintThumbnail(Graphics g)
        {
            if (this.m_objImage != null)
            {
                // Draw image
                System.Drawing.Drawing2D.Matrix mat = g.Transform.Clone();
                g.MultiplyTransform(this.m_matTransform, MatrixOrder.Prepend);
                g.PageUnit = GraphicsUnit.Pixel;
                Rectangle rectImage = new Rectangle(0, 0, m_objImage.Width, m_objImage.Height);
                g.DrawImage(m_objImage, rectImage, rectImage, GraphicsUnit.Pixel);
                g.Transform = mat;
            }
        }

        public event EventHandler ShownAreaChanged;

        public event EventHandler ImageChanged;

        protected bool m_bUseMagicWand;
        public bool UseMagicWand
        {
            get
            {
                return m_bUseMagicWand;
            }
            set
            {
                m_bUseMagicWand = value;
            }
        }

        protected float m_flMagicWandTolerance;
        public float MagicWandTolerance
        {
            get
            {
                return m_flMagicWandTolerance;
            }
            set
            {
                m_flMagicWandTolerance = value;
            }
        }

        protected SizeF m_sizeCurrent;
        public SizeF CurrentSize
        {
            get
            {
                return m_sizeCurrent;
            }
        }

        private void PlotBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    e.Handled = this.CancelMouseAction();
                    break;
            }
        }

        protected bool CancelMouseAction()
        {
            bool changed = false;
            if (this.m_bCaretSelect)
            {
                this.m_bCaretSelect = false;
                InvalidateCaret();

                if (this.StatusTextChanged != null) this.StatusTextChanged(this, new EventArgs());

                changed = true;
            }
            else if (!this.m_pointTemp.IsEmpty)
            {
                PointF point = this.m_pointTemp;
                this.m_pointTemp = new PointF();
                this.InvalidateMarkerImage(point);

                if (this.m_bmOverlay != null)
                {
                    Rectangle rectOverlay = new Rectangle(this.m_bmOverlay.offset, this.m_bmOverlay.bitmap.Size);
                    this.m_bmOverlay = null;
                    this.InvalidateRectangeImage(rectOverlay);
                }

                if (this.StatusTextChanged != null) this.StatusTextChanged(this, new EventArgs());

                changed = true;
            }
            return changed;
        }

        private void PlotBox_MouseLeave(object sender, EventArgs e)
        {
            this.CancelMouseAction();
        }

    }

    /// <summary>
    /// Fills a bitmap using a non-recursive flood-fill.
    /// </summary>
    public abstract class AbstractMagicWand
    {
        protected Bitmap m_bm;
        protected System.Collections.BitArray m_baChecked;
        protected byte[,] m_arrMatches;

        protected Bitmap ImageToBitmap(Image img)
        {
            Bitmap bm = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bm);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(img, new Rectangle(0, 0, bm.Width, bm.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
            g.Dispose();
            return bm;
        }

        public AbstractMagicWand(Image img)
        {
            // Ensure that the image is a bitmap.
            try
            {
                this.m_bm = (Bitmap)img;
            }
            catch (InvalidCastException ex)
            {
                this.m_bm = this.ImageToBitmap(img);
            }

            // Ensure that the bitmap uses a supported pixel format.
            if (img.PixelFormat != PixelFormat.Format32bppArgb && img.PixelFormat != PixelFormat.Format32bppRgb && img.PixelFormat != PixelFormat.Format24bppRgb) this.m_bm = this.ImageToBitmap(img);

            // Allocate arrays for checked pixels and match values, matching the current image size.
            this.m_baChecked = new BitArray(img.Width * img.Height);
            this.m_arrMatches = new byte[img.Width, img.Height];
        }

        public Bitmap Bitmap
        {
            get
            {
                return this.m_bm;
            }
        }

        public abstract AbstractMagicWand.Selection Select(Point pos, double maxdelta);

        public abstract class Selection
        {
            protected BitmapData m_bmd;
            protected int m_byteperpixel;
            protected double m_maxdelta;
            protected Color m_colorRef;
            protected System.Collections.BitArray m_baChecked;
            protected byte[,] m_arrMatches;

            public class Overlay
            {
                public Point offset;
                public Bitmap bitmap;
                public Overlay()
                {
                    this.offset = new Point(0, 0);
                }
                public Overlay(int x, int y)
                {
                    this.offset = new Point(x, y);
                }
            }

            public Selection(AbstractMagicWand wand, BitArray checks, byte[,] matches, Point pos, double maxdelta)
            {
                // Lock the bitmap data
                this.m_bmd = wand.Bitmap.LockBits(new Rectangle(0, 0, wand.Bitmap.Width, wand.Bitmap.Height), ImageLockMode.ReadOnly, wand.Bitmap.PixelFormat);

                this.m_baChecked = checks;
                this.m_baChecked.SetAll(false);
                this.m_arrMatches = matches;
                this.m_byteperpixel = 4;
                if (this.m_bmd.PixelFormat == PixelFormat.Format24bppRgb) this.m_byteperpixel = 3;
                this.m_maxdelta = maxdelta;
                this.m_colorRef = this.GetPixelColor(pos.X, pos.Y);
                this.DoMagicWand(pos);

                // Unlock the bitmap
                wand.Bitmap.UnlockBits(this.m_bmd);
            }

            protected Color GetPixelColor(int X, int Y)
            {
                // Always assumes 32 bit per pixels
                int offset = Y * this.m_bmd.Stride + (this.m_byteperpixel * X);
                int A = 255;
                int B = System.Runtime.InteropServices.Marshal.ReadByte(this.m_bmd.Scan0, offset + 0);
                int G = System.Runtime.InteropServices.Marshal.ReadByte(this.m_bmd.Scan0, offset + 1);
                int R = System.Runtime.InteropServices.Marshal.ReadByte(this.m_bmd.Scan0, offset + 2);
                if (this.m_bmd.PixelFormat == PixelFormat.Format32bppArgb) A = System.Runtime.InteropServices.Marshal.ReadByte(this.m_bmd.Scan0, offset + 3);
                return Color.FromArgb(A,R,G,B);
            }

            protected abstract void DoMagicWand(Point pos);

            protected bool CheckPixel(int X, int Y)
            {
                // Check whether this pixel was checked earlier.
                int iPixel = Y * this.m_bmd.Width + X;
                if (this.m_baChecked[iPixel]) return false;

                Color color = this.GetPixelColor(X, Y);
                double delta = Math.Sqrt(((color.A - this.m_colorRef.A) * (color.A - this.m_colorRef.A) + (color.R - this.m_colorRef.R) * (color.R - this.m_colorRef.R) + (color.G - this.m_colorRef.G) * (color.G - this.m_colorRef.G) + (color.B - this.m_colorRef.B) * (color.B - this.m_colorRef.B)) / 4);
                this.m_baChecked[iPixel] = true;

                byte value = (byte)Math.Round(Math.Max(0D, 1D - delta / this.m_maxdelta) * 255D);
                this.m_arrMatches[X, Y] = value;

                return value>0;
            }

            public PointF GetMeanPoint()
            {
                double meanx = 0D, meany = 0D, total = 0D;
                int iPixel = 0;
                for (int iY = 0; iY < this.m_bmd.Height; iY++)
                {
                    for (int iX = 0; iX < this.m_bmd.Width; iX++)
                    {
                        if (this.m_baChecked[iPixel++])
                        {
                            meanx += iX * this.m_arrMatches[iX, iY];
                            meany += iY * this.m_arrMatches[iX, iY];
                            total += this.m_arrMatches[iX, iY];
                        }
                    }
                }
                return new PointF((float)(meanx / total), (float)(meany / total));
            }

            public Overlay GetOverlay(Color overlayColor, int maxalpha)
            {
                // First boundaries of magic wand selected area (use padding of 1 px)
                int minY = this.m_bmd.Height;
                int minX = this.m_bmd.Width;
                int maxY = 0;
                int maxX = 0;
                int iPixel = 0;
                for (int iY = 0; iY < this.m_bmd.Height; iY++)
                {
                    for (int iX = 0; iX < this.m_bmd.Width; iX++)
                    {
                        if (this.m_baChecked[iPixel++])
                        {
                            minX = Math.Min(minX, iX);
                            maxX = Math.Max(maxX, iX);
                            minY = Math.Min(minY, iY);
                            maxY = Math.Max(maxY, iY);
                        }
                    }
                }

                float scale = maxalpha / 255F;

                // Create overlay
                Overlay overlay = new Overlay(minX, minY);
                overlay.bitmap = new Bitmap(maxX - minX + 1, maxY - minY + 1, PixelFormat.Format32bppArgb);
                BitmapData olbmd = overlay.bitmap.LockBits(new Rectangle(0, 0, overlay.bitmap.Width, overlay.bitmap.Height), ImageLockMode.WriteOnly, overlay.bitmap.PixelFormat);
                int offset;
                for (int iY = minY; iY <= maxY; iY++)
                {
                    for (int iX = minX; iX <= maxX; iX++)
                    {
                        if (this.m_baChecked[iY * this.m_bmd.Width + iX])
                        {
                            offset = (iY - minY) * olbmd.Stride + 4 * (iX - minX);
                            System.Runtime.InteropServices.Marshal.WriteByte(olbmd.Scan0, offset + 0, overlayColor.B);
                            System.Runtime.InteropServices.Marshal.WriteByte(olbmd.Scan0, offset + 1, overlayColor.G);
                            System.Runtime.InteropServices.Marshal.WriteByte(olbmd.Scan0, offset + 2, overlayColor.R);
                            System.Runtime.InteropServices.Marshal.WriteByte(olbmd.Scan0, offset + 3, (byte)Math.Round(scale*this.m_arrMatches[iX, iY]));
                        }
                    }
                }
                overlay.bitmap.UnlockBits(olbmd);
                return overlay;
            }
        }

    }

    public class MagicWandPixel : AbstractMagicWand
    {
        public MagicWandPixel(Image img)
            : base(img)
        {
        }

        public override AbstractMagicWand.Selection Select(Point pos, double maxdelta)
        {
            return new MagicWandPixel.Selection(this, this.m_baChecked, this.m_arrMatches, pos, maxdelta);
        }

        public new class Selection : AbstractMagicWand.Selection
        {
            public Selection(MagicWandPixel wand, BitArray checks, byte[,] matches, Point pos, double maxdelta)
                : base(wand, checks, matches, pos, maxdelta)
            {
            }

            protected override void DoMagicWand(Point pos)
            {
                // Initialize structures that will store magic wand state.
                System.Collections.Generic.Stack<Point> stack = new System.Collections.Generic.Stack<Point>();

                // Register starting point as checked and matching pixel.
                this.m_arrMatches[pos.X, pos.Y] = 255;
                this.m_baChecked[pos.Y*this.m_bmd.Width+pos.X] = true;

                // Fill the first pixel and recursively fill all it's neighbors
                Point curpos;
                stack.Push(pos);
                do
                {
                    curpos = stack.Pop();
                    if (curpos.X < this.m_bmd.Width - 1 && CheckPixel(curpos.X + 1, curpos.Y)) stack.Push(new Point(curpos.X + 1, curpos.Y));
                    if (curpos.X > 0 && CheckPixel(curpos.X - 1, curpos.Y)) stack.Push(new Point(curpos.X - 1, curpos.Y));
                    if (curpos.Y < this.m_bmd.Height - 1 && CheckPixel(curpos.X, curpos.Y + 1)) stack.Push(new Point(curpos.X, curpos.Y + 1));
                    if (curpos.Y > 0 && CheckPixel(curpos.X, curpos.Y - 1)) stack.Push(new Point(curpos.X, curpos.Y - 1));
                } while (stack.Count > 0);
            }
        }
    }

    public class MagicWandLines : AbstractMagicWand
    {
        public MagicWandLines(Image img)
            : base(img)
        {
        }

        public override AbstractMagicWand.Selection Select(Point pos, double maxdelta)
        {
            return new MagicWandLines.Selection(this, this.m_baChecked, this.m_arrMatches, pos, maxdelta);
        }

        public new class Selection : AbstractMagicWand.Selection
        {
            public Selection(MagicWandLines wand, BitArray checks, byte[,] matches, Point pos, double maxdelta)
                : base(wand, checks, matches, pos, maxdelta)
            {
            }

            protected class Line
            {
                public int Y, X1, X2, origin;
                public Line(int Y, int X1, int origin)
                {
                    this.Y = Y;
                    this.X1 = X1;
                    this.X2 = X1;
                    this.origin = origin;
                }
            }

            protected struct LineCheck
            {
                public int Y, X1, X2;
                public LineCheck(int Y, int X1, int X2)
                {
                    this.X1 = X1;
                    this.X2 = X2;
                    this.Y = Y;
                }
            }

            protected override void DoMagicWand(Point pos)
            {
                // Initialize structures that will store magic wand state.
                System.Collections.Generic.Stack<Line> stack = new System.Collections.Generic.Stack<Line>();

                // Push first pixel to check on to the stack.
                stack.Push(new Line(pos.Y, pos.X, 0));

                // Register first pixel as match.
                this.m_arrMatches[pos.X, pos.Y] = 255;
                this.m_baChecked[pos.Y * this.m_bmd.Width + pos.X] = true;

                // For each line on the stack: extend it left and right,
                // then search for matching lines above and below,
                // and add these to the stack.
                Line curline, newline;
                int curX, oriX1, oriX2;
                System.Collections.Generic.List<LineCheck> listChecks = new System.Collections.Generic.List<LineCheck>(3);
                do
                {
                    curline = stack.Pop();

                    // Store original left and right boundaries.
                    oriX1 = curline.X1;
                    oriX2 = curline.X2;

                    // Explore pixels to the left (extend current line)
                    for (curX = curline.X1 - 1; curX >= 0; curX--)
                    {
                        if (!this.CheckPixel(curX, curline.Y)) break;
                        curline.X1--;
                    }

                    // Explore pixels to the right (extend current line)
                    for (curX = curline.X2 + 1; curX < this.m_bmd.Width; curX++)
                    {
                        if (!this.CheckPixel(curX, curline.Y)) break;
                        curline.X2++;
                    }

                    // Build a list of upper and lower neighboring lines to check.
                    listChecks.Clear();
                    if (curline.origin == 0)
                    {
                        // First point - check full top and bottom.
                        listChecks.Add(new LineCheck(curline.Y - 1, curline.X1, curline.X2));
                        listChecks.Add(new LineCheck(curline.Y + 1, curline.X1, curline.X2));
                    }
                    else
                    {
                        // Coming from top of bottom - check only remaining unexplored area.
                        listChecks.Add(new LineCheck(curline.Y - curline.origin, curline.X1, curline.X2));
                        if (curline.X1 < oriX1) listChecks.Add(new LineCheck(curline.Y + curline.origin, curline.X1, oriX1 - 1));
                        if (curline.X2 > oriX2) listChecks.Add(new LineCheck(curline.Y + curline.origin, oriX2 + 1, curline.X2));
                    }

                    // Loop over lines above and below that need to be checked.
                    foreach (LineCheck lineCheck in listChecks)
                    {
                        // Ensure the y coordinate lies within its valid range.
                        if (lineCheck.Y < 0 || lineCheck.Y >= this.m_bmd.Height) continue;

                        newline = null;
                        for (curX = lineCheck.X1; curX <= lineCheck.X2; curX++)
                        {
                            if (this.CheckPixel(curX, lineCheck.Y))
                            {
                                // Matching pixel. Create a new line structure to hold the match if needed,
                                // then add the match value.
                                if (newline == null)
                                {
                                    newline = new Line(lineCheck.Y, curX, curline.Y - lineCheck.Y);
                                    stack.Push(newline);
                                }
                                else newline.X2++;
                            }
                            else
                            {
                                // Non-matching pixel. Dereference the line structure that held the previous matches.
                                if (newline != null) newline = null;
                            }
                        }
                    }

                } while (stack.Count > 0);
            }
        }
    }
}
