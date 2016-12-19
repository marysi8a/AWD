using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using nGantt.GanttChart;
using nGantt.PeriodSplitter;
namespace GanttDemo
{
    public struct zadania
    {
        public int start;
        public int end;
        public int nr;
        public int r;
        public int p;
        public int q;

    };
    public partial class Form1 : Form
    {
        private int GantLenght { get; set; }
        private ObservableCollection<ContextMenuItem> ganttTaskContextMenuItems = new ObservableCollection<ContextMenuItem>();
        private ObservableCollection<SelectionContextMenuItem> selectionContextMenuItems = new ObservableCollection<SelectionContextMenuItem>();
        List<GanttRow> RowList = new List<GanttRow>();
        List<GanttRowGroup> RowGroupList = new List<GanttRowGroup>();
        List<zadania> Kolejka = new List<zadania>();
        private string nazwaPliku = "";
        private string tresc = "";
        DateTime initial = new DateTime();
        private int CMAX = 0;
        public Form1()
        { 
            initial = DateTime.Now;
            initial = new DateTime(initial.Year, initial.Month, initial.Day, 0, 0, 0);
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GantLenght = 1;
            dateTimePicker.Value = DateTime.Today;
            DateTime minDate = DateTime.Today;
            DateTime maxDate = minDate.AddDays(GantLenght);

            // Set selection -mode
            ganttControl1.TaskSelectionMode = nGantt.GanttControl.SelectionMode.Single;
            // Enable GanttTasks to be selected
            ganttControl1.AllowUserSelection = true;

            // listen to the GanttRowAreaSelected event
            ganttControl1.GanttRowAreaSelected += new EventHandler<PeriodEventArgs>(ganttControl1_GanttRowAreaSelected);
            
            // define ganttTask context menu and action when each item is clicked
            ganttTaskContextMenuItems.Add(new ContextMenuItem(ViewClicked, "View..."));
            ganttTaskContextMenuItems.Add(new ContextMenuItem(EditClicked, "Edit..."));
            ganttTaskContextMenuItems.Add(new ContextMenuItem(DeleteClicked, "Delete..."));
            ganttControl1.GanttTaskContextMenuItems = ganttTaskContextMenuItems;

            // define selection context menu and action when each item is clicked
            selectionContextMenuItems.Add(new SelectionContextMenuItem(NewClicked, "New..."));
            ganttControl1.SelectionContextMenuItems = selectionContextMenuItems;

        }

        private void NewClicked(Period selectionPeriod)
        {
            MessageBox.Show("New clicked for task " + selectionPeriod.Start.ToString() + " -> " + selectionPeriod.End.ToString());
        }

        private void ViewClicked(GanttTask ganttTask)
        {
            MessageBox.Show("New clicked for task " + ganttTask.Name);
        }

        private void EditClicked(GanttTask ganttTask)
        {
            MessageBox.Show("Edit clicked for task " + ganttTask.Name);
        }

        private void DeleteClicked(GanttTask ganttTask)
        {
            MessageBox.Show("Delete clicked for task " + ganttTask.Name);
        }

        void ganttControl1_GanttRowAreaSelected(object sender, PeriodEventArgs e)
        {
            MessageBox.Show(e.SelectionStart.ToString("0:HH:mm:ss d.MM.yyyy") + " -> " + e.SelectionEnd.ToString("0:HH:mm:ss d.MM.yyyy"));
        }

        private System.Windows.Media.Brush DetermineBackground(TimeLineItem timeLineItem)
        {
            if (timeLineItem.End.Date.DayOfWeek == DayOfWeek.Saturday || timeLineItem.End.Date.DayOfWeek == DayOfWeek.Sunday)
                return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightBlue);
            else
                return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
        }

        private void CreateData(DateTime minDate, DateTime maxDate)
        {
            // Set max and min dates
            ganttControl1.Initialize(minDate, maxDate);
            //header displaying
            // Create timelines and define how they should be presented
            //ganttControl1.CreateTimeLine(new PeriodYearSplitter(minDate, maxDate), FormatYear);
            //ganttControl1.CreateTimeLine(new PeriodMonthSplitter(minDate, maxDate), FormatMonth);
            //var gridLineTimeLine = ganttControl1.CreateTimeLine(new PeriodYearSplitter(minDate, maxDate), FormatYear);
            //ganttControl1.CreateTimeLine(new PeriodDaySplitter(minDate, maxDate), FormatDayName);

            // Set the timeline to atatch gridlines to
            //ganttControl1.SetGridLinesTimeline(gridLineTimeLine, DetermineBackground);

            // Create and data
            RowGroupList.Add(ganttControl1.CreateGanttRowGroup(""));
          
           
        }
    
        private string FormatYear(Period period)
        {
            return (
                period.Start.DayOfWeek.ToString() + ", " +
                period.Start.Day.ToString() + "." +
                period.Start.Month.ToString() + "." +
                period.Start.Year.ToString());
        }

        private string FormatMonth(Period period)
        {
            return period.Start.Month.ToString();
        }

        private string FormatDay(Period period)
        {
            return period.Start.Day.ToString();
        }

        private string FormatDayName(Period period)
        {
            return period.Start.DayOfWeek.ToString();
        }


        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            Clear();
        }

        private void AddRow()
        {
            RowList.Add(ganttControl1.CreateGanttRow(RowGroupList[0], "Maszyna"));
        }

       
        private void AddBlock(int start, int end, int machinenr, string name)
        {
            // ganttControl1.AddGanttTask(RowList[machinenr - 1], new GanttTask() { Start = DateTime.Now.AddHours(start), End = DateTime.Now.AddHours(end), Name = "GanttRow 2:GanttTask 1" });
             ganttControl1.AddGanttTask(RowList[machinenr - 1], new GanttTask() { Start = initial.AddSeconds(start), End = initial.AddSeconds(end), Name = name +  "\nStart: " + initial.AddSeconds(start).ToString("HH:mm:ss") + "\nEnd: " + initial.AddSeconds(end).ToString("HH:mm:ss") });
        }

       

        void podzial(int nr)
        {
            Kolejka.Clear();
            if (nr == 0)
            {
                tresc = tresc.Replace("\r\n", " ");
                string[] s = tresc.Split();
                int n = Int32.Parse(s[5]);
                int a, b, c;
                int j = 12;
                for (int i = 1; i <= n; i++)
                {
                    a = int.Parse(s[j]);
                    b = int.Parse(s[j + 2]);
                    c = int.Parse(s[j + 3]);
                    zadania tmp = new zadania();
                    tmp.nr = a;
                    tmp.start = b;
                    tmp.end = c;
                    Kolejka.Add(tmp);
                    j += 4;
                }
            }
            if (nr == 1 || nr == 3)
            {
                string[] s = tresc.Split();
                int n = Int32.Parse(s[5]);
                CMAX = Int32.Parse(s[12]);
                int a, b, c, d;
                int j = 17;
                for (int i = 1; i <= n; i++)
                {
                    a = int.Parse(s[j]);
                    b = int.Parse(s[j + 2]);
                    c = int.Parse(s[j + 3]);
                    d = int.Parse(s[j + 4]);
                    zadania tmp = new zadania();
                    tmp.nr = a;
                    tmp.r = b;
                    tmp.p = c;
                    tmp.q = d;
                    Kolejka.Add(tmp);
                    j += 6;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            OpenFileDialog oknoWyboruPliku = new OpenFileDialog();
            oknoWyboruPliku.Filter = "TXT|*.txt";
            oknoWyboruPliku.Title = "Wczytaj plik";
            oknoWyboruPliku.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            if (oknoWyboruPliku.ShowDialog() == DialogResult.OK)
            {
                label1.Text = "" + oknoWyboruPliku.FileName + "";
                nazwaPliku = oknoWyboruPliku.FileName;
                label1.Text = nazwaPliku;
            }
            if(nazwaPliku == "")
            {
                MessageBox.Show("Brak pliku");
                return;
            }
            nazwaPliku = Path.GetFileName(oknoWyboruPliku.FileName);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (nazwaPliku == "")
            {
                MessageBox.Show("Brak pliku");
                return;
            }
            Process proc = new Process();
            if (comboBox1.SelectedIndex == 0)
            {
                proc.StartInfo.FileName = "SPD.exe";
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Arguments = nazwaPliku;
            }
            else
            {
                proc.StartInfo.FileName = "main.exe";
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Arguments = (comboBox1.SelectedIndex) + " " + nazwaPliku;
            }
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();

            tresc = proc.StandardOutput.ReadToEnd();
            TextBox1.Text = tresc;
            proc.WaitForExit();
            proc.Close();
            podzial(comboBox1.SelectedIndex);

            Clear();
            AddRow();
            List<zadania> tmp = new List<zadania>();
            int time = 0;

            if (comboBox1.SelectedIndex == 0)
            {
                foreach (var s in Kolejka)
                {
                    AddBlock(s.start, s.end, 1, s.nr.ToString() + ". Task");
                }
            }
            else
            {
                for (int i = 0; i < Kolejka.Count; i++)
                {
                    if (i == 0)
                    {
                        AddBlock(Kolejka[i].r, Kolejka[i].r + Kolejka[i].p, 1, Kolejka[i].nr.ToString() + ". Task");
                        time = Kolejka[i].r + Kolejka[i].p;
                    }
                    else
                    {
                        if (time > Kolejka[i].r)
                        {
                            AddBlock(time, time + Kolejka[i].p, 1, Kolejka[i].nr.ToString() + ". Task");
                            time += Kolejka[i].p;
                        }
                        else
                        {
                            AddBlock(time + Kolejka[i].r, time + Kolejka[i].r + Kolejka[i].p, 1, Kolejka[i].nr.ToString() + ". Task");
                            time += (Kolejka[i].r + Kolejka[i].p);
                        }

                        //if (Kolejka[i].r + Kolejka[i].p + Kolejka[i].q > CMAX)
                        //{
                        //    tmp.Add(Kolejka[i]);
                        //}
                    }
                }
            }
        }

        void Clear()
        {
            DateTime minDate = DateTime.Today;
            DateTime maxDate = minDate.AddDays(GantLenght);
            ganttControl1.ClearGantt();
            RowList.Clear();
            RowGroupList.Clear();
            CreateData(minDate, maxDate);
        }

        private void TextChanged(object sender, EventArgs e)
        {
            int max = 580;
            Size sz = new Size(TextBox1.ClientSize.Width, max);
            TextFormatFlags flags = TextFormatFlags.WordBreak;
            int padding = 3;
            int borders = TextBox1.Height - TextBox1.ClientSize.Height;
            Size sz1 = TextRenderer.MeasureText(TextBox1.Text, TextBox1.Font, sz, flags);
            if (sz1.Height > max)
            {
                sz1.Height = max;
            }
            int h = sz1.Height + borders + padding;
            if (TextBox1.Top + h > this.ClientSize.Height - 10)
            {
                h = this.ClientSize.Height - 10 - TextBox1.Top;
            }
            TextBox1.Height = h;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextBox1.Text = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n";
            Clear();
            if (comboBox1.SelectedIndex == 2)
            {
                elementHost1.Visible = false;
                TextBox1.Location = new Point(TextBox1.Location.X, 80);
            }
            else
            {
                elementHost1.Visible = true;
                TextBox1.Location = new Point(TextBox1.Location.X, 308);
            }
        }
    }
}
