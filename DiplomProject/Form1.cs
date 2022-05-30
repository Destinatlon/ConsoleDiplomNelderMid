using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiplomProject
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var tmp = textBox1.Text.Split(' ').ToArray();
            double[] x = new double[tmp.Length];
            for(int i = 0; i < tmp.Length; i++)
            {
                x[i] = Double.Parse(tmp[i]);
            }
            double k = Double.Parse(textBox2.Text);
            MyNelderMid nd = new MyNelderMid(x,k);
            nd.ND();
            for(int i = 0; i < x.Length; i++)
            {
                dataGridView1.Columns.Add($"x{i+1}", $"x{i+1}");
                dataGridView2.Columns.Add($"x{i + 1}", $"x{i + 1}");
            }
            //dataGridView1.Columns.Add("f(x)", "f(x)");
            dataGridView1.RowCount = nd.xStore.Count;
            for (int i = 0; i < nd.xStore.Count; i++)
            {
                for (int j = 0; j < nd.xStore.ElementAt(i).Length; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = nd.xStore.ElementAt(i)[j];
                }
            }
            dataGridView2.RowCount = 1;
            for (int j = 0; j < nd.xStore.Last().Length; j++)
            {
                dataGridView2.Rows[0].Cells[j].Value = Math.Round(nd.xStore.Last()[j],4);
            }
            textBox3.Text = Convert.ToString(Math.Round(nd.F(nd.xStore.Last()),4));
        }

        class MyNelderMid
        {
            public double[,] x;
            public List<double[]> xStore;
            public Dictionary<double[], double> fxx;
            int n, m;
            double alpha = 1;
            double beta = 0.5;
            double gamma = 2;
            double k;
            public MyNelderMid(double[] x,double k)//передаю начальную координату
            {
                n = x.Length;
                m = n + 1;
                this.k = k;//шаг
                this.x = new double[m, n];
                int tmp = 0;
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        this.x[i, j] = x[j];
                    }
                }
                for (int i = 1; i < m; i++)
                {
                    this.x[i, tmp] += k;//создаю симплекс
                    tmp++;
                }
                xStore = new List<double[]>();
            }

            private void restoreSimplex()
            {
                for (int j = 1; j < m; j++)//вычисляю новый симлекс и значение каждой точки
                {
                    fxx[fxx.ElementAt(j).Key] = F(fxx.ElementAt(j).Key);
                }
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        x[i, j] = fxx.ElementAt(i).Key[j];
                    }
                }
            }
            double fh, fg, fl;
            double[] xh, xg, xl;
            double[] xc;
            private void sort()
            {
                fxx = new Dictionary<double[], double> { };
                Dictionary<double[], double> dict = new Dictionary<double[], double> { };
                for (int i = 0; i < m; i++)
                {
                    double[] tmp = new double[n];
                    for (int j = 0; j < n; j++)//режу построчно
                    {
                        tmp[j] = x[i, j];
                    }
                    dict.Add(tmp, F(tmp));
                    fxx.Add(tmp, F(tmp));
                }

                dict = dict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                fxx = fxx.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                fh = dict.Last().Value;
                xh = dict.Last().Key;
                fl = dict.First().Value;
                xl = dict.First().Key;
                var item = dict.First(kvp => kvp.Value == fh);
                dict.Remove(item.Key);
                fg = dict.Last().Value;
                xg = dict.Last().Key;
                xc = new double[n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < dict.Count; j++)
                    {
                        xc[i] += dict.ElementAt(j).Key[i];
                    }
                    xc[i] /= n;
                }
            }
            double[] xr;
            double fr;
            public void reflection()
            {
                xr = new double[n];
                for (int i = 0; i < n; i++)
                {
                    xr[i] = (1 + alpha) * xc[i] - alpha * xh[i];
                }
                fr = F(xr);
            }
            double[] xe;
            double fe;
            public void checkCondition()
            {
                if (fr < fl)
                {
                    xe = new double[n];
                    for (int i = 0; i < n; i++)
                    {
                        xe[i] = (1 - gamma) * xc[i] + gamma * xr[i];
                    }
                    fe = F(xe);
                    if (fe < fr)
                    {
                        Array.Copy(xe, xh, n);
                        return;
                    }
                    else
                    {
                        Array.Copy(xr, xh, n);
                        return;
                    }
                }
                else if (fl < fr && fr < fg)
                {
                    Array.Copy(xr, xh, n);
                    return;
                }
                else if (fg < fr && fr < fh)
                {
                    Swap(ref xr, ref xh);
                    Swap(ref fr, ref fh);
                    squezze();
                }
                else if (fh < fr)
                {
                    squezze();
                }
                if (fs < fh)
                {
                    squezze();
                    Swap(ref xh, ref xs);
                    return;
                }
                else if (fs > fh)
                {
                    gl_squezze();
                    return;
                }
            }
            double[] xs;
            double fs;
            private void squezze()//сжатие
            {
                xs = new double[n];
                for (int i = 0; i < n; i++)
                {
                    xs[i] = beta * xh[i] + (1 - beta) * xc[i];
                }
                fs = F(xs);
            }
            private void gl_squezze()//гомотетия
            {
                for (int j = 1; j < m; j++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        fxx.ElementAt(j).Key[i] = xl[i] + (fxx.ElementAt(j).Key[i] - xl[i]) * 0.5;
                    }
                    fxx[fxx.ElementAt(j).Key] = F(fxx.ElementAt(j).Key);
                }
            }
            private void Swap(ref double x1, ref double x2)
            {
                var temp = x1;
                x1 = x2;
                x2 = temp;
            }
            private void Swap(ref double[] x1, ref double[] x2)
            {
                double[] temp = new double[n];
                Array.Copy(x1, temp, n);
                Array.Copy(x2, x1, n);
                Array.Copy(temp, x2, n);

            }
            public double F(double[] x)
            {
                //return (1 - x[0]) * (1 - x[0]) + 100 * (x[1] - x[0] * x[0]) * (x[1] - x[0] * x[0]);
                //return x[0] * x[0] + x[0] * x[1] + x[1] * x[1] - 6 * x[0] - 9 * x[1];
                //return 3 * x[0] * x[0] + x[0] * x[1] + 2 * x[1] * x[1] - x[0] - 4 * x[1];
                //return 4 + Math.Pow(x[0] * x[0] + x[1] * x[1], 2.0 / 3.0);
                return -(-x[0] * x[0] - 5 * x[1] * x[1] - 3 * x[2] * x[2] + x[0] * x[1] - 2 * x[0] * x[2] + 2 * x[1] * x[2] + 11 * x[0] + 2 * x[1] + 18 * x[2] + 10);
            }
            private void addToStore()
            {
                double[] tmp = new double[n];
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        tmp[j] = x[i, j];
                    }
                    xStore.Add(tmp);
                    tmp = new double[n];
                }
            }
            public void ND()
            {
                int count = 0;
                for (int i = 0; i < 10000; i++)
                {
                    addToStore();
                    sort();
                    reflection();
                    checkCondition();
                    restoreSimplex();
                    count++;
                    if (Math.Abs(x[0, 0] - x[1, 0]) < 0.001 && Math.Abs(x[0, 1] - x[1, 1]) < 0.001)
                    {
                        break;
                    }
                }
                Console.WriteLine($"Cycle: {count}");
            }
            public override string ToString()
            {
                string str = String.Empty;
                for (int i = 0; i < fxx.Count; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        str += $"x{j + 1}:{fxx.ElementAt(i).Key[j]}\t";
                    }
                    str += $"\t{fxx.ElementAt(i).Value}\n";
                }
                return str;
            }
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)&& e.KeyChar != ',';
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != ' ';
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // creating Excel Application  
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            // creating new WorkBook within Excel application  
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            // creating new Excelsheet in workbook  
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            // see the excel sheet behind the program  
            app.Visible = true;
            // get the reference of first sheet. By default its name is Sheet1.  
            // store its reference to worksheet  
            //worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            // changing the name of active sheet  
            worksheet.Name = "Exported from gridview";
            // storing header part in Excel  
            for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
            {
                worksheet.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
            }
            // storing Each row and column value to excel sheet  
            int rows = 2;
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value;
                }
                rows++;
            }
            worksheet.Cells[rows, 1] = "Final point and value";
            worksheet.Cells[rows, dataGridView1.Columns.Count+1] = "F(x)";
            rows++;
            for (int j = 0; j < dataGridView2.Columns.Count; j++)
            {
                worksheet.Cells[rows, j + 1] = dataGridView2.Rows[0].Cells[j].Value;
            }
            worksheet.Cells[rows, dataGridView1.Columns.Count + 1] = textBox3.Text;
            // save the application  
            workbook.SaveAs("C:\\Users\\Professional\\source\\repos\\ConsoleDiplomNelderMid\\DiplomProject\\output.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            MessageBox.Show("Saved successfully");
        }
    }
}
