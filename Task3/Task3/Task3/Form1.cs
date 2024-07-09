using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
namespace Task3
{
    public class Form1 : System.Windows.Forms.Form
    {
        private System.Drawing.Bitmap m_Bitmap;
        private System.Drawing.Bitmap m_Undo;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem FileLoad;
        private System.Windows.Forms.MenuItem FileSave;
        private System.Windows.Forms.MenuItem FileExit;
        private System.Windows.Forms.MenuItem Convert24bit;
        private System.Windows.Forms.MenuItem Undo;

        //
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
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.FileLoad = new System.Windows.Forms.MenuItem();
            this.FileSave = new System.Windows.Forms.MenuItem();
            this.FileExit = new System.Windows.Forms.MenuItem();
            this.Undo = new System.Windows.Forms.MenuItem();
            this.Convert24bit = new System.Windows.Forms.MenuItem();

            //MAin Menu
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                      this.menuItem1,
                                                                                      this.menuItem3,
                                                                                      this.menuItem2});
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
                                                                                      this.Convert24bit});
            this.menuItem3.Text = "Convert Bitmap Format";
            // 
            // 24 bit to 8 bit
            // 
            this.Convert24bit.Index = 0;
            this.Convert24bit.Text = "Convert 24 bit to 8bit";
            this.Convert24bit.Click += new System.EventHandler(this.Filter_Convert24bit);
            //
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(488, 421);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Conversion to 8bit Grayscale";
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
            base.OnPaint(e); // Call base method first

            if (m_Bitmap != null)
            {
                Graphics g = e.Graphics;
                g.DrawImage(m_Bitmap, AutoScrollPosition.X, AutoScrollPosition.Y, m_Bitmap.Width, m_Bitmap.Height);
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

        private void Filter_Convert24bit(object sender, EventArgs e)
        {
            m_Undo = (Bitmap)m_Bitmap.Clone(); // Save current state for undo
            m_Bitmap = BitmapFilter.Convert24BitTo8Bit(m_Bitmap); // Apply conversion
            this.AutoScrollMinSize = new Size(m_Bitmap.Width, m_Bitmap.Height); // Update scroll size if needed
            this.Invalidate(); // Redraw the form
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
