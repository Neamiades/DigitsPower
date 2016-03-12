using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace DigitsPower
{
    class methods
    {

        public static BigInteger Point_Multiplication_Affine_Coord_19(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger[,] mas_k;
            mas_k = Convert_to_DBNS_1(pow, AdditionalParameters.A, AdditionalParameters.B);

            Int64 lastindex = mas_k.GetLength(0) - 1;
            BigInteger t = found;
            BigInteger res = 1;

            for (Int64 i = 0; i < mas_k[lastindex, 1]; i++)
                t = t * t % mod;


            for (Int64 i = 0; i < mas_k[lastindex, 2]; i++)
                t = t * t * t % mod;

            if (mas_k[lastindex, 0] == -1)
                res = res * AdditionalParameters.inv(mod, t) % mod;
            else if (mas_k[lastindex, 0] == 1)
                res = res * t % mod;

            for (Int64 i = lastindex - 1; i >= 0; i--)
            {
                BigInteger u = mas_k[i, 1] - mas_k[i + 1, 1];
                BigInteger v = mas_k[i, 2] - mas_k[i + 1, 2];
                for (Int64 j = 0; j < u; j++)                
                    t = t * t % mod;                

                for (Int64 j = 0; j < v; j++)                
                    t = t * t * t % mod;

                if (mas_k[i, 0] == -1)
                    res = res * AdditionalParameters.inv(mod, t) % mod;
                else if (mas_k[i, 0] == 1)
                    res = res * t % mod;
            }
            return res;
        }

        public static BigInteger Point_Multiplication_Affine_Coord_20(BigInteger found, BigInteger pow, BigInteger mod)
        {
            BigInteger[,] mas_k;

            BigInteger t = found;
            BigInteger res = 1;

            mas_k = Convert_to_DBNS_2(pow, AdditionalParameters.A, AdditionalParameters.B);

            if (mas_k[0, 0] == -1)
                res = AdditionalParameters.inv(mod, t);
            else if (mas_k[0, 0] == 1)
                res = t;

            for (Int64 i = 0; i < mas_k.GetLength(0) - 1; i++)
            {
                BigInteger u = mas_k[i, 1] - mas_k[i + 1, 1];
                BigInteger v = mas_k[i, 2] - mas_k[i + 1, 2];
                for (Int64 j = 0; j < u; j++)                
                    res = res * res % mod;

                for (Int64 j = 0; j < v; j++)
                    res = res * res * res % mod;

                if (mas_k[i+1, 0] < 0)
                    res = res * AdditionalParameters.inv(mod, t) % mod;
                else
                    res = res * t % mod;     
            }

            for (Int64 i = 0; i < mas_k[mas_k.GetLength(0) - 1, 1]; i++)
                res = res * res % mod;

            for (Int64 i = 0; i < mas_k[mas_k.GetLength(0) - 1, 2]; i++)
                res = res * res * res % mod;
            return res;
            
        }

        public static BigInteger[,] Convert_to_DBNS_1(BigInteger k, Int64 a_max, Int64 b_max)
        {
            List<BigInteger[]> mass_k_l = new List<BigInteger[]>();
            Int64 s = 1;
            List<Int64> app;
            Int64 a = a_max;
            Int64 b = b_max;
            while (k > 0)
            {
                app = Best_Approximation_1(k, a, b);
                a = app[0];
                b = app[1];
                BigInteger z = bif.Pow(2, a) * bif.Pow(3, b);
                mass_k_l.Add(new BigInteger[3] { s, a, b });
                if (k < z)                
                    s = -s;
                
                k = bif.Abs(k - z);
            }
            BigInteger[,] mass_k = new BigInteger[mass_k_l.Count, 3];
            for (int j = 0; j < mass_k_l.Count; j++)
            {
                mass_k[j, 0] = mass_k_l[j][0];
                mass_k[j, 1] = mass_k_l[j][1];
                mass_k[j, 2] = mass_k_l[j][2];
            }
            return mass_k;
        }

        public static BigInteger[,] Convert_to_DBNS_2(BigInteger k, Int64 a_max, Int64 b_max)
        {
            List<BigInteger[]> mass_k_l = new List<BigInteger[]>();
            BigInteger i = 0;
            BigInteger s = 1;
            List<Int64> app;
            Int64 a = a_max;
            Int64 b = b_max;
            while (k > 0)
            {
                i++;
                app = Best_Approximation_2(k, a, b);
                a = app[0];
                b = app[1];
                var test1 = PowFunctions.TwoPow(a);
                var test2 = bif.Pow(3, b);
                BigInteger z = (PowFunctions.TwoPow(a) * bif.Pow(3, b));
                mass_k_l.Add(new BigInteger[3] { s, a, b });
                if (k < z)                
                    s = -s;
                
                k = bif.Abs(k - z);
            }

            BigInteger[,] mass_k = new BigInteger[mass_k_l.Count, 3];
            for (int j = 0; j < mass_k_l.Count; j++)
            {
                mass_k[j, 0] = mass_k_l[j][0];
                mass_k[j, 1] = mass_k_l[j][1];
                mass_k[j, 2] = mass_k_l[j][2];
            }
            return mass_k;
        }

        public static List<Int64> Best_Approximation_1(BigInteger k, Int64 a_max, Int64 b_max)
        {
            Int64 a;
            Int64 b;
            Int64 min_x = a_max;
            Int64 y = (Int64)Math.Round((double)(-min_x) * log_dif_base(3, 2) + bif.log_dif_base(k, 3));
            if (y > b_max)
                y = b_max;
            else if (y < 0)
                y = 0;

            double min_delta = Math.Abs((y + (double)min_x * log_dif_base(3, 2) - bif.log_dif_base(k, 3)));
            for (Int64 x = 0; x < a_max; x++)
            {
                y = (Int64)Math.Round(-x * log_dif_base(3, 2) + bif.log_dif_base(k, 3));
                if (y > b_max)
                    y = b_max;
                else if (y < 0)
                    y = 0;

                double delta = Math.Abs(y + x * log_dif_base(3, 2) - bif.log_dif_base(k, 3));
                if (min_delta > delta)
                {
                    min_x = x;
                    min_delta = delta;
                }
            }

            a = min_x;
            b = (Int64)Math.Round((double)-min_x * log_dif_base(3, 2) + bif.log_dif_base(k, 3));
            if (b > b_max)            
                b = b_max;

            List<Int64> r = new List<Int64>();
            r.Add(a);
            r.Add(b);
            return r;
        }

        public static List<Int64> Best_Approximation_2(BigInteger k, Int64 a_max, Int64 b_max)
        {
            Int64 a = 0;
            Int64 b = 0;
            Int64 legth_k = get_number_bit(k, 2);
            Int64[,] PreComputation = new Int64[b_max + 1, 3];
            Int64 i;
            for (i = 0; i <= b_max; i++)
            {
                PreComputation[i, 0] = i;
                PreComputation[i, 1] = (Int64)Math.Pow(3, i);
                Int64 temp = get_number_bit(PreComputation[i, 1], 2);
                if (temp > legth_k)
                {
                    b_max = i - 1;
                    break;
                }
                PreComputation[i, 2] = (PreComputation[i, 1] << (int)(legth_k - temp));
            }

            for (i = 0; i <= b_max; i++)
            {
                Int64 i_min = i;
                Int64 min = PreComputation[i, 2];
                for (Int64 u = i + 1; u <= b_max; u++)
                {
                    if (min > PreComputation[u, 2])
                    {
                        i_min = u;
                        min = PreComputation[u, 2];
                    }
                }
                for (Int64 j = 0; j < 3; j++)
                {
                    Int64 temp = PreComputation[i_min, j];
                    PreComputation[i_min, j] = PreComputation[i, j];
                    PreComputation[i, j] = temp;
                }
            }
            i = b_max + 1;
            Int64 length_1 = 0;
            Int64 length_max = 0;

            while (i > 0 && length_1 >= length_max)
            {
                int j = 0;
                length_max = length_1;
                string str1 = bif.ToBin(k);
                string str2 = Convert.ToString((PreComputation[i - 1, 2]), 2);
                while (j < legth_k - 1 && j < Math.Min(str2.Length, str1.Length) && (str2[j] == str1[j]))
                {
                    j = j + 1;
                }
                if (j != 0)
                {
                    length_1 = legth_k - (legth_k - j);
                }
                else length_1 = legth_k;
                i = i - 1;
            }
            if (length_1 < length_max)
            {
                i = i + 2;
            }
            else i = 1;

            Int64 b1 = PreComputation[i - 1, 0];
            Int64 a1 = legth_k - get_number_bit(PreComputation[i - 1, 1], 2);
            if (a1 < 0)
            {
                a1 = 0;
            }
            Int64 a2, b2;
            if (i < b_max + 1)
            {
                b2 = PreComputation[i, 0];
                a2 = legth_k - get_number_bit(PreComputation[i, 1], 2);
                if (a2 < 0)
                {
                    a2 = 0;
                }
            }
            else
            {
                b2 = 0;
                a2 = 0;
            }

            Re_Compute_a_b(ref a1, ref b1, a_max, b_max, k);
            Re_Compute_a_b(ref a2, ref b2, a_max, b_max, k);
            if (Math.Abs((double)k - Math.Pow(2, a1) * Math.Pow(3, b1)) < Math.Abs((double)k - Math.Pow(2, a2) * Math.Pow(3, b2)))
            {
                a = a1;
                b = b1;
            }
            else
            {
                a = a2;
                b = b2;
            }
            if ((a != a_max) && (Math.Abs((double)k -Math.Pow(2, a + 1) * Math.Pow(3, b)) < Math.Abs((double)k - Math.Pow(2, a) * Math.Pow(3, b))))
            {
                a = a + 1;
            }
            List<Int64> r = new List<Int64>();
            r.Add(a);
            r.Add(b);
            return r;
        }

        public static double log_dif_base(Int64 _base, Int64 argument)
        {
            return (Math.Log(argument) / Math.Log(_base));
        }

        public static Int64 get_number_bit(BigInteger value, Int64 _base)
        {
            Int64 number_bit = (Int64)Math.Floor(bif.log_dif_base(value, _base));
            if (((Int64)bif.log_dif_base(value, _base) > number_bit) || number_bit == 0)
                number_bit++;
            return number_bit;
        }

        public static void Re_Compute_a_b(ref Int64 a, ref Int64 b, Int64 a_max, Int64 b_max, BigInteger k)
        {
            if (a > a_max)
            {
                Int64 temp = get_number_bit((Int64)Math.Pow(2, a - a_max), 3) - 1;
                b = b + temp;

                if (b > b_max)
                {
                    b = b_max;
                }
                if (a_max > 0)
                {
                    temp = a_max - 1;
                }
                else temp = 0;
                if ((Math.Abs((double)k - Math.Pow(2, a_max) * Math.Pow(3, b)) < Math.Abs((double)k - Math.Pow(2, temp) * Math.Pow(3, (b + 1)))) || b == b_max)
                {
                    a = a_max;
                }
                else
                {
                    a = temp;
                    b = b + 1;
                }
            }
        }
    }
}
