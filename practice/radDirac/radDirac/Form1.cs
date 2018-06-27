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

namespace radDirac
{

    public interface IOrbital
    {
        void Assign(int _n, int _l, double _j);
        void Adjust();
        //void Draw(void);
        void Output();
        double CalcDG(int i, double[] P, double G, double F);
        double CalcDF(int i, double[] P, double G, double F);

        int PreInteg(double[] P);
        int HammingA(double[] P, int i_jnt);
        int HammingB(double[] P, int i_jnt);
        void Normalize(double a, int i_jnt);

    }

    public partial class Form1 : Form
    {
        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=
        //    Program raddirac
        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=



        //---- experimental settings
        const double Z = 1.0;
        public const double DX = 0.0625;        // size of a cell in x-space
        public const int R_NUM = 256;           // total number of grids
        public const int R_FAR = R_NUM - 1;       // index of farthest grid
        const int R_O = 160;           // index of Bohr radius

        //---- graphics settings
        //const int WIN_WIDTH = 512;
        //const int WIN_HEIGHT = 256;
        //const int BASE_Y = 160;
        //const double MAG_Y = 196.0;
        //const double OUTPUT_R_MAX = 64.0;       // max radius output [a.u]

        public Form1()
        {
            InitializeComponent();
        }

        //---- table of Radius
        public static double[] R;

        //---- declaration of class Potential
        public static double[] U; // rV, potential muliplied by radius




        private void button1_Click(object sender, EventArgs e)
        {
            //radDirac
            //Orbital O = new Orbital();

            R = new double[R_NUM];
            for (int i = 0; i <= R_FAR; i++)
            {
                R[i] = (1.0 / Z) * Math.Exp(DX * (i - R_O));
            }

            U = new double[R_NUM];
            for (int i = 0; i <= R_FAR; i++)
            {
                U[i] = -Z;                      // Coulomb potential of nuclear
            }

            Orbital O = new Orbital(); //R, Uの設定の前に置くとR, UがNullでエラー
            for (int n = 1; n <= 3; n++)
            {
                for (int l = 0; l <= n - 1; l++)
                {
                    for (double j = l - 0.5; j <= l + 0.5; j += 1.0)
                    {
                        if (j < 0.0) continue;

                        O.Assign(n, l, j);
                        O.Adjust();
                        //O.Output();
                    }
                }
            }

            Console.WriteLine("End");
        }


        ////---- draws wave function
        ////void Orbital::Draw(void)
        ////{
        ////	const double MAG_X = (double)WIN_WIDTH / OUTPUT_R_MAX;
        ////	NXDrawMoveto();
        ////	for (int i = 0; i <= R_FAR; i++) {
        ////		NXDrawLineto(int(R[i] * MAG_X), BASE_Y - int(G[i] * MAG_Y));
        ////	}
        ////}



    }
}
