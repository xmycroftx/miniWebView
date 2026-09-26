using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Windows;

namespace FloatCore3
{
    public partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.maxButton = new System.Windows.Forms.Button();
            this.minButton = new System.Windows.Forms.Button();
            this.titleButton = new System.Windows.Forms.Button();
            this.imageList3 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.textBox2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.webView21, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.closeButton, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(642, 579);
            this.tableLayoutPanel1.TabIndex = 2;
            this.tableLayoutPanel1.DoubleClick += new System.EventHandler(this.maxButton_Click);
            this.tableLayoutPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tableLayoutPanel1_MouseDown);
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.textBox2.Location = new System.Drawing.Point(3, 556);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(636, 20);
            this.textBox2.TabIndex = 3;
            this.textBox2.Visible = false;
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView21.Location = new System.Drawing.Point(0, 25);
            this.webView21.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(642, 515);
            this.webView21.Source = new System.Uri("https://www.google.com", System.UriKind.Absolute);
            this.webView21.TabIndex = 3;
            this.webView21.ZoomFactor = 1D;
            this.webView21.CoreWebView2InitializationCompleted += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs>(this.webView21_CoreWebView2InitializationCompleted);
            this.webView21.SourceChanged += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs>(this.webView21_SourceChanged);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.WindowText;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.textBox1.ImeMode = System.Windows.Forms.ImeMode.On;
            this.textBox1.Location = new System.Drawing.Point(0, 540);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(642, 13);
            this.textBox1.TabIndex = 4;
            this.textBox1.Visible = false;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.Color.Black;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Font = new System.Drawing.Font("ROG Fonts", 6.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.ImageKey = "closenob.png";
            this.closeButton.ImageList = this.imageList2;
            this.closeButton.Location = new System.Drawing.Point(600, 3);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(39, 18);
            this.closeButton.TabIndex = 3;
            this.closeButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "close.png");
            this.imageList2.Images.SetKeyName(1, "maximize.png");
            this.imageList2.Images.SetKeyName(2, "minimize.png");
            this.imageList2.Images.SetKeyName(3, "Sprite-0001.ico");
            this.imageList2.Images.SetKeyName(4, "maximizenob.png");
            this.imageList2.Images.SetKeyName(5, "minimizenob.png");
            this.imageList2.Images.SetKeyName(6, "closenob.png");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "close.png");
            this.imageList1.Images.SetKeyName(1, "maximize.png");
            this.imageList1.Images.SetKeyName(2, "minimize.png");
            this.imageList1.Images.SetKeyName(3, "Sprite-0001.ico");
            this.imageList1.Images.SetKeyName(4, "maximizenob.png");
            this.imageList1.Images.SetKeyName(5, "minimizenob.png");
            this.imageList1.Images.SetKeyName(6, "closenob.png");
            // 
            // maxButton
            // 
            this.maxButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maxButton.BackColor = System.Drawing.Color.Black;
            this.maxButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.maxButton.Font = new System.Drawing.Font("ROG Fonts", 6.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.maxButton.ImageKey = "maximizenob.png";
            this.maxButton.ImageList = this.imageList1;
            this.maxButton.Location = new System.Drawing.Point(561, 6);
            this.maxButton.Margin = new System.Windows.Forms.Padding(0);
            this.maxButton.Name = "maxButton";
            this.maxButton.Size = new System.Drawing.Size(39, 18);
            this.maxButton.TabIndex = 3;
            this.maxButton.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.maxButton.UseVisualStyleBackColor = false;
            this.maxButton.Click += new System.EventHandler(this.maxButton_Click);
            // 
            // minButton
            // 
            this.minButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minButton.BackColor = System.Drawing.Color.Black;
            this.minButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minButton.Font = new System.Drawing.Font("ROG Fonts", 6.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minButton.ImageKey = "minimizenob.png";
            this.minButton.ImageList = this.imageList1;
            this.minButton.Location = new System.Drawing.Point(526, 6);
            this.minButton.Name = "minButton";
            this.minButton.Size = new System.Drawing.Size(27, 18);
            this.minButton.TabIndex = 4;
            this.minButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.minButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.minButton.UseVisualStyleBackColor = false;
            this.minButton.Visible = false;
            this.minButton.Click += new System.EventHandler(this.minButton_Click);
            // 
            // titleButton
            // 
            this.titleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.titleButton.ImageKey = "icon.png";
            this.titleButton.ImageList = this.imageList3;
            this.titleButton.Location = new System.Drawing.Point(1, 0);
            this.titleButton.Margin = new System.Windows.Forms.Padding(0);
            this.titleButton.Name = "titleButton";
            this.titleButton.Size = new System.Drawing.Size(28, 26);
            this.titleButton.TabIndex = 5;
            this.titleButton.UseVisualStyleBackColor = true;
            // 
            // imageList3
            // 
            this.imageList3.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList3.ImageStream")));
            this.imageList3.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList3.Images.SetKeyName(0, "close.png");
            this.imageList3.Images.SetKeyName(1, "maximize.png");
            this.imageList3.Images.SetKeyName(2, "minimize.png");
            this.imageList3.Images.SetKeyName(3, "maximizenob.png");
            this.imageList3.Images.SetKeyName(4, "minimizenob.png");
            this.imageList3.Images.SetKeyName(5, "closenob.png");
            this.imageList3.Images.SetKeyName(6, "icon.png");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 585);
            this.ControlBox = false;
            this.Controls.Add(this.titleButton);
            this.Controls.Add(this.minButton);
            this.Controls.Add(this.maxButton);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Opacity = 0.8D;
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.webView21_MouseOver);
            this.Deactivate += new System.EventHandler(this.webView21_MouseLeave);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button maxButton;
        private System.Windows.Forms.Button minButton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.Button titleButton;
        private System.Windows.Forms.ImageList imageList3;

    }
}

