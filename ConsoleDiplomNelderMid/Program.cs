using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDiplomNelderMid
{
    public class MyMethod
    {
        static int i;
        static double min, max;
        static double xl1, xl2, xh1, xh2, xg1, xg2, xc1, xo1, xo2, xc2, xr1, xr2, xs1, xs2, fl, fh, fg, fc, fo, fr, fs;
        static double przn1, przn2, przn3, przn4;
        static double[,] P = new double[500, 2];
        static double[] F = new double[500];
        static int il, ih, ig, k;
        
        static double alpha = 1; 
        static double beta = 0.5;
        static double gamma = 2; 
        static double e = 0.2;
        static double tret = 1.0 / 3.0;
        static double dob;
        static double[,] X = new double[3, 2] { { -2, -2 }, { -1, 2 }, { 1, 3.5 } };
        static double[] FF = new double[3];
        public void NM()
        {
            k = 0;
            i = 0;
            FF[0] = f(X[0, 0], X[0, 1]); P[i, 0] = X[0, 0]; P[i, 1] = X[0, 1]; F[i] = f(P[i, 0], P[i, 1]);
            FF[1] = f(X[1, 0], X[1, 1]); i++; P[i, 0] = X[1, 0]; P[i, 1] = X[1, 1]; F[i] = f(P[i, 0], P[i, 1]);
            FF[2] = f(X[2, 0], X[2, 1]); i++; P[i, 0] = X[2, 0]; P[i, 1] = X[2, 1]; F[i] = f(P[i, 0], P[i, 1]);
            while (k < 70)
            {
                k++;
                il = 0; min = FF[0]; xl1 = X[0, 0]; xl2 = X[0, 0]; fl = f(xl1, xl2);
                for (int i = 0; i <= 2; i++)
                {
                    if (FF[i] < min)
                    {
                        min = FF[i];
                        xl1 = X[i, 0]; xl2 = X[i, 1];
                        fl = f(xl1, xl2);
                        il = i;
                    }
                }
            }
            max = FF[0]; xh1 = X[0, 0]; ih = 0; xh2 = X[0, 1]; fh = f(xh1, xh2);
            for (int i = 0; i <= 2; i++)
            {
                if (FF[i] > max)
                {
                    max = FF[i];
                    xh1 = X[i, 0]; xh2 = X[i, 1];
                    fh = f(xh1, xh2);
                    ih = i;
                }
            }
            xg1 = X[0, 0]; xg2 = X[0, 1]; ig = 0; fg = f(xg1, xg2);
            for (int i = 0; i <= 2; i++)
            {
                if ((FF[i] > min) && (FF[i] < max))
                {

                    xg1 = X[i, 0]; xg2 = X[i, 1];
                    fg = f(xg1, xg2);
                    ig = i;
                }
            }
            xc1 = 0.5 * (xl1 + xg1); xc2 = 0.5 * (xl2 + xg2); fc = f(xc1, xc2);
            //критерий остановки
            dob = Math.Pow((fl - fc), 2) + Math.Pow((fh - fc), 2) + Math.Pow((fg - fc), 2);

            dob = konez(xl1, xl2, xh1, xh2, xg1, xg2);                     //возможно внести 1/3 под корень
                                                                           //отражение
            xo1 = xc1 + alpha * (xc1 - xh1); xo2 = xc2 + alpha * (xc2 - xh2); fo = f(xo1, xo2);
            if (fo < fl)
            {
                xr1 = xc1 + gamma * (xo1 - xc1); xr2 = xc2 + gamma * (xo2 - xc2); fr = f(xr1, xr2);
                if (fr < fo)
                {
                    xh1 = xr1; X[ih, 0] = xr1; xh2 = xr2; X[ih, 1] = xr2; FF[ih] = f(xh1, xh2);
                    i++; P[i, 0] = xr1; P[i, 1] = xr2;
                    F[i] = f(P[i, 0], P[i, 1]);
                }
                else
                {
                    xh1 = xo1; X[ih, 0] = xo1; xh2 = xo2; X[ih, 1] = xo2; FF[ih] = f(xh1, xh2);
                    i++; P[i, 0] = xo1; P[i, 1] = xo2;
                    F[i] = f(P[i, 0], P[i, 1]);
                }
            }
            if ((fl <= fo) && (fo < fh))
            {
                xs1 = xc1 + beta * (xh1 - xc1); xs2 = xc2 + beta * (xh2 - xc2);
                if (fs < fo)
                {
                    xh1 = xs1; X[ih, 0] = xs1; xh2 = xs2; X[ih, 1] = xs2; FF[ih] = f(xh1, xh2);
                    i++; P[i, 0] = xs1; P[i, 1] = xs2;
                    F[i] = f(P[i, 0], P[i, 1]);
                }
                else
                {
                    xh1 = xo1; X[ih, 0] = xo1; xh2 = xo2; X[ih, 1] = xo2; FF[ih] = f(xh1, xh2);
                    i++; P[i, 0] = xo1; P[i, 1] = xo2;
                    F[i] = f(P[i, 0], P[i, 1]);
                }
            }
            if (fo >= fh)
            {
                xh1 = xl1 + 0.5 * (xh1 - xl1); xh2 = xl2 + 0.5 * (xh2 - xl2); X[ih, 0] = xh1; X[ih, 1] = xh2; FF[ih] = f(xh1, xh2);
                i++; P[i, 0] = xh1; P[i, 1] = xh2;
                F[i] = f(P[i, 0], P[i, 1]);
                xg1 = xl1 + 0.5 * (xg1 - xl1); xg2 = xl2 + 0.5 * (xg2 - xl2); X[ig, 0] = xg1; X[ig, 1] = xg2; FF[ig] = f(xg1, xg2);
                i++; P[i, 0] = xg1; P[i, 1] = xg2;
                F[i] = f(P[i, 0], P[i, 1]);
            }
            przn1 = Math.Round(xl1, 1);
            przn2 = Math.Round(xl2, 1);
            przn3 = f(przn1, przn2);
        }
       
        static double konez(double l1, double l2, double h1, double h2, double g1, double g2)
        {
            double[] m = new double[4];
            double max;
            m[0] = h1 - l1;
            m[1] = h2 - l2;
            m[2] = g1 - l1;
            m[3] = g2 - l2;
            max = m[0];
            for (int i = 0; i <= 3; i++)
            {
                if (m[i] > max)
                    max = m[i];

            }
            return max;
        }
        static double f(double x1, double x2)
        {
            //return 4 * (x1 - 2) * (x1 - 2) + 6 * (x2 - 4) * (x2 - 4);
            // return x1*x1+(x2-1)*(x2-1)+x2*x2;

            return (1-x1)*(1-x1)+100*(x2-x1*x1)*(x2-x1*x1);
        }
        public override string ToString()
        {
            var str =String.Empty;
            str += "x:";
            foreach(var i in FF)
            {
                str += $"{i}\t";
            }
            return str;
        }
    }
    class Program
    {

        static void Main(string[] args)
        {
            MyMethod mm = new MyMethod();
            mm.NM();
            Console.WriteLine(mm.ToString());
        }
    }
}

