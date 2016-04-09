using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Diagnostics;

namespace DigitsPower
{
    public delegate BigInteger Inverse(BigInteger mod, BigInteger found);
    public delegate BigInteger Multiply(BigInteger a, BigInteger b);

    public class MyList<T> : List<T>
    {
        //public MyList() : base() { }
        public T this[BigInteger index]    // Indexer declaration
        {
            get { return base[(int)index]; }
            set { base[(int)index] = value; }
        }
    }
    abstract class PowFunctions
    {
#region binary
        
        public static BigInteger BinaryRL(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res = 1;
            BigInteger t = found;

            string Binary = ConvToBinary(pow);
            for (int i = Binary.Length - 1; i >= 0; i--)
            {
                if (Binary[i] == '1')
                    res = (t * res) % mod;//Приведення до степеня після кожного кроку
                t = (t * t) % mod;
            }
            return res;
        }
        public static BigInteger BinaryLR(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res = 1;
            BigInteger t = found;
            string Binary = ConvToBinary(pow);
            for (int i = 0; i < Binary.Length; i++)
            {
                res = (res * res) % mod;
                if (Binary[i] == '1')
                    res = (t * res) % mod;//Приведення до степеня після кожного кроку
            }
            return res;
        }
        
        public static BigInteger Euclid_2_1(BigInteger mod, BigInteger found)
        {
            BigInteger u, v, A, B, C, D, y, t1, t2, t3, q, d, inv;

            u = mod;
            v = found;

            A = 1;
            B = 0;
            C = 0;
            D = 1;

            while (v != 0)
            {
                q = u / v;
                t1 = u - q * v;
                t2 = A - q * C;
                t3 = B - q * D;

                u = v;
                A = C;
                B = D;

                v = t1;
                C = t2;
                D = t3;
            }
            d = u; y = B;

            if (y >= 0) inv = y;
            else inv = y + mod;
            return inv;
        }
        public static BigInteger NAFBinaryRL(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res = 1;
            BigInteger c = found;
            MyList<BigInteger> x = ToNAF(pow);
            for (int i = x.Count - 1; i > -1; i--)
            {
                if (x[i] == 1) res = (res * c) % mod;
                else if (x[i] == -1) res = (res * Euclid_2_1(mod,c)) % mod;
                c = (c * c) % mod;
            }
            return res;
        }
        public static BigInteger NAFBinaryLR(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res = 1;
            BigInteger c = found;
            MyList<BigInteger> x = ToNAF(pow);
            for (int i = 0; i < x.Count; i++)
            {
                res = (res * res) % mod;
                if (x[i] == 1) res = (res * c) % mod;
                else if (x[i] == -1) res = (res * Euclid_2_1(mod, c)) % mod;
            }
            return res;
        }

        public static BigInteger AddSubRL(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res = 1;
            BigInteger c = found;

            string BinaryT = ConvToBinary(3 * pow);
            string BinaryN = ConvToBinary(pow);
            while (BinaryN.Length < BinaryT.Length) BinaryN = "0" + BinaryN;

            for (int i = BinaryN.Length - 2; i >= 0; i--)
            {
                if (BinaryT[i] != '0' && BinaryN[i] == '0') res = (res * c) % mod;
                else if (BinaryT[i] == '0' && BinaryN[i] != '0') res = (res * Euclid_2_1(mod, c)) % mod;
                c = (c * c) % mod;
            }

            return res;
        }
        public static BigInteger AddSubLR(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res = 1;
            BigInteger c = found;

            string BinaryT = ConvToBinary(3 * pow);
            string BinaryN = ConvToBinary(pow);
            while (BinaryN.Length < BinaryT.Length) BinaryN = "0" + BinaryN;

            for (int i = 0; i < BinaryN.Length - 1; i++)
            {
                res = (res * res) % mod;
                if (BinaryT[i] != '0' && BinaryN[i] == '0') res = (res * c) % mod;
                else if (BinaryT[i] == '0' && BinaryN[i] != '0') res = (res * Euclid_2_1(mod, c)) % mod;
            }

            return res;
        }
        public static BigInteger Joye_double_and_add(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res = 1;
            BigInteger t = found;

            string Binary = bif.ToBin(pow);
            for (int i = Binary.Length - 1; i >= 0; i--)
            {
                if (Binary[i] == '1')
                {
                    res = res * res % mod;
                    res = res * t % mod;
                }
                else
                {
                    t = t * t % mod;
                    t = res * t % mod;
                }
            }
            return res;
        }
        public static BigInteger MontgomeryLadder(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res = 1;
            BigInteger t = found;

            string Binary = bif.ToBin(pow);
            for (int i = 0; i < Binary.Length; i++)
            {
                if (Binary[i] == '0')
                {
                    t = t * res % mod;
                    res = res * res % mod;
                }
                else
                {
                    res = res * t % mod;
                    t = t * t % mod;
                }
            }
            return res;
        }

        public static BigInteger DBNS1RL(BigInteger found, BigInteger pow, BigInteger mod, bool convert_method)
        {
            return methods.Point_Multiplication_Affine_Coord_19(found, pow, mod, convert_method);
        }
        public static BigInteger DBNS1LR(BigInteger found, BigInteger pow, BigInteger mod, bool convert_method)
        {
            return methods.Point_Multiplication_Affine_Coord_20(found, pow, mod, convert_method);
        }

        public static BigInteger DBNS2RL(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv)
        {
            List<int[]> mas_k = ToDBNS2RL(pow);

            BigInteger res = 1;
            BigInteger t = found;

            //for (int i = 0; i < mas_k.Count; i++)
            //{
            //    t = t * TwoPow(mas_k[i][1]) * (BigInteger)Math.Pow(3,mas_k[i][2]);
            //    res += mas_k[i][0] * t;
            //}

            //if (pow != res)
            //{
            //    throw new InvalidProgramException();
            //}



            for (int i = 0; i < mas_k.Count; i++)
            {
                for (int j = 0; j < mas_k[i][1]; j++)
                    t = (t * t) % mod;
                for (int j = 0; j < mas_k[i][2]; j++)
                    t = ((t * t) % mod * t) % mod;

                if (mas_k[i][0] == 1)
                    res = res * t % mod;
                else if (mas_k[i][0] == -1)
                    res = res * inv(t, mod) % mod;
            }
            return res;
        }
        public static BigInteger DBNS2LR(BigInteger found, BigInteger pow, BigInteger mod,  Inverse inv)
        {
            List<int[]> mas_k = ToDBNS2LR(pow);
            BigInteger res = found;

            for (int i = mas_k.Count - 1; i > 0; i--)
            {
                for (int j = 0; j < mas_k[i][1]; j++)
                    res = res * res % mod;
                for (int j = 0; j < mas_k[i][2]; j++)
                    res = ((res * res) % mod * res) % mod;

                if (mas_k[i][0] == 1)
                    res = (res * found) % mod;
                else if (mas_k[i][0] == -1)
                    res = res * inv(found, mod) % mod;
            }
            for (int j = 0; j < mas_k[0][1]; j++)
                res = (res * res) % mod;
            for (int j = 0; j < mas_k[0][2]; j++)
                res = ((res * res) % mod * res) % mod;
            return res;
        }

        //helps methods
        public static BigInteger TwoPow(BigInteger pow)
        {
            BigInteger result = 1;
            for (BigInteger i = 0; i < pow; i++)
            {
                result = result << 1;
            }
            return result;
        }
        private static string ConvToBinary(BigInteger num)
        {
            string x = "";
            while (num != 0)
            {
                x += num % 2;
                num /= 2;
            }
            char[] charArray = x.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        private static MyList<BigInteger> ToNAF(BigInteger pow)
        {

            MyList<BigInteger> res = new MyList<BigInteger>();
            BigInteger k = pow;
            while (k >= 1)
            {
                if (k % 2 != 0)
                {
                    res.Insert(0, 2 - k % 4);
                    k = k - res[0];
                }
                else
                    res.Insert(0, 0);
                k = k / 2;
            }
            return res;
        }
        private static MyList<int[]> ToDBNS2RL(BigInteger pow)
        {
            var x = new MyList<int[]>();
            BigInteger v = pow;
            int i = 0;
            while (v != 0)
            {
                x.Insert(i, new int[3]);
                int a = 0;
                while (v % 2 == 0 && v != 0)
                {
                    a++;
                    v /= 2;
                }
                int b = 0;
                while (v % 3 == 0 && v != 0)
                {
                    b++;
                    v /= 3;
                }
                x[i][1] = a;
                x[i][2] = b;

                v--;
                if (v != 0)
                {
                    if (v % 6 != 0)
                    {
                        v += 2;
                        x[i][0] = -1;
                    }
                    else
                        x[i][0] = 1;
                }
                else
                    x[i][0] = 1;
                i++;
            }
            return x;
        }
        private static List<int[]> ToDBNS2LR(BigInteger pow)
        {
            List<int[]> x = new List<int[]>();
            x.Add(new int[3]);
            int i = 0;
            BigInteger v = pow;
            x[i][0] = 0;
            while (v != 0)
            {
                int a = 0;
                while (v % 2 == 0 && v != 0)
                {
                    a++;
                    v = v / 2;
                }

                int b = 0;
                while (v % 3 == 0 && v != 0)
                {
                    b++;
                    v = v / 3;
                }
                x[i][1] = a;
                x[i][2] = b;
                i++;
                x.Add(new int[3]);
                v--;
                if (v != 0)
                {
                    if (v % 6 != 0)
                    {
                        v = v + 2;
                        x[i][0] = -1;
                    }
                    else
                        x[i][0] = 1;
                }
            }
            return x;
        }
    

    #endregion
#region window
        private static MyList<BigInteger> Table(BigInteger found, BigInteger pow, int w, BigInteger mod) //Из-за преобразований может работать медленней, 
        {
            var table = new MyList<BigInteger>();

            table.Add(found);
            for (BigInteger i = 0; i < BigInteger.Parse((Math.Pow(2, w) - 2).ToString()); i++)
                table.Add((table[i] * found) % mod);//Приведення до степеня після кожного кроку
            return table;
        }
        public static BigInteger WindowRL(BigInteger found, BigInteger pow,  BigInteger mod, int w, out double table_time)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            MyList<BigInteger> table = Table(found, pow, w, mod);
            stw.Stop();

            table_time = (double)(stw.ElapsedTicks / 10);
            
            BigInteger res = 1;

            List<string> bins = windows(ConvToBinary(pow), w);

            for (int i = bins.Count - 1; i > -1; i--)
            {
                int c = Convert.ToInt32(bins[i], 2);
                if (c != 0) res = (res * table[c - 1]) % mod;//Приведення до степеня після кожного кроку

                for (int k = 0; k < w; k++)
                    for (int j = 0; j < table.Count; j++)
                        table[j] = (table[j] * table[j]) % mod;
            }
            return res;
        }
        public static BigInteger WindowLR(BigInteger found, BigInteger pow, BigInteger mod, int w, out double table_time)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            MyList<BigInteger> table = Table(found, pow, w, mod);
            stw.Stop();

            table_time = (double)(stw.ElapsedTicks / 10);
            BigInteger res = 1;

            List<string> bins = windows(ConvToBinary(pow), w);

            for (int i = 0; i < bins.Count; i++)
            {
                for (int k = 0; k < w; k++)
                    res = (res * res) % mod;

                int c = Convert.ToInt32(bins[i], 2);
                if (c != 0) res = (res * table[c - 1]) % mod;//Приведення до степеня після кожного кроку
            }
            return res;
        }

        public static BigInteger SlideRL(BigInteger found, BigInteger pow, BigInteger mod, int w, out double table_time)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            BigInteger power = (BigInteger)Math.Pow(2, w - 1);
            var table = SlideRLTable(found, mod, power, w);
            
            stw.Stop();
            table_time = (stw.ElapsedTicks / 10);

            BigInteger res = 1;
            BigInteger temp = found;

            string binary = bif.ToBin(pow);

            while (binary.Length > 0)
            {
                int index = binary.Length - 1;
                if (binary.Length < w || binary[index - w + 1] == '0')
                {
                    if (binary[index] == '1')
                        res = (res * temp) % mod;

                    for (int j = 0; j < table.Count; j++)
                        table[j] = (table[j] * table[j]) % mod;

                    temp = (temp * temp) % mod;

                    binary = binary.Remove(index, 1);
                }
                else
                {
                    int c = Convert.ToInt32(binary.Substring(index - w + 1, w), 2);
                    res = (res * table[c - power]) % mod;

                    temp = (temp * table[table.Count - 1]) % mod;

                    for (int k = 0; k < w; k++)
                        for (int j = 0; j < table.Count; j++)
                            table[j] = (table[j] * table[j]) % mod;

                    binary = binary.Remove(index - w + 1, w);
                }
            }
            return res;
        }
        public static BigInteger SlideLR(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            BigInteger pow = 2 * ((BigInteger)Math.Pow(2, w) - (BigInteger)Math.Pow((-1), w)) / 3 - 1;
            var table = NAFLRTable(found, mod, pow, w);
            stw.Stop();

            table_time = (stw.ElapsedTicks / 10);
            MyList<BigInteger> x = power.ToNAF();
            
            BigInteger res = 1;
            for (BigInteger i = x.Count - 1; i > -1;)
            {
                var max = new MyList<BigInteger>();
                if (x[i] == 0)
                {
                    max.Add(0);
                    max.Add(1);
                }
                else
                    max = FindLargest2(x, i, w);

                for (int d = 0; d < max[1]; d++)
                    res = (res * res) % mod;

                if (max[0] > 0)
                    res = (res * table[(int)(bif.Abs(max[0]) / 2)]) % mod;
                else if (max[0] < 0)
                    res = (res * mod.Euclid_2_1(table[(bif.Abs(max[0]) / 2)])) % mod;

                i = i - (int)max[1];
            }
            return res;
        }
        
        public static BigInteger NAFSlideRL(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            BigInteger res = 1;
            MyList<BigInteger> x = power.ToNAF();

            Stopwatch stw = new Stopwatch();
            stw.Start();

            BigInteger pow = 2 * ((int)Math.Pow(2, w) - (int)Math.Pow((-1), w)) / 3 - 1;
            MyList<BigInteger> table = NAFRLTable(found, mod, pow, w);

            stw.Stop();
            table_time = (double)(stw.ElapsedTicks / 10);

            for (int i = 0; i < x.Count;)
            {
                List<BigInteger> max = FindLargest1(x, i, w);

                if (max[0] > 0)
                    res = res * table[(bif.Abs(max[0]) / 2)] % mod;
                else if (max[0] < 0)
                    res = res * mod.Euclid_2_1(table[(bif.Abs(max[0]) / 2)]) % mod;

                for (int d = 0; d < max[1]; d++)
                    for (int j = 0; j < table.Count; j++)
                        table[j] = table[j] * table[j] % mod;

                i = i + (int)max[1];
            }
            return res;

            //BigInteger res = 1;
            //MyList<BigInteger> x = power.ToNAF();
            //Stopwatch stw = new Stopwatch();
            //stw.Start();

            //MyList<BigInteger> table = new MyList<BigInteger>();
            //BigInteger pow = 2 * ((BigInteger)Math.Pow(2, w) - (BigInteger)Math.Pow((-1), w)) / 3 - 1;
            //for (BigInteger i = 1; i <= pow; i += 2)
            //    table.Add(BinaryRL(found, i, mod));

            //stw.Stop();

            //table_time = (double)(stw.ElapsedTicks / 10);
            //for (BigInteger i = x.Count - 1; i > -1; )
            //{
            //    MyList<BigInteger> max = new MyList<BigInteger>();
            //    if (x[i] == 0)
            //    {
            //        max.Add(0);
            //        max.Add(1);
            //    }
            //    else
            //        max = FindLargest1(x, i, w);

            //    if (max[0] > 0)
            //        res = (res * table[(BigInteger)(Math.Abs((decimal)max[0]) / 2)]) % mod;
            //    else if (max[0] < 0)
            //        res = (res * Euclid_2_1(mod, table[(BigInteger)(Math.Abs((decimal)max[0]) / 2)])) % mod;

            //    for (BigInteger d = 0; d < max[1]; d++)
            //    {
            //        for (BigInteger j = 0; j < table.Count; j++)
            //        {
            //            table[j] = ((table[j] * table[j]) % mod);
            //        }
            //    }

            //    i = i - max[1];
            //}
            //return res;
        }
        public static BigInteger NAFSlideLR(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            BigInteger res = 1;
            MyList<BigInteger> x = power.ToNAF();

            Stopwatch stw = new Stopwatch();
            stw.Start();

            BigInteger pow = 2 * ((int)Math.Pow(2, w) - (int)Math.Pow((-1), w)) / 3 - 1;
            MyList<BigInteger> table = NAFLRTable(found, mod, pow, w);

            stw.Stop();
            table_time = (double)(stw.ElapsedTicks / 10);

            for (int i = x.Count - 1; i > -1;)
            {
                List<BigInteger> max = new List<BigInteger>();
                if (x[i] == 0)
                {
                    max.Add(0);
                    max.Add(1);
                }
                else
                    max = FindLargest2(x, i, w);

                for (int d = 0; d < max[1]; d++)
                    res = res * res % mod;

                if (max[0] > 0)
                    res = res * table[(bif.Abs(max[0]) / 2)] % mod;
                else if (max[0] < 0)
                    res = res * mod.Euclid_2_1(table[(bif.Abs(max[0]) / 2)]) % mod;

                i = i - (int)max[1];
            }
            return res;

            //BigInteger res = 1;
            //MyList<BigInteger> x = ToNAF(power);
            //Stopwatch stw = new Stopwatch();
            //stw.Start();
            //MyList<BigInteger> table = new MyList<BigInteger>();
            //BigInteger pow = 2 * ((BigInteger)Math.Pow(2, w) - (BigInteger)Math.Pow((-1), w)) / 3 - 1;
            //for (BigInteger i = 1; i <= pow; i += 2)
            //    table.Add(BinaryLR(found, i, mod));

            //stw.Stop();

            //table_time = (double)(stw.ElapsedTicks / 10);
            //for (BigInteger i = 0; i < x.Count; )
            //{
            //    MyList<BigInteger> max = new MyList<BigInteger>();
            //    if (x[i] == 0)
            //    {
            //        max.Add(0);
            //        max.Add(1);
            //    }
            //    else
            //        max = FindLargest2(x, i, w);

            //    for (BigInteger d = 0; d < max[1]; d++)
            //        res = (res * res) % mod;

            //    if (max[0] > 0)
            //        res = res * table[(BigInteger)(Math.Abs((decimal)max[0]) / 2)];
            //    else if (max[0] < 0)
            //        res = res * Euclid_2_1(mod, table[(BigInteger)(Math.Abs((decimal)max[0]) / 2)]);

            //    i = i + max[1];
            //}
            //return res;
        }

        public static BigInteger NAFWindowRL(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            BigInteger res = 1;
            MyList<BigInteger> x = ToNAF(power);
            MyList<BigInteger> table = new MyList<BigInteger>();
            BigInteger pow = (BigInteger)Math.Pow(2, w - 1);

            Stopwatch stw = new Stopwatch();
            stw.Start();
            for (BigInteger i = 1; i < pow; i += 2)
                table.Add(BinaryRL(found, i, mod));
            stw.Stop();

            table_time = (double)(stw.ElapsedTicks / 10);
            for (int i = x.Count - 1; i > -1; i--)
            {
                if (x[i] > 0)
                    res = (res * table[(int)(x[i] / 2)]) % mod;
                else if (x[i] < 0)
                    res = (res * Euclid_2_1(mod, table[(BigInteger)(-x[i] / 2)])) % mod;

                for (int j = 0; j < table.Count; j++)
                    table[j] = (table[j] * table[j]) % mod;//мод?
            }
            return res;
        }
        public static BigInteger NAFWindowLR(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            BigInteger res = 1;
            var x = ToNAF(power);
            var table = new MyList<BigInteger>();
            BigInteger pow = (BigInteger)Math.Pow(2, w - 1);

            Stopwatch stw = new Stopwatch();
            stw.Start();
            for (BigInteger i = 1; i < pow; i += 2)
                table.Add(BinaryLR(found, i, mod));
            stw.Stop();

            table_time = (double)(stw.ElapsedTicks / 10);
            for (BigInteger i = 0; i < x.Count; i++)
            {
                res = (res * res) % mod;
                if (x[i] > 0)
                    res = (res * table[(BigInteger)(x[i] / 2)]) % mod;
                else if (x[i] < 0)
                    res = (res * Euclid_2_1(mod, table[(BigInteger)(-x[i] / 2)])) % mod;
            }
            return res;
        }
        
        public static BigInteger wNAFSlideRL(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            BigInteger res = 1;
            var x = ToWNAF(power, w);

            var table = new MyList<BigInteger>();
            BigInteger pow = 2 * ((BigInteger)Math.Pow(2, w) - (BigInteger)Math.Pow((-1), w)) / 3 - 1;
            Stopwatch stw = new Stopwatch();
            stw.Start();

            for (BigInteger i = 1; i <= pow; i += 2)
                table.Add(BinaryRL(found, i, mod));

            stw.Stop();

            table_time = (double)(stw.ElapsedTicks / 10);

            for (BigInteger i = x.Count - 1; i > -1; )
            {
                var max = FindLargest1(x, i, w);

                if (max[0] > 0)
                    res = (res * table[(BigInteger)(Math.Abs((decimal)x[i]) / 2)]) % mod;
                else if (max[0] < 0)
                    res = (res * Euclid_2_1(mod, table[(BigInteger)(Math.Abs((decimal)x[i]) / 2)])) % mod;

                for (int d = 0; d < max[1]; d++)
                    for (int j = 0; j < table.Count; j++)
                        table[j] = (table[j] * table[j]) % mod;

                i = i - max[1];
            }
            return res;
        }
        public static BigInteger wNAFSlideLR(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            BigInteger res = 1;
            var x = ToWNAF(power,w);

            var table = new MyList<BigInteger>();
            BigInteger pow = 2 * ((BigInteger)Math.Pow(2, w) - (BigInteger)Math.Pow((-1), w)) / 3 - 1;
            Stopwatch stw = new Stopwatch();
            stw.Start();
            for (BigInteger i = 1; i <= pow; i += 2)
                table.Add(BinaryLR(found, i, mod));
            stw.Stop();

            table_time = (double)(stw.ElapsedTicks / 10);
            for (BigInteger i = 0; i < x.Count; )
            {
                var max = new MyList<BigInteger>();
                if (x[i] == 0)
                {
                    max.Add(0);
                    max.Add(1);
                }
                else
                    max = FindLargest2(x, i, w);

                for (int d = 0; d < max[1]; d++)
                    res = (res * res) % mod;

                if (max[0] > 0)
                    res = (res * table[(BigInteger)(Math.Abs((decimal)max[0]) / 2)]) % mod;
                else if (max[0] < 0)
                    res = (res * Euclid_2_1(mod, table[(BigInteger)(Math.Abs((decimal)max[0]) / 2)])) % mod;

                i = i + max[1];
            }
            return res;
        }

        //help methods
        private static MyList<BigInteger> FindLargest1(MyList<BigInteger> x, BigInteger i, int w)
        {
            BigInteger j = i;
            BigInteger pow = 1;
            BigInteger temp = x[i];

            BigInteger max_j = i;
            BigInteger max = x[i];
            while (j - i + 1 < w && j < x.Count - 1)
            {
                j++;

                pow = pow * 2;
                temp = temp + pow * x[j];

                if (temp % 2 != 0)
                {
                    max_j = j;
                    max = temp;
                }

            }
            max_j = max_j - i + 1;

            MyList<BigInteger> r = new MyList<BigInteger>();
            r.Add(max);
            r.Add(max_j);
            return r;
        }
        private static MyList<BigInteger> FindLargest2(MyList<BigInteger> x, BigInteger i, int w)
        {
            BigInteger j = i;
            BigInteger pow = 1;
            BigInteger temp = x[i];
            BigInteger max_j = i;
            BigInteger max = x[i];
            while (i - j + 1 < w && j > 0)
            {
                pow = 1;
                temp = 0;
                j--;
                for (BigInteger t = j; t <= i; t++)
                {
                    temp = temp + pow * x[t];
                    pow = pow * 2;
                }

                if (temp % 2 != 0)
                {
                    max_j = j;
                    max = temp;
                }
            }
            max_j = i - max_j + 1;

            MyList<BigInteger> r = new MyList<BigInteger>();
            r.Add(max);
            r.Add(max_j);
            return r;
        }

        private static MyList<BigInteger> NAFRLTable(BigInteger found, BigInteger mod, BigInteger power, int w)
        {
            var table = new MyList<BigInteger>();

            for (int i = 1; i <= power; i += 2)
                table.Add(BinaryRL(found, i, mod));
            return table;
        }
        private static MyList<BigInteger> NAFLRTable(BigInteger found, BigInteger mod, BigInteger power, int w)
        {
            var table = new MyList<BigInteger>();

            for (int i = 1; i <= power; i += 2)
                table.Add(BinaryLR(found, i, mod));
            return table;
        }
        private static MyList<BigInteger> SlideRLTable(BigInteger found, BigInteger mod, BigInteger power, int w)
        {
            var table = new MyList<BigInteger>();
            table.Add(BinaryRL(found, power, mod));
            for (BigInteger i = 0; i < power - 1; i++)
                table.Add(table[i] * found % mod);
            return table;
        }
        private static MyList<BigInteger> SlideLRTable(BigInteger found, BigInteger mod, BigInteger power, int w)
        {
            var table = new MyList<BigInteger>();
            table.Add(BinaryLR(found, power, mod));
            for (BigInteger i = 0; i < power - 1; i++)
                table.Add(table[i] * found % mod);
            return table;
        }

        private static List<string> windows(string s, int w)
        {
            string st = s;

            while (st.Length % w != 0)
                st = "0" + st;

            List<string> bins = new List<string>();
            for (int i = 0; i < st.Length / w; i++)
                bins.Add(st.Substring(i * w, w));

            return bins;
        }
        private static MyList<BigInteger> ToWNAF(BigInteger power, int w)
        {
            var res = new MyList<BigInteger>();
            BigInteger k = power;
            BigInteger r;
            while (k >= 1)
            {
                if (k % 2 != 0)
                {
                    r = k % (BigInteger)Math.Pow(2, w);

                    if (r >= (BigInteger)Math.Pow(2, w - 1))
                        res.Insert(0, r - (BigInteger)Math.Pow(2, w));
                    else
                        res.Insert(0, r);

                    k = k - res[0];
                }
                else
                    res.Insert(0, 0);
                k = k / 2;
            }
            return res;
        }
        #endregion
    }
    abstract class GenFunctions
    {
        public static List<int> ReadString(string s)
        {
            string st = "";
            List<string> sts = new List<string>();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                    continue;
                if (s[i] != ';')
                    st = st + s[i];
                else
                {
                    sts.Add(st);
                    st = "";
                }
            }
            if (st != "")
            {
                sts.Add(st);
                st = "";
            }

            List<int> x = new List<int>();
            List<int> pows = new List<int>();
            for (int i = 0; i < sts.Count; i++)
            {
                x.Clear();
                for (int j = 0; j < sts[i].Length; j++)
                {
                    if (sts[i][j] != '-')
                        st = st + sts[i][j];
                    if (sts[i][j] == '-')
                    {
                        x.Add(Convert.ToInt16(st));
                        st = "";
                    }
                }
                if (st != "")
                {
                    x.Add(Convert.ToInt16(st));
                    st = "";
                }

                if (x.Count == 1)
                    pows.Add(x[0]);
                else
                {
                    int step;
                    if (x.Count == 2)
                        step = 1;
                    else
                        step = x[2];
                    for (int j = x[0]; j <= x[1]; j += step)
                        pows.Add(j);
                }
            }
            pows.Sort();
            for (int i = 0; i < pows.Count - 1; i++)
            {
                if (pows[i] == pows[i + 1])
                {
                    pows.RemoveAt(i);
                    i--;
                }
            }
            return pows;
        }
        public static int rand(int l, int h)
        {
            Random r = new Random();
            int x = r.Next(l, h);
            return x;
        }
        public delegate BigInteger random_num(int n, string type);

        public static BigInteger random_max(int n, string type = "Bytes")//n - length in bytes
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[n + 1];
            rng.GetBytes(bytes);
            if (type == "Bytes")
            {
                bytes[n - 1] |= 0x80;//128
                bytes[n] = 0;
                return new BigInteger(bytes);
            }
            else
            {
                string s = "01";
                for (int i = 0; i < n - 1; i++)
                {
                    s += bytes[i] % 2 == 0 ? '1' : '0';
                }

                return new BigInteger(Convert.ToInt32(s, 2));
            }
        }

        public static BigInteger random_max_bites(int n)//n - length in bytes
        {
            string s = "";
            var rand = new Random();
            for (int i = 0; i < n + 1; i++)
            {
                s += rand.Next(2) == 1 ? '1' : '0';
            }
            
            BigInteger b = new BigInteger(Convert.ToInt32(s, 2));

            return b;
        }
 
        public static BigInteger random_two(int n, string type)//n - length in bytes
        {
            if (type == "Bytes")
            {
                var rng = new RNGCryptoServiceProvider();
                byte[] bytes = new byte[n + 1];
                bytes[n] = 1;
                for (int i = 0; i < bytes.Length - 1; i++)
                {
                    bytes[i] = 0;
                }

                return new BigInteger(bytes);
            }
            else
            {
                string s = "";
                s += '1';
                for (int i = 1; i < n + 1; i++)
                {
                    s += '0';
                }
                return new BigInteger(Convert.ToInt32(s, 2));
            }
            
        }
        public static BigInteger random_simple(int n, string type)//n - length in bytes
        {
            BigInteger b;
            again:
            b = random_odd(n, type);
            if (type == "Bytes")
            {
                if (b.IsProbablePrime())
                    return b;
            }
            else
            {
                if (b.IsProbablePrime())
                    //if (Primality_Tests.Ferma(b))
                    return b;
            } 
            goto again;
        }
        public static BigInteger random_odd(int n, string type)//n - length in bytes
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[n + 1];
            rng.GetBytes(bytes);
            if (type == "Bytes")
            {
                bytes[n - 1] |= 0x80;
                bytes[n] = 0;
                bytes[0] |= 1;
                if (bytes[0] % 2 == 0)
                {
                    bytes[0]++;
                }

                return new BigInteger(bytes);
            }
            else
            {
                char[] mas = new char[n + 1];
                mas[n - 1] = '1';
                mas[n] = '0';
                mas[0] = '1';
                for (int i = 1; i < n - 1; i++)
                {
                    mas[i] += bytes[i] % 2 == 0 ? '1' : '0';
                }
                Array.Reverse(mas);
                return new BigInteger(Convert.ToInt32(new string(mas), 2));
            }
            
        }
    }

    public static class BigIntegerExtensions
    {
        //Тест Миллера-Рабина на простоту
        public static bool IsProbablePrime(this BigInteger source, int certainty = 5)
        {
            if (source == 2 || source == 3)
                return true;
            if (source < 2 || source % 2 == 0)
                return false;

            BigInteger d = source - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            // There is no built-in method for generating random BigInteger values.
            // Instead, random BigIntegers are constructed from randomly generated
            // byte arrays of the same length as the source.
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[source.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < certainty; i++)
            {
                do
                {
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= source - 2);

                BigInteger x = BigInteger.ModPow(a, d, source);
                if (x == 1 || x == source - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, source);
                    if (x == 1)
                        return false;
                    if (x == source - 1)
                        break;
                }

                if (x != source - 1)
                    return false;
            }

            return true;
        }

        public static BigInteger Euclid_2_1(this BigInteger mod, BigInteger found)
        {
            BigInteger u, v, A, B, C, D, y, t1, t2, t3, q, d, inv;

            u = mod;
            v = found;

            A = 1;
            B = 0;
            C = 0;
            D = 1;

            while (v != 0)
            {
                q = u / v;
                t1 = u - q * v;
                t2 = A - q * C;
                t3 = B - q * D;

                u = v;
                A = C;
                B = D;

                v = t1;
                C = t2;
                D = t3;
            }
            d = u; y = B;

            if (y >= 0) inv = y;
            else inv = y + mod;
            return inv;
        }
        public static MyList<BigInteger> ToNAF(this BigInteger k)
        {
            MyList<BigInteger> mas_k = new MyList<BigInteger>();
            int i = 0;
            while (k >= 1)
            {
                if (k % 2 != 0)
                {
                    mas_k.Add(2 - (k % 4));
                    k = k - mas_k[i];
                }
                else
                    mas_k.Add(0);


                k = k / 2;
                i++;
            }
            return mas_k;
        }
        public static BigInteger TwoPow(this BigInteger pow)
        {
            BigInteger result = 1;
            for (BigInteger i = 0; i < pow; i++)
            {
                result = result << 1;
            }
            return result;
        }
        private static string ConvToBinary(this BigInteger num)
        {
            string x = "";
            while (num != 0)
            {
                x += num % 2;
                num /= 2;
            }
            char[] charArray = x.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        //Тест Миллера-Рабина на простоту код студента
        public static bool Prime_Test_Miller_Rabin(this BigInteger y)
        {
            BigInteger b = 0, T, a;
            bool flag = true;
            int k = 0, i = 0; //a - osnova
            if ((y & 1) == 0) return false;  // nuzhno ne4etnoe 4islo
            if (y == 3) return true;
            do
            {
                int N = (int)Math.Ceiling(Math.Ceiling(BigInteger.Log(y - 1, 2)) / 8); //kol-vo byte v 4isle (y-1)
                int rnd = GenFunctions.rand(1, N);                //
                a = GenFunctions.random_max(rnd);          //generiruem osnovu <<a>> v diapazone ot 1 do (y-1)
            }
            while ((a >= y - 1) || (a <= 1));


            if (BigInteger.GreatestCommonDivisor(a, y) != 1) return false;
            else
            {
                while (flag == true)
                {
                    k++;
                    b = (y - 1) >> k;                    // y-1 = 2^k * b,
                    if ((b & 1) == 1) flag = false;      // b - ne4etnoe
                }
                T = BigInteger.ModPow(a, b, y);
                if ((T == 1) || (T == y - 1)) return true;

                else
                {
                    for (i = 1; i < k; i++)
                    {
                        T = BigInteger.ModPow(T, 2, y);
                        if (T == y - 1) return true;
                    }
                }
            }

            return false;
        }

        //public static bool Ferma(BigInteger x)
        //{
        //    if (x == 2)
        //        return true;
        //    BigInteger tmp = x - BigInteger.Parse("2");
        //    //Random r = new Random(DateTime.Now.Millisecond);
        //    for (int i = 0; i < 100; i++)
        //    {
        //        BigInteger a = GenFunctions.random_max(x - BigInteger.Parse("2")) + 2;
        //        if (BigInteger.GreatestCommonDivisor(a, x) != 1)
        //            return false;
        //        if (pows(a, x - 1, x) != 1)
        //            return false;
        //    }
        //    return true;
        //}

        //public static BigInteger mul(BigInteger a, BigInteger b, BigInteger m)
        //{
        //    if (b == 1)
        //        return a;
        //    if (b % 2 == 0)
        //    {
        //        BigInteger t = mul(a, b / 2, m);
        //        return (2 * t) % m;
        //    }
        //    return (mul(a, b - 1, m) + a) % m;
        //}

        //public static BigInteger pows(BigInteger a, BigInteger b, BigInteger m)
        //{
        //    if (b == 0)
        //        return 1;
        //    if (b % 2 == 0)
        //    {
        //        BigInteger t = pows(a, b / 2, m);
        //        return mul(t, t, m) % m;
        //    }
        //    return (mul(pows(a, b - 1, m), a, m)) % m;
        //}
    }

}
