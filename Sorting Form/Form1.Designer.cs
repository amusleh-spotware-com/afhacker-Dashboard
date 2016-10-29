namespace Sorting_Form
{
    partial class SortForm
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
            this.correlationGridLabel = new MetroFramework.Controls.MetroLabel();
            this.trendGridLabel = new MetroFramework.Controls.MetroLabel();
            this.volatilityGridLabel = new MetroFramework.Controls.MetroLabel();
            this.correlationColumnsComboBox = new MetroFramework.Controls.MetroComboBox();
            this.crrelationSortDirectionComboBox = new MetroFramework.Controls.MetroComboBox();
            this.volatilityColumnComboBox = new MetroFramework.Controls.MetroComboBox();
            this.volatilitySortDirectionComboBox = new MetroFramework.Controls.MetroComboBox();
            this.trendColumnsComboBox = new MetroFramework.Controls.MetroComboBox();
            this.trendSortDirectionComboBox = new MetroFramework.Controls.MetroComboBox();
            this.sortSettingOkButton = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // correlationGridLabel
            // 
            this.correlationGridLabel.AutoSize = true;
            this.correlationGridLabel.Location = new System.Drawing.Point(23, 77);
            this.correlationGridLabel.Name = "correlationGridLabel";
            this.correlationGridLabel.Size = new System.Drawing.Size(110, 20);
            this.correlationGridLabel.TabIndex = 0;
            this.correlationGridLabel.Text = "Correlation Grid:";
            this.correlationGridLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // trendGridLabel
            // 
            this.trendGridLabel.AutoSize = true;
            this.trendGridLabel.Location = new System.Drawing.Point(23, 176);
            this.trendGridLabel.Name = "trendGridLabel";
            this.trendGridLabel.Size = new System.Drawing.Size(77, 20);
            this.trendGridLabel.TabIndex = 1;
            this.trendGridLabel.Text = "Trend Grid:";
            this.trendGridLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // volatilityGridLabel
            // 
            this.volatilityGridLabel.AutoSize = true;
            this.volatilityGridLabel.Location = new System.Drawing.Point(23, 127);
            this.volatilityGridLabel.Name = "volatilityGridLabel";
            this.volatilityGridLabel.Size = new System.Drawing.Size(92, 20);
            this.volatilityGridLabel.TabIndex = 2;
            this.volatilityGridLabel.Text = "Volatility Grid:";
            this.volatilityGridLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // correlationColumnsComboBox
            // 
            this.correlationColumnsComboBox.FormattingEnabled = true;
            this.correlationColumnsComboBox.ItemHeight = 24;
            this.correlationColumnsComboBox.Items.AddRange(new object[] {
            "Symbol",
            "Correlation(%)"});
            this.correlationColumnsComboBox.Location = new System.Drawing.Point(164, 67);
            this.correlationColumnsComboBox.Name = "correlationColumnsComboBox";
            this.correlationColumnsComboBox.Size = new System.Drawing.Size(121, 30);
            this.correlationColumnsComboBox.TabIndex = 3;
            this.correlationColumnsComboBox.UseSelectable = true;
            // 
            // crrelationSortDirectionComboBox
            // 
            this.crrelationSortDirectionComboBox.FormattingEnabled = true;
            this.crrelationSortDirectionComboBox.ItemHeight = 24;
            this.crrelationSortDirectionComboBox.Items.AddRange(new object[] {
            "Ascending",
            "Descending"});
            this.crrelationSortDirectionComboBox.Location = new System.Drawing.Point(307, 67);
            this.crrelationSortDirectionComboBox.Name = "crrelationSortDirectionComboBox";
            this.crrelationSortDirectionComboBox.Size = new System.Drawing.Size(121, 30);
            this.crrelationSortDirectionComboBox.TabIndex = 4;
            this.crrelationSortDirectionComboBox.UseSelectable = true;
            // 
            // volatilityColumnComboBox
            // 
            this.volatilityColumnComboBox.FormattingEnabled = true;
            this.volatilityColumnComboBox.ItemHeight = 24;
            this.volatilityColumnComboBox.Items.AddRange(new object[] {
            "Symbol",
            "Correlation(%)"});
            this.volatilityColumnComboBox.Location = new System.Drawing.Point(164, 117);
            this.volatilityColumnComboBox.Name = "volatilityColumnComboBox";
            this.volatilityColumnComboBox.Size = new System.Drawing.Size(121, 30);
            this.volatilityColumnComboBox.TabIndex = 5;
            this.volatilityColumnComboBox.UseSelectable = true;
            // 
            // volatilitySortDirectionComboBox
            // 
            this.volatilitySortDirectionComboBox.FormattingEnabled = true;
            this.volatilitySortDirectionComboBox.ItemHeight = 24;
            this.volatilitySortDirectionComboBox.Items.AddRange(new object[] {
            "Ascending",
            "Descending"});
            this.volatilitySortDirectionComboBox.Location = new System.Drawing.Point(307, 117);
            this.volatilitySortDirectionComboBox.Name = "volatilitySortDirectionComboBox";
            this.volatilitySortDirectionComboBox.Size = new System.Drawing.Size(121, 30);
            this.volatilitySortDirectionComboBox.TabIndex = 6;
            this.volatilitySortDirectionComboBox.UseSelectable = true;
            // 
            // trendColumnsComboBox
            // 
            this.trendColumnsComboBox.FormattingEnabled = true;
            this.trendColumnsComboBox.ItemHeight = 24;
            this.trendColumnsComboBox.Items.AddRange(new object[] {
            "Symbol",
            "Correlation(%)"});
            this.trendColumnsComboBox.Location = new System.Drawing.Point(164, 166);
            this.trendColumnsComboBox.Name = "trendColumnsComboBox";
            this.trendColumnsComboBox.Size = new System.Drawing.Size(121, 30);
            this.trendColumnsComboBox.TabIndex = 7;
            this.trendColumnsComboBox.UseSelectable = true;
            // 
            // trendSortDirectionComboBox
            // 
            this.trendSortDirectionComboBox.FormattingEnabled = true;
            this.trendSortDirectionComboBox.ItemHeight = 24;
            this.trendSortDirectionComboBox.Items.AddRange(new object[] {
            "Ascending",
            "Descending"});
            this.trendSortDirectionComboBox.Location = new System.Drawing.Point(307, 166);
            this.trendSortDirectionComboBox.Name = "trendSortDirectionComboBox";
            this.trendSortDirectionComboBox.Size = new System.Drawing.Size(121, 30);
            this.trendSortDirectionComboBox.TabIndex = 8;
            this.trendSortDirectionComboBox.UseSelectable = true;
            // 
            // sortSettingOkButton
            // 
            this.sortSettingOkButton.Location = new System.Drawing.Point(179, 213);
            this.sortSettingOkButton.Name = "sortSettingOkButton";
            this.sortSettingOkButton.Size = new System.Drawing.Size(75, 23);
            this.sortSettingOkButton.TabIndex = 9;
            this.sortSettingOkButton.Text = "Ok";
            this.sortSettingOkButton.UseSelectable = true;
            // 
            // SortForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 250);
            this.Controls.Add(this.sortSettingOkButton);
            this.Controls.Add(this.trendSortDirectionComboBox);
            this.Controls.Add(this.trendColumnsComboBox);
            this.Controls.Add(this.volatilitySortDirectionComboBox);
            this.Controls.Add(this.volatilityColumnComboBox);
            this.Controls.Add(this.crrelationSortDirectionComboBox);
            this.Controls.Add(this.correlationColumnsComboBox);
            this.Controls.Add(this.volatilityGridLabel);
            this.Controls.Add(this.trendGridLabel);
            this.Controls.Add(this.correlationGridLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SortForm";
            this.Resizable = false;
            this.ShowIcon = false;
            this.Text = "Sort Settings";
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel correlationGridLabel;
        private MetroFramework.Controls.MetroLabel trendGridLabel;
        private MetroFramework.Controls.MetroLabel volatilityGridLabel;
        private MetroFramework.Controls.MetroComboBox correlationColumnsComboBox;
        private MetroFramework.Controls.MetroComboBox crrelationSortDirectionComboBox;
        private MetroFramework.Controls.MetroComboBox volatilityColumnComboBox;
        private MetroFramework.Controls.MetroComboBox volatilitySortDirectionComboBox;
        private MetroFramework.Controls.MetroComboBox trendColumnsComboBox;
        private MetroFramework.Controls.MetroComboBox trendSortDirectionComboBox;
        private MetroFramework.Controls.MetroButton sortSettingOkButton;
    }
}

