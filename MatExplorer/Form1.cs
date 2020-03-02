using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MatExplorer
{
    public partial class Form1 : Form
    {
        public List<string> sheetIDs = new List<string>();
        public SheetInfo currentSheetInfo;

        public static int g_width = 0;
        public static int g_height = 0;
        public static GoogleSheets sheet;

        public Dictionary<string, string>[] draftInfo = null;
        public Dictionary<string, int>[] draftInfo_categories = null;
        public Dictionary<string, string>[] corrInfo = null;
        public Dictionary<string, int>[] corrInfo_categories = null;
        public int next_col = 1;
        public static int maxPages = 10;

        //public List<Mat> matlist = new List<Mat>();
        public List<Manuscript> manList = new List<Manuscript>();

        public Form1()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);


            sheet = new GoogleSheets();

            
            sheetIDs.Add(@"1J5_1PDObU7ZHEfEABypaPDKC8ppmjSGv3CeLMr_kz9Y"); // Accident
            sheetIDs.Add(@"1UOKQUcinr1YDik_J7dpO7UjGTFbrnXMoE1KNTr3F930"); // Bardon Bus
            sheetIDs.Add(@"1MZGv0wj9NtfXPtL-s-1R2YgZSguhIHKc94LpjDWco1U"); // Chaddeleys and Flemings
            sheetIDs.Add(@"1YoJ4ix2_hN-qEEOcWkZFlx_1hRongDzOsC3l0s98sh4"); // Dulse
            sheetIDs.Add(@"1IrqPm6pciMknPf6DliHubwh_CZxDUo5rT--Dd-_14tM"); // Hard luck stories 
            sheetIDs.Add(@"1HdgxEhcqER9VTKH_61qX3He7CT9dNmpd_zDqZdoUNVM"); // Labor Day Dinner
            sheetIDs.Add(@"1vDCXa-e_2GhFcUcXLoBtXo1bnqQDp6YMUy-_NOK3ngM"); // Moons of Jupiter
            sheetIDs.Add(@"1Ykn93bo8HH0PpfEaZ2AqJsWdRx_aUYF6MbiVuuyEoWg"); // Mrs Cross and Mrs Kidd            
            sheetIDs.Add(@"1O8c8PWhWTBUZQjEOjUUNpPzv1B93EEWfuFIgVCbLiyI"); // Privilege
            sheetIDs.Add(@"12hKFpeEAjwSfIQOo6L6fPaP4PrIin5zT9CKi-SWZyUg"); // Providence
            sheetIDs.Add(@"1aXK51JWFjVhFkHwFJ9RbPb2pEJkhJ8NTKX7ol3Hewlk"); // Prue
            sheetIDs.Add(@"1bFfCrsXGA_SM_V1qW-lDNOu_5Lmj0AlY2eUTjhKF_sg"); // Royal Beatings
            
            //sheetIDs.Add(@"1cTP06iwtGePdXfNj8HbO7dsrhtWlHhecLtVc0_RpsE8"); // Simon's Luck *****
            
            sheetIDs.Add(@"19EO1joffnYoTOypijmYD942U7YUITjSwlPA6Va3SGsw"); // Spelling            
            sheetIDs.Add(@"1IMFgjiiwR60lavptcNM1XCue_k3AVEHCyow-crP2yp4"); // The Beggar Maid
            sheetIDs.Add(@"1v74L4dcgbopflanlcWdcznE0hgAAqv5KsEyn58DDvww"); // The Turkey Season
            sheetIDs.Add(@"1Mxg0KLXWB5yOyzJidcusV7HZM8XiShv4b70vkTU0_aI"); // Visitors
            sheetIDs.Add(@"1d8Frpt6nN9vzYBk4eUHkk1a8yMqSpYlVwatv1r80eQA"); // Who Do You Think You Are
            
            sheetIDs.Add(@"1X5sPMFzUH7B1zpLBTo3FdNrspGyXS5E1cGQzzcKdHbg"); // Wild swans 

            // create the data menu
            for (int i = 0; i < sheetIDs.Count; i++)
            {
                var si = sheet.getSheetInfo(sheetIDs[i]);
                currentSheetInfo = si;

                foreach (var page in si.pages)
                {
                    // garbage, ignore these sheets
                    if (page.Contains("Sheet") || page.Contains("Empty") || page.Contains("template") || page.Contains("Template"))
                        continue;

                    bool corr = false;
                    if (page.Contains("Correspondence") || page.Contains("correspondence") || si.title.Contains("Correspondence") || si.title.Contains("correspondence"))
                        corr = true;

                    // create menu item
                    ToolStripItem it = new ToolStripMenuItem(si.title + ": " + page, null, new EventHandler((sender, e) =>
                    {
                        currentSheetInfo = si;
                        GoogleSheets.setSelectedPage(ref currentSheetInfo, page);
                        this.Text = "Material Explorer        " + currentSheetInfo.title + ": " + currentSheetInfo.pages[currentSheetInfo.selectedPage];

                        loadItems(!corr);
                    }));

                    if (corr)
                        correspondenceToolStripMenuItem.DropDownItems.Add(it);
                    else
                        dataToolStripMenuItem.DropDownItems.Add(it);
                }

                /*
                if (i == 0) currentSheetInfo = si;
                ToolStripMenuItem item = new ToolStripMenuItem(si.title);
                foreach (var page in si.pages)
                    item.DropDownItems.Add(page, null, new EventHandler((sender, e) =>
                    {
                        currentSheetInfo = si;
                        GoogleSheets.setSelectedPage(ref currentSheetInfo, page);
                        this.Text = "Material Explorer        " + currentSheetInfo.title + ": " + currentSheetInfo.pages[currentSheetInfo.selectedPage];
                        loadItems();
                        //Console.WriteLine("Sheet " + currentSheetInfo.title + ", page " + currentSheetInfo.pages[currentSheetInfo.selectedPage] + " was selected.");
                    }));
                dataToolStripMenuItem.DropDownItems.Add(item);
                */
            }
            this.Text = "Material Explorer        " + currentSheetInfo.title + ": " + currentSheetInfo.pages[0];

            /*

            this.Paint += new PaintEventHandler((sender, e) =>
            {
                if (e.ClipRectangle.Width < .9 * this.Width)
                    this.Invalidate(this.ClientRectangle);
                else
                {
                    for (int i = 2; i < Controls.Count; i++)
                    {
                        var c = Controls[i];
                        if (c.GetType().Name == "ListView" && c.Visible)
                        {
                            var cp = Controls[i - 1];
                            Pen pen = new Pen(Color.Gray);
                            pen.Width = 2;

                            if (Math.Abs(cp.Bottom - c.Top) + Math.Abs(cp.Left - c.Left) > 5)
                                e.Graphics.DrawLine(pen, cp.Left, cp.Bottom, c.Left, c.Top);
                        }
                    }
                }

            });
            */
        }

        private int startX = 250;
        private int startY = 50;
        private void loadItems(bool draft)
        {

            // load data if we don't already have it
            if (draftInfo == null)
                readInfo(draft);

            Dictionary<string, string>[] info = draft ? draftInfo : corrInfo;
            Dictionary<string, int>[] categories = draft ? draftInfo_categories : corrInfo_categories;
            ListView lv = draft ? listView : correspondenceListView;

            // load the headers into the appropriate listview
            info = sheet.readColumns(currentSheetInfo.id, "");
            lv.Columns.Clear();
            lv.Columns.Add("Fields", listView.Width - 22);
            lv.HeaderStyle = ColumnHeaderStyle.None;

            foreach (var k in info[0].Keys)
                lv.Items.Add(k);

            // load a material item
            for (int i = 0; i < info.Length; i++)
            {
                Manuscript m = new Manuscript();
                m.setData(info[i], categories[i], startX, startY);
                int BUDGE = 250;
                int BUDGEY = 10;
                startX += BUDGE; startY += BUDGEY;
                if (startX > this.Width - 350)
                {
                    startX = 250;
                    startY = startY + (BUDGE - (this.Width - 250) / BUDGE * BUDGEY) + 20;
                }
                manList.Add(m);
                this.Controls.Add(m);
            }
        }

        private int[] getCatCol(List<List<int>> cat, int col)
        {
            int[] column = new int[cat.Count];
            for (int i = 0; i < cat.Count; i++)
            {
                column[i] = cat[i][col];
            }
            return column;
        }

        public void readInfo(bool draft)
        {
            Dictionary<string, string>[] info;
            Dictionary<string, int>[] categories;

            info = sheet.readColumns(currentSheetInfo.id, "");

            // figure out maximum number of pages 
            maxPages = 1;
            for (int j = 0; j < info.Length; j++)
            {
                if (!info[j].ContainsKey("Number of pages"))
                    continue;
                string num = info[j]["Number of pages"];
                if (num == null || num.Length < 1)
                    continue;
                int m = int.Parse(System.Text.RegularExpressions.Regex.Match(num, @"\d+").Value);
                if (m > maxPages)
                    maxPages = m;
            }

            // categorize each column
            categories = new Dictionary<string, int>[info.Length];
            for (int i = 0; i < categories.Length; i++)
                categories[i] = new Dictionary<string, int>();
            foreach (var k in info[0].Keys)
            {
                int currentCategoryNumber = -1;
                for (int j = 0; j < info.Length; j++)
                {
                    if (info[j][k] == null || info[j][k].Length < 1)
                    {
                        categories[j][k] = -1;
                        continue;
                    }
                        
                    int m = 0;
                    for (; m < j; m++)
                    {
                        if (info[j][k] == info[m][k]) // check to see if we've seen this in the previous columns
                            break;
                    }
                    categories[j][k] = (m < j) ? categories[m][k] : ++currentCategoryNumber; // then either it's the same # as the previous category number, or a new #
                }
            }

            if (draft)
            {
                draftInfo = info;
                draftInfo_categories = categories;
            }
            else
            {
                corrInfo = info;
                corrInfo_categories = categories;
            }

            return;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            g_height = this.Height;
            g_width = this.Width;
        }

        private void listView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {

            List<SelectedElement> selectedItems = new List<SelectedElement>();
            foreach (int index in listView.SelectedIndices)
            {
                SelectedElement s = new SelectedElement();
                s.key = listView.Items[index].Text;
                selectedItems.Add(s);
            }

            foreach (var m in manList)
                m.SelectedElements = selectedItems;
        } // end item selection changed


        private void saveGeoCSV(string filename, Dictionary<string, string>[] info)
        {
            if (info == null)
                throw new Exception("No draft info!!!");

            bool newfile = !System.IO.File.Exists(filename);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, true, System.Text.Encoding.UTF8))
            {
                if (newfile) file.WriteLine("Story, Document, Field, Location, Latitude, Longitude, Type, Real");

                foreach (var doc in info)
                {
                    foreach (var entry in doc)
                    {
                        if (entry.Key.Contains("(Location)"))
                        {
                            if (entry.Value == null || entry.Value.Length < 1 || entry.Value.Contains("N/A"))
                                continue;
                            string type = "Setting";
                            string prefix = "";
                            string real = "";
                            if (entry.Key.Contains("Primary"))
                            {
                                prefix = "Primary Setting";
                                real = doc[prefix + " (Non-fictional)"];
                            }
                            else if (entry.Key.Contains("Setting") || entry.Key.Contains("Geographic Ref"))
                            {
                                var index1 = entry.Key.IndexOf('#');
                                var index2 = entry.Key.IndexOf(' ', index1);
                                var num = entry.Key.Substring(index1 + 1, index2 - index1 - 1);
                                prefix = entry.Key.Substring(0, index2);
                                if (entry.Key.Contains("Setting"))
                                    real = doc[prefix + " (Non-fictional)"];
                                else
                                    type = "Reference";
                            }
                            else
                                throw new Exception("Error, we have a location that doesn't seem to be a setting or a geographic reference");

                            string latlong = doc[prefix + " (Geographic Coordinates)"];
                            if (latlong == null || latlong.Length <= 0 || !latlong.Contains(','))
                                latlong = ",";

                            file.WriteLine(currentSheetInfo.title + ",\"" + doc["Accession Number"].Replace("\"", "\"\"") + "\"," + prefix + ",\"" + entry.Value + "\"," + latlong + "," + type + "," + real);
                        }
                    }

                }
            } // end using
        }


        private void cSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //saveCSV("geographic_draft_info.csv");

            for (int i = 0; i < sheetIDs.Count; i++)
            {
                var si = sheet.getSheetInfo(sheetIDs[i]);
                currentSheetInfo = si;

                foreach (var page in si.pages)
                {
                    // garbage, ignore these sheets
                    if (page.Contains("Sheet") || page.Contains("Empty") || page.Contains("template") || page.Contains("Template"))
                        continue;
                    if (page.Contains("Correspondence") || page.Contains("correspondence") || si.title.Contains("Correspondence") || si.title.Contains("correspondence"))
                        continue;

                    if (page.Contains("Draft"))
                    {
                        Dictionary<string, string>[] info = sheet.readColumns(currentSheetInfo.id, "");
                        saveGeoCSV("geographic_draft_info.csv", info);
                        break; // only need the first "draft" page, can ignore the rest
                    }
                }
            }

            MessageBox.Show("Finished writing CSV to geographic_draft_info.csv");
        }

        private void createGeneralCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Dictionary<string, string>[]> allInfo = new List<Dictionary<string, string>[]>();
            List<string> stories = new List<string>();

            for (int i = 0; i < sheetIDs.Count; i++)
            {
                var si = sheet.getSheetInfo(sheetIDs[i]);
                currentSheetInfo = si;

                foreach (var page in si.pages)
                {
                    // garbage, ignore these sheets
                    if (page.Contains("Sheet") || page.Contains("Empty") || page.Contains("template") || page.Contains("Template"))
                        continue;
                    if (page.Contains("Correspondence") || page.Contains("correspondence") || si.title.Contains("Correspondence") || si.title.Contains("correspondence"))
                        continue;

                    if (page.Contains("Draft"))
                    {
                        Dictionary<string, string>[] info = sheet.readColumns(currentSheetInfo.id, "");
                        allInfo.Add(info);
                        stories.Add(si.title);
                        break; // only need the first "draft" page, can ignore the rest
                    }
                }
            }

            List<string> keys = new List<string>();
            // get list of all column headers
            for (int i = 0; i < stories.Count; i++)
            {
                foreach (var doc in allInfo[i])
                    foreach (var entry in doc)
                    {
                        if (!keys.Contains(entry.Key))
                        {
                            if (entry.Key.Contains("Character #") || entry.Key.Contains("Setting #") || entry.Key.Contains("Ref #") || entry.Key.Contains("ref #") || entry.Key.Contains("Ref#") || entry.Key.Contains("Ref. #") || 
                                entry.Key.Contains("Character(s) #") || entry.Key.Contains("Scanned image") || entry.Key.Contains("Protagonist") || entry.Key.Contains("Cultural References"))
                                continue;
                            keys.Add(entry.Key);
                        }
                    }
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter("draft_info.csv", false, System.Text.Encoding.UTF8))
            {
                // header
                file.Write("Story,");
                foreach (var key in keys)
                    file.Write(process(key) + ",");
                file.WriteLine("Num Deletions,Num Revisions,Num Outside Rev");

                // body
                for (int i=0; i<stories.Count; i++)
                {                    
                    foreach (var doc in allInfo[i])
                    {
                        int numDeletions = 0, numRevisions = 0, outside = 0;

                        file.Write(process(stories[i]) + ",");
                        foreach (var key in keys)
                        {
                            if (doc.ContainsKey(key))
                            {
                                file.Write(process(doc[key]) + ',');
                                if (key.Contains("Typed Deletions (# per page, including 0 for none, separated by commas)"))
                                    numDeletions = SplitAndSum(doc[key]);
                                if (key.Contains("Author's Editorial Revisions (# per page, including 0 for none, separated by commas)"))
                                    numRevisions = SplitAndSum(doc[key]);
                                if (key.Contains("Outside Editorial Revisions (# per page, including 0 for none, separated by commas)"))
                                    outside = SplitAndSum(doc[key]);
                            }
                            else
                                file.Write(',');
                        } // end for each key
                        file.WriteLine(numDeletions.ToString() + "," + numRevisions.ToString() + "," + outside);
                    } // end for each doc
                } // end for
                file.Close();
            } // end using


            MessageBox.Show("Finished writing CSV to draft_info.csv");

        } // end create general csv tool strim menu item clicked


        string process(string s)
        {
            if (s != null)
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            else
                return "";
        }

        int SplitAndSum(string s)
        {
            int sum = 0;
            if (s != null)
            {
                string[] items = s.Split(',');
                foreach (var x in items)
                {
                    try
                    {
                        sum += int.Parse(x);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }
            return sum;
        }

    }
}
