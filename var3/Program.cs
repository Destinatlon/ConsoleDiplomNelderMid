using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace var3
{
    class MyNelderMid 
    {
        private double[,] x;
        private Dictionary<double[], double> fxx;
        int n,m;
        double alpha = 1;
        double beta = 0.5;
        double gamma = 2;
        public MyNelderMid(double[] x)//передаю начальную координату
        {
            n = x.Length;
            m = n + 1;
            int k = 1;//шаг
            this.x = new double[m,n];
            int tmp = 0;
            for(int i = 0; i < m; i++)
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
        }

        private void restoreSimplex()
        {
            for (int j = 1; j < m; j++)//вычисляю новый симлекс и значение каждой точки
            {
                fxx[fxx.ElementAt(j).Key] = F(fxx.ElementAt(j).Key);
            }
            for (int i = 0; i < m; i++) 
            {
                for(int j = 0; j < n; j++)
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
            dict= dict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            fxx= fxx.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            fh = dict.Last().Value;
            xh = dict.Last().Key;
            fl = dict.First().Value;
            xl = dict.First().Key;
            var item = dict.First(kvp => kvp.Value == fh);
            dict.Remove(item.Key);
            fg = dict.Last().Value;
            xg = dict.Last().Key;
            xc = new double[n];
            for(int i = 0; i < n; i++)
            {
                for(int j = 0; j < dict.Count; j++)
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
            for(int i = 0; i < n; i++)
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
                for(int i = 0; i < n; i++)
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
            else if(fl<fr && fr < fg)
            {
                Array.Copy(xr, xh, n);
                return;
            }
            else if(fg<fr && fr < fh)
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
            else if(fs>fh)
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
            for(int i = 0; i < n; i++)
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
            double[] temp=new double[n];
            Array.Copy(x1, temp, n);
            Array.Copy(x2, x1, n);
            Array.Copy(temp, x2, n);

        }
        private double F(double[] x) 
        {
            return (1 - x[0]) * (1 - x[0]) + 100 * (x[1] - x[0] * x[0]) * (x[1] - x[0] * x[0]);
            //return x[0] * x[0] + x[0] * x[1] + x[1] * x[1] - 6 * x[0] - 9 * x[1];
            //return 3 * x[0] * x[0] + x[0] * x[1] + 2 * x[1] * x[1] - x[0] - 4 * x[1];
            //return 4 + Math.Pow(x[0] * x[0] + x[1] * x[1], 2.0 / 3.0);
            //return -(-x[0] * x[0] - 5 * x[1] * x[1] - 3 * x[2] * x[2] + x[0] * x[1] - 2 * x[0] * x[2] + 2 * x[1] * x[2] + 11 * x[0] + 2 * x[1] + 18 * x[2] + 10);
        }
        public void ND()
        {
            int count=0;
            for(int i = 0; i < 10000; i++)
            {
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
            for(int i = 0; i < fxx.Count; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    str += $"x{j + 1}:{fxx.ElementAt(i).Key[j]}\t";
                }
                str += $"\t{fxx.ElementAt(i).Value}\n";
            }
            return str;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            double[] x = { -1,0,5,6,8 };
            MyNelderMid nd = new MyNelderMid(x);
            nd.ND();
            Console.WriteLine(nd);
        }
    }
}
