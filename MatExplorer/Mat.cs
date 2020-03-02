using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Windows.Forms;


namespace MatExplorer
{



    public class Mat
    {
        private Control parentControl;
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
        public Dictionary<string,string> data;
        public Dictionary<string,int> categories;
        private const int MAINWIDTH = 120;

        public Mat(Dictionary<string,string> datas, Dictionary<string,int> datacategories, int x, int y, Control parentcontrol)
        {
            data = datas;
            categories = datacategories;
            parentControl = parentcontrol;
            count += 1;
            
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
            picturebox.SetBounds(x,y,w,h);
            picturebox.MouseUp += mouseUp;
            picturebox.MouseDown += mouseDown;
            picturebox.MouseMove += mouseMove;
            parentControl.Controls.Add(picturebox);

            // setup the button(s)
            /*
            buttons = new MiniButtons();
            buttons.Top = picturebox.Bottom;
            buttons.Left = picturebox.Left;
            buttons.sizeBox.Click += new System.EventHandler((sender, e) =>
            {
                sizeWidget.Visible = !(sizeWidget.Visible);
                buttons.sizeBox.BackColor = sizeWidget.Visible ? Color.SlateBlue : Color.LightGray;
            });
            */

            // setup the size widget
            int MAXHEIGHT = 180;
            sizeWidget = new Panel();
            sizeWidget.Top = picturebox.Top;
            sizeWidget.Left = picturebox.Left-10;
            sizeWidget.Width = 10;
            sizeWidget.BackColor = Color.SlateGray;
            // calculate height (proportional to number of pages)
            int numpages = (data.ContainsKey("Number of pages") && data["Number of pages"] != null && data["Number of pages"].Length > 1) 
                ? int.Parse(System.Text.RegularExpressions.Regex.Match(data["Number of pages"], @"\d+").Value) : 0;
            sizeWidget.Height = (int)(MAXHEIGHT * (float)((float)numpages / (float)Form1.maxPages));
            parentControl.Controls.Add(sizeWidget);
                       
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

            // create the popup metadata list
            listview = new ListView();
            listview.View = View.Details;
            //LV.AllowColumnReorder = true;
            listview.FullRowSelect = true;
            listview.GridLines = true;
            listview.Sorting = SortOrder.None;
            listview.Columns.Add("Field", 150);
            listview.Columns.Add("Value", 300);
            listview.SetBounds(picturebox.Left, picturebox.Bottom, 305, 400);
            listview.Visible = false;
            listview.HeaderStyle = ColumnHeaderStyle.None;
            listview.MouseUp += mouseUp;
            listview.MouseDown += mouseDown;
            listview.MouseMove += mouseMove;
            var table = getTable();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow drow = table.Rows[i];
                if (drow.RowState != DataRowState.Deleted)
                {
                    ListViewItem lvi = new ListViewItem(drow[0].ToString());
                    lvi.SubItems.Add(drow[1].ToString());
                    listview.Items.Add(lvi);
                }
            } // end for i
            parentControl.Controls.Add(listview);

        } // end constructor

        private List<SelectedElement> selectedelements = new List<SelectedElement>();
        public  List<SelectedElement> SelectedElements
        {            
            set
            {
                selectedelements.Clear();
                foreach (var m in FieldMarkers)
                    parentControl.Controls.Remove(m);
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
                    parentControl.Controls.Add(m);
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
                    return Color.FromArgb(127,59,8);
                case 1:
                    return Color.FromArgb(179,88,6);
                case 2:
                    return Color.FromArgb(224,130,20);
                case 3: 
                    return Color.FromArgb(254,224,182);
                case 4:
                    return Color.FromArgb(216,218,235);
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
            c.Top = m.Y + c.Top - mouseYStart;
            c.Left = m.X + c.Left - mouseXStart;
            
            if (c.GetType().Name != "ListView")
            {
                listview.Top = m.Y + listview.Top - mouseYStart;
                listview.Left = m.X + listview.Left - mouseXStart;
            }

            //buttons.Top = this.picturebox.Bottom;
            //buttons.Left = this.picturebox.Left;
            sizeWidget.Top = this.picturebox.Top;
            sizeWidget.Left = this.picturebox.Left-10;
            foreach (var fm in FieldMarkers)
            {
                fm.Top = m.Y + fm.Top - mouseYStart;
                fm.Left = m.X + fm.Left - mouseXStart;
            }
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
            else if (m.Button == MouseButtons.Right)
            {
                listview.Visible = !listview.Visible;
                if (!listview.Visible)
                {
                    listview.Top = picturebox.Bottom;
                    listview.Left = picturebox.Left;
                }
            } // end else

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


    }


}
