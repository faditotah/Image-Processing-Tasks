using System;
using System.Drawing;
using System.Windows.Forms;

namespace CompiledTasks
{
    public class Form1 : Form
    {
        private Bitmap m_Bitmap;
        private Bitmap m_Undo;
        private Bitmap d_Bitmap;
        private MainMenu mainMenu1;
        private MenuItem menuItem1;
        private MenuItem menuItem3;
        private MenuItem menuItem2;
        private MenuItem menuItem4;
        private MenuItem menuItem5;
        private MenuItem menuItem6;
        private MenuItem menuItem7;
        private MenuItem FileLoad;
        private MenuItem FileSave;
        private MenuItem FileExit;
        private MenuItem MeanThresh;
        private MenuItem StaticThresh;
        private MenuItem HorizontalCon;
        private MenuItem VerticalCon;
        private MenuItem Convert24bit;
        private MenuItem Convert1bit;
        private MenuItem Dilation;
        private MenuItem Erosion;
        private MenuItem noWhite;
        
        private MenuItem Undo;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private Form1()
        {
            InitializeComponent();
            m_Undo = new Bitmap(2, 2);
        }

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
        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();            
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.FileLoad = new System.Windows.Forms.MenuItem();
            this.FileSave = new System.Windows.Forms.MenuItem();
            this.FileExit = new System.Windows.Forms.MenuItem();
            this.Undo = new System.Windows.Forms.MenuItem();
            this.MeanThresh = new System.Windows.Forms.MenuItem();
            this.StaticThresh = new System.Windows.Forms.MenuItem();
            this.HorizontalCon = new System.Windows.Forms.MenuItem();
            this.VerticalCon = new System.Windows.Forms.MenuItem();
            this.Convert24bit = new System.Windows.Forms.MenuItem();
            this.Convert1bit = new System.Windows.Forms.MenuItem();
            this.Dilation = new System.Windows.Forms.MenuItem();
            this.Erosion = new System.Windows.Forms.MenuItem();
            this.noWhite = new System.Windows.Forms.MenuItem();

            //MAin Menu
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.menuItem1, 
                                                                                      this.menuItem2,
                                                                                      this.menuItem3,
                                                                                      this.menuItem4,
                                                                                      this.menuItem5,
                                                                                      this.menuItem6,
                                                                                      this.menuItem7
            });
            //menu item 1
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.FileLoad,
                                                                                      this.FileSave,
                                                                                      this.FileExit});
            this.menuItem1.Text = "File";

            //FileLoad
            this.FileLoad.Index = 0;
            this.FileLoad.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
            this.FileLoad.Text = "Load";
            this.FileLoad.Click += new System.EventHandler(this.File_Load);

            //FileSave
            this.FileSave.Index = 1;
            this.FileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.FileSave.Text = "Save";
            this.FileSave.Click += new System.EventHandler(this.File_Save);
            // 
            // FileExit
            // 
            this.FileExit.Index = 2;
            this.FileExit.Text = "Exit";
            this.FileExit.Click += new System.EventHandler(this.File_Exit);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.Undo});
            this.menuItem2.Text = "Edit";
            // 
            // Undo
            // 
            this.Undo.Index = 0;
            this.Undo.Text = "Undo";
            this.Undo.Click += new System.EventHandler(this.OnUndo);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 2;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.MeanThresh,
                                                                                      this.StaticThresh});
            this.menuItem3.Text = "Binarize";
            // 
            // BW Mean
            // 
            this.MeanThresh.Index = 0;
            this.MeanThresh.Text = "Mean Binarization";
            this.MeanThresh.Click += new System.EventHandler(this.Filter_MeanThresh);
            //
            // BW Static
            // 
            this.StaticThresh.Index = 1;
            this.StaticThresh.Text = "Static Binarization";
            this.StaticThresh.Click += new System.EventHandler(this.Filter_StaticThresh);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 3;
            this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.HorizontalCon, 
                                                                                    this.VerticalCon});
            this.menuItem4.Text = "Concatenate";
            // 
            // HorizontalCon
            // 
            this.HorizontalCon.Index = 0;
            this.HorizontalCon.Text = "Horizontal Concatenation";
            this.HorizontalCon.Click += new System.EventHandler(this.Filter_HorizontalCon);
            //
            // VerticalCon
            // 
            this.VerticalCon.Index = 1;
            this.VerticalCon.Text = "Vertical Concatenation";
            this.VerticalCon.Click += new System.EventHandler(this.Filter_VerticalCon);
            //
            // menuItem5
            // 
            this.menuItem5.Index = 4;
            this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.Convert24bit, 
                                                                                    this.Convert1bit});
            this.menuItem5.Text = "Convert Bitmap Format";
            // 
            // Conver24bit
            // 
            this.Convert24bit.Index = 0;
            this.Convert24bit.Text = "Convert 24bit to 8bit";
            this.Convert24bit.Click += new System.EventHandler(this.Filter_Convert24bit);
            // 
            // Conver1bit
            // 
            this.Convert1bit.Index = 1;
            this.Convert1bit.Text = "Convert 1bit to 8bit";
            this.Convert1bit.Click += new System.EventHandler(this.Filter_Convert1bit);
            //
            // menuItem6
            // 
            this.menuItem6.Index = 5;
            this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.Dilation, 
                this.Erosion, this.noWhite});
            this.menuItem6.Text = "Morphological Operations";
            // 
            // Dilation
            // 
            this.Dilation.Index = 0;
            this.Dilation.Text = "Dilation";
            this.Dilation.Click += new System.EventHandler(this.Filter_Dilation);
            // 
            // Erosion
            // 
            this.Erosion.Index = 1;
            this.Erosion.Text = "Erosion";
            this.Erosion.Click += new System.EventHandler(this.Filter_Erosion);
            //
            // menuItem7
            // 
            this.menuItem7.Index = 6;
            this.menuItem7.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.noWhite});
            this.menuItem7.Text = "Edit Borders";
            //
            // Boundary removal
            // 
            this.noWhite.Index = 0;
            this.noWhite.Text = "Remove White Boundaries";
            this.noWhite.Click += new System.EventHandler(this.Filter_noWhite);
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(488, 421);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Image Processing Tasks";
            this.Load += new System.EventHandler(this.Form1_Load);

        }

        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new Form1());
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_Bitmap != null)
            {
                Graphics g = e.Graphics;
                g.DrawImage(m_Bitmap, new Rectangle(this.AutoScrollPosition.X, this.AutoScrollPosition.Y, (int)(m_Bitmap.Width), (int)(m_Bitmap.Height)));
            }
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
        }

        private void File_Load(object sender, System.EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Bitmap files (*.bmp)|*.bmp|Jpeg files (*.jpg)|*.jpg|All valid files (*.bmp/*.jpg)|*.bmp/*.jpg";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                m_Bitmap = (Bitmap)Bitmap.FromFile(openFileDialog.FileName, false);
                this.AutoScroll = true;
                this.AutoScrollMinSize = new Size((int)(m_Bitmap.Width), (int)(m_Bitmap.Height));
                this.Invalidate();
            }
        }

        private void File_Save(object sender, System.EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = "c:\\";
            saveFileDialog.Filter = "Bitmap files (*.bmp)|*.bmp|Jpeg files (*.jpg)|*.jpg|All valid files (*.bmp/*.jpg)|*.bmp/*.jpg";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (DialogResult.OK == saveFileDialog.ShowDialog())
            {
                m_Bitmap.Save(saveFileDialog.FileName);
            }
        }

        private void File_Exit(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void Filter_MeanThresh(object sender, System.EventArgs e)
        {
            m_Undo = (Bitmap)m_Bitmap.Clone();
            m_Bitmap = BitmapFilter.Threshold(m_Bitmap, BitmapFilter.Mean(m_Bitmap)); // Apply horizontal concatenation
            Invalidate(); // Redraw the form
        }
        private void Filter_StaticThresh(object sender, System.EventArgs e)
        {
            Threshold dlg = new Threshold();
            dlg.nValue = 0;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                m_Undo = (Bitmap)m_Bitmap.Clone();
                m_Bitmap = BitmapFilter.Threshold(m_Bitmap, dlg.nValue);
                Invalidate();
            }
        }
        private void Filter_HorizontalCon(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Bitmap files (*.bmp)|*.bmp|Jpeg files (*.jpg)|*.jpg|All valid files (*.bmp/*.jpg)|*.bmp/*.jpg";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                d_Bitmap = (Bitmap)Bitmap.FromFile(openFileDialog.FileName, false);
                this.AutoScroll = true;
                this.AutoScrollMinSize = new Size((int)(d_Bitmap.Width), (int)(d_Bitmap.Height));
                this.Invalidate();
            }
            Bitmap m_BitmapClone = (Bitmap)m_Bitmap.Clone();
            m_Undo = (Bitmap)m_BitmapClone.Clone();
            m_BitmapClone = BitmapFilter.HorizontalCon(m_BitmapClone, d_Bitmap);
            m_Bitmap = m_BitmapClone;
            this.AutoScrollMinSize = new Size(m_Bitmap.Width + d_Bitmap.Width, Math.Max(m_Bitmap.Height, d_Bitmap.Height)); 
            this.Invalidate(); // Redraw the form
            
        }


        private void Filter_VerticalCon(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Bitmap files (*.bmp)|*.bmp|Jpeg files (*.jpg)|*.jpg|All valid files (*.bmp/*.jpg)|*.bmp/*.jpg";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                d_Bitmap = (Bitmap)Bitmap.FromFile(openFileDialog.FileName, false);
                this.AutoScroll = true;
                this.AutoScrollMinSize = new Size((int)(d_Bitmap.Width), (int)(d_Bitmap.Height));
                this.Invalidate();
            }
            Bitmap m_BitmapClone = (Bitmap)m_Bitmap.Clone();
            m_Undo = (Bitmap)m_BitmapClone.Clone();
            m_BitmapClone = BitmapFilter.VerticalCon(m_BitmapClone, d_Bitmap);
            m_Bitmap = m_BitmapClone;
            this.AutoScrollMinSize = new Size(Math.Max(m_Bitmap.Width, d_Bitmap.Width), m_Bitmap.Height + d_Bitmap.Height);

            // Redraw the form
            this.Invalidate();
        }
        private void Filter_Convert24bit(object sender, EventArgs e)
        {
            m_Undo = (Bitmap)m_Bitmap.Clone(); // Save current state for undo
            m_Bitmap = BitmapFilter.Convert24bit(m_Bitmap); // Apply conversion
            this.AutoScrollMinSize = new Size(m_Bitmap.Width, m_Bitmap.Height); // Update scroll size if needed
            this.Invalidate(); // Redraw the form
        }
        private void Filter_Convert1bit(object sender, EventArgs e)
        {
            m_Undo = (Bitmap)m_Bitmap.Clone(); // Save current state for undo
            m_Bitmap = BitmapFilter.Convert1bit(m_Bitmap); // Apply conversion
            this.AutoScrollMinSize = new Size(m_Bitmap.Width, m_Bitmap.Height); // Update scroll size if needed
            this.Invalidate(); // Redraw the form
        }
        private void Filter_Dilation(object sender, System.EventArgs e)
        {
            m_Undo = (Bitmap)m_Bitmap.Clone();
            m_Bitmap = BitmapFilter.Dilate(m_Bitmap); // Apply horizontal concatenation
            Invalidate(); // Redraw the form
        }
        private void Filter_Erosion(object sender, System.EventArgs e)
        {
            m_Undo = (Bitmap)m_Bitmap.Clone();
            m_Bitmap = BitmapFilter.Erosion(m_Bitmap); // Apply horizontal concatenation
            Invalidate(); // Redraw the form
        }
        private void Filter_noWhite(object sender, System.EventArgs e)
        {
            
            m_Undo = (Bitmap)m_Bitmap.Clone();
            m_Bitmap = BitmapFilter.noWhite(m_Bitmap); // Apply horizontal concatenation
            Invalidate(); // Redraw the form
            
        }

        private void OnUndo(object sender, System.EventArgs e)
        {
            Bitmap temp = (Bitmap)m_Bitmap.Clone();
            m_Bitmap = (Bitmap)m_Undo.Clone();
            m_Undo = (Bitmap)temp.Clone();
            this.Invalidate();
        }
    }
}