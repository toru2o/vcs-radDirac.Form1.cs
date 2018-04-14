using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Rosalind
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        (int Count, double Sum, double SumOfSquares) testTuple(int[] lt)
        {
            var computation = (count: 0, sum: 0.0, sumOfSquares: 0.0);

            foreach (var item in lt)
            {
                computation.count++;
                computation.sum += item;
                computation.sumOfSquares += item * item;
            }
            return computation;
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


        List<string> getIdData(string fname) {
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
                 } else {
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

            void print_perm()
            {
                for (int i = 0; i < r; i++)
                {
                    Console.Write("{0:d}", buffer[i]);
                }
                Console.WriteLine("");
            }

            Debug.Assert(n >= r, "should be n >= r");
            permISub(0);
            return seq;

        }

        void fwriteLine(string fname, string line)
        {
            //2012 Dec 7, 空白区切りTextファイルに出力
            //using System.Text;
            //using System.IO;
            int i;

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
                Console.WriteLine("]");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            Tool tl = new Tool();
            string fname = @"D:\VCS2013\Rosalind\data\rosalind_pper.txt";
            string output = @"D:\VCS2013\Rosalind\data\output.txt";

            //テキストファイルの読み込み
            //using System.Text;
            //using System.IO;

            //string line = "";
            List<string> lines = new List<string>();
            using (StreamReader sr = new StreamReader(fname, Encoding.GetEncoding("Shift_JIS")))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            foreach (var v in lines)
            {
                Console.WriteLine(v);
            }
            string[] data = lines[0].Split(' ');
            foreach (var x in data)
            {
                Console.WriteLine(x);
            }
            long n = long.Parse(data[0]);
            long r = long.Parse(data[1]);
            Console.WriteLine("{0}, {1}", n, r);

            long s = 1L;
            long i;
            for (i = 0L; i < r; i++)
            {
                s *= n - i;
                s = s % 1000000L;
            }
            Console.WriteLine(s);
            string[] ans = new string[] { s.ToString() };
            fwriteLines(output, ans);

            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("time={0}", span);
            Console.WriteLine("End");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //prob
            DateTime start = DateTime.Now;
            Tool tl = new Tool();
            string fname = "D:/VCS2013/Rosalind/data/rosalind_prob.txt";
            string output = "D:/VCS2013/Rosalind/data/output.txt";
            List<string> lines = freadLines(fname);
            foreach (var v in lines)
            {
                Console.WriteLine(v);
            }
            string ss = lines[0];
            string[] lt = lines[1].Split(' ');
            double[] pGC = new double[lt.Length];
            for (int i = 0; i < pGC.Length; i++) 
            {
                pGC[i] = double.Parse(lt[i]);
            }
            //writeArray(pGC);
            writeAr(pGC);

            string[] ans = new string[pGC.Length];
            for (int j = 0; j < pGC.Length; j++)
            {
                double p1 = Math.Log10(pGC[j] / 2.0);
                double p2 = Math.Log10((1.0 - pGC[j]) / 2.0);
                double p = 0.0;
                for (int i = 0; i < ss.Length; i++)
                {
                    if (ss.Substring(i,1) == "G" || ss.Substring(i,1) == "C")
                    {
                        p += p1;
                    }
                    else
                    {
                        p += p2;
                    }
                }
                ans[j] = p.ToString();
            }
            writeAr(ans);
            string ans2 = String.Join(" ", ans);
            string[] ans3 = new string[] { ans2 };
            fwriteLines(output, ans3);

            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("time={0}", span);
            Console.WriteLine("End");

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // test
            string s = "abcd";
            Console.WriteLine(s[0]);
            if (s[0] == 'a')
            {
                Console.WriteLine("char");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //sign
            string fname = "D:/VCS2013/Rosalind/data/rosalind_sign.txt";
            string output = "D:/VCS2013/Rosalind/data/output.txt";
            List<string> lines = freadLines(fname);
            foreach (var v in lines)
            {
                Console.WriteLine(v);
            }
            int n = int.Parse(lines[0]);
            List<int[]> seq = permutateInt(n, n);
            //writeAr(seq[0]);
            for (int i = 0; i < n; i++)
            {
                int m = seq.Count;
                for (int j = 0; j < m; j++)
                {
                    int[] ar = new int[n];
                    seq[j].CopyTo(ar, 0);
                    ar[i] = -ar[i];
                    seq.Add(ar);
                }
            }

            string[] ans = new string[seq.Count];
            for (int i=0; i < seq.Count; i++)
            {
                ans[i] = arrayToStr(seq[i]);
            }
            foreach (var v in ans)
            {
                Console.WriteLine(v);
            }
            fwriteLines(output, ans);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //sseq
            string fname = "D:/VCS2013/Rosalind/data/rosalind_sseq.txt";
            string output = "D:/VCS2013/Rosalind/data/output.txt";
            List<string> lines = getIdData(fname);
            foreach (var v in lines)
            {
                Console.WriteLine(v);
            }
            string ss = lines[1];
            string word = lines[3];
            List<string> lt = new List<string>();
            int j = 0;
            for (int i = 0; i < ss.Length; i++)
            {
                if (j == word.Length)
                {
                    break;
                }
                if (ss[i] == word[j])
                {
                    lt.Add((i + 1).ToString());
                    j++;
                }
            }
            foreach (var v in lt)
            {
                Console.Write(v + " ");
            }
            Console.WriteLine("");
            //string[] ans = lt.ToArray();
            //fwriteLines(output, ans);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            //tran
            string fname = "D:/VCS2013/Rosalind/data/rosalind_tran.txt";
            string output = "D:/VCS2013/Rosalind/data/output.txt";
            List<string> lines = getIdData(fname);
            //foreach (var v in lines)
            //{
            //    Console.WriteLine(v);
            //}
            string s1 = lines[1];
            string s2 = lines[3];
            Dictionary<char, string> dict = new Dictionary<char, string>() {
                { 'A', "purine"},
            	{ 'G', "purine"},
            	{ 'C', "pyrimidine"},
            	{ 'T', "pyrimidine"},
            };
            foreach (var p in dict) 
            {
                Console.WriteLine("{0}, {1}", p.Key.ToString(), p.Value);
            };
            int n1 = 0; //transion
            int n2 = 0; //transversion

            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i] == s2[i])
                {
                    continue;
                }
                if (dict[s1[i]] == dict[s2[i]])
                {
                    n1++;
                }
                else
                {
                    n2++;
                }
            }
            double ans = (double)n1 / (double)n2;
            Console.WriteLine(ans);
            string[] ans2 = new string[] { ans.ToString() };
            fwriteLines(output, ans2);

        }

        private void button7_Click(object sender, EventArgs e)
        {
            //tree
            DateTime start = DateTime.Now;
            string fname = "D:/VCS2013/Rosalind/data/rosalind_tree1.txt";
            string output = "D:/VCS2013/Rosalind/data/output.txt";
            List<string> lines = freadLines(fname);
            foreach (var v in lines)
            {
                Console.WriteLine(v);
            }
            int n = int.Parse(lines[0]);

            List<int>[] adj = new List<int>[n];
            for (int i = 0; i < n; i++)
            {
                adj[i] = new List<int>();
            }
            for (int i = 1; i < lines.Count; i++)
            {
                string[] ss = lines[i].Split(' ');
                int n1 = int.Parse(ss[0]) - 1;
                int n2 = int.Parse(ss[1]) - 1;
                adj[n1].Add(n2);
                adj[n2].Add(n1);
            }

            bool[] connected = new bool[n];
            int p = 0;
            connected[0] = true;
            for (int j = 1; j < n; j++)
            {
                if (!isConnected(adj, 0, j))
                {
                    adj[0].Add(j);
                    adj[j].Add(0);
                    p++;
                }
                connected[j] = true;
            }
            Console.WriteLine("ans= {0}", p);
            fwriteLine(output, p.ToString());
            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("time={0}", span);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // cat int -> longでOK
            string fname = "D:/VCS2013/Rosalind/data/rosalind_cat.txt";
            string output = "D:/VCS2013/Rosalind/data/output.txt";
            List<string> lines = getIdData(fname);
            string ss = lines[1];
            Console.WriteLine(ss);

            (long ctA, long ctU, long ctC, long ctG) count(string S)
            {
                var ct = (ctA: 0L, ctU: 0L, ctC: 0L, ctG: 0L);
                for (int i = 0; i < S.Length; i++)
                {
                    switch (S[i]) 
                    {
                        case 'A':
                            ct.ctA++;
                            break;
                        case 'U':
                            ct.ctU++;
                            break;
                        case 'C':
                            ct.ctC++;
                            break;
                        case 'G':
                            ct.ctG++;
                            break;
                    }
                }
                return ct;
            }

            bool is_complement(char a, char b)
            {
                bool bl = false;

                switch (a)
                {
                    case 'A':
                        if (b == 'U')
                        {
                            bl = true;
                        }
                        break;
                    case 'U':
                        if (b == 'A')
                        {
                            bl = true;
                        }
                        break;
                    case 'C':
                        if (b == 'G')
                        {
                            bl = true;
                        }
                        break;
                    case 'G':
                        if (b == 'C')
                        {
                            bl = true;
                        }
                        break;
                }
                return bl;
            }

            Dictionary<string, long> memo = new Dictionary<string, long>(); //{} dictionary to memoize solutions in
            memo[""] = 1L;                 //empty string is already a perfect matching

            long find_noncrossmatch(string S)
            {
                if (memo.ContainsKey(S))
                {
                    return memo[S]; // returns memoized solution if one exists
                }
                (long ctA, long ctU, long ctC, long ctG) ct;
                ct = count(S);

                if (ct.ctA != ct.ctU || ct.ctC != ct.ctG)
                {
                    memo[S] = 0L;
                    return 0; // no perfect matching exists if the number of complementary bases is not equal
                }

                long result = 0L;
                for (int i = 1; i < S.Length; i++)
                {
                    if (is_complement(S[0], S[i]))
                    { // look for all posible edges from the first base
                      // iteratively compute how many ways can we make a matching with one edge fixed
                        result += find_noncrossmatch(S.Substring(1, i - 1)) * find_noncrossmatch(S.Substring(i + 1));
                    }
                }
                result = result % 1000000L;
                memo[S] = result; // memoize solution
                return result;
            }

            long ans = find_noncrossmatch(ss);
            Console.WriteLine(ans);
            //fmt.Println(ans)

            //fwriteLine(output, fmt.Sprint(ans))

        }

        private void button9_Click(object sender, EventArgs e)
        {
            // corr
            string fname = "D:/VCS2013/Rosalind/data/rosalind_corr.txt";
            string output = "D:/VCS2013/Rosalind/data/output.txt";
            List<string> lines = getIdData(fname);
            Console.WriteLine(lines);
            int n = lines.Count / 2;
            var compl = new Dictionary<char, string>
            {
                { 'A', "T"},
                { 'T', "A" },
                { 'C', "G" },
                { 'G', "C" }
            };

            var read1 = new string[n];
            var read2 = new string[n];
            int m = lines[1].Length;

            for (int i = 0; i < n; i++)
            {
                read1[i] = lines[2 * i + 1];
                for (int j = 0; j < m; j++)
                {
                    read2[i] += compl[read1[i][m - 1 - j]];
                }
                Console.WriteLine(read2[i]);
            }
            var correct = new int[n];
	        for (int i = 0; i < n; i++)
            {
                if (correct[i] >= 1) continue;
                for (int j = i + 1; j < n; j++)
                {
                    if (correct[j] >= 1) continue;
                    if (read1[i] == read1[j] || read1[i] == read2[j])
                    {
                        correct[i] += 1;
                        correct[j] += 1;
                    }
                }
            }
            writeAr(correct);

            var ans = new List<string>();
            for (int i = 0; i < n; i++)
            {
                if (correct[i] > 0) continue;
                for (int j = 0; j < n; j++)
                {
                    if (j == i || correct[j] == 0) continue;
                    int ct = 0;
                    for (int k = 0; k < m; k++)
                    {
                        if (ct > 1) break;
                        if (read1[i][k] != read1[j][k]) ct++;
                    }
                    if (ct == 1)
                    {
                        ans.Add(read1[i] + "->" + read1[j]);
                        break;
                    }

                    ct = 0;
                    for (int k = 0; k < m; k++)
                    {
                        if (ct > 1) break;
                        if (read1[i][k] != read2[j][k]) ct++;
                    }
                    if (ct == 1)
                    {
                        ans.Add(read1[i] + "->" + read2[j]);
                        break;
                    }
                }
            }
            foreach (var v in ans) Console.WriteLine(v);

            //fwriteLines(output, ans)

        }

        private void button10_Click(object sender, EventArgs e)
        {
            //inod
            string fname = "D:/VCS2013/Rosalind/data/rosalind_inod.txt";
            string output = "D:/VCS2013/Rosalind/data/output.txt";
            List<string> lines = freadLines(fname);
            Console.WriteLine(lines[0]);

            int N = int.Parse(lines[0]);
            //2枚のleafを1個のinternal nodeで繋ぐ
            // n := N / 2 //n個のinternal node(枝1本)を追加。追加したnode同士を繋いでよいが最終的には連結しなければならない
            // r := N % 2 //残ったleaf数(0 or 1)
            //結局leaf相当枚数 n+r を繋いでゆく作業(最終的にはこれらleaf相当は連結されなければならない
            // n+r=2 --> 2つの枝を繋いで終わり(追加nodeなし)
            // n+r=3 --> 2つの枝を繋ぐと(nodeを1個追加) --> n+r=2
            // n+r=4 --> 枝を2つずつ繋いで(nodeを2個追加) --> n+r=2
            int node = N / 2;
            Console.WriteLine(node);
            int n = N / 2 + (N % 2);
            while (n > 2)
            {
                node += n / 2;
                n = n / 2 + (n % 2);
                Console.WriteLine(n);
            }
            if (n == 2)
            {
                Console.WriteLine(node);
                //fwriteLine(output, fmt.Sprint(node))
            }
            else
            {
                Console.WriteLine("no answer");
                Console.WriteLine("{0}, {1}", n, node);
            }

        }
    }
}
