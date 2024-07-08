using System;


namespace CompiledTasks
{
    /// <summary>
    /// Summary description for Parameter.
    /// </summary>
    public class noWhite : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.TextBox Value1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Value2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Value3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Value4;
        private System.Windows.Forms.Label label4;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public int nValue1
        {
            get
            {
                return (Convert.ToInt32(Value1.Text, 10));
            }
            set { Value1.Text = value.ToString(); }
        }
        
        public int nValue2
        {
            get
            {
                return (Convert.ToInt32(Value2.Text, 10));
            }
            set { Value2.Text = value.ToString(); }
        }
        
        public int nValue3
        {
            get
            {
                return (Convert.ToInt32(Value3.Text, 10));
            }
            set { Value3.Text = value.ToString(); }
        }
        public int nValue4
        {
            get
            {
                return (Convert.ToInt32(Value4.Text, 10));
            }
            set { Value4.Text = value.ToString(); }
        }
        
        public noWhite()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.Value1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Value2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Value3 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();            
            this.Value4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(24, 180);
            this.OK.Name = "OK";
            this.OK.TabIndex = 0;
            this.OK.Text = "OK";
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(112, 180);
            this.Cancel.Name = "Cancel";
            this.Cancel.TabIndex = 1;
            this.Cancel.Text = "Cancel";
            // 
            // Value1
            // 
            this.Value1.Location = new System.Drawing.Point(88, 48);
            this.Value1.Name = "Left";
            this.Value1.TabIndex = 2;
            this.Value1.Text = "textBox1";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(32, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Left";
            // 
            // Value2
            // 
            this.Value2.Location = new System.Drawing.Point(88, 80);
            this.Value2.Name = "Right";
            this.Value2.TabIndex = 4;
            this.Value2.Text = "textBox2";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(32, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Right";
            // 
            // Value3
            // 
            this.Value3.Location = new System.Drawing.Point(88, 112);
            this.Value3.Name = "Top";
            this.Value3.TabIndex = 6;
            this.Value3.Text = "textBox3";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(32, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Top";
            // 
            // Value4
            // 
            this.Value4.Location = new System.Drawing.Point(88, 144);
            this.Value4.Name = "Bottom";
            this.Value4.TabIndex = 8;
            this.Value4.Text = "textBox4";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(32, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "Bottom";
            // 
            // Parameter
            // 
            this.AcceptButton = this.OK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(216, 216);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.label1,
                                                                          this.Value1,
                                                                          this.label2, 
                                                                          this.Value2,
                                                                          this.label3,
                                                                          this.Value3,
                                                                          this.label4,
                                                                          this.Value4,
                                                                          this.Cancel,
                                                                          this.OK});
         
            this.Name = "noWhite";
            this.Text = "Offset";
            this.Load += new System.EventHandler(this.noWhite_Load);
            this.CenterToParent();
            this.ResumeLayout(false);

        }
        #endregion

        private void noWhite_Load(object sender, System.EventArgs e)
        {

        }
    }
}

