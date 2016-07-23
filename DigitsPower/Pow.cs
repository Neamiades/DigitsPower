using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Diagnostics;
using static DigitsPower.HelpMethods;
using static System.Math;
using static DigitsPower.MontgomeryMethods;

namespace DigitsPower
{
    public delegate BigInteger Inverse(BigInteger mod, BigInteger found);
    public delegate BigInteger Multiply(BigInteger a, BigInteger b, BigInteger m);

    public class MyList<T> : List<T>
    {
        public T this[BigInteger index]    // Indexer declaration
        {
            get { return base[(int)index]; }
            set { base[(int)index] = value; }
        }
    }
    static class PowFunctions
    {
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        #region binary

        public static BigInteger mul(BigInteger a, BigInteger b, BigInteger m)
        {
            return ((a * b) % m);
        }

        public static BigInteger BinaryRL(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res, t, inverse;
            int powLen, i;

            res = 1;
            t = found;

            inverse = toMontgomeryDomain(ref t, ref res, mod);
            
            powLen = (int)(Log((double)pow, 2) + 1);
            for (i = 0; i < powLen; ++i)
            {
                if (1 == ((pow >> i) & 1))
                    res = MontgomeryMultDomain(t, res, mod, inverse);
                t = MontgomeryMultDomain(t, t, mod, inverse);
            }

            res = outMontgomeryDomain(res, mod, inverse);

            return res;
        }
        public static BigInteger BinaryLR(BigInteger found, BigInteger pow, BigInteger mod) 
        {
            BigInteger res, t, inverse;
            int powLen;

            res = 1;
            t = found;
            inverse = toMontgomeryDomain(ref t, ref res, mod);
            powLen = (int)(Log((double)pow, 2));
            for (int i = powLen; 0 <= i; i--)
            {
                res = MontgomeryMultDomain(res, res, mod, inverse);
                if (1 == ((pow >> i) & 1))
                    res = MontgomeryMultDomain(t, res, mod, inverse);
            }
            res = outMontgomeryDomain(res, mod, inverse);
            return res;
        }
        
        public static BigInteger NAFBinaryRL(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res, c, inverse;
            MyList<BigInteger> x;

            res = 1;
            c = found;
            x = ToNAF(pow);
            inverse = toMontgomeryDomain(ref c, ref res, mod);
            for (int i = x.Count - 1; i > -1; i--)
            {
                if (x[i] == 1)
                    res = MontgomeryMultDomain(res, c, mod, inverse);
                else if (x[i] == -1)
                    res = MontgomeryMultDomain(res, Euclid_2_1(mod, c), mod, inverse);
                c = MontgomeryMultDomain(c, c, mod, inverse);
            }
            res = outMontgomeryDomain(res, mod, inverse);
            return res;
        }

        public static BigInteger NAFBinaryLR(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res, c, inverse;
            MyList<BigInteger> x;

            res = 1;
            c = found;
            x = ToNAF(pow);
            inverse = toMontgomeryDomain(ref c, ref res, mod);
            for (int i = 0; i < x.Count; i++)
            {
                res = MontgomeryMultDomain(res, res, mod, inverse);
                if (x[i] == 1)
                    res = MontgomeryMultDomain(res, c, mod, inverse);
                else if (x[i] == -1)
                    res = MontgomeryMultDomain(res, Euclid_2_1(mod, c), mod, inverse);
            }
            res = outMontgomeryDomain(res, mod, inverse);
            return res;
        }

        public static BigInteger AddSubRL(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res, c, pow3;
            int powLenT, powLenN;

            res = 1;
            c = found;
            pow3 = pow * 3;
            powLenT = (int)(Log(3 * (double)pow, 2) + 1);
            powLenN = (int)(Log((double)pow, 2) + 1);
            
            for (int i = 1; i < powLenT; i++)
            {
                if (((pow3 >> i) & 1) != 0 && ((pow >> i) & 1) == 0)
                    res = mul(res, c, mod);
                else if (((pow3 >> i) & 1) == 0 && ((pow >> i) & 1) != 0)
                    res = mul(res, Euclid_2_1(mod, c), mod);
                c = mul(c, c, mod);
            }
            return res;
        }

        public static BigInteger AddSubLR(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res, c, pow3;
            int powLenT, powLenN;

            res = 1;
            c = found;
            pow3 = pow * 3;
            powLenT = (int)(Log(3 * (double)pow, 2) + 1);
            powLenN = (int)(Log((double)pow, 2) + 1);

            for (int i = powLenT; 0 < i; i--)
            {
                res = mul(res, res, mod);
                if (((pow3 >> i) & 1) != 0 && ((pow >> i) & 1) == 0)
                    res = mul(res, c, mod);
                else if (((pow3 >> i) & 1) == 0 && ((pow >> i) & 1) != 0)
                    res = mul(res, Euclid_2_1(mod, c), mod);
            }

            return res;
        }
  
        public static BigInteger Joye_double_and_add(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res, t, inverse;
            int powLen;

            res = 1;
            t = found;
            inverse = toMontgomeryDomain(ref t, ref res, mod);
            powLen = (int)(Log((double)pow, 2) + 1);
            for (int i = 0; i < powLen; i++)
            {
                if (((pow >> i) & 1) == 1)
                {
                    res = MontgomeryMultDomain(res, res, mod, inverse);
                    res = MontgomeryMultDomain(res, t, mod, inverse);
                }
                else
                {
                    t = MontgomeryMultDomain(t, t, mod, inverse);
                    t = MontgomeryMultDomain(res, t, mod, inverse);
                }
            }
            res = outMontgomeryDomain(res, mod, inverse);
            return res;
        }

        public static BigInteger MontgomeryLadder(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res, t, inverse;
            int powLen;

            res = 1;
            t = found;
            inverse = toMontgomeryDomain(ref t, ref res, mod);
            powLen = (int)(Log((double)pow, 2));
            for (int i = powLen; 0 <= i; i--)
            {
                if (((pow >> i) & 1) == 0)
                {
                    t = MontgomeryMultDomain(t, res, mod, inverse);
                    res = MontgomeryMultDomain(res, res, mod, inverse);
                }
                else
                {
                    res = MontgomeryMultDomain(res, t, mod, inverse);
                    t = MontgomeryMultDomain(t, t, mod, inverse);
                }
            }
            res = outMontgomeryDomain(res, mod, inverse);
            return res;
        }

        public static BigInteger DBNS1RL(BigInteger found, BigInteger pow, BigInteger mod, bool convert_method, long A, long B)
        {
            BigInteger[,] mas_k;
            mas_k = convert_method ? Convert_to_DBNS_1(pow, A, B)
                                   : Convert_to_DBNS_2(pow, A, B);

            long lastindex = mas_k.GetLength(0) - 1;
            BigInteger t = found;
            BigInteger res = 1;

            for (long i = 0; i < mas_k[lastindex, 1]; i++)
                t = mul(t, t, mod);


            for (long i = 0; i < mas_k[lastindex, 2]; i++)
                t = mul(mul(t, t, mod), t, mod);

            if (mas_k[lastindex, 0] == -1)
                res = mul(res, Euclid_2_1(mod, t), mod);
            else if (mas_k[lastindex, 0] == 1)
                res = mul(res, t, mod);

            for (long i = lastindex - 1; i >= 0; i--)
            {
                BigInteger u = mas_k[i, 1] - mas_k[i + 1, 1];
                BigInteger v = mas_k[i, 2] - mas_k[i + 1, 2];
                for (long j = 0; j < u; j++)
                    t = mul(t, t, mod);

                for (long j = 0; j < v; j++)
                    t = mul(mul(t, t, mod), t, mod);

                if (mas_k[i, 0] == -1)
                    res = mul(res, Euclid_2_1(mod, t), mod);
                else if (mas_k[i, 0] == 1)
                    res = mul(res, t, mod);
            }
            return res;
        }

        public static BigInteger DBNS1LR(BigInteger found, BigInteger pow, BigInteger mod, bool convert_method, long A, long B)
        {
            BigInteger[,] mas_k;

            BigInteger t = found;
            BigInteger res = 1;

            mas_k = convert_method ? Convert_to_DBNS_1(pow, A, B)
                                   : Convert_to_DBNS_2(pow, A, B);

            if (mas_k[0, 0] == -1)
                res = Euclid_2_1(mod, t);
            else if (mas_k[0, 0] == 1)
                res = t;

            for (long i = 0; i < mas_k.GetLength(0) - 1; i++)
            {
                BigInteger u = mas_k[i, 1] - mas_k[i + 1, 1];
                BigInteger v = mas_k[i, 2] - mas_k[i + 1, 2];
                for (long j = 0; j < u; j++)
                    res = mul(res, res, mod);

                for (long j = 0; j < v; j++)
                    res = mul(mul(res, res, mod), res, mod);

                if (mas_k[i + 1, 0] < 0)
                    res = mul(res, Euclid_2_1(mod, t), mod);
                else
                    res = mul(res, t, mod);
            }

            for (long i = 0; i < mas_k[mas_k.GetLength(0) - 1, 1]; i++)
                res = mul(res, res, mod);

            for (long i = 0; i < mas_k[mas_k.GetLength(0) - 1, 2]; i++)
                res = mul(mul(res, res, mod), res, mod);
            return res;
        }

        public static BigInteger DBNS2RL(BigInteger found, BigInteger pow, BigInteger mod)
        {
            MyList<int[]> mas_k = ToDBNS2RL(pow);

            BigInteger res = 1;
            BigInteger t = found;

            for (int i = 0; i < mas_k.Count; i++)
            {
                for (int j = 0; j < mas_k[i][1]; j++)
                    t = mul(t, t, mod);
                for (int j = 0; j < mas_k[i][2]; j++)
                    t = mul((mul(t, t, mod)), t, mod);

                if (mas_k[i][0] == 1)
                    res = mul(res, t, mod);
                else if (mas_k[i][0] == -1)
                    res = mul(res, Euclid_2_1(t, mod), mod);
            }
            return res;
        }

        public static BigInteger DBNS2LR(BigInteger found, BigInteger pow, BigInteger mod)
        {
            MyList<int[]> mas_k = ToDBNS2LR(pow);
            BigInteger res = found;

            for (int i = mas_k.Count - 1; i > 0; i--)
            {
                for (int j = 0; j < mas_k[i][1]; j++)
                    res = mul(res, res, mod);
                for (int j = 0; j < mas_k[i][2]; j++)
                    res = mul(mul(res,  res, mod), res, mod);

                if (mas_k[i][0] == 1)
                    res = mul(res, found, mod);
                else if (mas_k[i][0] == -1)
                    res = mul(res, Euclid_2_1(found, mod), mod);
            }
            for (int j = 0; j < mas_k[0][1]; j++)
                res = mul(res, res, mod);
            for (int j = 0; j < mas_k[0][2]; j++)
                res = mul(mul(res,  res, mod), res, mod);
            return res;
        }
        
        public static BigInteger Bonus1(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res, t, inverse;
            int powLen;

            res = 1;
            t = found;
            inverse = toMontgomeryDomain(ref t, ref res, mod);
            powLen = (int)(Log((double)pow, 2));
            for (int i = powLen; 0 <= i; i--)
            {
                res = MontgomeryMultDomain(res, res, mod, inverse);
                if (1 == ((pow >> i) & 1))
                    res = MontgomeryMultDomain(t, res, mod, inverse);
            }
            res = outMontgomeryDomain(res, mod, inverse);
            return res;
        }
        public static BigInteger Bonus2(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger res, t, inverse;
            int powLen;

            res = 1;
            t = found;
            inverse = toMontgomeryDomain(ref t, ref res, mod);
            powLen = (int)(Log((double)pow, 2));
            for (int i = powLen; 0 <= i; i--)
            {
                res = MontgomeryMultDomain(res, res, mod, inverse);
                if (1 == ((pow >> i) & 1))
                    res = MontgomeryMultDomain(t, res, mod, inverse);
            }
            res = outMontgomeryDomain(res, mod, inverse);
            return res;
        }
        #endregion
        #region window
        private static MyList<BigInteger> Table(BigInteger found, BigInteger pow, int w, BigInteger mod, Multiply mul)
        {
            var table = new MyList<BigInteger>();

            table.Add(found);
            for (BigInteger i = 0; i < BigInteger.Parse((Pow(2, w) - 2).ToString()); i++)
                table.Add(mul(table[i], found, mod));//Приведення до степеня після кожного кроку
            return table;
        }

        public static BigInteger WindowRL(BigInteger found, BigInteger pow,  BigInteger mod, int w, out double table_time)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            MyList<BigInteger> table = Table(found, pow, w, mod, mul);
            stw.Stop();

            table_time = stw.Elapsed.TotalMilliseconds;
            
            BigInteger res = 1;

            List<string> bins = windows(ConvToBinary(pow), w);

            for (int i = bins.Count - 1; i > -1; i--)
            {
                int c = Convert.ToInt32(bins[i], 2);
                if (c != 0) res = mul(res,  table[c - 1], mod);

                for (int k = 0; k < w; k++)
                    for (int j = 0; j < table.Count; j++)
                        table[j] = mul(table[j], table[j], mod);
            }
            return res;
        }

        public static BigInteger WindowLR(BigInteger found, BigInteger pow, BigInteger mod, int w, out double table_time)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            MyList<BigInteger> table = Table(found, pow, w, mod, mul);
            stw.Stop();

            table_time = stw.Elapsed.TotalMilliseconds;
            BigInteger res = 1;

            List<string> bins = windows(ConvToBinary(pow), w);

            for (int i = 0; i < bins.Count; i++)
            {
                for (int k = 0; k < w; k++)
                    res = mul(res, res, mod);

                int c = Convert.ToInt32(bins[i], 2);
                if (c != 0) res = mul(res, table[c - 1], mod);
            }
            return res;
        }

        public static BigInteger WindowLRMod1(BigInteger found, BigInteger pow, BigInteger mod, int w, out double table_time)
        {
            int i, t;
            Stopwatch stw;
            BigInteger res;
            List<BigInteger> table;

            stw = new Stopwatch();
            stw.Start();
            table = new List<BigInteger>();
            table.Add(found);
            for (i = 1; i < w; i++)
                table.Add(mul(table[i - 1], table[i - 1], mod));
            stw.Stop();
            table_time = stw.Elapsed.TotalMilliseconds;

            res = 1;
            i = (int)(Log((double)pow, 2));
            while (i >= 0)
            {
                res = mul(res, res, mod);
                if (1 == ((pow >> i) & 1))
                {
                    if (0 < i && 0 == ((pow >> (i - 1)) & 1))
                    {
                        t = 1;
                        while (t < w
                            && t < i
                            && 0 == ((pow >> (i - t)) & 1))
                        {
                            t++;
                        }
                        res = (BigInteger)Pow((double)res, (double)TwoPow(t - 1));
                        res = mul(res, table[t - 1], mod);
                        i -= t;
                    }
                    else
                    {
                        res = mul(res, found, mod);
                        --i;
                    }
                }
                else
                    --i;
            }
            return res;
        }

        public static BigInteger WindowLRMod22(BigInteger found, BigInteger pow, BigInteger mod, int w, out double table_time)
        {
            int i, t;
            Stopwatch stw;
            BigInteger res;
            List<BigInteger> table;
            var pow_bin = ConvToBinary(pow);

            stw = new Stopwatch();
            stw.Start();
            table = new List<BigInteger>();
            table.Add(found);
            for (i = 1; i <= w; i++)
                table.Add(mul((BigInteger)(Pow(2, i) - 1), found, mod));
            stw.Stop();
            table_time = stw.Elapsed.TotalMilliseconds;

            res = 1;
            i = (int)(Log((double)pow, 2));
            while (i >= 0)
            {
                res = mul(res, res, mod);
                if (1 == ((pow >> i) & 1))
                {
                    t = 1;
                    while (t < w && t < i && 1 == ((pow >> (i - t)) & 1))
                        t++;
                    res = (BigInteger)Pow((double)res, (double)TwoPow(t - 1));
                    res = mul(res, table[t - 1], mod);
                    i -= t > 1 ? t - 1 : 1;
                }
                else
                    i--;
            }
            return res;
        }

        public static BigInteger WindowLRMod2(BigInteger found, BigInteger pow, BigInteger mod, int w, out double table_time)
        {
            int i, t;
            Stopwatch stw;
            BigInteger res;
            List<BigInteger> table;
            var pow_bin = ConvToBinary(pow);

            stw = new Stopwatch();
            stw.Start();
            table = new List<BigInteger>();
            table.Add(found);
            for (i = 1; i <= w; i++)
                table.Add(BinaryRL(found, TwoPow(i) - 1, mod));
            stw.Stop();
            table_time = stw.Elapsed.TotalMilliseconds;

            res = 1;
            i = (int)(Log((double)pow, 2));
            while (i >= 0)
            {
                res = mul(res, res, mod);
                if (1 == ((pow >> i) & 1))
                {
                    t = 1;
                    while (t < w && t < i && 1 == ((pow >> (i - t)) & 1))
                        t++;
                    res = BinaryRL(res, TwoPow(t - 1), mod);
                    res = mul(res, table[t - 1], mod);
                    i -= t > 1 ? t - 1 : 1;
                }
                else
                    i--;
            }
            return res;
        }

        public static BigInteger WindowLRMod3(BigInteger found, BigInteger pow, BigInteger mod, int w, out double table_time)
        {
            int i, t;
            Stopwatch stw;
            BigInteger res;
            List<BigInteger> table;
            var pow_bin = ConvToBinary(pow);

            stw = new Stopwatch();
            stw.Start();
            table = new List<BigInteger>();
            table.Add(found);
            for (i = 2; i <= w; i++)
                table.Add(BinaryRL(found, TwoPow(i - 1) + 1, mod));
            stw.Stop();
            table_time = stw.Elapsed.TotalMilliseconds;
            
            res = 1;
            i = (int)(Log((double)pow, 2));
            while (i > 0)
            {
                res = mul(res, res, mod);
                if (1 == ((pow >> i) & 1))
                {
                    if (0 == ((pow >> (i - 1)) & 1))
                    {
                        t = 3;
                        while (t < w && t < i && 0 == ((pow >> (i - t)) & 1))
                            t++;
                        res = BinaryRL(res, TwoPow(t - 1), mod);
                        res = mul(res, table[t - 1], mod);

                        for (t = 3; t <= w && t < i; t++)
                        {
                            if ('0' != pow_bin[(int)i - t])
                                break;
                        }
                        res *= (BigInteger)Pow(2, t - 1);
                        res += table[t];
                        i -= t - 1;
                    }
                    else
                    {
                        res *= 2;
                        res += table[2];
                        i--;
                    }
                    i--;
                }
            }
            return res;
        }

        public static BigInteger WindowLRMod(BigInteger found, BigInteger pow, BigInteger mod, int w, out double table_time)
        {
            long i;
            int t;
            Stopwatch stw;
            BigInteger res;
            string pow_bin;
            BigInteger[] table1, table2;

            stw = new Stopwatch();
            stw.Start();
            table1 = new BigInteger[w+1];
            table2 = new BigInteger[w+1];
            table1[1] = table2[1] = found;
            for (i = 2; i < w; i++) {
                table1[i] = ((BigInteger)Pow(2, i + 1) - 1) * found;
                table2[i] = ((BigInteger)Pow(2, i) + 1) * found;
            }
            stw.Stop();
            table_time = stw.Elapsed.TotalMilliseconds;

            pow_bin = ConvToBinary(pow);
            res = 0;
            i = (long)Round(Log((double)pow, 2));
            while (i > 0)
            {
                if ('1' == pow_bin[(int)i])
                {
                    if ('0' == pow_bin[(int)i - 1])
                    {
                        for (t = 3; t <= w && t < i; t++)
                        {
                            if ('0' != pow_bin[(int)i - t])
                                break;
                        }
                        if (t == i)
                        {
                            res *= 2;
                            res += found;
                            continue;
                        }
                        res *= (BigInteger)Pow(2, t);
                        res += table2[t];
                    }
                    else
                    {
                        for (t = 2; t <= w && t < i; t++)
                        {
                            if ('1' != pow_bin[(int)i - t])
                                break;
                        }
                        res *= (BigInteger)Pow(2, t);
                        res += table1[t];
                    }
                    i -= t;
                }
                else
                {
                    res *= 2;
                    i--;
                }
            }
            return res;
        }

        public static BigInteger SlideRL(BigInteger found, BigInteger pow, BigInteger mod, int w, out double table_time)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            BigInteger power = TwoPow(w - 1);
            var table = SlideRLTable(found, mod, power, w);
            
            stw.Stop();
            table_time = stw.Elapsed.TotalMilliseconds;

            BigInteger res = 1;
            BigInteger temp = found;

            string binary = bif.ToBin(pow);

            while (binary.Length > 0)
            {
                int index = binary.Length - 1;
                if (binary.Length < w || binary[index - w + 1] == '0')
                {
                    if (binary[index] == '1')
                        res = mul(res, temp, mod);

                    for (int j = 0; j < table.Count; j++)
                        table[j] = mul(table[j], table[j], mod);

                    temp = mul(temp, temp, mod);

                    binary = binary.Remove(index, 1);
                }
                else
                {
                    int c = Convert.ToInt32(binary.Substring(index - w + 1, w), 2);
                    res = mul(res, table[c - power], mod);

                    temp = mul(temp, table[table.Count - 1], mod);

                    for (int k = 0; k < w; k++)
                        for (int j = 0; j < table.Count; j++)
                            table[j] = mul(table[j], table[j], mod);

                    binary = binary.Remove(index - w + 1, w);
                }
            }
            return res;
        }

        public static BigInteger SlideLR(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            Stopwatch stw = new Stopwatch();
            stw.Start();
            BigInteger pow = 2 * (TwoPow(w) - (BigInteger)Pow((-1), w)) / 3 - 1;
            var table = NAFLRTable(found, mod, pow, w);
            stw.Stop();

            table_time = stw.Elapsed.TotalMilliseconds;
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
                    res = mul(res, res, mod);

                if (max[0] > 0)
                    res = mul(res, table[(bif.Abs(max[0]) / 2)], mod);
                else if (max[0] < 0)
                    res = mul(res, Euclid_2_1(mod, table[(bif.Abs(max[0]) / 2)]), mod);

                i = i - max[1];
            }
            return res;
        }
        
        public static BigInteger NAFSlideRL(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            BigInteger res = 1;
            MyList<BigInteger> x = power.ToNAF();

            Stopwatch stw = new Stopwatch();
            stw.Start();

            BigInteger pow = 2 * ((int)TwoPow(w) - (int)Pow((-1), w)) / 3 - 1;
            MyList<BigInteger> table = NAFRLTable(found, mod, pow, w);

            stw.Stop();
            table_time = stw.Elapsed.TotalMilliseconds;

            for (BigInteger i = 0; i < x.Count;)
            {
                List<BigInteger> max = FindLargest1(x, i, w);

                if (max[0] > 0)
                    res = mul(res, table[(bif.Abs(max[0]) / 2)], mod);
                else if (max[0] < 0)
                    res = mul(res, Euclid_2_1(mod, table[(bif.Abs(max[0]) / 2)]), mod);

                for (int d = 0; d < max[1]; d++)
                    for (int j = 0; j < table.Count; j++)
                        table[j] = mul(table[j], table[j], mod);

                i = i + max[1];
            }
            return res;
        }

        public static BigInteger NAFSlideLR(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            BigInteger res = 1;
            MyList<BigInteger> x = power.ToNAF();

            Stopwatch stw = new Stopwatch();
            stw.Start();

            BigInteger pow = 2 * ((int)Pow(2, w) - (int)Pow((-1), w)) / 3 - 1;
            MyList<BigInteger> table = NAFLRTable(found, mod, pow, w);

            stw.Stop();
            table_time = stw.Elapsed.TotalMilliseconds;

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
                    res = mul(res, res, mod);

                if (max[0] > 0)
                    res = mul(res, table[(bif.Abs(max[0]) / 2)], mod);
                else if (max[0] < 0)
                    res = mul(res, Euclid_2_1(mod, table[(bif.Abs(max[0]) / 2)]), mod);

                i = i - (int)max[1];
            }
            return res;
        }

        public static BigInteger NAFWindowRL(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            BigInteger res = 1;
            MyList<BigInteger> x = ToNAF(power);
            MyList<BigInteger> table = new MyList<BigInteger>();
            BigInteger pow = TwoPow( w - 1);

            Stopwatch stw = new Stopwatch();
            stw.Start();
            for (BigInteger i = 1; i < pow; i += 2)
                table.Add(BinaryRL(found, i, mod));
            stw.Stop();

            table_time = stw.Elapsed.TotalMilliseconds;
            for (int i = x.Count - 1; i > -1; i--)
            {
                if (x[i] > 0)
                    res = mul(res,  table[(int)(x[i] / 2)], mod);
                else if (x[i] < 0)
                    res = mul(res,  Euclid_2_1(mod, table[(-x[i] / 2)]), mod);

                for (int j = 0; j < table.Count; j++)
                    table[j] = mul(table[j], table[j], mod);
            }
            return res;
        }

        public static BigInteger NAFWindowLR(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            BigInteger res = 1;
            var x = ToNAF(power);
            var table = new MyList<BigInteger>();
            BigInteger pow = (BigInteger)Pow(2, w - 1);

            Stopwatch stw = new Stopwatch();
            stw.Start();
            for (BigInteger i = 1; i < pow; i += 2)
                table.Add(BinaryLR(found, i, mod));
            stw.Stop();

            table_time = stw.Elapsed.TotalMilliseconds;
            for (BigInteger i = 0; i < x.Count; i++)
            {
                res = mul(res, res, mod);
                if (x[i] > 0)
                    res = mul(res, table[(x[i] / 2)], mod);
                else if (x[i] < 0)
                    res = mul(res, Euclid_2_1(mod, table[(-x[i] / 2)]), mod);
            }
            return res;
        }
        
        public static BigInteger wNAFSlideRL(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            BigInteger res = 1;
            var x = ToWNAF(power, w);

            var table = new MyList<BigInteger>();
            BigInteger pow = 2 * ((BigInteger)Pow(2, w) - (BigInteger)Pow((-1), w)) / 3 - 1;
            Stopwatch stw = new Stopwatch();
            stw.Start();

            for (BigInteger i = 1; i <= pow; i += 2)
                table.Add(BinaryRL(found, i, mod));

            stw.Stop();

            table_time = stw.Elapsed.TotalMilliseconds;

            for (BigInteger i = x.Count - 1; i > -1; )
            {
                var max = FindLargest1(x, i, w);

                if (max[0] > 0)
                    res = mul(res, table[(BigInteger)(Abs((decimal)x[i]) / 2)], mod);
                else if (max[0] < 0)
                    res = mul(res, Euclid_2_1(mod, table[(BigInteger)(Abs((decimal)x[i]) / 2)]), mod);

                for (int d = 0; d < max[1]; d++)
                    for (int j = 0; j < table.Count; j++)
                        table[j] = mul(table[j], table[j], mod);

                i = i - max[1];
            }
            return res;
        }

        public static BigInteger wNAFSlideLR(BigInteger found, BigInteger power, BigInteger mod, int w, out double table_time)
        {
            BigInteger res = 1;
            var x = ToWNAF(power,w);

            var table = new MyList<BigInteger>();
            BigInteger pow = 2 * ((BigInteger)Pow(2, w) - (BigInteger)Pow((-1), w)) / 3 - 1;
            Stopwatch stw = new Stopwatch();
            stw.Start();
            for (BigInteger i = 1; i <= pow; i += 2)
                table.Add(BinaryLR(found, i, mod));
            stw.Stop();

            table_time = stw.Elapsed.TotalMilliseconds;
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
                    res = mul(res, res, mod);

                if (max[0] > 0)
                    res = mul(res, table[(BigInteger)(Abs((decimal)max[0]) / 2)], mod);
                else if (max[0] < 0)
                    res = mul(res, Euclid_2_1(mod, table[(BigInteger)(Abs((decimal)max[0]) / 2)]), mod);

                i = i + max[1];
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

        public static BigInteger random_max(int n, string type = "Bytes")
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

        public static BigInteger random_max_bites(int n)//n - length in bit
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
        public static BigInteger Pow(this BigInteger value, BigInteger exponent)
        {
            BigInteger _base = 1;
            for (BigInteger i = 0; i < exponent; i++)
            {
                _base *= value;
            }
            return _base;
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
                int N = (int)Ceiling(Ceiling(BigInteger.Log(y - 1, 2)) / 8); //kol-vo byte v 4isle (y-1)
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
    }

}
