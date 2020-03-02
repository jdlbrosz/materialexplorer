using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MatExplorer
{
    /* finding info from
    https://developers.google.com/sheets/api/guides/concepts
    https://developers.google.com/sheets/api/guides/values#appending_values
    https://github.com/gsuitedevs/dotnet-samples/blob/master/sheets/SheetsQuickstart/SheetsQuickstart.cs
    */

    public struct SheetInfo
    {
        public string title;
        public List<string> pages;
        public string id;
        public int selectedPage;
    }

    public class GoogleSheets
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "MatExplorer";

        SheetsService service;

        public GoogleSheets()
        {
            init();
        }

        void init()
        {
            UserCredential credential;

            using (var stream = new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets, Scopes, "user", CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });


        } // end init

        public SheetInfo getSheetInfo(string sheetID)
        {
            var s = service.Spreadsheets.Get(sheetID).Execute();
            List<string> pageNames = new List<string>();
            foreach (var p in s.Sheets)
                pageNames.Add(p.Properties.Title);

            SheetInfo si;
            si.id = sheetID;
            si.title = s.Properties.Title;
            si.pages = pageNames;
            si.selectedPage = 0;
            return si;
        }

        public static void setSelectedPage(ref SheetInfo si, string pageName)
        {
            for (int i=0; i<si.pages.Count; i++)
                if (si.pages[i].Contains(pageName))
                {
                    si.selectedPage = i;
                    break;
                }
        } // end set selected page
        


        // append a line to the spreadsheet
        /*
        public void update(IList<IList<Object>> line, string sheetRange)
        {
            // Define request parameters.
            String spreadsheetId = "1S85WT3eefmnP4AzAPigF-r8GcCaxCLZIQjW6wHrobFg";


            SpreadsheetsResource.ValuesResource.AppendRequest request =
                    service.Spreadsheets.Values.Append(new ValueRange() { Values = line }, spreadsheetId, sheetRange);
            request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            var response = request.Execute();
        }
    */

        

            /*
        public List<string[]> readColumns(string sheetID, string sheetName)
        { 
            string range = sheetName + "!A:Z"; //@"Version as of 02/05 meeting!A:Z";
            SpreadsheetsResource.ValuesResource.GetRequest rq = service.Spreadsheets.Values.Get(sheetID, range);
            ValueRange response = rq.Execute();
            IList<IList<Object>> values1 = response.Values;

            //rq = service.Spreadsheets.Values.Get(s, range2);
            //response = rq.Execute();
            //IList<IList<Object>> values2 = response.Values;

            List<string[]> st = new List<string[]>();
            int maxcol = 0;
            foreach (var i in values1)
                if (maxcol < i.Count)
                    maxcol = i.Count;
            for (int j = 0; j < maxcol; j++)
            {
                string[] col = new string[values1.Count];
                st.Add(col);
            }

            for (int i=0; i<values1.Count; i++)
            {
                for (int j = 0; j < values1[i].Count; j++)
                {
                    string s = values1[i][j].ToString();
                    if (s != null)
                        s = s.Trim();
                    st[j][i] = s;
                }
            }

            return st;
        }
        */

        public Dictionary<string,string>[] readColumns(string sheetID, string sheetName)
        {
            List<Dictionary<string,string>> columns = new List<Dictionary<string,string>>();

            string range = sheetName + "!A:Z"; 
            SpreadsheetsResource.ValuesResource.GetRequest rq = service.Spreadsheets.Values.Get(sheetID, range);
            ValueRange response = rq.Execute();
            IList<IList<Object>> values1 = response.Values;

            List<string[]> st = new List<string[]>();
            int maxcol = 0;
            foreach (var i in values1)
                if (maxcol < i.Count)
                    maxcol = i.Count;
            for (int j = 0; j < maxcol; j++)
            {
                string[] col = new string[values1.Count];
                st.Add(col);
            }

            for (int i = 0; i < values1.Count; i++)
            {
                for (int j = 0; j < values1[i].Count; j++)
                {
                    string s = values1[i][j].ToString();
                    if (s != null)
                        s = s.Trim();
                    st[j][i] = s;
                }
            }

            for (int i=1; i<st.Count; i++)
            {
                Dictionary<string,string> d = new Dictionary<string, string>();
                for (int j = 0; j < st[i].Length; j++)
                    if (st[0][j] != null && st[0][j] != "")
                    {
                        if (st[0][j].Contains("Accession Number"))
                            st[0][j] = "Accession Number";
                        d.Add(st[0][j], st[i][j]);
                    }

                bool hasinfo = true;
                if (d["Accession Number"] != null && d["Accession Number"] != "")
                    hasinfo = true;
                /*
                if (d.ContainsKey("Number of pages"))
                    hasinfo = hasinfo && d["Number of pages"] != null && d["Number of pages"] != null;
                else
                    hasinfo = hasinfo && d["Document Info (From finding aid)"] != null && d["Document Info (From finding aid)"] != null;

    */
                if (hasinfo)
                    columns.Add(d);
            }

            return columns.ToArray();
        }

    }
}
