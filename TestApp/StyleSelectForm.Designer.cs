namespace TestApp
{
    partial class StyleSelectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.btnSerialise = new System.Windows.Forms.Button();
            this.btnDeseriliase = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cmbSizes = new System.Windows.Forms.ComboBox();
            this.cmbStyles = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnSerialise
            // 
            this.btnSerialise.Location = new System.Drawing.Point(12, 12);
            this.btnSerialise.Name = "btnSerialise";
            this.btnSerialise.Size = new System.Drawing.Size(75, 23);
            this.btnSerialise.TabIndex = 0;
            this.btnSerialise.Text = "Serialise";
            this.btnSerialise.UseVisualStyleBackColor = true;
            this.btnSerialise.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnDeseriliase
            // 
            this.btnDeseriliase.Location = new System.Drawing.Point(93, 12);
            this.btnDeseriliase.Name = "btnDeseriliase";
            this.btnDeseriliase.Size = new System.Drawing.Size(75, 23);
            this.btnDeseriliase.TabIndex = 0;
            this.btnDeseriliase.Text = "Deserialise";
            this.btnDeseriliase.UseVisualStyleBackColor = true;
            this.btnDeseriliase.Click += new System.EventHandler(this.btnDeseriliase_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 95);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(155, 155);
            this.textBox1.TabIndex = 1;
            // 
            // cmbSizes
            // 
            this.cmbSizes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSizes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSizes.FormattingEnabled = true;
            this.cmbSizes.Location = new System.Drawing.Point(12, 41);
            this.cmbSizes.Name = "cmbSizes";
            this.cmbSizes.Size = new System.Drawing.Size(155, 21);
            this.cmbSizes.TabIndex = 2;
            // 
            // cmbStyles
            // 
            this.cmbStyles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbStyles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStyles.FormattingEnabled = true;
            this.cmbStyles.Location = new System.Drawing.Point(12, 68);
            this.cmbStyles.Name = "cmbStyles";
            this.cmbStyles.Size = new System.Drawing.Size(155, 21);
            this.cmbStyles.TabIndex = 2;
            // 
            // StyleSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(179, 262);
            this.Controls.Add(this.cmbStyles);
            this.Controls.Add(this.cmbSizes);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnDeseriliase);
            this.Controls.Add(this.btnSerialise);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "StyleSelectForm";
            this.Text = "Select Style";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSerialise;
        private System.Windows.Forms.Button btnDeseriliase;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox cmbSizes;
        private System.Windows.Forms.ComboBox cmbStyles;
    }
}

