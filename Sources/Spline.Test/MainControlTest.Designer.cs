using ZedGraph;

namespace Spline.Test
{
    partial class MainControlTest
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ZedGraphControl = new ZedGraph.ZedGraphControl();
            this.tbLoad = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // ZedGraphControl
            // 
            this.ZedGraphControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ZedGraphControl.Location = new System.Drawing.Point(12, 12);
            this.ZedGraphControl.Name = "ZedGraphControl";
            this.ZedGraphControl.ScrollGrace = 0D;
            this.ZedGraphControl.ScrollMaxX = 0D;
            this.ZedGraphControl.ScrollMaxY = 0D;
            this.ZedGraphControl.ScrollMaxY2 = 0D;
            this.ZedGraphControl.ScrollMinX = 0D;
            this.ZedGraphControl.ScrollMinY = 0D;
            this.ZedGraphControl.ScrollMinY2 = 0D;
            this.ZedGraphControl.Size = new System.Drawing.Size(770, 470);
            this.ZedGraphControl.TabIndex = 0;
            // 
            // tbLoad
            // 
            this.tbLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbLoad.Location = new System.Drawing.Point(12, 488);
            this.tbLoad.Name = "tbLoad";
            this.tbLoad.Size = new System.Drawing.Size(100, 20);
            this.tbLoad.TabIndex = 1;
            this.tbLoad.Text = "TestData.txt";
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoad.Location = new System.Drawing.Point(118, 485);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.BtnLoadClick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // MainControlTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 523);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.tbLoad);
            this.Controls.Add(this.ZedGraphControl);
            this.Name = "MainControlTest";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private ZedGraph.ZedGraphControl ZedGraphControl;
		private System.Windows.Forms.TextBox tbLoad;
		private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

