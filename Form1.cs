using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Microsoft.Win32;
using System.Runtime.InteropServices;
#if WIA1
using WIALib;
#elif WIA2
using WIA;
#endif

namespace PlotReader
{
    /// <summary>
	/// Summary description for Form1.
	/// </summary>
    public class FormMain : System.Windows.Forms.Form
    {
        private System.Windows.Forms.MenuStrip mainMenu1;
        private System.Windows.Forms.ToolStripMenuItem menuOpen;
        private System.Windows.Forms.ToolStripMenuItem menuZoom100;
        private System.Windows.Forms.ToolStripMenuItem menuZoom200;
        private System.Windows.Forms.ToolStripMenuItem menuZoom400;
        private System.Windows.Forms.ToolStripMenuItem menuZoom50;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripMenuItem menuZoom;
        private System.Windows.Forms.ToolStripMenuItem menuZoomFit;
        private System.Windows.Forms.ToolStripMenuItem menuClearAllPoints;
        private System.Windows.Forms.ToolStripMenuItem menuSave;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteSelectedPoint;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem menuMarkerSize5;
        private System.Windows.Forms.ToolStripMenuItem menuMarkerSize7;
        private System.Windows.Forms.ToolStripMenuItem menuMarkerSize9;
        private System.Windows.Forms.ToolStripMenuItem menuMarkerSize11;
        private System.Windows.Forms.ToolStripMenuItem menuMarkerSize;
        private System.Windows.Forms.ToolStripMenuItem menuMarkerTypeDot;
        private System.Windows.Forms.ToolStripMenuItem menuMarkerTypeCross;
        private System.Windows.Forms.ToolStripMenuItem menuMarkerType;
        private System.Windows.Forms.ToolStripMenuItem menuRotate;
        private System.Windows.Forms.ToolStripMenuItem menuMarkerTypeFatCross;
        private System.Windows.Forms.ToolStripMenuItem menuRotateLeft;
        private System.Windows.Forms.ToolStripMenuItem menuRotateRight;
        private System.Windows.Forms.ToolStripMenuItem menuRotate180;
        private System.Windows.Forms.ToolStripMenuItem menuStraightenAxes;
        private System.Windows.Forms.ToolStripMenuItem menuRotateReset;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem menuColors;
        private System.Windows.Forms.ToolStripMenuItem menuScanner;
        private System.Windows.Forms.ToolStripMenuItem menuMarkerSize15;
        private System.Windows.Forms.ToolStripMenuItem menuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private System.Windows.Forms.ToolStripMenuItem menuPaste;
        private System.Windows.Forms.ToolStripMenuItem menuCopy;
        private System.Windows.Forms.ToolStrip toolBar1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripButton toolBarOpen;
        private System.Windows.Forms.ToolStripButton toolBarSave;
        private System.Windows.Forms.ToolStripButton toolBarCopy;
        private System.Windows.Forms.ToolStripButton toolBarPaste;
        private System.Windows.Forms.ToolStripButton toolBarSelect;
        private System.Windows.Forms.ToolStripButton toolBarAdd;
        private System.Windows.Forms.ToolStripButton toolBarMove;
        private System.Windows.Forms.ToolStripStatusLabel statusBarPanel1;
        private System.Windows.Forms.ToolStripStatusLabel statusBarPanel2;
        private System.Windows.Forms.ToolStripStatusLabel statusBarImage;
        private System.Windows.Forms.ToolStripStatusLabel statusBarPlot;
        private System.Windows.Forms.ToolStripStatusLabel statusBarStatus;
        private System.Windows.Forms.ToolStripMenuItem menuEdit;
        private System.Windows.Forms.ToolStripMenuItem menuSortPoints;

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem menuShowImageNavigator;
        private ToolStripSeparator toolBarButton1;
        private ToolStripSeparator toolBarButton2;
        private ToolStripSeparator menuItem3;
        private ToolStripSeparator menuItem4;
        private ToolStripCheckBox toolStripCheckBoxAutoCenter;
        private ToolStripTextBox toolStripTextBoxTolerance;
        private ToolStripSeparator toolStripSeparator1;
        private Panel panel1;
        private PlotBox plotBox1;
        private Label labelZoom1;
        private NumericUpDown updownZoom;
        private ThumnailBox thumnailBox1;
        private TabControl tabControl1;
        private TabPage tabPoints;
        private ListView listViewPoints;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private Label labelDataPointsTitle;
        private TabPage tabAxes;
        private Label labelAxesAngle;
        private GroupBox groupXAxis;
        private CheckBox checkXLogScale;
        private CheckBox checkSetX2;
        private CheckBox checkSetX1;
        private Label labelXValue1;
        private TextBox textX1;
        private TextBox textX2;
        private Label labelXValue2;
        private GroupBox groupYAxis;
        private CheckBox checkYLogScale;
        private TextBox textY2;
        private CheckBox checkSetY2;
        private Label labelYValue2;
        private CheckBox checkSetY1;
        private TextBox textY1;
        private Label labelYValue1;
        private Label labelAxesCalibration;
        private Label labelZoom2;
        private LinkLabel linkLabelAlert;

        protected class DataPoint : ListViewItem, IComparable
        {
            public double X, Y;
            public bool HasValue;
            public override string ToString()
            {
                if (!this.HasValue) return "(?, ?)";
                return "(" + this.X.ToString("F3") + ", " + this.Y.ToString("F3") + ")";
            }
            public DataPoint(int index)
            {
                this.HasValue = false;
                this.Tag = index;
                this.Text = "?";
                this.SubItems.Add("?");
            }
            public DataPoint(int index, double dX, double dY)
            {
                this.Tag = index;
                this.X = dX;
                this.Y = dY;
                this.HasValue = true;
                this.Text = this.X.ToString("F3");
                this.SubItems.Add(this.Y.ToString("F3"));
            }

            public int CompareTo(object obj)
            {
                DataPoint dp = (DataPoint)obj;
                if (!this.HasValue) return (((int)this.Tag).CompareTo((int)dp.Tag));
                int iCompX = this.X.CompareTo(dp.X);
                if (iCompX == 0) return this.Y.CompareTo(dp.Y);
                return iCompX;
            }
        }

        private string m_strRegistryRoot = "Software\\Jorn Bruggeman\\PlotReader";
        public FormMain()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            this.saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            // Default marker: fat cross, size 9.
            this.menuMarkerSizeX_Click(this.menuMarkerSize11, null);
            this.menuMarkerTypeX_Click(this.menuMarkerTypeCross, null);

            // Default colors
            this.m_arrDefaultColors = new Color[4] { Color.Red, Color.Orange, Color.Green, Color.Blue };
            for (int i = 0; i < 4; i++) this.plotBox1.SetColor(i, m_arrDefaultColors[i]);

            // Read PlotReader settings from registry.
            RegistryKey regkeyRoot = Registry.CurrentUser.OpenSubKey(this.m_strRegistryRoot, false);
            if (regkeyRoot != null)
            {
                int iValue;
                string strValue;

                strValue = (string)regkeyRoot.GetValue("MarkerType", (string)"Cross");
                foreach (ToolStripMenuItem menuitem in this.menuMarkerType.DropDownItems)
                    if (menuitem.Text == strValue) this.menuMarkerTypeX_Click(menuitem, null);

                iValue = (int)regkeyRoot.GetValue("MarkerSize", (int)11);
                foreach (ToolStripMenuItem menuitem in this.menuMarkerSize.DropDownItems)
                {
                    string val = menuitem.Text.Substring(0, menuitem.Text.IndexOf(" "));
                    if (int.Parse(val) == iValue) this.menuMarkerSizeX_Click(menuitem, null);
                }

                iValue = (int)regkeyRoot.GetValue("SortPoints", (int)1);
                if (iValue == 1) this.menuSortPoints_Click(null, null);

                iValue = (int)regkeyRoot.GetValue("ShowNavigator", (int)1);
                if (iValue == 0) menuZoomControl_Click(this, null);

                RegistryKey regkeyColors = regkeyRoot.OpenSubKey("Colors", false);
                if (regkeyColors != null)
                {
                    for (int i = 0; i < 4; i++)
                        this.plotBox1.SetColor(i, Color.FromArgb((int)regkeyColors.GetValue(i.ToString(), this.m_arrDefaultColors[i].ToArgb())));
                }
            }

            // Allocate array for holding info on running calibration.
            this.m_iWaitingForAxis = new int[2];

            // Check if 'From scanner or camera' menu option should be enabled.
            this.menuScanner.Enabled = this.IsScanningPossible();

            // Placeholder for (temporary) scanned files.
            this.m_strTempScannedFile = "";
        }

        private Color[] m_arrDefaultColors;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Save PlotReader settings to registry.
                RegistryKey regkeyRoot = Registry.CurrentUser.CreateSubKey(this.m_strRegistryRoot);
                if (regkeyRoot != null)
                {
                    int iValue;
                    string strValue = null;

                    foreach (ToolStripMenuItem menuitem in this.menuMarkerType.DropDownItems)
                        if (menuitem.Checked) strValue = menuitem.Text;
                    if (strValue != null) regkeyRoot.SetValue("MarkerType", strValue);

                    iValue = this.plotBox1.MarkerSize;
                    regkeyRoot.SetValue("MarkerSize", iValue);

                    iValue = (this.menuSortPoints.Checked ? 1 : 0);
                    regkeyRoot.SetValue("SortPoints", iValue);

                    iValue = (this.menuShowImageNavigator.Checked ? 1 : 0);
                    regkeyRoot.SetValue("ShowNavigator", iValue);

                    RegistryKey regkeyColors = regkeyRoot.CreateSubKey("Colors");
                    if (regkeyColors != null)
                        for (int i = 0; i < 4; i++)
                            regkeyColors.SetValue(i.ToString(), this.plotBox1.GetColor(i).ToArgb());
                }

                if (this.m_frmOutput != null) this.m_frmOutput.Dispose();

                if (components != null)
                {
                    components.Dispose();
                }
                this.DeleteTempFile();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.mainMenu1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.menuScanner = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeleteSelectedPoint = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClearAllPoints = new System.Windows.Forms.ToolStripMenuItem();
            this.menuView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.menuZoomFit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuZoom50 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuZoom100 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuZoom200 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuZoom400 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRotate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRotateLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRotateRight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRotate180 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStraightenAxes = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRotateReset = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMarkerSize = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMarkerSize5 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMarkerSize7 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMarkerSize9 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMarkerSize11 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMarkerSize15 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMarkerType = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMarkerTypeDot = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMarkerTypeCross = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMarkerTypeFatCross = new System.Windows.Forms.ToolStripMenuItem();
            this.menuColors = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowImageNavigator = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSortPoints = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusBarPanel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBarImage = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBarPanel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBarPlot = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusBarStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolBar1 = new System.Windows.Forms.ToolStrip();
            this.toolBarOpen = new System.Windows.Forms.ToolStripButton();
            this.toolBarPaste = new System.Windows.Forms.ToolStripButton();
            this.toolBarButton1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBarSave = new System.Windows.Forms.ToolStripButton();
            this.toolBarCopy = new System.Windows.Forms.ToolStripButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBarMove = new System.Windows.Forms.ToolStripButton();
            this.toolBarAdd = new System.Windows.Forms.ToolStripButton();
            this.toolBarSelect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripCheckBoxAutoCenter = new PlotReader.ToolStripCheckBox();
            this.toolStripTextBoxTolerance = new System.Windows.Forms.ToolStripTextBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.plotBox1 = new PlotReader.PlotBox();
            this.labelZoom1 = new System.Windows.Forms.Label();
            this.updownZoom = new System.Windows.Forms.NumericUpDown();
            this.thumnailBox1 = new PlotReader.ThumnailBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPoints = new System.Windows.Forms.TabPage();
            this.listViewPoints = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.labelDataPointsTitle = new System.Windows.Forms.Label();
            this.tabAxes = new System.Windows.Forms.TabPage();
            this.labelAxesAngle = new System.Windows.Forms.Label();
            this.groupXAxis = new System.Windows.Forms.GroupBox();
            this.checkXLogScale = new System.Windows.Forms.CheckBox();
            this.checkSetX2 = new System.Windows.Forms.CheckBox();
            this.checkSetX1 = new System.Windows.Forms.CheckBox();
            this.labelXValue1 = new System.Windows.Forms.Label();
            this.textX1 = new System.Windows.Forms.TextBox();
            this.textX2 = new System.Windows.Forms.TextBox();
            this.labelXValue2 = new System.Windows.Forms.Label();
            this.groupYAxis = new System.Windows.Forms.GroupBox();
            this.checkYLogScale = new System.Windows.Forms.CheckBox();
            this.textY2 = new System.Windows.Forms.TextBox();
            this.checkSetY2 = new System.Windows.Forms.CheckBox();
            this.labelYValue2 = new System.Windows.Forms.Label();
            this.checkSetY1 = new System.Windows.Forms.CheckBox();
            this.textY1 = new System.Windows.Forms.TextBox();
            this.labelYValue1 = new System.Windows.Forms.Label();
            this.labelAxesCalibration = new System.Windows.Forms.Label();
            this.labelZoom2 = new System.Windows.Forms.Label();
            this.linkLabelAlert = new System.Windows.Forms.LinkLabel();
            this.mainMenu1.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.toolBar1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updownZoom)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPoints.SuspendLayout();
            this.tabAxes.SuspendLayout();
            this.groupXAxis.SuspendLayout();
            this.groupYAxis.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuEdit,
            this.menuView,
            this.menuItem1});
            this.mainMenu1.Location = new System.Drawing.Point(0, 0);
            this.mainMenu1.Name = "mainMenu1";
            this.mainMenu1.Size = new System.Drawing.Size(696, 24);
            this.mainMenu1.TabIndex = 0;
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOpen,
            this.menuPaste,
            this.menuScanner,
            this.menuItem3,
            this.menuSave,
            this.menuCopy,
            this.menuItem4,
            this.menuExit});
            this.menuFile.MergeIndex = 0;
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(37, 20);
            this.menuFile.Text = "File";
            // 
            // menuOpen
            // 
            this.menuOpen.Image = ((System.Drawing.Image)(resources.GetObject("menuOpen.Image")));
            this.menuOpen.MergeIndex = 0;
            this.menuOpen.Name = "menuOpen";
            this.menuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuOpen.Size = new System.Drawing.Size(225, 22);
            this.menuOpen.Text = "&Open...";
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // menuPaste
            // 
            this.menuPaste.Image = ((System.Drawing.Image)(resources.GetObject("menuPaste.Image")));
            this.menuPaste.MergeIndex = 1;
            this.menuPaste.Name = "menuPaste";
            this.menuPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.menuPaste.Size = new System.Drawing.Size(225, 22);
            this.menuPaste.Text = "&Paste from clipboard";
            this.menuPaste.Click += new System.EventHandler(this.menuPaste_Click);
            // 
            // menuScanner
            // 
            this.menuScanner.MergeIndex = 2;
            this.menuScanner.Name = "menuScanner";
            this.menuScanner.Size = new System.Drawing.Size(225, 22);
            this.menuScanner.Text = "From scanner or camera...";
            this.menuScanner.Click += new System.EventHandler(this.menuScanner_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.MergeIndex = 3;
            this.menuItem3.Name = "menuItem3";
            this.menuItem3.Size = new System.Drawing.Size(222, 6);
            // 
            // menuSave
            // 
            this.menuSave.Enabled = false;
            this.menuSave.Image = ((System.Drawing.Image)(resources.GetObject("menuSave.Image")));
            this.menuSave.MergeIndex = 4;
            this.menuSave.Name = "menuSave";
            this.menuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSave.Size = new System.Drawing.Size(225, 22);
            this.menuSave.Text = "&Save data points...";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuCopy
            // 
            this.menuCopy.Enabled = false;
            this.menuCopy.Image = ((System.Drawing.Image)(resources.GetObject("menuCopy.Image")));
            this.menuCopy.MergeIndex = 5;
            this.menuCopy.Name = "menuCopy";
            this.menuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.menuCopy.Size = new System.Drawing.Size(225, 22);
            this.menuCopy.Text = "&Copy to clipboard...";
            this.menuCopy.Click += new System.EventHandler(this.menuCopy_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.MergeIndex = 6;
            this.menuItem4.Name = "menuItem4";
            this.menuItem4.Size = new System.Drawing.Size(222, 6);
            // 
            // menuExit
            // 
            this.menuExit.MergeIndex = 7;
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(225, 22);
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuEdit
            // 
            this.menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDeleteSelectedPoint,
            this.menuClearAllPoints});
            this.menuEdit.MergeIndex = 1;
            this.menuEdit.Name = "menuEdit";
            this.menuEdit.Size = new System.Drawing.Size(39, 20);
            this.menuEdit.Text = "Edit";
            // 
            // menuDeleteSelectedPoint
            // 
            this.menuDeleteSelectedPoint.Enabled = false;
            this.menuDeleteSelectedPoint.Image = ((System.Drawing.Image)(resources.GetObject("menuDeleteSelectedPoint.Image")));
            this.menuDeleteSelectedPoint.MergeIndex = 0;
            this.menuDeleteSelectedPoint.Name = "menuDeleteSelectedPoint";
            this.menuDeleteSelectedPoint.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.menuDeleteSelectedPoint.Size = new System.Drawing.Size(208, 22);
            this.menuDeleteSelectedPoint.Text = "Delete selected point";
            this.menuDeleteSelectedPoint.Click += new System.EventHandler(this.menuDeleteSelectedPoint_Click);
            // 
            // menuClearAllPoints
            // 
            this.menuClearAllPoints.Enabled = false;
            this.menuClearAllPoints.MergeIndex = 1;
            this.menuClearAllPoints.Name = "menuClearAllPoints";
            this.menuClearAllPoints.Size = new System.Drawing.Size(208, 22);
            this.menuClearAllPoints.Text = "Clear all";
            this.menuClearAllPoints.Click += new System.EventHandler(this.menuClearAllPoints_Click);
            // 
            // menuView
            // 
            this.menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuZoom,
            this.menuRotate,
            this.menuMarkerSize,
            this.menuMarkerType,
            this.menuColors,
            this.menuShowImageNavigator,
            this.menuSortPoints});
            this.menuView.MergeIndex = 2;
            this.menuView.Name = "menuView";
            this.menuView.Size = new System.Drawing.Size(44, 20);
            this.menuView.Text = "View";
            // 
            // menuZoom
            // 
            this.menuZoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuZoomFit,
            this.menuZoom50,
            this.menuZoom100,
            this.menuZoom200,
            this.menuZoom400});
            this.menuZoom.Enabled = false;
            this.menuZoom.Image = ((System.Drawing.Image)(resources.GetObject("menuZoom.Image")));
            this.menuZoom.MergeIndex = 0;
            this.menuZoom.Name = "menuZoom";
            this.menuZoom.Size = new System.Drawing.Size(192, 22);
            this.menuZoom.Text = "Zoom";
            // 
            // menuZoomFit
            // 
            this.menuZoomFit.MergeIndex = 0;
            this.menuZoomFit.Name = "menuZoomFit";
            this.menuZoomFit.Size = new System.Drawing.Size(105, 22);
            this.menuZoomFit.Text = "Fit";
            this.menuZoomFit.Click += new System.EventHandler(this.menuZoomFit_Click);
            // 
            // menuZoom50
            // 
            this.menuZoom50.MergeIndex = 1;
            this.menuZoom50.Name = "menuZoom50";
            this.menuZoom50.Size = new System.Drawing.Size(105, 22);
            this.menuZoom50.Text = "50 %";
            this.menuZoom50.Click += new System.EventHandler(this.menuZoomX_Click);
            // 
            // menuZoom100
            // 
            this.menuZoom100.Checked = true;
            this.menuZoom100.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuZoom100.MergeIndex = 2;
            this.menuZoom100.Name = "menuZoom100";
            this.menuZoom100.Size = new System.Drawing.Size(105, 22);
            this.menuZoom100.Text = "100 %";
            this.menuZoom100.Click += new System.EventHandler(this.menuZoomX_Click);
            // 
            // menuZoom200
            // 
            this.menuZoom200.MergeIndex = 3;
            this.menuZoom200.Name = "menuZoom200";
            this.menuZoom200.Size = new System.Drawing.Size(105, 22);
            this.menuZoom200.Text = "200 %";
            this.menuZoom200.Click += new System.EventHandler(this.menuZoomX_Click);
            // 
            // menuZoom400
            // 
            this.menuZoom400.MergeIndex = 4;
            this.menuZoom400.Name = "menuZoom400";
            this.menuZoom400.Size = new System.Drawing.Size(105, 22);
            this.menuZoom400.Text = "400 %";
            this.menuZoom400.Click += new System.EventHandler(this.menuZoomX_Click);
            // 
            // menuRotate
            // 
            this.menuRotate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRotateLeft,
            this.menuRotateRight,
            this.menuRotate180,
            this.menuStraightenAxes,
            this.menuRotateReset});
            this.menuRotate.Enabled = false;
            this.menuRotate.MergeIndex = 1;
            this.menuRotate.Name = "menuRotate";
            this.menuRotate.Size = new System.Drawing.Size(192, 22);
            this.menuRotate.Text = "Rotate";
            // 
            // menuRotateLeft
            // 
            this.menuRotateLeft.MergeIndex = 0;
            this.menuRotateLeft.Name = "menuRotateLeft";
            this.menuRotateLeft.Size = new System.Drawing.Size(206, 22);
            this.menuRotateLeft.Text = "Rotate left";
            this.menuRotateLeft.Click += new System.EventHandler(this.menuRotateX_Click);
            // 
            // menuRotateRight
            // 
            this.menuRotateRight.MergeIndex = 1;
            this.menuRotateRight.Name = "menuRotateRight";
            this.menuRotateRight.Size = new System.Drawing.Size(206, 22);
            this.menuRotateRight.Text = "Rotate right";
            this.menuRotateRight.Click += new System.EventHandler(this.menuRotateX_Click);
            // 
            // menuRotate180
            // 
            this.menuRotate180.MergeIndex = 2;
            this.menuRotate180.Name = "menuRotate180";
            this.menuRotate180.Size = new System.Drawing.Size(206, 22);
            this.menuRotate180.Text = "Rotate 180�";
            this.menuRotate180.Click += new System.EventHandler(this.menuRotateX_Click);
            // 
            // menuStraightenAxes
            // 
            this.menuStraightenAxes.Enabled = false;
            this.menuStraightenAxes.MergeIndex = 3;
            this.menuStraightenAxes.Name = "menuStraightenAxes";
            this.menuStraightenAxes.Size = new System.Drawing.Size(206, 22);
            this.menuStraightenAxes.Text = "Straighten axes";
            this.menuStraightenAxes.Click += new System.EventHandler(this.menuStraightenAxes_Click);
            // 
            // menuRotateReset
            // 
            this.menuRotateReset.MergeIndex = 4;
            this.menuRotateReset.Name = "menuRotateReset";
            this.menuRotateReset.Size = new System.Drawing.Size(206, 22);
            this.menuRotateReset.Text = "Reset original orientation";
            this.menuRotateReset.Click += new System.EventHandler(this.menuRotateReset_Click);
            // 
            // menuMarkerSize
            // 
            this.menuMarkerSize.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMarkerSize5,
            this.menuMarkerSize7,
            this.menuMarkerSize9,
            this.menuMarkerSize11,
            this.menuMarkerSize15});
            this.menuMarkerSize.Enabled = false;
            this.menuMarkerSize.MergeIndex = 2;
            this.menuMarkerSize.Name = "menuMarkerSize";
            this.menuMarkerSize.Size = new System.Drawing.Size(192, 22);
            this.menuMarkerSize.Text = "Marker size";
            // 
            // menuMarkerSize5
            // 
            this.menuMarkerSize5.MergeIndex = 0;
            this.menuMarkerSize5.Name = "menuMarkerSize5";
            this.menuMarkerSize5.Size = new System.Drawing.Size(101, 22);
            this.menuMarkerSize5.Text = "5 px";
            this.menuMarkerSize5.Click += new System.EventHandler(this.menuMarkerSizeX_Click);
            // 
            // menuMarkerSize7
            // 
            this.menuMarkerSize7.MergeIndex = 1;
            this.menuMarkerSize7.Name = "menuMarkerSize7";
            this.menuMarkerSize7.Size = new System.Drawing.Size(101, 22);
            this.menuMarkerSize7.Text = "7 px";
            this.menuMarkerSize7.Click += new System.EventHandler(this.menuMarkerSizeX_Click);
            // 
            // menuMarkerSize9
            // 
            this.menuMarkerSize9.MergeIndex = 2;
            this.menuMarkerSize9.Name = "menuMarkerSize9";
            this.menuMarkerSize9.Size = new System.Drawing.Size(101, 22);
            this.menuMarkerSize9.Text = "9 px";
            this.menuMarkerSize9.Click += new System.EventHandler(this.menuMarkerSizeX_Click);
            // 
            // menuMarkerSize11
            // 
            this.menuMarkerSize11.MergeIndex = 3;
            this.menuMarkerSize11.Name = "menuMarkerSize11";
            this.menuMarkerSize11.Size = new System.Drawing.Size(101, 22);
            this.menuMarkerSize11.Text = "11 px";
            this.menuMarkerSize11.Click += new System.EventHandler(this.menuMarkerSizeX_Click);
            // 
            // menuMarkerSize15
            // 
            this.menuMarkerSize15.MergeIndex = 4;
            this.menuMarkerSize15.Name = "menuMarkerSize15";
            this.menuMarkerSize15.Size = new System.Drawing.Size(101, 22);
            this.menuMarkerSize15.Text = "15 px";
            this.menuMarkerSize15.Click += new System.EventHandler(this.menuMarkerSizeX_Click);
            // 
            // menuMarkerType
            // 
            this.menuMarkerType.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMarkerTypeDot,
            this.menuMarkerTypeCross,
            this.menuMarkerTypeFatCross});
            this.menuMarkerType.Enabled = false;
            this.menuMarkerType.MergeIndex = 3;
            this.menuMarkerType.Name = "menuMarkerType";
            this.menuMarkerType.Size = new System.Drawing.Size(192, 22);
            this.menuMarkerType.Text = "Marker type";
            // 
            // menuMarkerTypeDot
            // 
            this.menuMarkerTypeDot.MergeIndex = 0;
            this.menuMarkerTypeDot.Name = "menuMarkerTypeDot";
            this.menuMarkerTypeDot.Size = new System.Drawing.Size(120, 22);
            this.menuMarkerTypeDot.Text = "Dot";
            this.menuMarkerTypeDot.Click += new System.EventHandler(this.menuMarkerTypeX_Click);
            // 
            // menuMarkerTypeCross
            // 
            this.menuMarkerTypeCross.MergeIndex = 1;
            this.menuMarkerTypeCross.Name = "menuMarkerTypeCross";
            this.menuMarkerTypeCross.Size = new System.Drawing.Size(120, 22);
            this.menuMarkerTypeCross.Text = "Cross";
            this.menuMarkerTypeCross.Click += new System.EventHandler(this.menuMarkerTypeX_Click);
            // 
            // menuMarkerTypeFatCross
            // 
            this.menuMarkerTypeFatCross.MergeIndex = 2;
            this.menuMarkerTypeFatCross.Name = "menuMarkerTypeFatCross";
            this.menuMarkerTypeFatCross.Size = new System.Drawing.Size(120, 22);
            this.menuMarkerTypeFatCross.Text = "Fat cross";
            this.menuMarkerTypeFatCross.Click += new System.EventHandler(this.menuMarkerTypeX_Click);
            // 
            // menuColors
            // 
            this.menuColors.Enabled = false;
            this.menuColors.Image = ((System.Drawing.Image)(resources.GetObject("menuColors.Image")));
            this.menuColors.MergeIndex = 4;
            this.menuColors.Name = "menuColors";
            this.menuColors.Size = new System.Drawing.Size(192, 22);
            this.menuColors.Text = "Colors...";
            this.menuColors.Click += new System.EventHandler(this.menuColors_Click);
            // 
            // menuShowImageNavigator
            // 
            this.menuShowImageNavigator.Checked = true;
            this.menuShowImageNavigator.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuShowImageNavigator.Enabled = false;
            this.menuShowImageNavigator.MergeIndex = 5;
            this.menuShowImageNavigator.Name = "menuShowImageNavigator";
            this.menuShowImageNavigator.Size = new System.Drawing.Size(192, 22);
            this.menuShowImageNavigator.Text = "Show image navigator";
            this.menuShowImageNavigator.Click += new System.EventHandler(this.menuZoomControl_Click);
            // 
            // menuSortPoints
            // 
            this.menuSortPoints.Enabled = false;
            this.menuSortPoints.MergeIndex = 6;
            this.menuSortPoints.Name = "menuSortPoints";
            this.menuSortPoints.Size = new System.Drawing.Size(192, 22);
            this.menuSortPoints.Text = "Sort points";
            this.menuSortPoints.Click += new System.EventHandler(this.menuSortPoints_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAbout});
            this.menuItem1.MergeIndex = 3;
            this.menuItem1.Name = "menuItem1";
            this.menuItem1.Size = new System.Drawing.Size(44, 20);
            this.menuItem1.Text = "Help";
            // 
            // menuAbout
            // 
            this.menuAbout.MergeIndex = 0;
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(176, 22);
            this.menuAbout.Text = "About PlotReader...";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarPanel1,
            this.statusBarImage,
            this.statusBarPanel2,
            this.statusBarPlot,
            this.statusBarStatus});
            this.statusBar.Location = new System.Drawing.Point(0, 575);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(696, 24);
            this.statusBar.TabIndex = 1;
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Size = new System.Drawing.Size(43, 19);
            this.statusBarPanel1.Text = "Image:";
            // 
            // statusBarImage
            // 
            this.statusBarImage.AutoSize = false;
            this.statusBarImage.Name = "statusBarImage";
            this.statusBarImage.Size = new System.Drawing.Size(75, 19);
            this.statusBarImage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusBarPanel2
            // 
            this.statusBarPanel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.statusBarPanel2.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.statusBarPanel2.Name = "statusBarPanel2";
            this.statusBarPanel2.Size = new System.Drawing.Size(35, 19);
            this.statusBarPanel2.Text = "Plot:";
            // 
            // statusBarPlot
            // 
            this.statusBarPlot.AutoSize = false;
            this.statusBarPlot.Name = "statusBarPlot";
            this.statusBarPlot.Size = new System.Drawing.Size(75, 19);
            this.statusBarPlot.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusBarStatus
            // 
            this.statusBarStatus.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.statusBarStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.statusBarStatus.Name = "statusBarStatus";
            this.statusBarStatus.Size = new System.Drawing.Size(453, 19);
            this.statusBarStatus.Spring = true;
            this.statusBarStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Image files (*.jpeg; *.gif; *.tiff; *.bmp; *.png; *.emf; *.wmf;*.ico)|*.jpg;*.jpe" +
                "g;*.gif;*.tif;*.tiff;*.bmp;*.png;*.emf;*.wmf;*.ico|All files (*.*)|*.*";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Character-separated text (*.csv)|*.csv|All files (*.*)|*.*";
            // 
            // toolBar1
            // 
            this.toolBar1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolBar1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBarOpen,
            this.toolBarPaste,
            this.toolBarButton1,
            this.toolBarSave,
            this.toolBarCopy,
            this.toolBarButton2,
            this.toolBarMove,
            this.toolBarAdd,
            this.toolBarSelect,
            this.toolStripSeparator1,
            this.toolStripCheckBoxAutoCenter,
            this.toolStripTextBoxTolerance});
            this.toolBar1.Location = new System.Drawing.Point(0, 24);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.Size = new System.Drawing.Size(696, 25);
            this.toolBar1.TabIndex = 7;
            // 
            // toolBarOpen
            // 
            this.toolBarOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolBarOpen.Image")));
            this.toolBarOpen.Name = "toolBarOpen";
            this.toolBarOpen.Size = new System.Drawing.Size(23, 22);
            this.toolBarOpen.ToolTipText = "Open an image from file";
            this.toolBarOpen.Click += new System.EventHandler(this.toolBarOpen_Click);
            // 
            // toolBarPaste
            // 
            this.toolBarPaste.Image = ((System.Drawing.Image)(resources.GetObject("toolBarPaste.Image")));
            this.toolBarPaste.Name = "toolBarPaste";
            this.toolBarPaste.Size = new System.Drawing.Size(23, 22);
            this.toolBarPaste.ToolTipText = "Paste image from clipboard";
            this.toolBarPaste.Click += new System.EventHandler(this.toolBarPaste_Click);
            // 
            // toolBarButton1
            // 
            this.toolBarButton1.Name = "toolBarButton1";
            this.toolBarButton1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolBarSave
            // 
            this.toolBarSave.Enabled = false;
            this.toolBarSave.Image = ((System.Drawing.Image)(resources.GetObject("toolBarSave.Image")));
            this.toolBarSave.Name = "toolBarSave";
            this.toolBarSave.Size = new System.Drawing.Size(23, 22);
            this.toolBarSave.ToolTipText = "Save data points to file";
            this.toolBarSave.Click += new System.EventHandler(this.toolBarSave_Click);
            // 
            // toolBarCopy
            // 
            this.toolBarCopy.Enabled = false;
            this.toolBarCopy.Image = ((System.Drawing.Image)(resources.GetObject("toolBarCopy.Image")));
            this.toolBarCopy.Name = "toolBarCopy";
            this.toolBarCopy.Size = new System.Drawing.Size(23, 22);
            this.toolBarCopy.ToolTipText = "Copy data points to clipboard";
            this.toolBarCopy.Click += new System.EventHandler(this.toolBarCopy_Click);
            // 
            // toolBarButton2
            // 
            this.toolBarButton2.Name = "toolBarButton2";
            this.toolBarButton2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolBarMove
            // 
            this.toolBarMove.Enabled = false;
            this.toolBarMove.Image = ((System.Drawing.Image)(resources.GetObject("toolBarMove.Image")));
            this.toolBarMove.Name = "toolBarMove";
            this.toolBarMove.Size = new System.Drawing.Size(23, 22);
            this.toolBarMove.ToolTipText = "Drag image";
            this.toolBarMove.Click += new System.EventHandler(this.toolBarMove_Click);
            // 
            // toolBarAdd
            // 
            this.toolBarAdd.Enabled = false;
            this.toolBarAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolBarAdd.Image")));
            this.toolBarAdd.Name = "toolBarAdd";
            this.toolBarAdd.Size = new System.Drawing.Size(23, 22);
            this.toolBarAdd.ToolTipText = "Add data points";
            this.toolBarAdd.Click += new System.EventHandler(this.toolBarAdd_Click);
            // 
            // toolBarSelect
            // 
            this.toolBarSelect.Enabled = false;
            this.toolBarSelect.Image = ((System.Drawing.Image)(resources.GetObject("toolBarSelect.Image")));
            this.toolBarSelect.Name = "toolBarSelect";
            this.toolBarSelect.Size = new System.Drawing.Size(23, 22);
            this.toolBarSelect.ToolTipText = "Select data points";
            this.toolBarSelect.Click += new System.EventHandler(this.toolBarSelect_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripCheckBoxAutoCenter
            // 
            this.toolStripCheckBoxAutoCenter.BackColor = System.Drawing.Color.Transparent;
            // 
            // toolStripCheckBoxAutoCenter
            // 
            this.toolStripCheckBoxAutoCenter.CheckBoxControl.AccessibleName = "toolStripCheckBoxAutoCenter";
            this.toolStripCheckBoxAutoCenter.CheckBoxControl.BackColor = System.Drawing.Color.Transparent;
            this.toolStripCheckBoxAutoCenter.CheckBoxControl.Enabled = false;
            this.toolStripCheckBoxAutoCenter.CheckBoxControl.Location = new System.Drawing.Point(179, 1);
            this.toolStripCheckBoxAutoCenter.CheckBoxControl.Name = "toolStripCheckBox1";
            this.toolStripCheckBoxAutoCenter.CheckBoxControl.Size = new System.Drawing.Size(88, 22);
            this.toolStripCheckBoxAutoCenter.CheckBoxControl.TabIndex = 1;
            this.toolStripCheckBoxAutoCenter.CheckBoxControl.Text = "auto-center";
            this.toolStripCheckBoxAutoCenter.CheckBoxControl.UseVisualStyleBackColor = false;
            this.toolStripCheckBoxAutoCenter.Enabled = false;
            this.toolStripCheckBoxAutoCenter.Name = "toolStripCheckBoxAutoCenter";
            this.toolStripCheckBoxAutoCenter.Size = new System.Drawing.Size(88, 22);
            this.toolStripCheckBoxAutoCenter.Text = "auto-center";
            this.toolStripCheckBoxAutoCenter.ToolTipText = "Automatically center new points based on neighboring region.";
            this.toolStripCheckBoxAutoCenter.CheckedChanged += new System.EventHandler(this.toolStripCheckBoxAutoCenter_CheckedChanged);
            // 
            // toolStripTextBoxTolerance
            // 
            this.toolStripTextBoxTolerance.Enabled = false;
            this.toolStripTextBoxTolerance.Name = "toolStripTextBoxTolerance";
            this.toolStripTextBoxTolerance.Size = new System.Drawing.Size(30, 25);
            this.toolStripTextBoxTolerance.Text = "20";
            this.toolStripTextBoxTolerance.ToolTipText = "Color tolerance for auto-centering (0-255)";
            this.toolStripTextBoxTolerance.TextChanged += new System.EventHandler(this.toolStripTextBoxTolerance_TextChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Silver;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.plotBox1);
            this.panel1.Controls.Add(this.labelZoom1);
            this.panel1.Controls.Add(this.updownZoom);
            this.panel1.Controls.Add(this.thumnailBox1);
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Controls.Add(this.labelZoom2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 86);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(696, 489);
            this.panel1.TabIndex = 8;
            // 
            // plotBox1
            // 
            this.plotBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.plotBox1.AutoScroll = true;
            this.plotBox1.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.plotBox1.Image = null;
            this.plotBox1.Location = new System.Drawing.Point(0, 0);
            this.plotBox1.MagicWandTolerance = 20F;
            this.plotBox1.MarkerSize = 7;
            this.plotBox1.MarkerType = PlotReader.PlotBox.PlotBoxMarkerType.Dot;
            this.plotBox1.MouseStatus = PlotReader.PlotBox.PlotBoxMouseState.Drag;
            this.plotBox1.Name = "plotBox1";
            this.plotBox1.Rotation = 0F;
            this.plotBox1.SelectedPoint = -1;
            this.plotBox1.SelectedPoints = ((System.Collections.ArrayList)(resources.GetObject("plotBox1.SelectedPoints")));
            this.plotBox1.Size = new System.Drawing.Size(504, 489);
            this.plotBox1.TabIndex = 7;
            this.plotBox1.UseMagicWand = false;
            this.plotBox1.Visible = false;
            this.plotBox1.Zoom = 100;
            this.plotBox1.ZoomToFit = false;
            this.plotBox1.PointAdded += new System.EventHandler(this.plotBox1_PointAdded);
            this.plotBox1.SelectionChanged += new System.EventHandler(this.plotBox1_SelectionChanged);
            this.plotBox1.ZoomChanged += new System.EventHandler(this.plotBox1_ZoomChanged);
            this.plotBox1.MouseEnter += new System.EventHandler(this.plotBox1_MouseEnter);
            this.plotBox1.AxisCoordinateSet += new System.EventHandler(this.plotBox1_AxisCoordinateSet);
            this.plotBox1.StatusTextChanged += new System.EventHandler(this.plotBox1_StatusTextChanged);
            this.plotBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plotBox1_MouseMove);
            this.plotBox1.MouseLeave += new System.EventHandler(this.plotBox1_MouseLeave);
            // 
            // labelZoom1
            // 
            this.labelZoom1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelZoom1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelZoom1.Location = new System.Drawing.Point(548, 99);
            this.labelZoom1.Name = "labelZoom1";
            this.labelZoom1.Size = new System.Drawing.Size(32, 16);
            this.labelZoom1.TabIndex = 12;
            this.labelZoom1.Text = "Zoom:";
            this.labelZoom1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelZoom1.Visible = false;
            // 
            // updownZoom
            // 
            this.updownZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updownZoom.Enabled = false;
            this.updownZoom.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.updownZoom.Location = new System.Drawing.Point(583, 95);
            this.updownZoom.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.updownZoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownZoom.Name = "updownZoom";
            this.updownZoom.Size = new System.Drawing.Size(48, 20);
            this.updownZoom.TabIndex = 9;
            this.updownZoom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.updownZoom.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.updownZoom.Visible = false;
            this.updownZoom.ValueChanged += new System.EventHandler(this.updownZoom_ValueChanged);
            // 
            // thumnailBox1
            // 
            this.thumnailBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.thumnailBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.thumnailBox1.Location = new System.Drawing.Point(510, 0);
            this.thumnailBox1.Name = "thumnailBox1";
            this.thumnailBox1.Owner = this.plotBox1;
            this.thumnailBox1.Size = new System.Drawing.Size(175, 92);
            this.thumnailBox1.TabIndex = 8;
            this.thumnailBox1.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPoints);
            this.tabControl1.Controls.Add(this.tabAxes);
            this.tabControl1.Enabled = false;
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(504, 126);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(192, 363);
            this.tabControl1.TabIndex = 10;
            this.tabControl1.Visible = false;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPoints
            // 
            this.tabPoints.Controls.Add(this.listViewPoints);
            this.tabPoints.Controls.Add(this.labelDataPointsTitle);
            this.tabPoints.Location = new System.Drawing.Point(4, 22);
            this.tabPoints.Name = "tabPoints";
            this.tabPoints.Size = new System.Drawing.Size(184, 337);
            this.tabPoints.TabIndex = 0;
            this.tabPoints.Text = "Points";
            // 
            // listViewPoints
            // 
            this.listViewPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewPoints.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewPoints.FullRowSelect = true;
            this.listViewPoints.HideSelection = false;
            this.listViewPoints.Location = new System.Drawing.Point(7, 25);
            this.listViewPoints.Name = "listViewPoints";
            this.listViewPoints.Size = new System.Drawing.Size(169, 304);
            this.listViewPoints.TabIndex = 1;
            this.listViewPoints.UseCompatibleStateImageBehavior = false;
            this.listViewPoints.View = System.Windows.Forms.View.Details;
            this.listViewPoints.SelectedIndexChanged += new System.EventHandler(this.listViewPoints_SelectedIndexChanged);
            this.listViewPoints.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewPoints_ColumnClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "x";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "y";
            // 
            // labelDataPointsTitle
            // 
            this.labelDataPointsTitle.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelDataPointsTitle.Location = new System.Drawing.Point(8, 8);
            this.labelDataPointsTitle.Name = "labelDataPointsTitle";
            this.labelDataPointsTitle.Size = new System.Drawing.Size(120, 23);
            this.labelDataPointsTitle.TabIndex = 0;
            this.labelDataPointsTitle.Text = "Identified data points:";
            // 
            // tabAxes
            // 
            this.tabAxes.Controls.Add(this.labelAxesAngle);
            this.tabAxes.Controls.Add(this.groupXAxis);
            this.tabAxes.Controls.Add(this.groupYAxis);
            this.tabAxes.Controls.Add(this.labelAxesCalibration);
            this.tabAxes.Location = new System.Drawing.Point(4, 22);
            this.tabAxes.Name = "tabAxes";
            this.tabAxes.Size = new System.Drawing.Size(184, 337);
            this.tabAxes.TabIndex = 1;
            this.tabAxes.Text = "Axes";
            // 
            // labelAxesAngle
            // 
            this.labelAxesAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelAxesAngle.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelAxesAngle.Location = new System.Drawing.Point(16, 344);
            this.labelAxesAngle.Name = "labelAxesAngle";
            this.labelAxesAngle.Size = new System.Drawing.Size(160, 16);
            this.labelAxesAngle.TabIndex = 5;
            this.labelAxesAngle.Text = "Angle between axes:";
            this.labelAxesAngle.Visible = false;
            // 
            // groupXAxis
            // 
            this.groupXAxis.Controls.Add(this.checkXLogScale);
            this.groupXAxis.Controls.Add(this.checkSetX2);
            this.groupXAxis.Controls.Add(this.checkSetX1);
            this.groupXAxis.Controls.Add(this.labelXValue1);
            this.groupXAxis.Controls.Add(this.textX1);
            this.groupXAxis.Controls.Add(this.textX2);
            this.groupXAxis.Controls.Add(this.labelXValue2);
            this.groupXAxis.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupXAxis.Location = new System.Drawing.Point(8, 104);
            this.groupXAxis.Name = "groupXAxis";
            this.groupXAxis.Size = new System.Drawing.Size(160, 112);
            this.groupXAxis.TabIndex = 0;
            this.groupXAxis.TabStop = false;
            this.groupXAxis.Text = "x-axis";
            // 
            // checkXLogScale
            // 
            this.checkXLogScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkXLogScale.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkXLogScale.Location = new System.Drawing.Point(8, 88);
            this.checkXLogScale.Name = "checkXLogScale";
            this.checkXLogScale.Size = new System.Drawing.Size(144, 16);
            this.checkXLogScale.TabIndex = 5;
            this.checkXLogScale.Text = "Logarithmic scale";
            this.checkXLogScale.CheckedChanged += new System.EventHandler(this.checkXLogScale_CheckedChanged);
            // 
            // checkSetX2
            // 
            this.checkSetX2.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkSetX2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkSetX2.Location = new System.Drawing.Point(8, 56);
            this.checkSetX2.Name = "checkSetX2";
            this.checkSetX2.Size = new System.Drawing.Size(40, 24);
            this.checkSetX2.TabIndex = 3;
            this.checkSetX2.Text = "Set 2";
            this.checkSetX2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkSetX2.Click += new System.EventHandler(this.buttonSetX2_Click);
            // 
            // checkSetX1
            // 
            this.checkSetX1.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkSetX1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkSetX1.Location = new System.Drawing.Point(8, 22);
            this.checkSetX1.Name = "checkSetX1";
            this.checkSetX1.Size = new System.Drawing.Size(40, 24);
            this.checkSetX1.TabIndex = 1;
            this.checkSetX1.Text = "Set 1";
            this.checkSetX1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkSetX1.Click += new System.EventHandler(this.buttonSetX1_Click);
            // 
            // labelXValue1
            // 
            this.labelXValue1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelXValue1.Location = new System.Drawing.Point(64, 26);
            this.labelXValue1.Name = "labelXValue1";
            this.labelXValue1.Size = new System.Drawing.Size(40, 16);
            this.labelXValue1.TabIndex = 1;
            this.labelXValue1.Text = "x value:";
            // 
            // textX1
            // 
            this.textX1.Location = new System.Drawing.Point(112, 24);
            this.textX1.MaxLength = 20;
            this.textX1.Name = "textX1";
            this.textX1.Size = new System.Drawing.Size(40, 20);
            this.textX1.TabIndex = 2;
            this.textX1.Text = "0";
            this.textX1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textX1.TextChanged += new System.EventHandler(this.textX1_TextChanged);
            // 
            // textX2
            // 
            this.textX2.Location = new System.Drawing.Point(112, 58);
            this.textX2.MaxLength = 20;
            this.textX2.Name = "textX2";
            this.textX2.Size = new System.Drawing.Size(40, 20);
            this.textX2.TabIndex = 4;
            this.textX2.Text = "0";
            this.textX2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textX2.TextChanged += new System.EventHandler(this.textX2_TextChanged);
            // 
            // labelXValue2
            // 
            this.labelXValue2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelXValue2.Location = new System.Drawing.Point(64, 60);
            this.labelXValue2.Name = "labelXValue2";
            this.labelXValue2.Size = new System.Drawing.Size(40, 16);
            this.labelXValue2.TabIndex = 1;
            this.labelXValue2.Text = "x value:";
            // 
            // groupYAxis
            // 
            this.groupYAxis.Controls.Add(this.checkYLogScale);
            this.groupYAxis.Controls.Add(this.textY2);
            this.groupYAxis.Controls.Add(this.checkSetY2);
            this.groupYAxis.Controls.Add(this.labelYValue2);
            this.groupYAxis.Controls.Add(this.checkSetY1);
            this.groupYAxis.Controls.Add(this.textY1);
            this.groupYAxis.Controls.Add(this.labelYValue1);
            this.groupYAxis.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupYAxis.Location = new System.Drawing.Point(8, 224);
            this.groupYAxis.Name = "groupYAxis";
            this.groupYAxis.Size = new System.Drawing.Size(160, 112);
            this.groupYAxis.TabIndex = 1;
            this.groupYAxis.TabStop = false;
            this.groupYAxis.Text = "y-axis";
            // 
            // checkYLogScale
            // 
            this.checkYLogScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkYLogScale.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkYLogScale.Location = new System.Drawing.Point(8, 88);
            this.checkYLogScale.Name = "checkYLogScale";
            this.checkYLogScale.Size = new System.Drawing.Size(144, 16);
            this.checkYLogScale.TabIndex = 10;
            this.checkYLogScale.Text = "Logarithmic scale";
            this.checkYLogScale.CheckedChanged += new System.EventHandler(this.checkYLogScale_CheckedChanged);
            // 
            // textY2
            // 
            this.textY2.Location = new System.Drawing.Point(112, 56);
            this.textY2.MaxLength = 20;
            this.textY2.Name = "textY2";
            this.textY2.Size = new System.Drawing.Size(40, 20);
            this.textY2.TabIndex = 9;
            this.textY2.Text = "0";
            this.textY2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textY2.TextChanged += new System.EventHandler(this.textY2_TextChanged);
            // 
            // checkSetY2
            // 
            this.checkSetY2.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkSetY2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkSetY2.Location = new System.Drawing.Point(8, 54);
            this.checkSetY2.Name = "checkSetY2";
            this.checkSetY2.Size = new System.Drawing.Size(40, 24);
            this.checkSetY2.TabIndex = 8;
            this.checkSetY2.Text = "Set 2";
            this.checkSetY2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkSetY2.Click += new System.EventHandler(this.buttonSetY2_Click);
            // 
            // labelYValue2
            // 
            this.labelYValue2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelYValue2.Location = new System.Drawing.Point(64, 58);
            this.labelYValue2.Name = "labelYValue2";
            this.labelYValue2.Size = new System.Drawing.Size(48, 16);
            this.labelYValue2.TabIndex = 1;
            this.labelYValue2.Text = "y value:";
            this.labelYValue2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkSetY1
            // 
            this.checkSetY1.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkSetY1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.checkSetY1.Location = new System.Drawing.Point(8, 24);
            this.checkSetY1.Name = "checkSetY1";
            this.checkSetY1.Size = new System.Drawing.Size(40, 24);
            this.checkSetY1.TabIndex = 6;
            this.checkSetY1.Text = "Set 1";
            this.checkSetY1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkSetY1.Click += new System.EventHandler(this.buttonSetY1_Click);
            // 
            // textY1
            // 
            this.textY1.Location = new System.Drawing.Point(112, 26);
            this.textY1.MaxLength = 20;
            this.textY1.Name = "textY1";
            this.textY1.Size = new System.Drawing.Size(40, 20);
            this.textY1.TabIndex = 7;
            this.textY1.Text = "0";
            this.textY1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textY1.TextChanged += new System.EventHandler(this.textY1_TextChanged);
            // 
            // labelYValue1
            // 
            this.labelYValue1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelYValue1.Location = new System.Drawing.Point(64, 28);
            this.labelYValue1.Name = "labelYValue1";
            this.labelYValue1.Size = new System.Drawing.Size(48, 16);
            this.labelYValue1.TabIndex = 1;
            this.labelYValue1.Text = "y value:";
            this.labelYValue1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelAxesCalibration
            // 
            this.labelAxesCalibration.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelAxesCalibration.Location = new System.Drawing.Point(8, 8);
            this.labelAxesCalibration.Name = "labelAxesCalibration";
            this.labelAxesCalibration.Size = new System.Drawing.Size(168, 112);
            this.labelAxesCalibration.TabIndex = 0;
            this.labelAxesCalibration.Text = resources.GetString("labelAxesCalibration.Text");
            // 
            // labelZoom2
            // 
            this.labelZoom2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelZoom2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelZoom2.Location = new System.Drawing.Point(636, 99);
            this.labelZoom2.Name = "labelZoom2";
            this.labelZoom2.Size = new System.Drawing.Size(8, 16);
            this.labelZoom2.TabIndex = 11;
            this.labelZoom2.Text = "%";
            this.labelZoom2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelZoom2.Visible = false;
            // 
            // linkLabelAlert
            // 
            this.linkLabelAlert.BackColor = System.Drawing.SystemColors.Info;
            this.linkLabelAlert.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkLabelAlert.Dock = System.Windows.Forms.DockStyle.Top;
            this.linkLabelAlert.LinkArea = new System.Windows.Forms.LinkArea(218, 12);
            this.linkLabelAlert.Location = new System.Drawing.Point(0, 49);
            this.linkLabelAlert.Name = "linkLabelAlert";
            this.linkLabelAlert.Padding = new System.Windows.Forms.Padding(3);
            this.linkLabelAlert.Size = new System.Drawing.Size(696, 37);
            this.linkLabelAlert.TabIndex = 9;
            this.linkLabelAlert.TabStop = true;
            this.linkLabelAlert.Text = resources.GetString("linkLabelAlert.Text");
            this.linkLabelAlert.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabelAlert.UseCompatibleTextRendering = true;
            this.linkLabelAlert.Visible = false;
            this.linkLabelAlert.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAlert_LinkClicked);
            // 
            // FormMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(696, 599);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.linkLabelAlert);
            this.Controls.Add(this.toolBar1);
            this.Controls.Add(this.mainMenu1);
            this.Controls.Add(this.statusBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu1;
            this.MinimumSize = new System.Drawing.Size(704, 600);
            this.Name = "FormMain";
            this.Text = "PlotReader";
            this.mainMenu1.ResumeLayout(false);
            this.mainMenu1.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.toolBar1.ResumeLayout(false);
            this.toolBar1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.updownZoom)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPoints.ResumeLayout(false);
            this.tabAxes.ResumeLayout(false);
            this.groupXAxis.ResumeLayout(false);
            this.groupXAxis.PerformLayout();
            this.groupYAxis.ResumeLayout(false);
            this.groupYAxis.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();

            Application.Run(new FormMain());
        }

        private void OpenImageFile(string strPath)
        {
            System.Drawing.Image image = null;
            try
            {
                image = System.Drawing.Image.FromFile(strPath);
            }
            catch (OutOfMemoryException ex)
            {
                MessageBox.Show(strPath + " is not a valid image file or the image format is not supported.", "PlotReader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading image", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.OpenImage(image);
        }

        protected bool m_bWarningShown;
        private void OpenImage(System.Drawing.Image image)
        {
            // Display all controls (hidden at start-up
            this.plotBox1.Visible = true;
            this.tabControl1.Visible = true;
            if (this.menuShowImageNavigator.Checked)
            {
                this.updownZoom.Visible = true;
                this.labelZoom1.Visible = true;
                this.labelZoom2.Visible = true;
                this.thumnailBox1.Visible = true;
            }

            // Set image.
            System.Drawing.Image oldImage = this.plotBox1.Image;
            this.plotBox1.Image = image;
            if (oldImage != null) oldImage.Dispose();

            // Set zoom factor to 100 %
            this.updownZoom.Value = 100;
            if (this.m_menuZoomChecked != null) this.m_menuZoomChecked.Checked = false;
            this.m_menuZoomChecked = this.menuZoom100;
            this.m_menuZoomChecked.Checked = true;

            // Enable rotation and zoom menus (deactivated at start-up).
            this.menuZoom.Enabled = true;
            this.menuRotate.Enabled = true;
            this.updownZoom.Enabled = true;

            // Enable marker size, marker type, and color menus (deactivated at start-up).
            this.menuMarkerSize.Enabled = true;
            this.menuMarkerType.Enabled = true;
            this.menuColors.Enabled = true;
            this.menuShowImageNavigator.Enabled = true;
            this.menuSortPoints.Enabled = true;

            // Reset calibration values for axes.
            this.textX1.Text = "";
            this.textX2.Text = "";
            this.textY1.Text = "";
            this.textY2.Text = "";

            // By default, no log scales.
            this.checkXLogScale.Checked = false;
            this.checkYLogScale.Checked = false;

            // Clear listbox (remove all points).
            this.listViewPoints.Items.Clear();

            // No data points yet; set menu items accordingly.
            this.menuClearAllPoints.Enabled = false;
            this.menuDeleteSelectedPoint.Enabled = false;
            this.menuSave.Enabled = false;
            this.menuCopy.Enabled = false;
            this.toolBarCopy.Enabled = false;
            this.toolBarSave.Enabled = false;

            // Clear status bar.
            this.SetStatusBarCoordinate(new PointF());

            // Switch to axes-tab.
            this.tabControl1.Enabled = true;
            this.tabControl1.SelectedIndex = 1;
            this.toolBarMove.Enabled = true;
            this.toolBarAdd.Enabled = true;
            this.toolBarSelect.Enabled = true;

            // Reset warning bar.
            this.m_bWarningShown = false;
            this.SetLabelAlert();

            // Activate button for first axis setpoint.
            this.checkSetX1.Select();
        }

        private void menuOpen_Click(object sender, System.EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.OpenImageFile(this.openFileDialog1.FileName);
            }
        }

        private ToolStripMenuItem m_menuZoomChecked;

        private void menuZoomX_Click(object sender, System.EventArgs e)
        {
            if (this.m_menuZoomChecked != null) this.m_menuZoomChecked.Checked = false;
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            string strVal = menuitem.Text.Substring(0, menuitem.Text.IndexOf(" "));
            int iVal = int.Parse(strVal);
            if (iVal != this.plotBox1.Zoom)
            {
                this.plotBox1.Zoom = iVal;
                this.updownZoom.Value = iVal;
                //this.plotBox1.AutoScrollPosition = new Point(0,0);
            }
            this.m_menuZoomChecked = menuitem;
            this.m_menuZoomChecked.Checked = true;
        }

        private void menuZoomFit_Click(object sender, System.EventArgs e)
        {
            if (this.m_menuZoomChecked != null) this.m_menuZoomChecked.Checked = false;
            this.plotBox1.ZoomToFit = true;
            this.UpdateUpDownZoom();
            this.m_menuZoomChecked = this.menuZoomFit;
            this.m_menuZoomChecked.Checked = true;
        }

        private void plotBox1_PointAdded(object sender, System.EventArgs e)
        {
            // Create new data point.
            ArrayList points = this.plotBox1.Points;
            PointF pointImage = (PointF)(points[points.Count - 1]);
            DataPoint dpnew = this.GetDataPoint(points.Count - 1, pointImage);

            // Add new data point at end of the list.
            this.listViewPoints.Items.Add(dpnew);

            // Activate all UI items that require one or more data points.
            this.menuSave.Enabled = true;
            this.menuCopy.Enabled = true;
            this.toolBarSave.Enabled = true;
            this.toolBarCopy.Enabled = true;
            this.menuClearAllPoints.Enabled = true;
        }

        private void plotBox1_SelectionChanged(object sender, System.EventArgs e)
        {
            // Get current PlotBox selection.
            ArrayList sel = this.plotBox1.SelectedPoints;

            // Enumerate over all listbox items to change selection.
            foreach (ListViewItem item in this.listViewPoints.Items) item.Selected = sel.Contains((int)item.Tag);

            if (sel.Count != 0)
            {
                this.menuDeleteSelectedPoint.Enabled = true;
                this.SetStatusBarCoordinate((PointF)(this.plotBox1.Points[(int)(sel[0])]));
            }
            else
            {
                this.menuDeleteSelectedPoint.Enabled = false;
            }
        }

        private void plotBox1_ZoomChanged(object sender, System.EventArgs e)
        {
            this.UpdateUpDownZoom();
            if (!this.plotBox1.ZoomToFit) this.UpdateMenuZoom();
        }

        private void UpdateMenuZoom()
        {
            if (this.m_menuZoomChecked != null) this.m_menuZoomChecked.Checked = false;
            switch (this.plotBox1.Zoom)
            {
                case 50:
                    this.m_menuZoomChecked = this.menuZoom50;
                    break;
                case 100:
                    this.m_menuZoomChecked = this.menuZoom100;
                    break;
                case 200:
                    this.m_menuZoomChecked = this.menuZoom200;
                    break;
                case 400:
                    this.m_menuZoomChecked = this.menuZoom400;
                    break;
                default:
                    this.m_menuZoomChecked = null;
                    break;
            }
            if (this.m_menuZoomChecked != null) this.m_menuZoomChecked.Checked = true;
        }

        private void UpdateUpDownZoom()
        {
            this.updownZoom.ValueChanged -= new EventHandler(this.updownZoom_ValueChanged);
            this.updownZoom.Value = this.plotBox1.Zoom;
            this.updownZoom.ValueChanged += new EventHandler(this.updownZoom_ValueChanged);
        }

        private bool m_bMouseOnPlotBox;

        private void SetStatusBarCoordinate(PointF pImage)
        {
            if (this.plotBox1.IsPointInImage(pImage))
            {
                this.statusBarImage.Text = String.Format("{0}, {1}", pImage.X.ToString("F01"), pImage.Y.ToString("F01"));
                this.statusBarImage.ToolTipText = "Cursor position in image coordinates (pixels)";
                if (this.plotBox1.AxesState == 2)
                {
                    double[] dPos = this.plotBox1.ImageToTrue(pImage);
                    this.statusBarPlot.Text = String.Format("{0}, {1}", dPos[0].ToString("F03"), dPos[1].ToString("F03"));
                    this.statusBarPlot.ToolTipText = "Cursor position in plot coordinates";
                }
                else
                {
                    this.statusBarPlot.Text = "?, ?";
                    this.statusBarPlot.ToolTipText = "Cannot calculate plot coordinates because axes are not (correctly) configured";
                }
            }
            else
            {
                this.statusBarImage.Text = "";
                this.statusBarPlot.Text = "";
                this.statusBarImage.ToolTipText = "";
                this.statusBarPlot.ToolTipText = "";
            }
        }

        private int[] m_iWaitingForAxis;
        private void plotBox1_AxisCoordinateSet(object sender, System.EventArgs e)
        {
            this.UpdatePoints();
            if (m_iWaitingForAxis[0] == 1)
            {
                if (m_iWaitingForAxis[1] == 1)
                {
                    this.checkSetX1.Checked = false;
                    this.textX1.SelectAll();
                    this.textX1.Focus();
                }
                else
                {
                    this.checkSetX2.Checked = false;
                    this.textX2.SelectAll();
                    this.textX2.Focus();
                }
            }
            else
            {
                if (m_iWaitingForAxis[1] == 1)
                {
                    this.checkSetY1.Checked = false;
                    this.textY1.SelectAll();
                    this.textY1.Focus();
                }
                else
                {
                    this.checkSetY2.Checked = false;
                    this.textY2.SelectAll();
                    this.textY2.Focus();
                }
            }
            m_iWaitingForAxis[0] = 0;
            m_iWaitingForAxis[1] = 0;
            this.toolBarMove.Checked = true;
        }

        private void buttonSetX1_Click(object sender, System.EventArgs e)
        {
            if (this.checkSetX1.Checked)
            {
                this.checkSetX2.Checked = false;
                this.checkSetY1.Checked = false;
                this.checkSetY2.Checked = false;

                this.plotBox1.ExpectAxisCoordinate(1, 1);
                m_iWaitingForAxis[0] = 1;
                m_iWaitingForAxis[1] = 1;

                this.toolBarMove.Checked = false;
            }
            else
            {
                this.plotBox1.StopExpectAxisCoordinate();
                this.toolBarMove.Checked = true;
                this.m_iWaitingForAxis[0] = 0;
                this.m_iWaitingForAxis[1] = 0;
            }
        }

        private void buttonSetX2_Click(object sender, System.EventArgs e)
        {
            if (this.checkSetX2.Checked)
            {
                this.checkSetX1.Checked = false;
                this.checkSetY1.Checked = false;
                this.checkSetY2.Checked = false;

                this.plotBox1.ExpectAxisCoordinate(1, 2);
                m_iWaitingForAxis[0] = 1;
                m_iWaitingForAxis[1] = 2;

                this.toolBarMove.Checked = false;
            }
            else
            {
                this.plotBox1.StopExpectAxisCoordinate();
                this.toolBarMove.Checked = true;
                this.m_iWaitingForAxis[0] = 0;
                this.m_iWaitingForAxis[1] = 0;
            }
        }

        private void buttonSetY1_Click(object sender, System.EventArgs e)
        {
            if (this.checkSetY1.Checked)
            {
                this.checkSetX1.Checked = false;
                this.checkSetX2.Checked = false;
                this.checkSetY2.Checked = false;

                this.plotBox1.ExpectAxisCoordinate(2, 1);
                m_iWaitingForAxis[0] = 2;
                m_iWaitingForAxis[1] = 1;

                this.toolBarMove.Checked = false;
            }
            else
            {
                this.plotBox1.StopExpectAxisCoordinate();
                this.toolBarMove.Checked = true;
                this.m_iWaitingForAxis[0] = 0;
                this.m_iWaitingForAxis[1] = 0;
            }
        }

        private void buttonSetY2_Click(object sender, System.EventArgs e)
        {
            if (this.checkSetY2.Checked)
            {
                this.checkSetX1.Checked = false;
                this.checkSetX2.Checked = false;
                this.checkSetY1.Checked = false;

                this.plotBox1.ExpectAxisCoordinate(2, 2);
                m_iWaitingForAxis[0] = 2;
                m_iWaitingForAxis[1] = 2;

                this.toolBarMove.Checked = false;
            }
            else
            {
                this.plotBox1.StopExpectAxisCoordinate();
                this.toolBarMove.Checked = true;
                this.m_iWaitingForAxis[0] = 0;
                this.m_iWaitingForAxis[1] = 0;
            }
        }

        private void AxisTrueValueChanged(object textbox, PlotBox.PlotBoxAxisType axis, int iNr)
        {
            string text = ((TextBox)textbox).Text;
            if (text == "")
            {
                this.plotBox1.SetAxisTrueCoordinate(axis, iNr, false, 0);
                this.UpdatePoints();
            }
            else
            {
                try
                {
                    double dValue = Double.Parse(text);
                    this.plotBox1.SetAxisTrueCoordinate(axis, iNr, true, dValue);
                }
                catch
                {
                    this.plotBox1.SetAxisTrueCoordinate(axis, iNr, false, 0);
                }
                finally
                {
                    this.UpdatePoints();
                }
            }
        }

        private void textX1_TextChanged(object sender, System.EventArgs e)
        {
            this.AxisTrueValueChanged(sender, PlotBox.PlotBoxAxisType.X, 1);
        }

        private void textX2_TextChanged(object sender, System.EventArgs e)
        {
            this.AxisTrueValueChanged(sender, PlotBox.PlotBoxAxisType.X, 2);
        }

        private void textY1_TextChanged(object sender, System.EventArgs e)
        {
            this.AxisTrueValueChanged(sender, PlotBox.PlotBoxAxisType.Y, 1);
        }

        private void textY2_TextChanged(object sender, System.EventArgs e)
        {
            this.AxisTrueValueChanged(sender, PlotBox.PlotBoxAxisType.Y, 2);
        }

        private void plotBox1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.SetStatusBarCoordinate(this.plotBox1.ScreenToImage(e.X, e.Y));
        }

        private void menuClearAllPoints_Click(object sender, System.EventArgs e)
        {
            this.listViewPoints.Items.Clear();
            this.plotBox1.ClearPoints();
            this.menuClearAllPoints.Enabled = false;
            this.menuDeleteSelectedPoint.Enabled = false;
            this.menuSave.Enabled = false;
            this.menuCopy.Enabled = false;
            this.toolBarCopy.Enabled = false;
            this.toolBarSave.Enabled = false;
        }

        private void UpdatePoints()
        {
            // Get current selection (using PlotBox indices)
            int[] arrSelPoints = (int[])(this.plotBox1.SelectedPoints.ToArray(typeof(int)));

            // Clear current selection in listbox (to prevent selection-changed events).
            foreach (ListViewItem item in this.listViewPoints.Items) item.Selected = false;

            // Remove all items from the listbox.
            this.listViewPoints.Items.Clear();

            // Build list of data points (with renewed coordinates).
            ListViewItem[] lstDataPoints = new ListViewItem[this.plotBox1.Points.Count];
            int iRealIndex = 0;
            foreach (PointF p in this.plotBox1.Points)
            {
                lstDataPoints[iRealIndex] = this.GetDataPoint(iRealIndex, p);
                iRealIndex++;
            }

            // Add data points to listbox.
            this.listViewPoints.Items.AddRange(lstDataPoints);

            // Re-select listbox items.
            foreach (int iPoint in arrSelPoints)
            {
                int iItem = 0;
                foreach (ListViewItem item in this.listViewPoints.Items)
                {
                    // Search through listbox items to find the data point to select.
                    if ((int)item.Tag == iPoint)
                    {
                        this.listViewPoints.Items[iItem].Selected = true;
                        break;
                    }
                    iItem++;
                }
            }

            if (this.plotBox1.AxesState == 2)
            {
                // Axes calibration complete
                this.labelAxesAngle.Text = "Angle between axes: " + this.plotBox1.AxesAngle.ToString("F01") + "°";
                this.labelAxesAngle.Visible = true;
                this.menuStraightenAxes.Enabled = true;
            }
            else
            {
                // Axes calibration not complete.
                this.labelAxesAngle.Visible = false;
                this.menuStraightenAxes.Enabled = false;
            }

            this.SetLabelAlert();
        }

        protected void SetLabelAlert()
        {
            this.linkLabelAlert.Visible = !(this.plotBox1.AxesState == 2 || (this.tabControl1.SelectedIndex==1 && !this.m_bWarningShown));
            if (this.linkLabelAlert.Visible)
            {
                this.m_bWarningShown = true;
                this.linkLabelAlert.Links.Clear();
                this.linkLabelAlert.Text = "The axes have not been fully configured yet. Therefore, PlotReader cannot calculate the true plot coordinates, and you cannot save data points to file or clipboard. To correct this, enter the required information on the axes tab.";
                this.linkLabelAlert.Links.Add(0, 44, 0);
                if (this.tabControl1.SelectedIndex != 1) this.linkLabelAlert.Links.Add(this.linkLabelAlert.Text.Length - 13, 12, 1);
            }
        }

        private string GetAxesWarnings(bool bStrict)
        {
            string res = "";
            foreach (PlotBox.AxisWarning w in this.plotBox1.AxesWarnings)
            {
                if ((w.Warning != PlotBox.AxisWarning.WarningType.CoordinateNotSet
                    && w.Warning != PlotBox.AxisWarning.WarningType.ValueNotSet)
                    || bStrict)
                {
                    res += ("* " + this.AxisWarningToString(w) + "\n");
                }
            }
            return res;
        }

        private string AxisWarningToString(PlotBox.AxisWarning w)
        {
            switch (w.Warning)
            {
                case PlotBox.AxisWarning.WarningType.CoordinateNotSet:
                    return "You have not yet set the location of the " + (w.PointNumber == 1 ? "first" : "second") + " calibration point for the " + (w.Axis == PlotBox.PlotBoxAxisType.X ? "x" : "y") + "-axis.";
                case PlotBox.AxisWarning.WarningType.CoordinatesIdentical:
                    return "You have set the same location for both calibration points of the " + (w.Axis == PlotBox.PlotBoxAxisType.X ? "x" : "y") + "-axis.";
                case PlotBox.AxisWarning.WarningType.AxesParallel:
                    return "Due to the location of the calibration points, the x-axis and y-axis run parallel.";
                case PlotBox.AxisWarning.WarningType.ValueNotSet:
                    return "You have not yet specified the " + (w.PointNumber == 1 ? "first" : "second") + " value for the " + (w.Axis == PlotBox.PlotBoxAxisType.X ? "x" : "y") + "-axis.";
                case PlotBox.AxisWarning.WarningType.ValuesIdentical:
                    return "You specified identical values for the " + (w.Axis == PlotBox.PlotBoxAxisType.X ? "x" : "y") + "-axis.";
                case PlotBox.AxisWarning.WarningType.ValueNonPositive:
                    return "You specified that the " + (w.Axis == PlotBox.PlotBoxAxisType.X ? "x" : "y") + "-axis uses a logarithmic scale, but set its " + (w.PointNumber == 1 ? "first" : "second") + " value to a non-positive number. Non-positive numbers cannot be represented on a log-scale.";
                default:
                    return "Unknown reason " + w.Warning.ToString() + ". This should not happen; e-mail support!";
            }
        }

        private DataPoint GetDataPoint(int index, PointF pointImage)
        {
            if (this.plotBox1.AxesState != 2)
            {
                // Axes calibration is incomplete (true plot coordinate not known).
                return new DataPoint(index);
            }
            else
            {
                double[] arrCoordinates = this.plotBox1.ImageToTrue(pointImage);
                return new DataPoint(index, arrCoordinates[0], arrCoordinates[1]);
            }
        }

        private void menuSave_Click(object sender, System.EventArgs e)
        {
            if (this.plotBox1.AxesState != 2)
            {
                MessageBox.Show(this, "The axes are not fully configured yet, or they are incorrectly configured. Therefore, PlotReader cannot obtain plot coordinates.\n\nDetails:\n" + this.GetAxesWarnings(true), "PlotReader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.m_frmOutput == null) this.m_frmOutput = new FormOutput(this.m_strRegistryRoot + "\\Output");

                if (this.m_frmOutput.ShowDialog(this) == DialogResult.OK)
                {
                    System.Globalization.NumberFormatInfo fmt = this.m_frmOutput.Culture;
                    string nrstyle = this.m_frmOutput.Format;
                    string sep = this.m_frmOutput.Separator;
                    System.IO.StreamWriter objStream = new System.IO.StreamWriter(this.saveFileDialog1.FileName, false, System.Text.Encoding.ASCII);
                    foreach (ListViewItem item in this.listViewPoints.Items)
                    {
                        PointF p = (PointF)this.plotBox1.Points[(int)item.Tag];
                        double[] pp = this.plotBox1.ImageToTrue(p);
                        objStream.WriteLine(pp[0].ToString(nrstyle, fmt) + sep + pp[1].ToString(nrstyle, fmt));
                    }
                    objStream.Close();
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 0)
            {
                // We have moved to the 'Points' tab.

                // Disable calibration point configuration.
                this.plotBox1.StopExpectAxisCoordinate();
                this.checkSetX1.Checked = false;
                this.checkSetX2.Checked = false;
                this.checkSetY1.Checked = false;
                this.checkSetY2.Checked = false;

                // Enable data point addition/selection.
                //this.toolBarAdd.Enabled = true;
                //this.toolBarSelect.Enabled = true;

                // Set mouse mode to data point addition.
                this.toolBarAdd_Click(null, null);
            }
            else
            {
                // We have moved to the 'Axes' tab.

                // Set mouse mode to image dragging.
                this.toolBarMove_Click(null, null);

                // Disable data point addition/selection.
                //this.toolBarAdd.Enabled = false;
                //this.toolBarSelect.Enabled = false;
            }
            this.SetLabelAlert();
        }

        private void menuDeleteSelectedPoint_Click(object sender, System.EventArgs e)
        {
            // If no points selected, do nothing.
            if (this.listViewPoints.SelectedIndices.Count == 0) return;

            // Save array with selection (using PlotBox indices).
            // Get current selection (using PlotBox indices)
            int[] arrSelPoints = (int[])(this.plotBox1.SelectedPoints.ToArray(typeof(int)));

            // Deselect all in listbox and plotbox (to prevent 'selection changed'
            // events while deleting items).
            this.listViewPoints.SelectedItems.Clear();
            this.plotBox1.SelectedPoint = -1;

            // Remove originally selected items from PlotBox.
            this.plotBox1.DeletePoints(arrSelPoints);

            // Make listbox reflect PlotBox contents.
            this.UpdatePoints();

            // If there are still points left, enable the 'Save' and 'Clear all points'
            // menu items.
            bool bHaspoints = (this.plotBox1.Points.Count != 0);
            this.menuClearAllPoints.Enabled = bHaspoints;
            this.menuSave.Enabled = bHaspoints;
            this.menuCopy.Enabled = bHaspoints;
            this.toolBarCopy.Enabled = bHaspoints;
            this.toolBarSave.Enabled = bHaspoints;
        }

        private ToolStripMenuItem m_objMarkerSizeItem;
        private void menuMarkerSizeX_Click(object sender, System.EventArgs e)
        {
            if (this.m_objMarkerSizeItem != null) this.m_objMarkerSizeItem.Checked = false;
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            string val = menuitem.Text.Substring(0, menuitem.Text.IndexOf(" "));
            this.plotBox1.MarkerSize = int.Parse(val);
            this.m_objMarkerSizeItem = menuitem;
            this.m_objMarkerSizeItem.Checked = true;
        }

        private ToolStripMenuItem m_objMarkerTypeItem;
        private void menuMarkerTypeX_Click(object sender, System.EventArgs e)
        {
            if (this.m_objMarkerTypeItem != null) this.m_objMarkerTypeItem.Checked = false;
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            switch (menuitem.Text)
            {
                case "Dot":
                    this.plotBox1.MarkerType = PlotBox.PlotBoxMarkerType.Dot;
                    break;
                case "Cross":
                    this.plotBox1.MarkerType = PlotBox.PlotBoxMarkerType.Cross;
                    break;
                case "Fat cross":
                    this.plotBox1.MarkerType = PlotBox.PlotBoxMarkerType.FatCross;
                    break;
                default:
                    break;
            }
            this.m_objMarkerTypeItem = menuitem;
            this.m_objMarkerTypeItem.Checked = true;
        }

        private void menuRotateX_Click(object sender, System.EventArgs e)
        {
            ToolStripMenuItem menuitem = (ToolStripMenuItem)sender;
            switch (menuitem.Text)
            {
                case "Rotate left":
                    this.plotBox1.Rotation -= 90F;
                    break;
                case "Rotate right":
                    this.plotBox1.Rotation += 90F;
                    break;
                case "Rotate 180�":
                    this.plotBox1.Rotation += 180F;
                    break;
                default:
                    break;
            }
        }

        private void menuStraightenAxes_Click(object sender, System.EventArgs e)
        {
            this.plotBox1.RotateFromAxes();
        }

        private void menuRotateReset_Click(object sender, System.EventArgs e)
        {
            this.plotBox1.Rotation = 0F;
        }

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void checkXLogScale_CheckedChanged(object sender, System.EventArgs e)
        {
            this.plotBox1.SetAxisLogState(PlotBox.PlotBoxAxisType.X, checkXLogScale.Checked);
            this.UpdatePoints();
        }

        private void checkYLogScale_CheckedChanged(object sender, System.EventArgs e)
        {
            this.plotBox1.SetAxisLogState(PlotBox.PlotBoxAxisType.Y, checkYLogScale.Checked);
            this.UpdatePoints();
        }

        private void menuColors_Click(object sender, System.EventArgs e)
        {
            FormColors frm = new FormColors();
            for (int i = 0; i < 4; i++)
            {
                frm.Colors[i] = this.plotBox1.GetColor(i);
                frm.DefaultColors[i] = this.m_arrDefaultColors[i];
            }
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                for (int i = 0; i < 4; i++) this.plotBox1.SetColor(i, frm.Colors[i]);
            }
        }
        private string m_strTempScannedFile;

        private void menuScanner_Click(object sender, System.EventArgs e)
        {
#if WIA1
			// create COM instance of WIA manager
			WIALib.WiaClass wiaManager = new WiaClass();

			if (wiaManager.Devices.Count == 0) 
			{
				MessageBox.Show(this, "No scanner or camera found.", "PlotReader", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
	
			// let user select device
			object selectUsingUI = System.Reflection.Missing.Value;
			WIALib.ItemClass wiaRoot = (ItemClass) wiaManager.Create(ref selectUsingUI );
			
			// If no device was selected (dialog was cancelled), return.
			if (wiaRoot == null) return;

			// this call shows the common WIA dialog to let 
			// the user select a picture:
			WIALib.CollectionClass wiaPics = wiaRoot.GetItemsFromUI(WiaFlag.SingleImage,
				WiaIntent.ImageTypeColor) as CollectionClass;
			
			// If no image was selected (dialog was cancelled), return.
			if (wiaPics == null) return;

			if (wiaPics.Count == 0) 
			{
				// Should not happen; wiaPics should have been null...
				MessageBox.Show(this, "No image was obtained from the scanner or camera.", "PlotReader", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			else if (wiaPics.Count > 1) 
			{
				// Should not happen; we specified WiaFlag.SingleImage...
				MessageBox.Show(this, "Multiple images were obtained from the scanner or camera. However, PlotReader will use the first image only.", "PlotReader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			// Get the first image.
			WIALib.ItemClass wiaItem = (ItemClass) Marshal.CreateWrapperOfType(wiaPics[0], typeof(WIALib.ItemClass));

			// create temporary file for image
			string imageFileName = System.IO.Path.GetTempFileName();    
    
			// transfer picture to our temporary file
			wiaItem.Transfer(imageFileName, false);

			// Open the image.
			this.OpenImageFile(imageFileName);

			// Store location of temporary file, and delete previous (if any)
			this.DeleteTempFile();
			this.m_strTempScannedFile = imageFileName;
#elif WIA2
            const string wiaFormatUnspecified = "{00000000-0000-0000-0000-000000000000}";
            CommonDialogClass wiaDiag = new CommonDialogClass();
            WIA.ImageFile wiaImage = null;

            wiaImage = wiaDiag.ShowAcquireImage(
                    WiaDeviceType.UnspecifiedDeviceType,
                    WiaImageIntent.UnspecifiedIntent,
                    WiaImageBias.MaximizeQuality,
                    wiaFormatUnspecified, true, true, false);
            if (wiaImage == null) return;
            WIA.Vector vector = wiaImage.FileData;
            this.OpenImage(Image.FromStream(new System.IO.MemoryStream((byte[])vector.get_BinaryData())));
#endif
        }

        private void DeleteTempFile()
        {
            if (this.m_strTempScannedFile != "")
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(this.m_strTempScannedFile);
                fi.Delete();
            }
        }

        private void menuAbout_Click(object sender, System.EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            formAbout.ShowDialog(this);
        }

        private bool IsScanningPossible()
        {
#if WIA1
			// Try to create main scanning object. If this fails, return false.
			try
			{
				WIALib.WiaClass wiaManager = new WiaClass();
				if (wiaManager == null) return false;
			}
			catch 
			{
				return false;
			}
			return true;
#elif WIA2
            try
            {
                CommonDialogClass wiaDiag = new CommonDialogClass();
                if (wiaDiag == null) return false;
            }
            catch
            {
                return false;
            }
            return true;
#else
            return false;
#endif
        }

        class Win32
        {
            public const uint CF_METAFILEPICT = 3;
            public const uint CF_ENHMETAFILE = 14;
            public const uint CF_BITMAP = 2;

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern bool OpenClipboard(IntPtr hWndNewOwner);

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern bool CloseClipboard();

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern IntPtr GetClipboardData(uint format);

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern bool IsClipboardFormatAvailable(uint format);
        }

        private void menuPaste_Click(object sender, System.EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();

            // Try to get metafile (vector format) from clipboard.
            if (iData.GetDataPresent(typeof(System.Drawing.Imaging.Metafile)))
            {
                this.OpenImage((System.Drawing.Imaging.Metafile)(iData.GetData(typeof(System.Drawing.Imaging.Metafile))));
            }
            // Try to get bitmap format from clipboard.
            else if (iData.GetDataPresent(typeof(System.Drawing.Bitmap)))
            {
                this.OpenImage((System.Drawing.Bitmap)(iData.GetData(typeof(System.Drawing.Bitmap))));
            }
            else
            {
                if (Win32.OpenClipboard(this.Handle))
                {
                    // Returns True 
                    if (Win32.IsClipboardFormatAvailable(Win32.CF_BITMAP))
                    {
                        // Returns 0 
                        IntPtr ptr = Win32.GetClipboardData(Win32.CF_BITMAP);
                        if (!ptr.Equals(new IntPtr(0)))
                        {
                            this.OpenImage(System.Drawing.Bitmap.FromHbitmap(ptr));
                        }
                    }
                    // Nothing valid on clipboard; tell user.
                    else
                    {
                        MessageBox.Show(this, "Clipboard does not contain correct format (bitmap or metafile).", "PlotReader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    Win32.CloseClipboard();
                }
            }
        }

        private FormOutput m_frmOutput = null;
        private void menuCopy_Click(object sender, System.EventArgs e)
        {
            if (this.plotBox1.AxesState != 2)
            {
                MessageBox.Show(this, "The axes are not fully configured yet, or they are incorrectly configured. Therefore, PlotReader cannot obtain plot coordinates.\n\nDetails:\n" + this.GetAxesWarnings(true), "PlotReader", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (this.m_frmOutput == null) this.m_frmOutput = new FormOutput(this.m_strRegistryRoot + "\\Output");

            if (this.m_frmOutput.ShowDialog(this) == DialogResult.OK)
            {
                string text = "";
                System.Globalization.NumberFormatInfo fmt = this.m_frmOutput.Culture;
                string nrstyle = this.m_frmOutput.Format;
                string sep = this.m_frmOutput.Separator;
                foreach (ListViewItem item in this.listViewPoints.Items)
                {
                    PointF p = (PointF)this.plotBox1.Points[(int)item.Tag];
                    double[] pp = this.plotBox1.ImageToTrue(p);
                    text += pp[0].ToString(nrstyle, fmt) + sep + pp[1].ToString(nrstyle, fmt) + Environment.NewLine;
                }
                Clipboard.SetDataObject(text, true);
            }
        }

        private void updownZoom_ValueChanged(object sender, System.EventArgs e)
        {
            this.plotBox1.Zoom = (int)(this.updownZoom.Value);
            this.UpdateMenuZoom();
        }

        private void plotBox1_MouseEnter(object sender, System.EventArgs e)
        {
            this.m_bMouseOnPlotBox = true;
            this.statusBarStatus.Text = this.plotBox1.StatusText;
        }

        private void plotBox1_MouseLeave(object sender, System.EventArgs e)
        {
            this.m_bMouseOnPlotBox = false;
            this.statusBarStatus.Text = "";
        }

        private int m_iNavigatorHeight;
        private void menuZoomControl_Click(object sender, System.EventArgs e)
        {
            if (this.menuShowImageNavigator.Checked)
            {
                this.m_iNavigatorHeight = this.tabControl1.Top - this.thumnailBox1.Top;
                this.menuShowImageNavigator.Checked = false;
                this.tabControl1.Height += this.m_iNavigatorHeight;
                this.tabControl1.Top -= this.m_iNavigatorHeight;

                this.labelZoom1.Visible = false;
                this.labelZoom2.Visible = false;
                this.updownZoom.Visible = false;
                this.thumnailBox1.Visible = false;
            }
            else
            {
                this.menuShowImageNavigator.Checked = true;
                this.tabControl1.Height -= this.m_iNavigatorHeight;
                this.tabControl1.Top += this.m_iNavigatorHeight;

                this.labelZoom1.Visible = true;
                this.labelZoom2.Visible = true;
                this.updownZoom.Visible = true;
                this.thumnailBox1.Visible = true;
            }
        }

        private void menuSortPoints_Click(object sender, System.EventArgs e)
        {
            SortOrder order = SortOrder.None;
            if (!this.menuSortPoints.Checked) order = SortOrder.Ascending;
            this.SortPoints(0, order);
        }

        private void toolBarOpen_Click(object sender, EventArgs e)
        {
            this.menuOpen_Click(sender, e);
        }

        private void toolBarPaste_Click(object sender, EventArgs e)
        {
            this.menuPaste_Click(sender, e);
        }

        private void toolBarSave_Click(object sender, EventArgs e)
        {
            this.menuSave_Click(sender, e);
        }

        private void toolBarCopy_Click(object sender, EventArgs e)
        {
            this.menuCopy_Click(sender, e);
        }

        private void toolBarMove_Click(object sender, EventArgs e)
        {
            this.toolBarSelect.Checked = false;
            this.toolBarMove.Checked = true;
            this.toolBarAdd.Checked = false;
            this.toolStripCheckBoxAutoCenter.Enabled = false;
            this.toolStripTextBoxTolerance.Enabled = false;
            this.plotBox1.MouseStatus = PlotBox.PlotBoxMouseState.Drag;
            if (this.tabControl1.SelectedIndex == 1)
            {
                this.checkSetX1.Checked = false;
                this.checkSetX2.Checked = false;
                this.checkSetY1.Checked = false;
                this.checkSetY2.Checked = false;
                this.plotBox1.StopExpectAxisCoordinate();
            }
        }

        private void toolBarAdd_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedIndex = 0;
            this.toolBarSelect.Checked = false;
            this.toolBarMove.Checked = false;
            this.toolBarAdd.Checked = true;
            this.toolStripCheckBoxAutoCenter.Enabled = true;
            this.toolStripTextBoxTolerance.Enabled = this.toolStripCheckBoxAutoCenter.CheckBoxControl.Checked;
            this.plotBox1.MouseStatus = PlotBox.PlotBoxMouseState.Add;
        }

        private void toolBarSelect_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedIndex = 0;
            this.toolBarSelect.Checked = true;
            this.toolBarMove.Checked = false;
            this.toolBarAdd.Checked = false;
            this.toolStripCheckBoxAutoCenter.Enabled = false;
            this.toolStripTextBoxTolerance.Enabled = false;
            this.plotBox1.MouseStatus = PlotBox.PlotBoxMouseState.Select;
        }

        private void toolStripCheckBoxAutoCenter_CheckedChanged(object sender, EventArgs e)
        {
            this.plotBox1.UseMagicWand = this.toolStripCheckBoxAutoCenter.CheckBoxControl.Checked;
            this.toolStripTextBoxTolerance.Enabled = this.toolStripCheckBoxAutoCenter.CheckBoxControl.Checked;
        }

        private void toolStripTextBoxTolerance_TextChanged(object sender, EventArgs e)
        {
            float flTolerance;
            if (float.TryParse(toolStripTextBoxTolerance.Text, out flTolerance)) this.plotBox1.MagicWandTolerance = flTolerance;
        }

        private void listViewPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayList sel = new ArrayList();
            foreach (ListViewItem item in this.listViewPoints.SelectedItems) sel.Add((int)item.Tag);
            this.plotBox1.SelectedPoints = sel;
            if (sel.Count != 0) this.SetStatusBarCoordinate((PointF)(this.plotBox1.Points[(int)(sel[0])]));
            this.menuDeleteSelectedPoint.Enabled = (sel.Count != 0);
        }

        private void plotBox1_StatusTextChanged(object sender, EventArgs e)
        {
            if (this.m_bMouseOnPlotBox) this.statusBarStatus.Text = this.plotBox1.StatusText;
        }

        private void linkLabelAlert_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            switch ((int)e.Link.LinkData)
            {
                case 0:
                    MessageBox.Show(this, this.GetAxesWarnings(true), "Axes configuration incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case 1:
                    this.tabControl1.SelectedTab = this.tabControl1.TabPages[1];
                    this.checkSetX1.Select();
                    break;
            }
        }

        class Sorter : IComparer
        {
            protected int m_iColumn;
            protected SortOrder m_sortOrder;
            public Sorter(int column, SortOrder order)
            {
                this.m_iColumn = column;
                this.m_sortOrder = order;
            }
            public SortOrder Order {
                get
                {
                    return this.m_sortOrder;
                }
            }
            public int Column
            {
                get
                {
                    return this.m_iColumn;
                }
            }
            int IComparer.Compare(Object x, Object y)
            {
                DataPoint dpX = (DataPoint)x;
                DataPoint dpY = (DataPoint)y;
                if (this.m_sortOrder == SortOrder.None) return ((int)dpX.Tag).CompareTo((int)dpY.Tag);
                int fac = 1;
                if (this.m_sortOrder == SortOrder.Descending) fac = -1;
                if (this.m_iColumn == 0)
                {
                    return fac*dpX.X.CompareTo(dpY.X);
                }
                else
                {
                    return fac * dpX.Y.CompareTo(dpY.Y);
                }
            }
        }

        private void listViewPoints_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            Sorter current = (Sorter)this.listViewPoints.ListViewItemSorter;
            SortOrder order = SortOrder.Ascending;
            if (current != null)
            {
                if (e.Column == current.Column && current.Order == SortOrder.Ascending) order = SortOrder.Descending;
            }
            this.SortPoints(e.Column, order);
        }

        protected void SortPoints(int column, SortOrder order)
        {
            this.listViewPoints.ListViewItemSorter = new Sorter(column, order);
            this.menuSortPoints.Checked = order!=SortOrder.None;
        }
    }
    //Declare a class that inherits from ToolStripControlHost.
    [System.Windows.Forms.Design.ToolStripItemDesignerAvailability(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.ToolStrip | System.Windows.Forms.Design.ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripCheckBox : ToolStripControlHost
    {
        // Call the base constructor passing in a MonthCalendar instance.
        public ToolStripCheckBox()
            : base(new CheckBox())
        {
            this.CheckBoxControl.BackColor = Color.Transparent;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CheckBox CheckBoxControl
        {
            get
            {
                return Control as CheckBox;
            }
        }
        // Subscribe and unsubscribe the control events you wish to expose.
        protected override void OnSubscribeControlEvents(Control c)
        {
            // Call the base so the base events are connected.
            base.OnSubscribeControlEvents(c);

            // Cast the control to a MonthCalendar control.
            CheckBox checkbox = (CheckBox)c;

            // Add the event.
            checkbox.CheckedChanged += new EventHandler(OnCheckedChanged);
        }

        protected override void OnUnsubscribeControlEvents(Control c)
        {
            // Call the base method so the basic events are unsubscribed.
            base.OnUnsubscribeControlEvents(c);

            // Cast the control to a MonthCalendar control.
            CheckBox checkbox = (CheckBox)c;

            // Remove the event.
            checkbox.CheckedChanged -= new EventHandler(OnCheckedChanged);
        }

        // Declare the DateChanged event.
        public event EventHandler CheckedChanged;

        // Raise the DateChanged event.
        private void OnCheckedChanged(object sender, EventArgs e)
        {
            if (CheckedChanged != null)
            {
                CheckedChanged(this, e);
            }
        }
    }
}
