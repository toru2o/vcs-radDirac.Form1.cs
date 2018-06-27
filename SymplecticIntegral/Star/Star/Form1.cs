using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Star
{
    public partial class Form1 : Form
    {

        //---- physical constants
        const double Mass = 1.0; // mass of a star
        const double G = 1.0; // Gravity constant
        //---- experimental settings
        const double dT = 1.0 / 100.0; //1.0/32.0 この設定では2つの星が近づき過ぎると軌道が変になる; // time slice
        const int N = 4;          // number of stars

        //---- declaration of structure Matter
        struct Matter
        {
            public Vector2 p; //momentum
            public Vector2 q; //position
            public Vector2 f; //force
        }

        //string[,] data; //animation用
        Matter[] M = new Matter[N];
        int x = 0;  // 球の x 座標
        int y = 0;  // 球の y 座標
        int[] xp = new int[N];
        int[] yp = new int[N];

        public Form1()
        {
            InitializeComponent();
        }

        //---- initializes properties of stars
        void Init(Matter[] M)
        {
            for (int n = 0; n < N; n++)
            {
                // locates stars on circumference
                double ang = 2.0 * Math.PI * (double)n / (double)N;
                M[n].p = new Vector2(-1.2 * Math.Sin(ang), 1.2 * Math.Cos(ang));
                M[n].q = new Vector2(Math.Cos(ang), Math.Sin(ang));
            }
        }

        //---- evolves the system
        void Evolve(Matter[] M, double dt)
        {
            for (int n = 0; n < N; n++)
            {
                Vector2 v = (M[n].p) * (0.5 * dt * (1.0 / Mass));
                M[n].q = M[n].q + v; 

            }
            CalcField(M);
        
            for (int n = 0; n < N; n++)
            {
                Vector2 v = (M[n].f) * dt;
                M[n].p = M[n].p + v;
            }

            for (int n = 0; n < N; n++)
            {
                Vector2 v = (M[n].p) * (0.5 * dt * (1.0 / Mass));
                M[n].q = M[n].q + v;
            }
        }

        //---- calculates gravity force amoung two stars
        Vector2 Interaction(Matter M1, Matter M2) 
        {
            Vector2 vec_r = M1.q - M2.q;
            double r = Vector2.Norm(vec_r);
            return vec_r * (-G * Mass * Mass / (r * r * r));
        }

        //---- calculates gravity field
        void CalcField(Matter[] M)
        {
            // resets M[].f
            for (int n1 = 0; n1 < N; n1++)
            {
                M[n1].f = new Vector2(0.0, 0.0);
            }
            // sums up M[].f
            for (int n1 = 0; n1 < N; n1++)
            {
                for (int n2 = n1 + 1; n2 < N; n2++)
                {
                    Vector2 f = Interaction(M[n1], M[n2]);
                    M[n1].f = M[n1].f + f; //Vector2の和
                    M[n2].f = M[n2].f - f;
                }
            }
        }

        //---- calculates Total Energy
        double CalcEnergy(Matter[] M)
        {
            double E = 0.0;
	        // sums up the energy
	        for (int n1 = 0; n1 < N; n1++)
            {
                E += (1.0 / (2.0 * Mass)) * Vector2.Norm2(M[n1].p); // kinetic energy
		        for (int n2 = n1 + 1; n2 < N; n2++)
                {
                    Vector2 v = (M[n1].q) - (M[n2].q);
                    E += -G * Mass * Mass / Vector2.Norm(v); // potential energy
                }
	        }
            return E;
        }


        void draw(Graphics g)
        {
            float xc = 400;
            g.TranslateTransform(xc, this.Height / 2);//座標軸の移動
            g.ScaleTransform(1.0f, -1.0f);//座標軸の反転, float指定が必要
            for (int j = 0; j < N; j++)
            {
                Rectangle rect = new Rectangle(xp[j], yp[j], 4, 4);
                g.FillEllipse(new SolidBrush(Color.Blue), rect);
                g.DrawEllipse(new Pen(Color.Blue), rect);
            }

        }


        void timer_Tick(object sender, EventArgs e)
        {
            Evolve(M, dT);
            double mx = 50;
            double my = 50;
            for (int i = 0; i < N; i++)
            {
                xp[i] = (int)((M[i].q).x * mx);
                yp[i] = (int)((M[i].q).y * my);
            }

            this.Invalidate();  // 再描画を促し,OnPaint起動
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            draw(g);
            //Evolve(M, dT); //これがあるとNG
            //System.Threading.Thread.Sleep(100);//タイマーを使用する場合は不要かも
        }



        private void button1_Click(object sender, EventArgs e)
        {
            //star
            //useChart use = new useChart();
            //use.chartSetup("Title", "Axis X", "Axis Y", "series-1");
            //double xMin = -5.0;
            //double xMax = 5.0;
            //double yMin = -5.0;
            //double yMax = 5.0;
            //use.xyLimit(xMin, xMax, yMin, yMax);


            double T = 0.0;
            double Te = 50.0; //20.0
            int ct = (int)(Te / dT);

            //string[,] data = new string[N, ct];
            Console.WriteLine("ct= {0:d}", ct);

            Init(M);

            //運動をanimation描画 data[j, i]をglobalにする
            Timer timer;
            timer = new Timer()
            {
                Interval = 1, //5
                Enabled = true,//timer.Start()と同じ
            };
            timer.Tick += new EventHandler(timer_Tick);
            this.DoubleBuffered = true;  // ダブルバッファリング


        }

        private void button2_Click(object sender, EventArgs e)
        {
            //star timer不使用
            // "アプリケーションはブレークモードになっています" のメッセージが出て止まるので
            // ツール-->オプション-->デバッグ-->全般　"マネージ互換モードの使用"にをチェックを入れる
            Init(M);

            this.DoubleBuffered = true;


            for (int k = 0; k < 50000; k++)
            {
                //Evolve(M, dT);
                double mx = 50;
                double my = 50;
                for (int i = 0; i < N; i++)
                {
                    xp[i] = (int)((M[i].q).x * mx);
                    yp[i] = (int)((M[i].q).y * my);
                }

                this.Refresh();//OnPaingを起動
                System.Threading.Thread.Sleep(1);
                Evolve(M, dT);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // chartControlで表示
            useChart use = new useChart();
            use.chartSetup("Title", "Axis X", "Axis Y", "series-1");
            double xMin = -5.0;
            double xMax = 5.0;
            double yMin = -5.0;
            double yMax = 5.0;
            use.xyLimit(xMin, xMax, yMin, yMax);

            double T = 0.0;
            double Te = 110.0; //20.0
            int ct = (int)(Te / dT);

            string[,] data = new string[N, ct];
            Console.WriteLine("ct= {0:d}", ct);

            Init(M);

            for (int i = 0; i < ct; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    data[j, i] = ((M[j].q).x).ToString() + " " + (M[j].q.y).ToString();
                    use.setScatter((M[j].q).x, (M[j].q).y);
                    Console.WriteLine(data[j, i]);
                }
                Evolve(M, dT);

                T += dT;
                // fmt.Println(*M[0].q)
            }
            use.showChart();

        }
    }
}
