using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Data
{
    public static class MyMathLib
    {

        public static void Frprmn_1(double[] p, double[] pMax, double[] pMin, int n, double ftol, ref int iter, ref double fret, Func<double[], double> pfunc, Action<int, double[], double[]> dfunc)
        {
            /*
             Given a starting point p[1..n], minimization is performed on a function pfunc, using its gradient as calculated by a routine dfunc.The convergence tolerance
            on the function value is input as ftol.Returned quantities are p(the location of the minimum),  iter(the number of iterations that were performed), and fret(the minimum value of the function.  Values of p are kept b/w pmax and pmin). 
            */
            int j, nMaxIter = 1000; ;
            double fp, fp1, xiSquare;
            double[] xi = new double[n], p1 = new double[n];
            Boolean bl;
            //            string strMsg = "";

            double epsilon = 2.0;

            int kk;
            //            string sFile = @"C:\Users\Intugent2\Desktop\Debug.csv";
            //            string[] sLines = new string[4];


            fp = pfunc(p);
            //            (sender as BackgroundWorker).ReportProgress(iter, strMsg);
            //            MessageBox.Show("");
            //            sLines[0] = "\n\n\n\n*******************************************************************************";
            //            sLines[1] = System.Text.Json.JsonSerializer.Serialize(p);
            //            sLines[1] = "Param, " + sLines[1].Substring(1, sLines[1].Length - 2);

            //            File.WriteAllLines(sFile, sLines);


            for (int k = 0; k < nMaxIter; k++)
            {
                dfunc(n, p, xi);



                xiSquare = 0.0;
                for (j = 0; j < n; j++) xiSquare = xiSquare + xi[j] * xi[j];
                if (fp > 1.0e-3) xiSquare = xiSquare / fp;

                //                    epsilon = 2.0* epsilon;
                kk = 0;
                do
                {
                    kk += 1;
                    bl = false;
                    for (j = 0; j < n; j++)
                    {
                        p1[j] = p[j] - epsilon * Math.Abs(p[j]) * xi[j] / xiSquare;

                        if (p1[j] > pMax[j]) p1[j] = pMax[j];
                        else if (p1[j] < pMin[j]) p1[j] = pMin[j];
                    }
                    fp1 = pfunc(p1);                 //Initializations.

                    //                    (sender as BackgroundWorker).ReportProgress(iter, strMsg);
                    //                    MessageBox.Show("");

                    if (fp1 > (1.0 - ftol) * fp)
                    {
                        epsilon = 0.5 * epsilon;
                        bl = true;
                    }
                    else
                    {
                        for (j = 0; j < n; j++) p[j] = p1[j];
                        fp = fp1;
                    }
                } while (bl && epsilon > ftol);
                /*
                                sLines[0] = "\n****** k, kk, epsi, fp ," + k.ToString() + ", " + kk.ToString() + ", " + epsilon.ToString() + ", " + fp.ToString() + ", *******";
                                sLines[1] = System.Text.Json.JsonSerializer.Serialize(p);
                                sLines[2] = System.Text.Json.JsonSerializer.Serialize(xi);
                                sLines[1] = "Param, " + sLines[1].Substring(1, sLines[1].Length - 2);
                                sLines[2] = "df/dparam, " + sLines[2].Substring(1, sLines[2].Length - 2);
                                //                File.AppendAllLines(sFile, sLines);
                */

                if (epsilon < 1.0e-8) break;

                iter = k;
                fret = fp;
            }


        }


    }
}
