using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

/*

// List<1次元配列> を表示
void writeList<Type>(List<Type[]> seq)
// 文字列（スライスに格納）から重複を許してm個を取り出して並べる順列を出力
List<string[]> permutateDup(string[] ss, int m)
bool isConnected(List<int>[] adjacent, int start, int goal)
List<string> getIdData(string fname)
//int[], double[]を空白区切りの文字列に変換
string arrayToStr<Type>(Type[] ar)
List<int[]> permutateInt(int n, int r)
void fwriteLine(string fname, string line)
void fwriteLines(string fname, string[] lines)
List<string> freadLines(string fname)
void writeArray(double[] ar)
// 配列表示の終わりは "] "
void writeAr<Type>(Type[] ar)
// 配列表示の終わりは "]"で改行
void writeLineAr<Type>(Type[] ar)

*/


namespace rosalind02
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        // List<1次元配列> を表示
        void writeList<Type>(List<Type[]> seq)
        {
            foreach (var s in seq)
            {
                writeAr(s);
            }
            Console.WriteLine("");
        }

        // 文字列（スライスに格納）から重複を許してm個を取り出して並べる順列を出力
        // Goのソースをそのまま移植したのではNG. permSub(xs.Add(ss[i]) はエラーになるので
        List<string[]> permutateDup(string[] ss, int m)
        {
            var seq = new List<string[]>();

            void permSub(List<string> xs)
            {
                if (xs.Count >= m)
                {
                    string[] lt = new string[xs.Count];
                    for (int i=0; i < lt.Length;i++)
                    {
                        lt[i] = xs[i];
                    }
                    //writeAr(lt);
                    seq.Add(lt);
                }
                else
                {
                    for (int i = 0; i < ss.Length; i++)
                    {
                        xs.Add(ss[i]);
                        permSub(xs);
                        xs.RemoveAt(xs.Count - 1); //Goと違ってこれが必要
                    }
                }
            }

            permSub(new List<string>());
            return seq;
        }


        // node番号は0, 1, 2, ...   1始まりにするとプログラムミスをしやすいので
        bool isConnected(List<int>[] adjacent, int start, int goal)
                {
                    List<int> walked = new List<int>();
                    bool ret = false;

                    void dfs(int goal2, List<int> walked2)
                    {
                        if (!ret)
                        {
                            int n = walked2[walked2.Count - 1];
                            if (n == goal2)
                            {
                                ret = true;
                            }
                            else
                            {
                                // node番号を1始まりとする場合は range adjacent[n-1]とする
                                foreach (var x in adjacent[n])
                                {
                                    if (!walked2.Contains(x))
                                    {
                                        walked2.Add(x);
                                        dfs(goal2, walked2);
                                    }
                                }
                            }
                        }
                    }
                    walked.Add(start);
                    dfs(goal, walked);
                    return ret;

                }


        List<string> getIdData(string fname)
        {
            List<string> lt = freadLines(fname);
            List<string> lines = new List<string>();
            string ss = "";
            foreach (var v in lt)
            {
                if (v[0] == '>')
                {
                    if (ss != "")
                    {
                        lines.Add(ss);
                        ss = "";
                    }
                    lines.Add(v);
                }
                else
                {
                    ss += v;
                }
            }
            lines.Add(ss);
            return lines;
        }


        //int[], double[]を空白区切りの文字列に変換
        string arrayToStr<Type>(Type[] ar)
        {
            string ss = ar[0].ToString();
            for (int i = 1; i < ar.Length; i++)
            {
                ss += " " + ar[i].ToString();
            }
            return ss;
        }

        List<int[]> permutateInt(int n, int r)
        {
            bool[] used = new bool[n + 1];
            int[] buffer = new int[r];
            List<int[]> seq = new List<int[]>();

            void permISub(int m)
            {
                if (m == r)
                {
                    //print_perm();
                    int[] tmp = new int[r];
                    Array.Copy(buffer, tmp, r);
                    seq.Add(tmp);
                    //Console.WriteLine("seq added");
                }
                else
                {
                    for (int i = 1; i <= n; i++)
                    {
                        if (used[i]) continue;
                        buffer[m] = i;
                        used[i] = true;
                        permISub(m + 1);
                        used[i] = false;
                    }
                }
            }

            //void print_perm()
            //{
            //    for (int i = 0; i < r; i++)
            //    {
            //        Console.Write("{0:d}", buffer[i]);
            //    }
            //    Console.WriteLine("");
            //}

            Debug.Assert(n >= r, "should be n >= r");
            permISub(0);
            return seq;

        }

        void fwriteLine(string fname, string line)
        {
            //2012 Dec 7, 空白区切りTextファイルに出力
            //using System.Text;
            //using System.IO;

            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
            StreamWriter writer =
              new StreamWriter(fname, false, sjisEnc);//書き込むファイルが既に存在している場合は、上書きする
            //fname=@"D:\Visual CS\CodingExampleCs\Test01.txt"; として　new StreamWriter(fname, ...)でも可

            writer.WriteLine(line);
            writer.Close();
        }


        void fwriteLines(string fname, string[] lines)
        {
            //2012 Dec 7, 空白区切りTextファイルに出力
            //using System.Text;
            //using System.IO;
            int i;
            int n = lines.Length;

            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
            StreamWriter writer =
              new StreamWriter(fname, false, sjisEnc);//書き込むファイルが既に存在している場合は、上書きする
            //fname=@"D:\Visual CS\CodingExampleCs\Test01.txt"; として　new StreamWriter(fname, ...)でも可

            for (i = 0; i < n; i++)
            {
                writer.WriteLine(lines[i]);
                //writer.WriteLine(a[i, n_Col - 1].ToString());
            }
            writer.Close();
        }

        List<string> freadLines(string fname)
        {
            //テキストファイルの読み込み
            //using System.Text;
            //using System.IO;

            List<string> lines = new List<string>();
            using (StreamReader sr = new StreamReader(fname, Encoding.GetEncoding("Shift_JIS")))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        void writeArray(double[] ar)
        {
            foreach (var v in ar)
            {
                Console.Write(v);
                Console.Write(", ");
            }
            Console.WriteLine("");
        }

        void writeAr<Type>(Type[] ar)
        {
            //foreach (var v in ar) genericではforeachは使えない
            if (ar.Length == 0)
            {
                Console.WriteLine("[ ]");
            }
            else
            {
                Console.Write("[");
                Console.Write(ar[0]);
                for (int i = 1; i < ar.Length; i++)
                {
                    Console.Write(", ");
                    Console.Write(ar[i]);
                }
                Console.Write("] ");
            }
        }


        void writeLineAr<Type>(Type[] ar)
        {
            //foreach (var v in ar) genericではforeachは使えない
            if (ar.Length == 0)
            {
                Console.WriteLine("[ ]");
            }
            else
            {
                Console.Write("[");
                Console.Write(ar[0]);
                for (int i = 1; i < ar.Length; i++)
                {
                    Console.Write(", ");
                    Console.Write(ar[i]);
                }
                Console.WriteLine("]");
            }
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            //test

            //string[] src = { "a", "b", "c", "d" };
            //string[] dst = src.Skip(1).Take(2).ToArray();
            //src[1] = "xx";
            //writeLineAr(dst);
            var s = "abcde";
            var x = s.Substring(1, 2);
            Console.WriteLine(x);
            Console.WriteLine("End");
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //kmer
            DateTime start = DateTime.Now;
            string fname = "D:/VCS2013/Rosalind/data/rosalind_kmer.txt";
            string output = "D:/VCS2013/Rosalind/data/output.txt";
            List<string> lines = getIdData(fname);
            Console.WriteLine(lines[1]);

            var ss = new string[] { "A", "C", "G", "T" };
            var lt = permutateDup(ss, 4);
            //writeList(lt);

            var kmer = new Dictionary<string, int>();

            for (int i = 0; i < lt.Count; i++)
            {
                string s = String.Join("", lt[i]);
                kmer[s] = 0;
            }

            for (int i = 0; i < lines[1].Length - 3; i++)
            {
                string s = lines[1].Substring(i, 4);
                kmer[s] += 1;
            }
            string ans = "";
            
            for (int i=0; i < lt.Count; i++)
            {
                string s = String.Join("", lt[i]);
                ans += kmer[s].ToString() + " ";
            }
            Console.WriteLine(ans);
            fwriteLine(output, ans);

            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("time={0}", span);
            Console.WriteLine("End");
        }
    }
}
