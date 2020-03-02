using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MatExplorer
{
    public partial class BoxVisControl : UserControl
    {
        public BoxVisControl()
        {
            InitializeComponent();
        }

        private const int SIZE = 18;

        public void setData(Dictionary<string,string> info, Dictionary<string,int> categories)
        {
            this.Width = 0;
            this.Height = SIZE * 7+1;

            // front matter
            this.Controls.Add(getNewButton(0, 0, "Title: " + info["Title"], Mat.getColorByCategory(categories["Title"])));
            this.Controls.Add(getNewButton(0, 1, "Document type: " + info["Document Type"], Mat.getColorByCategory(categories["Document Type"])));
            this.Controls.Add(getNewButton(0, 2, "Narrative voice: " + info["Narrative Voice"], Mat.getColorByCategory(categories["Narrative Voice"])));

            // characters
            this.Controls.Add(getNewButton(1, 0, 0, 2, "Protagonist's Name: " + info["Protagonist's Name"], Mat.getColorByCategory(categories["Protagonist's Name"])));
            this.Controls.Add(getNewButton(1, 0, 1, 2, "Protagonist's Occupation: " + info["Protagonist's Occupation"], Mat.getColorByCategory(categories["Protagonist's Occupation"])));
            // find all the characters
            int col = 1;
            foreach (var entry in info)
            {
                if (entry.Key.Contains("Character #") && entry.Key.Contains("Name"))
                {
                    var index1 = entry.Key.IndexOf('#');
                    var index2 = entry.Key.IndexOf(' ', index1);
                    var num = entry.Key.Substring(index1 + 1, index2 - index1 - 1);

                    this.Controls.Add(getNewButton(1, col, 0, 3, "Name: " + info["Character #" + num + " Name"], Mat.getColorByCategory(categories["Character #" + num + " Name"])));
                    this.Controls.Add(getNewButton(1, col, 1, 3, "Occupation: " + info["Character #" + num + " Occupation"], Mat.getColorByCategory(categories["Character #" + num + " Occupation"])));
                    this.Controls.Add(getNewButton(1, col, 2, 3, "Relation to protag.: " + info["Character #" + num + " Relation to Protagonist"], Mat.getColorByCategory(categories["Character #" + num + " Relation to Protagonist"])));
                    col++;
                }
            }
            if (col * 2 * SIZE / 3 > Width)
                Width = col * 2 * SIZE / 3 + 1;

            // settings
            this.Controls.Add(getNewButton(2, 0, "Setting: " + info["Primary Setting (Location)"], Mat.getColorByCategory(categories["Primary Setting (Location)"])));
            // find all the settings
            col = 1;
            foreach (var entry in info)
            {
                if (entry.Key.Contains("Setting #") && entry.Key.Contains("Location"))
                {
                    var index1 = entry.Key.IndexOf('#');
                    var index2 = entry.Key.IndexOf(' ', index1);
                    var num = entry.Key.Substring(index1 + 1, index2 - index1 - 1);
                    this.Controls.Add(getNewButton(2, col, entry.Key + ": " + info["Setting #" + num + " (Location)"], Mat.getColorByCategory(categories["Setting #" + num + " (Location)"])));
                    col++;
                }
            }
            if (col * 2 * SIZE / 3 > Width)
                Width = col * 2 * SIZE / 3 + 1;

            // high cultural references
            col = 0;
            foreach (var entry in info)
            {
                if (entry.Key.Contains("High Cultural Ref #"))
                {
                    this.Controls.Add(getNewButton(3, col, entry.Key + ": " + info[entry.Key], Mat.getColorByCategory(categories[entry.Key])));
                    col++;
                }
            }
            if (col * 2 * SIZE / 3 > Width)
                Width = col * 2 * SIZE / 3 + 1;

            // mass cultural references
            col = 0;
            foreach (var entry in info)
            {
                if (entry.Key.Contains("Mass Cultural Ref #"))
                {
                    this.Controls.Add(getNewButton(4, col, entry.Key + ": " + info[entry.Key], Mat.getColorByCategory(categories[entry.Key])));
                    col++;
                }
            }
            if (col * 2 * SIZE / 3 > Width)
                Width = col * 2 * SIZE / 3 + 1;

            // geographic references
            col = 0;
            foreach (var entry in info)
            {
                if (entry.Key.Contains("Geographic Ref #") && entry.Key.Contains("(Location)"))
                {
                    this.Controls.Add(getNewButton(5, col, entry.Key + ": " + info[entry.Key], Mat.getColorByCategory(categories[entry.Key])));
                    col++;
                }
            }
            if (col * 2 * SIZE / 3 > Width)
                Width = col * 2 * SIZE / 3+1;

            // materials
            this.Controls.Add(getNewButton(6, 0, "Paper size: " + info["Paper size (legal/letter/other)"], Mat.getColorByCategory(categories["Paper size (legal/letter/other)"])));
            this.Controls.Add(getNewButton(6, 1, "Paper colour: " + info["Paper colour"], Mat.getColorByCategory(categories["Paper colour"])));
            this.Controls.Add(getNewButton(6, 2, "Type ribbon: " + info["Type ribbon"], Mat.getColorByCategory(categories["Type ribbon"])));
            this.Controls.Add(getNewButton(6, 3, "External note: " + info["External Note (Y/N)"], Mat.getColorByCategory(categories["External Note (Y/N)"])));
            this.Controls.Add(getNewButton(6, 4, "Number of pages: " + info["Number of pages"], Mat.getColorByCategory(categories["Number of pages"])));
            this.Controls.Add(getNewButton(6, 5, "Author's revisions: " + info["Author's Editiorial Revisions (Y/N)"], Mat.getColorByCategory(categories["Author's Editiorial Revisions (Y/N)"])));
            this.Controls.Add(getNewButton(6, 6, "Editorial revisions: " + info["Outside Editorial Revisions (Y/N)"], Mat.getColorByCategory(categories["Outside Editorial Revisions (Y/N)"])));
        }

        public Button getNewButton(int row, int col, string description, Color color)
        {
            return getNewButton(row, col, 0, 1, description, color);
        }

        public Button getNewButton(int row, int col, int slice, int numslices, string description, Color color)
        {
            if (slice < 0 || slice >= numslices) throw new Exception("Invalid slice value of " + slice);
            Button b = new Button();
            b.Size = new Size(2*SIZE/3+1, SIZE/numslices+1);
            b.Top = row * SIZE + slice*SIZE/numslices;
            b.Left = col * 2*SIZE/3;
            b.BackColor = color;
            b.FlatStyle = FlatStyle.Flat;
            b.Margin = new Padding(0);
            b.TabStop = false;
            b.FlatAppearance.BorderColor = Color.DarkGoldenrod;
            b.FlatAppearance.BorderSize = 1;
            b.Click += B_Click;
            b.MouseDown += mouseDown;
            b.MouseUp += mouseUp;
            b.MouseMove += mouseMove;
            ToolTip tp = new ToolTip();           
            tp.SetToolTip(b, description);
            return b;
        }


        private void B_Click(object sender, EventArgs e)
        {
        }

        private int mouseXStart;
        private int mouseYStart;
        private bool mouseDrag = false;

        private void mouseMove(object sender, MouseEventArgs m)
        {
            var c = sender as Control;
            if (!mouseDrag || null == c) return;

            c.Parent.Top = m.Y + c.Parent.Top - mouseYStart;
            c.Parent.Left = m.X + c.Parent.Left - mouseXStart;
            //c.Top = m.Y + c.Top - mouseYStart;
            //c.Left = m.X + c.Left - mouseXStart;
        }

        private void mouseUp(object sender, MouseEventArgs m)
        {
            mouseDrag = false;
        }

        private void mouseDown(object sender, MouseEventArgs m)
        {
            if (m.Button == MouseButtons.Left)
            {
                mouseDrag = true;
                mouseXStart = m.X;
                mouseYStart = m.Y;
            }
        }

    }
}
