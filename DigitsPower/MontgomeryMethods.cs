using System;
using System.Numerics;
using static System.Math;
using static DigitsPower.HelpMethods;



namespace DigitsPower
{
    public static class MontgomeryMethods
    {
        public static bool Test(BigInteger a, BigInteger b, BigInteger m)
        {
            return MontgomeryMult(a, b, m) == (a * b) % m;
        }

        public static BigInteger MontgomeryMult(BigInteger a, BigInteger b, BigInteger m)
        {
            // a >= 0, b <= m - 1 (where m is mod); a and b must be in binary form
            // c = 2 ^ (2 * n) % m (where n is number of bit m digit)
            // m must me odd
            if (b >= m)
                throw new NullReferenceException();

            string binary_a = ConvToBinary(a);
            int n = ConvToBinary(m).Length;
            
            BigInteger c = TwoPow(2 * n) % m;
            BigInteger result = 0;

            n = binary_a.Length;
            for (int i = n - 1; i > -1; i--) //from first bit to oldest
            {
                if (binary_a[i] == '1')
                    result += b;
                if (result % 2 == 1) //r - odd
                    result += m;
                result /= 2;
            }

            if (result >= m)
                result -= m;

            a = result; // (A * B * r ^ (-1)) % mod
            b = c;      // r ^ 2
            result = 0;

            for (int i = n - 1; i > -1; i--)
            {
                if (binary_a[i] == '1')
                    result += b;
                if (result % 2 == 1) //r - odd
                    result += m;
                result /= 2;
            }

            if (result >= m)
                result -= m;

            return result;  // (A * B) % mod
        }

        public static BigInteger MontgomeryMult2(BigInteger x, BigInteger y, BigInteger m)
        {
            // a >= 0, b <= m - 1 (where m is mod); a and b must be in binary form
            // c = 2 ^ (2 * n) % m (where n is number of bit m digit)

            string binary_a = ConvToBinary(x);
            int n = binary_a.Length;

            BigInteger c = TwoPow(2 * n) % m;
            BigInteger r = 0;

            for (int i = 0; i < n - 1; i++)
            {
                if (binary_a[i] == '1')
                    r += y;
                if (r % 2 == 1) //r - odd
                    r += m;
                r /= 2;
            }

            if (r >= m)
                r -= m;

            x = r;
            y = c;
            r = 0;

            for (int i = 0; i < n - 1; i++)
            {
                if (binary_a[i] == '1')
                    r += y;
                if (r % 2 == 1) //r - odd
                    r += m;
                r /= 2;
            }

            if (r >= m)
                r -= m;

            return r;
        }

        public static BigInteger ith_digit_finder(BigInteger n, BigInteger b, BigInteger i)
            {
                /**
                n = number whose digits we need to extract
                b = radix in which the number if represented
                i = the ith bit (ie, index of the bit that needs to be extracted)
                **/
                while (i > 0)
                {
                    n /= b;
                    i--;
                }
                return (n % b);
            }

        public static BigInteger eeuclid(BigInteger m, BigInteger b, ref BigInteger inverse)
            {        /// eeuclid( modulus, num whose inv is to be found, variable to put inverse )
                     /// Algorithm used from Stallings book
                BigInteger A1 = 1, A2 = 0, A3 = m,
                    B1 = 0, B2 = 1, B3 = b,
                    T1, T2, T3, Q;

                while (true)
                {
                    if (B3 == 0)
                    {
                        inverse = 0;
                        return A3;      // A3 = gcd(m,b)
                    }

                    if (B3 == 1)
                    {
                        inverse = B2; // B2 = b^-1 mod m
                        return B3;      // A3 = gcd(m,b)
                    }

                    Q = A3 / B3;

                    T1 = A1 - Q * B1;
                    T2 = A2 - Q * B2;
                    T3 = A3 - Q * B3;

                    A1 = B1; A2 = B2; A3 = B3;
                    B1 = T1; B2 = T2; B3 = T3;

                }
            }

        public static BigInteger mon_red(BigInteger m, BigInteger m_dash, BigInteger T, int n, BigInteger radix)
            {
                /**
                m = modulus
                m_dash = m' = -m^-1 mod b
                T = number whose modular reduction is needed, the o/p of the function is TR^-1 mod m
                n = number of bits in m (2n is the number of bits in T)
                b = radix used (for practical implementations, is equal to 2, which is the default value)
                **/
                BigInteger A, ui, temp, Ai;       // Ai is the ith bit of A, need not be lBigInteger probably
                if (m_dash < 0) m_dash = m_dash + radix;
                A = T;
                for (int i = 0; i < n; i++)
                {
                    ///    ui = ( (A%b)*m_dash ) % b;        // step 2.1; A%b gives ai (MISTAKE -- A%b will always give the last digit of A if A is represented in base b); hence we need the function ith_digit_finder()
                    Ai = ith_digit_finder(A, radix, i);
                    ui = ((Ai % radix) * m_dash) % radix;
                    temp = ui * m * (BigInteger)Pow((double)radix, i);
                    A = A + temp;
                }
                A = A / (BigInteger)Pow((double)radix, n);
                if (A >= m) A = A - m;
                return A;
            }

        public static int length_finder(BigInteger a, int c)
            {
                return Convert.ToString((long)a, 2).Length;
        }

        public static BigInteger MonC(BigInteger m, BigInteger T)
            {
                BigInteger d = 0, inverse = 0;
                int radix = 2;
                eeuclid(radix, m, ref d);      // eeuclid( modulus, num whose inverse is to be found, address of variable which is to store inverse)
                 
                return mon_red(m, -d, T, length_finder(m, radix), radix);
            }

    //        public static BigInteger Res_2(BigInteger R, BigInteger N, BigInteger N_2, BigInteger T)
    //        {
    //            /*
    //                input: Integers R and N with gcd(R, N) = 1,
    //                    Integer N′ in [0, R − 1] such that NN′ ≡ −1 mod R,
    //                    Integer T in the range [0, RN − 1]
    //                output: Integer S in the range [0, N − 1] such that S ≡ TR−1 mod N
    //            */
    //            var m = ((T % R) * N_2) % R;
    //            var t = (T + m * N) / R;
    //            if (t >= N)
    //            {
    //                return t - N;
    //            }
    //            else
    //            {
    //                return t;
    //            }
    //        }

    //        public static BigInteger MonPro(BigInteger a, BigInteger b)
    //        {
    //            var t = a * b;
    //            var u = (t + (t * n_ev % r) * n) / r;
    //            while (u < 0)
    //            {
    //                u += n;
    //            }
    //            return u % n;
    //        }

    //        public static BigInteger MonExp(BigInteger a, BigInteger e, BigInteger n)
    //        {
    //            BigInteger d = 0, inverse = 0;
    //            Eeuclid(radix, mod, ref d);      // eeuclid( modulus, num whose inverse is to be found, address of variable which is to store inverse)
    //            string Binary = ConvToBinary(mod);
    //            return mon_red(mod, -d, b, Binary.Length, radix);
    //        }


    //        public static void bn_digit_mult__int(BigInteger a, BigInteger b,
    //                                             ref BigInteger result_lo, ref BigInteger result_hi)
    //        {
    //            /* Length of digit in bits */
    //            BigInteger BN_DIGIT_BITS = (BN_DIGIT_SIZE * 8);
    //            /* Maximum value of digit */
    //            BigInteger BN_MAX_DIGIT = ~0;
    //#if (BN_CC_MULL_DIV)
    //            BigInteger tm = ((a) * (b));
    //            result_lo = (tm & BN_MAX_DIGIT);
    //            result_hi = (tm >> sizeof(BigInteger) * 8);
    //#else
    //             BigInteger reg_res_hi = 0, reg_res_lo = 0;
    //             BigInteger reg_multiplier, reg_multiplicand, reg_multiplicand_hi = 0;

    //            /* Speed optimizations. */
    //            if (0 == a || 0 == b)
    //            {
    //                (*result_lo) = 0;
    //                (*result_hi) = 0;
    //                return;
    //            }
    //            if (a > b)
    //            { /* Optimize cycles count. */
    //                reg_multiplicand = a;
    //                reg_multiplier = b;
    //            }
    //            else {
    //                reg_multiplicand = b;
    //                reg_multiplier = a;
    //            }
    //            if (1 == reg_multiplier)
    //            {
    //                reg_res_lo = reg_multiplicand;
    //                goto ok_exit;
    //            }
    //            if (0 != bn_digit_is_pow2(reg_multiplicand))
    //            {
    //                reg_multiplicand_hi = bn_digit_ctz(reg_multiplicand);
    //                reg_res_lo = (reg_multiplier << reg_multiplicand_hi);
    //                reg_res_hi = (reg_multiplier >> (BN_DIGIT_BITS - reg_multiplicand_hi));
    //                goto ok_exit;
    //            }
    //            if (0 != bn_digit_is_pow2(reg_multiplier))
    //            {
    //                reg_multiplicand_hi = bn_digit_ctz(reg_multiplier);
    //                reg_res_lo = (reg_multiplicand << reg_multiplicand_hi);
    //                reg_res_hi = (reg_multiplicand >> (BN_DIGIT_BITS - reg_multiplicand_hi));
    //                goto ok_exit;
    //            }
    //            /* Calculation. */
    //#if One
    //	            /* It is Knuth's Algorithm M from [Knu2] section 4.3.1.
    //	             * Derived from muldwu.c in the Hacker's Delight collection.
    //	             * http://www.hackersdelight.org/hdcodetxt/mont64.c.txt
    //	             * Montgomery Multiplication:
    //	             * http://www.hackersdelight.org/MontgomeryMultiplication.pdf */
    //	            BigInteger u0, u1, v0, v1, k, t;
    //	            BigInteger w0, w1, w2;

    //	            u1 = (reg_multiplicand >> (BN_DIGIT_BIT_CNT / 2));
    //	            u0 = (reg_multiplicand & (BN_MAX_DIGIT >> (BN_DIGIT_BIT_CNT / 2)));
    //	            v1 = (reg_multiplier >> (BN_DIGIT_BIT_CNT / 2));
    //	            v0 = (reg_multiplier & (BN_MAX_DIGIT >> (BN_DIGIT_BIT_CNT / 2)));

    //	            t = (u0 * v0);
    //	            w0 = (t & (BN_MAX_DIGIT >> (BN_DIGIT_BIT_CNT / 2)));
    //	            k = (t >> (BN_DIGIT_BIT_CNT / 2));

    //	            t = ((u1 * v0) + k);
    //	            w1 = (t & (BN_MAX_DIGIT >> (BN_DIGIT_BIT_CNT / 2)));
    //	            w2 = (t >> (BN_DIGIT_BIT_CNT / 2));

    //	            t = ((u0 * v1) + w1);
    //	            k = (t >> (BN_DIGIT_BIT_CNT / 2));

    //	            reg_res_lo = ((t << (BN_DIGIT_BIT_CNT / 2)) + w0);
    //	            reg_res_hi = ((u1 * v1) + w2 + k);
    //#else
    //            /* Simple and slow multiplication. */
    //            while (0 != reg_multiplier)
    //            {
    //                if (1 & reg_multiplier)
    //                {
    //                    reg_res_lo += reg_multiplicand;
    //                    if (reg_res_lo < reg_multiplicand)
    //                        reg_res_hi++;
    //                    reg_res_hi += reg_multiplicand_hi;
    //                }
    //                reg_multiplicand_hi <<= 1;
    //                if (BN_DIGIT_HI_BIT & reg_multiplicand)
    //                    reg_multiplicand_hi |= 1;
    //                reg_multiplicand <<= 1;
    //                reg_multiplier >>= 1;
    //            }
    //#endif
    //                        ok_exit:
    //                        (*result_lo) = reg_res_lo;
    //                        (*result_hi) = reg_res_hi;
    //                        return;
    //#endif
    //            /* BN_CC_MULL_DIV */
    //        }
    }

    //public class MontgomeryReducer
    //{
    //    // Input parameter
    //    private BigInteger modulus;  // Must be an odd number at least 3

    //    // Computed numbers
    //    private BigInteger reducer;       // Is a power of 2
    //    private int reducerBits;          // Equal to log2(reducer)
    //    private BigInteger reciprocal;    // Equal to reducer^-1 mod modulus
    //    private BigInteger mask;          // Because x mod reducer = x & (reducer - 1)
    //    private BigInteger factor;        // Equal to (reducer * reducer^-1 - 1) / n
    //    private BigInteger convertedOne;  // Equal to convertIn(BigInteger.ONE)



    //    // The modulus must be an odd number at least 3
    //    public MontgomeryReducer(BigInteger modulus)
    //    {
    //        // Modulus
    //        if (modulus == null)
    //            throw new NullReferenceException();
    //        if (modulus % 2 == 0 || modulus < 3)
    //            throw new NullReferenceException(); // Modulus must be an odd number at least 3
    //        this.modulus = modulus;

    //        // Reducer
    //        reducerBits = (modulus.bitLength() / 8 + 1) * 8;  // This is a multiple of 8
    //        var reducer = new BigInteger(1) << (reducerBits);  // This is a power of 256
    //        mask = reducer.subtract(BigInteger.ONE);
    //        assert reducer.compareTo(modulus) > 0 && reducer.gcd(modulus).equals(BigInteger.ONE);

    //        // Other computed numbers
    //        reciprocal = reducer.modInverse(modulus);
    //        factor = reducer.multiply(reciprocal).subtract(BigInteger.ONE).divide(modulus);
    //        convertedOne = reducer.mod(modulus);
    //    }



    //    // The range of x is unlimited
    //    public BigInteger convertIn(BigInteger x)
    //    {
    //        return x.shiftLeft(reducerBits).mod(modulus);
    //    }


    //    // The range of x is unlimited
    //    public BigInteger convertOut(BigInteger x)
    //    {
    //        return x.multiply(reciprocal).mod(modulus);
    //    }


    //    // Inputs and output are in Montgomery form and in the range [0, modulus)
    //    public BigInteger multiply(BigInteger x, BigInteger y)
    //    {
    //        assert x.signum() >= 0 && x.compareTo(modulus) < 0;
    //        assert y.signum() >= 0 && y.compareTo(modulus) < 0;
    //        BigInteger product = x.multiply(y);
    //        BigInteger temp = product.and(mask).multiply(factor).and(mask);
    //        BigInteger reduced = product.add(temp.multiply(modulus)).shiftRight(reducerBits);
    //        BigInteger result = reduced.compareTo(modulus) < 0 ? reduced : reduced.subtract(modulus);
    //        assert result.signum() >= 0 && result.compareTo(modulus) < 0;
    //        return result;
    //    }


    //    // Input x (base) and output (power) are in Montgomery form and in the range [0, modulus); input y (exponent) is in standard form
    //    public BigInteger pow(BigInteger x, BigInteger y)
    //    {
    //        assert x.signum() >= 0 && x.compareTo(modulus) < 0;
    //        if (y.signum() == -1)
    //            throw new IllegalArgumentException("Negative exponent");

    //        BigInteger z = convertedOne;
    //        for (int i = 0, len = y.bitLength(); i < len; i++)
    //        {
    //            if (y.testBit(i))
    //                z = multiply(z, x);
    //            x = multiply(x, x);
    //        }
    //        return z;
    //    }

    //}
}



/*
// mont holds numbers useful for working in Mongomery representation.
type mont struct {
    n  uint     // m.BitLen()
    m  *big.Int // modulus, must be odd
    r2 *big.Int // (1<<2n) mod m
}
 
// constructor
func newMont(m *big.Int) *mont {
    if m.Bit(0) != 1 {
        return nil
    }
    n := uint(m.BitLen())
    x := big.NewInt(1)
    x.Sub(x.Lsh(x, n), m)
    return &mont{n, new(big.Int).Set(m), x.Mod(x.Mul(x, x), m)}
}
 
// Montgomery reduction algorithm
func (m mont) reduce(t *big.Int) *big.Int {
    a := new(big.Int).Set(t)
    for i := uint(0); i < m.n; i++ {
        if a.Bit(0) == 1 {
            a.Add(a, m.m)
        }
        a.Rsh(a, 1)
    }
    if a.Cmp(m.m) >= 0 {
        a.Sub(a, m.m)
    }
    return a
}
 
// example use:
func main() {
    const n = 100 // bit length for numbers in example
 
    // generate random n-bit odd number for modulus m
    rnd := rand.New(rand.NewSource(time.Now().UnixNano()))
    one := big.NewInt(1)
    r1 := new(big.Int).Lsh(one, n-1)
    r2 := new(big.Int).Lsh(one, n-2)
    m := new(big.Int)
    m.Or(r1, m.Or(m.Lsh(m.Rand(rnd, r2), 1), one))
 
    // make Montgomery reduction object around m
    mr := newMont(m)
 
    // generate a couple more numbers in the range 0..m.
    // these are numbers we will do some computations on, mod m.
    x1 := new(big.Int).Rand(rnd, m)
    x2 := new(big.Int).Rand(rnd, m)
 
    // t1, t2 are examples of T, from the task description.
    // Generated this way, they will be in the range 0..m^2, and so < mR.
    t1 := new(big.Int).Mul(x1, mr.r2)
    t2 := new(big.Int).Mul(x2, mr.r2)
 
    // reduce.  r1 and r2 are now montgomery representations of x1 and x2.
    r1 = mr.reduce(t1)
    r2 = mr.reduce(t2)
 
    // this is the end of what is described in the task so far.
    fmt.Println("b:  2")
    fmt.Println("n: ", mr.n)
    fmt.Println("r: ", new(big.Int).Lsh(one, mr.n))
    fmt.Println("m: ", mr.m)
    fmt.Println("t1:", t1)
    fmt.Println("t2:", t2)
    fmt.Println("r1:", r1)
    fmt.Println("r2:", r2)
 
    // but now demonstrate that it works:
    fmt.Println()
    fmt.Println("Original x1:       ", x1)
    fmt.Println("Recovererd from r1:", mr.reduce(r1))
    fmt.Println("Original x2:       ", x2)
    fmt.Println("Recovererd from r2:", mr.reduce(r2))
 
    // and demonstrate a use:
    fmt.Println("\nMontgomery computation of x1 ^ x2 mod m:")
    // this is the modular exponentiation algorithm, except we call
    // mont.reduce instead of using a mod function.
    prod := mr.reduce(mr.r2)             // 1
    base := mr.reduce(t1.Mul(x1, mr.r2)) // x1^1
    exp := new(big.Int).Set(x2)          // not reduced
    for exp.BitLen() > 0 {
        if exp.Bit(0) == 1 {
            prod = mr.reduce(prod.Mul(prod, base))
        }
        exp.Rsh(exp, 1)
        base = mr.reduce(base.Mul(base, base))
    }
    fmt.Println(mr.reduce(prod))
 
    // show library-based equivalent computation as a check
    fmt.Println("\nLibrary-based computation of x1 ^ x2 mod m:")
    fmt.Println(new(big.Int).Exp(x1, x2, m))
}
*/
