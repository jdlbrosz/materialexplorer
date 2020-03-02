using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace MatExplorer
{
    public struct SelectedElement
    {
        public string key;
        public string value;
        public int category;
    }

    public partial class Manuscript : UserControl
    {
        public PictureBox picturebox;
        public ListView listview;
        //public MiniButtons buttons;
        public Panel sizeWidget;
        public List<Label> FieldMarkers = new List<Label>();

        private static int count = 0;
        Image image;

        public string title;
        Pen pen = new Pen(Color.DimGray);
        Font font = new Font("Arial", 8);
        Font detailfont = new Font("Arial", 7);
        public Dictionary<string, string> data;
        public Dictionary<string, int> categories;
        private const int MAINWIDTH = 120;

        private const int SIZE = 18;

        public Manuscript()
        {
            InitializeComponent();
        }



        public void setData(Dictionary<string, string> datas, Dictionary<string, int> datacategories, int x, int y)
        {
            data = datas;
            categories = datacategories;
            count += 1;

            this.Top = y;
            this.Left = x;

            string imgfile = null;
            title = data["Accession Number"];
            if (data.ContainsKey("Image") && data["Image"] != null && data["Image"].Length > 0)
                imgfile = "./img/" + data["Image"].Replace('/', '.');

            // load the image (if any)
            picturebox = new PictureBox();
            if (imgfile != null && System.IO.File.Exists(imgfile))
                image = Image.FromFile(imgfile);
            else
                image = Image.FromFile(@"Notecard.jpeg");
            picturebox.Image = image;
            picturebox.Height = picturebox.Image.Height;
            picturebox.SizeMode = PictureBoxSizeMode.StretchImage;
            double ratio = (double)picturebox.Image.Width / (double)picturebox.Image.Height;
            int w = MAINWIDTH;
            int h = (int)(MAINWIDTH / ratio);
            this.Width = w + 10;
            this.Height = h + 20;
            picturebox.SetBounds(10, 0, w, h);
            picturebox.MouseUp += mouseUp;
            picturebox.MouseDown += mouseDown;
            picturebox.MouseMove += mouseMove;
            this.Controls.Add(picturebox);

            if (picturebox.Height > Height)
                Height = picturebox.Height;
         
            // setup the size widget
            int MAXHEIGHT = 180;
            sizeWidget = new Panel();
            sizeWidget.Top = picturebox.Top;
            sizeWidget.Left = picturebox.Left - 10;
            sizeWidget.Width = 10;
            sizeWidget.BackColor = Color.SlateGray;
            // calculate height (proportional to number of pages)
            int numpages = (data.ContainsKey("Number of pages") && data["Number of pages"] != null && data["Number of pages"].Length > 1)
                ? int.Parse(System.Text.RegularExpressions.Regex.Match(data["Number of pages"], @"\d+").Value) : 0;
            sizeWidget.Height = (int)(MAXHEIGHT * (float)((float)numpages / (float)Form1.maxPages));
            Controls.Add(sizeWidget);
            if (sizeWidget.Height > this.Height)
                this.Height = sizeWidget.Height;

            // setup the block vis
            /*
            MatExplorer.BoxVisControl bvc = new BoxVisControl();
            bvc.setData(datas, datacategories);
            bvc.Top = 45;
            bvc.Left = 15;
            Controls.Add(bvc);
            if (bvc.Height + bvc.Top > Height)
                Height = bvc.Height + bvc.Top;
            if (bvc.Width + bvc.Left > Width)
                Width = bvc.Width + bvc.Left;
            bvc.BringToFront();
            */
            setupBlockVis(data, datacategories);

            picturebox.SendToBack();


            // setup the paint event handler            
            picturebox.Paint += new PaintEventHandler((sender, e) =>
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                SizeF textSize = e.Graphics.MeasureString(title, font);
                PointF locationToDraw = new PointF();
                locationToDraw.X = 5;//(p.Width / 2) - (textSize.Width / 2);
                locationToDraw.Y = 5;// (p.Height / 2) - (textSize.Height / 2);
                e.Graphics.DrawString(title, font, Brushes.Black, locationToDraw);

                float Y = 5;
                if (selectedelements.Count > 0)
                {
                    var s = selectedelements[selectedelements.Count - 1];
                    Y += textSize.Height * 1.4f;
                    textSize = e.Graphics.MeasureString(s.value, detailfont);
                    RectangleF r = new RectangleF(5, Y, picturebox.Width - 8, picturebox.Height - 2.2f * textSize.Height);
                    e.Graphics.DrawString(s.value, detailfont, Brushes.Black, r);
                }
            });

        } // end set data


        public void setupBlockVis(Dictionary<string, string> info, Dictionary<string, int> categories)
        {
            this.Width = 0;
            if (this.Height < SIZE * 7 + 1 + 45)
                this.Height = SIZE * 7 + 1 + 45;

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
                Width = col * 2 * SIZE / 3 + 1;

            // materials
            this.Controls.Add(getNewButton(6, 0, "Paper size: " + info["Paper size (legal/letter/other)"], Mat.getColorByCategory(categories["Paper size (legal/letter/other)"])));
            this.Controls.Add(getNewButton(6, 1, "Paper colour: " + info["Paper colour"], Mat.getColorByCategory(categories["Paper colour"])));
            this.Controls.Add(getNewButton(6, 2, "Type ribbon: " + info["Type ribbon"], Mat.getColorByCategory(categories["Type ribbon"])));
            this.Controls.Add(getNewButton(6, 3, "External note: " + info["External Note (Y/N)"], Mat.getColorByCategory(categories["External Note (Y/N)"])));
            this.Controls.Add(getNewButton(6, 4, "Number of pages: " + info["Number of pages"], Mat.getColorByCategory(categories["Number of pages"])));
            this.Controls.Add(getNewButton(6, 5, "Author's revisions: " + info["Author's Editiorial Revisions (Y/N)"], Mat.getColorByCategory(categories["Author's Editiorial Revisions (Y/N)"])));
            this.Controls.Add(getNewButton(6, 6, "Editorial revisions: " + info["Outside Editorial Revisions (Y/N)"], Mat.getColorByCategory(categories["Outside Editorial Revisions (Y/N)"])));
        }

        private List<SelectedElement> selectedelements = new List<SelectedElement>();
        public List<SelectedElement> SelectedElements
        {
            set
            {
                selectedelements.Clear();
                foreach (var m in FieldMarkers)
                    Controls.Remove(m);
                FieldMarkers.Clear();

                foreach (SelectedElement e in value)
                {
                    SelectedElement s = new SelectedElement();
                    s.key = e.key;
                    s.value = data[e.key];
                    s.category = categories[e.key];
                    selectedelements.Add(s);
                }

                for (int i = 0; i < selectedelements.Count; i++)
                {
                    var m = createFieldMarker(selectedelements[i], i);
                    FieldMarkers.Add(m);
                    Controls.Add(m);
                }

                picturebox.Invalidate();
            }
        }

        private Label createFieldMarker(SelectedElement e, int markernumber)
        {
            const int HEIGHT = 12;
            const int PADDING = 4;
            const int MAXCHARS = 34;

            Label b = new Label();
            b.SetBounds(picturebox.Left, picturebox.Bottom + (HEIGHT + PADDING) * markernumber + PADDING, MAINWIDTH, HEIGHT);
            b.AutoSize = true;
            if (e.value != null && e.value.Length > MAXCHARS)
                b.Text = e.value.Substring(0, MAXCHARS) + "...";
            else
                b.Text = e.value;
            b.BackColor = getColorByCategory(e.category);
            b.FlatStyle = FlatStyle.Flat;
            b.Font = detailfont;

            return b;
        }


        public static Color getColorByCategory(int cat)
        {
            switch (cat)
            {
                case -1:
                    return Color.White;
                case 0:
                    return Color.FromArgb(127, 59, 8);
                case 1:
                    return Color.FromArgb(179, 88, 6);
                case 2:
                    return Color.FromArgb(224, 130, 20);
                case 3:
                    return Color.FromArgb(254, 224, 182);
                case 4:
                    return Color.FromArgb(216, 218, 235);
                case 5:
                    return Color.FromArgb(178, 171, 210);
                case 6:
                    return Color.FromArgb(128, 115, 172);
                case 7:
                    return Color.FromArgb(84, 39, 136);
                case 8:
                    return Color.FromArgb(45, 0, 75);
                default:
                    return Color.DarkGray;
            } // end switch
        } // end get color


        private int mouseXStart;
        private int mouseYStart;
        private bool mouseDrag = false;

        private void mouseMove(object sender, MouseEventArgs m)
        {
            var c = sender as Control;
            if (!mouseDrag || null == c) return;
            c.Parent.Top = m.Y + c.Parent.Top - mouseYStart;
            c.Parent.Left = m.X + c.Parent.Left - mouseXStart;

            /*
            //buttons.Top = this.picturebox.Bottom;
            //buttons.Left = this.picturebox.Left;
            sizeWidget.Top = this.picturebox.Top;
            sizeWidget.Left = this.picturebox.Left - 10;
            foreach (var fm in FieldMarkers)
            {
                fm.Top = m.Y + fm.Top - mouseYStart;
                fm.Left = m.X + fm.Left - mouseXStart;
            }
            */
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
                BringToFront();
            }
        }

        private DataTable List2DataTable(List<string[]> list)
        {
            DataTable table = new DataTable();
            for (int i = 0; i < 2; i++)
                table.Columns.Add();
            foreach (var array in list)
                table.Rows.Add(array);
            return table;
        }

        private DataTable getTable()
        {
            DataTable table = new DataTable();
            for (int i = 0; i < 2; i++)
                table.Columns.Add();
            foreach (var k in data.Keys)
            {
                DataRow r = table.NewRow();
                r[0] = k;
                r[1] = data[k];
                table.Rows.Add(r);
            }
            return table;
        }

        public Button getNewButton(int row, int col, string description, Color color)
        {
            return getNewButton(row, col, 0, 1, description, color);
        }

        public Button getNewButton(int row, int col, int slice, int numslices, string description, Color color)
        {
            int top = 45;
            int left = 15;

            if (slice < 0 || slice >= numslices) throw new Exception("Invalid slice value of " + slice);
            Button b = new Button();
            b.Size = new Size(2 * SIZE / 3 + 1, SIZE / numslices + 1);
            b.Top = top + row * SIZE + slice * SIZE / numslices;
            b.Left = left + col * 2 * SIZE / 3;
            b.BackColor = color;
            b.FlatStyle = FlatStyle.Flat;
            b.Margin = new Padding(0);
            b.TabStop = false;
            b.FlatAppearance.BorderColor = Color.DarkGoldenrod;
            b.FlatAppearance.BorderSize = 1;
            //b.Click += B_Click;
            b.MouseDown += mouseDown;
            b.MouseUp += mouseUp;
            b.MouseMove += mouseMove;
            ToolTip tp = new ToolTip();
            tp.SetToolTip(b, description);
            b.BringToFront();
            return b;
        }

    } // end class
} // end namespace
