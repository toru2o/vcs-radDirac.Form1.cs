using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace radSchro
{
    public partial class Form1 : Form
    {

        //---- definition of fundermental constants
        //---- physical settings
        const double Z = 1.0;

        //---- experimental settings
        const double DX = 0.0625;        // size of a cell in x-space
        const int R_NUM = 256;           // total number of grids
        const int R_FAR = R_NUM - 1;       // index of farthest grid
        const int R_O = 160;           // index of Bohr radius


        //---- graphics settings
        const int WIN_WIDTH = 512;
        const int WIN_HEIGHT = 256;
        const int BASE_Y = 160;
        const double MAG_Y = 196.0;
        const double OUTPUT_R_MAX = 64.0;       // max radius output [a.u]

        double[] R = new double[R_NUM];
        double[] U = new double[R_NUM];   // rV, potential muliplied by radius

        //class Orbital
        int n, l;                   // quantum numbers
        double E;                      // energy of this orbital
        double[] G = new double[R_NUM];     // wave function
        double[] F = new double[R_NUM];




        public Form1()
        {
            InitializeComponent();
        }


        //-- sets quantum numbers
        void Assign(int _n, int _l)
        {
            n = _n;
            l = _l;
            E = -0.5 * Z * Z / (n * n) - 0.01;  // a guess of the true energy
        }

        double sqr(double x)
        {
            return x * x;
        }

        //---- prepares for integration
        int PreInteg(double[] P)
        {

            int i;
            // sets parameters for differential equation
            for( i=0 ; i<=R_FAR ; i++ )
            {
                P[i] = 2.0* R[i]* U[i] - 2.0* sqr(R[i])* E + l*(l+1);
            }
            // sets G,F at near origin
            for( i=0 ; i<4 ; i++ ){
                G[i] = Math.Pow(R[i], l+1);
                F[i] = (double)(l+1)* G[i];
            }
            // sets G,F at far away
            double a = -Math.Sqrt(-2.0 * E);
            for( i=R_FAR ; i>R_FAR-4 ; i-- )
            {
                G[i] = Math.Exp(a* R[i]);
                F[i] = (a* R[i])* G[i];
            }

            // searches the cross grid and reports it as the joint grid
            for( i=R_FAR ; P[i]>0.0 && i>=0 ; i-- );

            return i;
        }

        //---- represents derivative of G
        double CalcDG(int i, double[] P, double G, double F)
        {
            return F;
        }

        //---- represents derivative of F
        double CalcDF(int i, double[] P, double G, double F)
        {
            return P[i] * G + F;
        }

        //---- integrates from origin to the fitting grid
        int HammingA(double[] P, int i_fit)
        {
            int i, node = 0;
            double Gp, Fp, Gm, Fm, Gc, Fc, Gpc = 0.0, Fpc = 0.0, dGm, dFm;
            double[] dG = new double[R_NUM];
            double[] dF = new double[R_NUM];

            // prepares derivatives at grids near origin
            for (i = 0; i < 4; i++)
            {
                dG[i] = CalcDG(i, P, G[i], F[i]);
                dF[i] = CalcDF(i, P, G[i], F[i]);
            }

            // integrates from the origin to the fitting grid
            for (; i <= i_fit; i++)
            {
                // calculates predictors
                Gp = G[i - 4] + (4.0 * DX / 3.0) * (2 * dG[i - 1] - dG[i - 2] + 2 * dG[i - 3]);
                Fp = F[i - 4] + (4.0 * DX / 3.0) * (2 * dF[i - 1] - dF[i - 2] + 2 * dF[i - 3]);
                // calculates modifiers
                Gm = Gp - (112.0 / 121.0) * (Gpc);
                Fm = Fp - (112.0 / 121.0) * (Fpc);
                // calculates derivative at modifiers
                dGm = CalcDG(i, P, Gm, Fm);
                dFm = CalcDF(i, P, Gm, Fm);
                // calculates correctors
                Gc = (9.0 / 8.0) * (G[i - 1]) - (1.0 / 8.0) * (G[i - 3]) + (3.0 * DX / 8.0) * (dGm + 2 * dG[i - 1] - dG[i - 2]);
                Fc = (9.0 / 8.0) * (F[i - 1]) - (1.0 / 8.0) * (F[i - 3]) + (3.0 * DX / 8.0) * (dFm + 2 * dF[i - 1] - dF[i - 2]);
                // calculates difference between predictors and correctors
                Gpc = Gp - Gc;
                Fpc = Fp - Fc;
                // calculates the next step
                G[i] = Gc + (9.0 / 121.0) * Gpc;
                F[i] = Fc + (9.0 / 121.0) * Fpc;
                // calculates derivative at the next step
                dG[i] = CalcDG(i, P, G[i], F[i]);
                dF[i] = CalcDF(i, P, G[i], F[i]);

                if (G[i] * G[i - 1] < 0) node++;
            }

            return node;
        }

        //---- integrates from far to the fitting grid
        int HammingB(double[] P, int i_fit)
        {
            int i, node = 0;
            double Gp, Fp, Gm, Fm, Gc, Fc, Gpc = 0.0, Fpc = 0.0, dGm, dFm;
            double[] dG = new double[R_NUM];
            double[] dF = new double[R_NUM];

            // prepares derivatives at far grids
            for (i = R_FAR; i > R_FAR - 4; i--)
            {
                dG[i] = CalcDG(i, P, G[i], F[i]);
                dF[i] = CalcDF(i, P, G[i], F[i]);
            }

            // integrates from far to the fitting grid
            for (; i >= i_fit; i--)
            {
                // calculates predictors
                Gp = G[i + 4] - (4.0 * DX / 3.0) * (2 * dG[i + 1] - dG[i + 2] + 2 * dG[i + 3]);
                Fp = F[i + 4] - (4.0 * DX / 3.0) * (2 * dF[i + 1] - dF[i + 2] + 2 * dF[i + 3]);
                // calculates modifiers
                Gm = Gp - (112.0 / 121.0) * (Gpc);
                Fm = Fp - (112.0 / 121.0) * (Fpc);
                // calculates derivative at modifiers
                dGm = CalcDG(i, P, Gm, Fm);
                dFm = CalcDF(i, P, Gm, Fm);
                // calculates correctors
                Gc = (9.0 / 8.0) * (G[i + 1]) - (1.0 / 8.0) * (G[i + 3]) - (3.0 * DX / 8.0) * (dGm + 2 * dG[i + 1] - dG[i + 2]);
                Fc = (9.0 / 8.0) * (F[i + 1]) - (1.0 / 8.0) * (F[i + 3]) - (3.0 * DX / 8.0) * (dFm + 2 * dF[i + 1] - dF[i + 2]);
                // calculates difference between predictors and correctors
                Gpc = Gp - Gc;
                Fpc = Fp - Fc;
                // calculates the next step
                G[i] = Gc + (9.0 / 121.0) * Gpc;
                F[i] = Fc + (9.0 / 121.0) * Fpc;
                // calculates derivative at the next step
                dG[i] = CalcDG(i, P, G[i], F[i]);
                dF[i] = CalcDF(i, P, G[i], F[i]);

                if (G[i] * G[i + 1] < 0) node++;
            }

            return node;
        }

        //---- normalizes wave function
        void Normalize(double a, int i_fit)
        {
            int i;
            double sum, norm;

            for (i = i_fit; i <= R_FAR; i++)
            {
                G[i] *= a;
                F[i] *= a;
            }
            for (i = 0, sum = 0.0; i <= R_FAR; i++)
            {
                sum += sqr(G[i]) * R[i] * DX;
            }
            for (i = 0, norm = Math.Sqrt(1.0 / sum); i <= R_FAR; i++)
            {
                G[i] *= norm;
                F[i] *= norm;
            }
        }



        //-- adjusts energy so that an eigen wave function can stand
        void Adjust()
        {
            double[] P = new double[R_NUM];
            double GA, GB, LogA, LogB, dE;
            int true_node = n - l - 1, node, i_fit;

            do
            {
                i_fit = PreInteg(P);                    // prepares integration
                node = HammingA(P, i_fit);             // integrates from origin
                GA = G[i_fit];                         // stores G at joint grid
                LogA = F[i_fit] / (R[i_fit] * G[i_fit]);     // stores logarithmic derivative of G at joint grid
                node += HammingB(P, i_fit);             // integrates from far
                GB = G[i_fit];                         // stores G at joint grid
                LogB = F[i_fit] / (R[i_fit] * G[i_fit]);     // stores logarithmic derivative of G at joint grid
                Normalize(GA / GB, i_fit);                // connects and normalize

                dE = -0.5 * sqr(G[i_fit]) * (LogB - LogA);      // estimates energy modification
                if (node != true_node)
                {                  // if node is violated
                    dE = (double)((true_node - node) / (n * n * n));    // rough modification
                }
                else if (Math.Abs(dE) > 1.0e-2)
                {            // if still large error
                    dE *= 0.5;                              // smaller modification
                }

                E += dE;                                    // modifies energy
                Debug.Assert(E <= 0.0, "Energy can not converge");
                //if (E > 0.0)
                //{                              // it can't be binding state
                //    fprintf(stderr, "Energy can not converge.\n");
                //    exit(1);
                //}
            } while (Math.Abs(dE) > 1e-8);                    // repeats until the modification is sufficiently small
            Console.WriteLine("Orbital energy of ({0:d} {1:d}) has converged to {2:f6} [a.u]", n, l, E);
            //fprintf(stderr, "Orbital energy of (%d %d) has converged to %18.16f [a.u]\n", n, l, E);
        }

        //---- outputs wave function
        void Output()
        {
            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
            string fname = "D:/VCS2013/practice/radSchro/data1.txt";
            StreamWriter writer = new StreamWriter(fname, false, sjisEnc);//書き込むファイルが既に存在している場合は、上書きする

            for (int i = 0; i <= R_FAR; i++)
            {
                writer.WriteLine("{0:f6} {1:f6}", R[i], G[i]);
                //if (R[i] > OUTPUT_R_MAX) break;
            }
            writer.Close();
        }



        private void button1_Click(object sender, EventArgs e)
        {

            //---- table of Radius
            for (int i = 0; i <= R_FAR; i++)
            {
                R[i] = (1.0 / Z) * Math.Exp(DX * (i - R_O));
                //Console.WriteLine(R[i]);
            }

            //---- declaration of Potential
            for (int i = 0; i <= R_FAR; i++)
            {
                U[i] = -Z;                      // Coulomb potential of nuclear
            }

            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
            string fname = "D:/VCS2013/practice/radSchro/data1.txt";
            StreamWriter writer = new StreamWriter(fname, false, sjisEnc);//書き込むファイルが既に存在している場合は、上書きする

            for (int n = 1; n <= 3; n++)
            {
                for (int l = 0; l < n; l++)
                {
                    Assign(n, l);
                    Adjust();
                    //Draw();
                    for (int i = 0; i <= R_FAR; i++)
                    {
                        writer.WriteLine("{0:f6} {1:f6}", R[i], G[i]);
                        //if (R[i] > OUTPUT_R_MAX) break;
                    }
                    writer.WriteLine("");
                }
            }
            writer.Close();
            //Output();

            Console.WriteLine("End");
        }

        ////---- draws wave function
        //void Orbital::Draw(void )
        //{
        //    const double MAG_X = (double)WIN_WIDTH / OUTPUT_R_MAX;
        //    NXDrawMoveto();
        //    for (int i = 0; i <= R_FAR; i++)
        //    {
        //        NXDrawLineto(int(R[i] * MAG_X), BASE_Y - int(G[i] * MAG_Y));
        //    }
        //}



        ////---- main function
        //int main(void )
        //{
        //    NXOpenWindow("Radial Wave Function of Hydrogen Atom", WIN_WIDTH, WIN_HEIGHT);

        //    NXClearWindow();




    }
}
