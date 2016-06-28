using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Numerics;

namespace DigitsPower
{
    abstract class BinaryPow
    {
        public Inverse InvMethod { get; set; }
        public Multiply MultMethod { get; set; }

        public string Found { get; set; }
        public string Degree { get; set; }
        public string Mod { get; set; }
        public string Choice { get; set; }

        //static object locker = new object();
        public BinaryPow(string found, string degree, string mod, string by, Inverse inv, Multiply mul)
        {
            Found = found;
            Degree = degree;
            Mod = mod;
            Choice = by;
            InvMethod = inv;
            MultMethod = mul;
        }

        public abstract string Name();

        protected abstract void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul);

        protected int[] MakeDigits(string[] mas)
        {
            int[] mas_i = new int[mas.Length]; ;

            for (int i = 0; i < mas.Length; i++)
            {
                var s = mas[i].Split('\\');
                mas_i[i] = Int32.Parse(s[s.Length - 1].Split('.')[0]);
            }
            Array.Sort(mas_i);
            return mas_i;
        }

        protected virtual void Gen(string[] one, string[] two, string[] three, string path, string di, string One, string Two, string Three)
        {
            int[] one_i = MakeDigits(one);
            int[] two_i = MakeDigits(two);
            int[] three_i = MakeDigits(three);


            for (int f_len = 0; f_len < one.Length; f_len++)
            {
                lock (this)
                {
                    using (FileStream fin = new FileStream(di + "\\" + One.Split('\\')[0] + "_" + one_i[f_len] + ".csv", FileMode.Create, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            sw.Write(Two.Split('\\')[0] + "\\" + Three.Split('\\')[0] + ";");

                            for (int m_len = 0; m_len < three.Length; m_len++)
                            {
                                sw.Write(three_i[m_len] + ";");
                            }
                            sw.WriteLine();
                            for (int d_len = 0; d_len < two.Length; d_len++)
                            {
                                sw.Write(two_i[d_len] + ";");
                                for (int m_len = 0; m_len < three.Length; m_len++)
                                {
                                    sw.Write(Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                   path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                   path + "\\" + Three + "\\" + three_i[m_len] + ".txt") + ";");
                                }
                                sw.WriteLine();
                            }
                        }
                    }
                }
            }
        }

        public virtual void Create_Result()
        {
            string path = Directory.GetCurrentDirectory();
            string[] founds = Directory.GetFiles(path + "\\Foundations\\" + Found);
            string[] degrees = Directory.GetFiles(path + "\\Degrees\\" + Degree);
            string[] mods = Directory.GetFiles(path + "\\Mods\\" + Mod);
            

            DirectoryInfo di;
            switch (Choice)
            {
                case "Foundations":
                   di = Directory.CreateDirectory($"{path}\\Results\\{Name()}_{Mod.Split('#')[0]}_{Found.Split('#')[0]}+_{Degree.Split('#')[0]}#{DateTime.Now.ToLocalTime().ToString().Replace(':', '-')}");
                    Gen(founds, degrees, mods, path, di.FullName, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                    break;
                case "Degrees":
                    di = Directory.CreateDirectory(String.Format("{0}\\{1}\\{2}_{3}_{4}_{5}+#{6}",
                                                   path, "Results", Name(), Mod.Split('#')[0], Found.Split('#')[0], 
                                                   Degree.Split('#')[0], DateTime.Now.ToLocalTime().ToString().Replace(':', '-')));
                    Gen(degrees, founds, mods, path, di.FullName, "Degrees\\" + Degree, "Foundations\\" + Found, "Mods\\" + Mod);
                    break;
                case "Mods":
                    di = Directory.CreateDirectory(String.Format("{0}\\{1}\\{2}_{3}+_{4}_{5}#{6}",
                                                   path, "Results", Name(), Mod.Split('#')[0], Found.Split('#')[0], 
                                                   Degree.Split('#')[0], DateTime.Now.ToLocalTime().ToString().Replace(':', '-')));
                    Gen(mods, founds, degrees, path, di.FullName, "Mods\\" + Mod, "Foundations\\" + Found, "Degrees\\" + Degree);
                    break;
                default:
                    try
                    {
                        di = Directory.CreateDirectory(String.Format("{0}\\{1}\\{2}_{3}_{4}+_{5}#{6}",
                                                       path, "Results", Name(), Mod.Split('#')[0], Found.Split('#')[0], 
                                                       Degree.Split('#')[0], DateTime.Now.ToLocalTime().ToString().Replace(':', '-')));
                        Gen(founds, degrees, mods, path, di.FullName, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
            }
        }

        protected double Watch(string Found_file, string Degree_file, string Mod_file)
        {
            Stopwatch stw = new Stopwatch();
            List<double> sums = new List<double>();
            double f_sum = 0;
            int f_count = 0;
            
            using (StreamReader srf = new StreamReader(Found_file))
            {
                string found;
                while ((found = srf.ReadLine()) != null)
                {
                    double d_sum = 0;
                    int d_count = 0;
                    using (StreamReader srd = new StreamReader(Degree_file))
                    {
                        string degree;
                        while ((degree = srd.ReadLine()) != null)
                        {
                            double m_sum = 0;
                            int m_count = 0;
                            using (StreamReader srm = new StreamReader(Mod_file))
                            {
                                string mod;
                                while ((mod = srm.ReadLine()) != null)
                                {
                                    stw.Start();
                                    LoopFunc(BigInteger.Parse(found), BigInteger.Parse(degree), BigInteger.Parse(mod), InvMethod, MultMethod);
                                    stw.Stop();

                                    m_sum +=  stw.Elapsed.TotalMilliseconds;
                                    m_count++;
                                    stw.Reset();
                                }

                            }
                            d_sum += m_sum / m_count;
                            d_count++;
                        }
                    }
                    f_sum += d_sum / d_count;
                    f_count++;
                }
            }
            return f_sum / f_count;
        }
    }

    abstract class BinaryPowUp
    {
        public Inverse InvMethod { get; set; }
        public Multiply MultMethod { get; set; }

        public string Found { get; set; }
        public string Degree { get; set; }
        public string Mod { get; set; }
        public string[] Choice { get; set; }
        List<int> aMax, bMax;

        public BinaryPowUp(string found, string degree, string mod, string by, Inverse inv, Multiply mul, string a_max, string b_max)
        {
            Found = found;
            Degree = degree;
            Mod = mod;
            Choice = by.Split(' ');
            InvMethod = inv;
            MultMethod = mul;
            aMax = GenFunctions.ReadString(a_max);
            bMax = GenFunctions.ReadString(b_max);
        }

        public abstract string Name();

        protected abstract void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul, params BigInteger[] list);

        protected int[] MakeDigits(string[] mas)
        {
            int[] mas_i = new int[mas.Length]; ;

            for (int i = 0; i < mas.Length; i++)
            {
                var s = mas[i].Split('\\');
                mas_i[i] = Int32.Parse(s[s.Length - 1].Split('.')[0]);
            }
            Array.Sort(mas_i);
            return mas_i;
        }

        protected List<BigInteger> GetFileDigits(string path)
        {
            List<BigInteger> result = new List<BigInteger>();
            using (StreamReader file = new StreamReader(path))
            {
                string digit;
                while ((digit = file.ReadLine()) != null)
                    result.Add(BigInteger.Parse(digit));
            }
            return result;
        }

        private DirectoryInfo CD(string[] by, string Found, string Degree, string Mod)
        {
            DirectoryInfo di;
            string path = Directory.GetCurrentDirectory();
            di = Directory.CreateDirectory(String.Format("{0}\\{1}\\{2}_A({3}-{4})B({5}-{6})_(" + by[0][0] + "_" + by[1][0] + "_" + by[2][0] + ")#{7}",
                                         path, "Results", Name(), aMax[0], aMax[aMax.Count - 1],
                                         bMax[0], bMax[bMax.Count - 1],
                                         DateTime.Now.ToLocalTime().ToString().Replace(':', '-')));

            using (StreamWriter sw = new StreamWriter(new FileStream(di.FullName + "\\" + "Info.txt", FileMode.Create, FileAccess.Write)))
            {
                sw.WriteLine(Mod.Split('#')[0]);
                sw.WriteLine(Found.Split('#')[0]);
                sw.WriteLine(Degree.Split('#')[0]);
            }

            return di;
        }

        protected virtual void GenAB(string[] one, string[] two, string[] three, string di, string One, string Two, string Three,  bool order = true)
        {
            string path = Directory.GetCurrentDirectory();
            int[] one_i = MakeDigits(one);
            int[] two_i = MakeDigits(two);
            int[] three_i = MakeDigits(three);
            DirectoryInfo amax_di, bmax_di;
            if (!order)
            {
                var x = aMax;
                aMax = bMax;
                bMax = aMax;
            }
            for (int amax_len = 0; amax_len < aMax.Count; amax_len++)
            {
                amax_di = Directory.CreateDirectory(di + "\\" + aMax[amax_len]);
                for (int bmax_len = 0; bmax_len < bMax.Count; bmax_len++)
                {
                    bmax_di = Directory.CreateDirectory(di + "\\" + amax_di.Name + "\\" + bMax[bmax_len]);
                    for (int f_len = 0; f_len < one.Length; f_len++)
                    {
                        FileStream fin = new FileStream(di + "\\" + amax_di.Name + "\\" + bmax_di.Name + "\\" + One.Split('\\')[0] + "_" + one_i[f_len] + ".csv", FileMode.Create, FileAccess.Write);

                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            sw.Write(Two.Split('\\')[0] + "\\" + Three.Split('\\')[0] + ";");

                            for (int m_len = 0; m_len < three.Length; m_len++)
                            {
                                sw.Write(three_i[m_len] + ";");
                            }
                            sw.WriteLine();
                            for (int d_len = 0; d_len < two.Length; d_len++)
                            {
                                sw.Write(two_i[d_len] + ";");
                                for (int m_len = 0; m_len < three.Length; m_len++)
                                {
                                    sw.Write(Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                   path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                   path + "\\" + Three + "\\" + three_i[m_len] + ".txt", aMax[amax_len], bMax[bmax_len]) + ";");
                                }
                                sw.WriteLine();
                            }
                        }
                        fin.Close();
                    }
                }
            }
        }

        protected virtual void GenA_B_(string[] one, string[] two, string[] three, string di, string One, string Two, string Three, bool order = true)
        {
            string path = Directory.GetCurrentDirectory();
            int[] one_i = MakeDigits(one);
            int[] two_i = MakeDigits(two);
            int[] three_i = MakeDigits(three);
            DirectoryInfo amax_di, bmax_di;
            if (!order)
            {
                var x = aMax;
                aMax = bMax;
                bMax = aMax;
            }
            for (int amax_len = 0; amax_len < aMax.Count; amax_len++)
            {
                amax_di = Directory.CreateDirectory(di + "\\" + aMax[amax_len]);
                for (int f_len = 0; f_len < one.Length; f_len++)
                {
                    bmax_di = Directory.CreateDirectory(di + "\\" + amax_di.Name + "\\" + One.Split('\\')[0] + "_" + one_i[f_len]);
                    for (int bmax_len = 0; bmax_len < bMax.Count; bmax_len++)
                    {
                        FileStream fin = new FileStream(di + "\\" + amax_di.Name + "\\" + bmax_di.Name + "\\" + bMax[bmax_len]  + ".csv", FileMode.Create, FileAccess.Write);

                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            sw.Write(Two.Split('\\')[0] + "\\" + Three.Split('\\')[0] + ";");

                            for (int m_len = 0; m_len < three.Length; m_len++)
                            {
                                sw.Write(three_i[m_len] + ";");
                            }
                            sw.WriteLine();
                            for (int d_len = 0; d_len < two.Length; d_len++)
                            {
                                sw.Write(two_i[d_len] + ";");
                                for (int m_len = 0; m_len < three.Length; m_len++)
                                {
                                    sw.Write(Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                   path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                   path + "\\" + Three + "\\" + three_i[m_len] + ".txt", aMax[amax_len], bMax[bmax_len]) + ";");
                                }
                                sw.WriteLine();
                            }
                        }
                        fin.Close();
                    }
                }
            }
        }

        protected virtual void GenA__B(string[] one, string[] two, string[] three, string di, string One, string Two, string Three, bool order = true)
        {
            string path = Directory.GetCurrentDirectory();
            int[] one_i = MakeDigits(one);
            int[] two_i = MakeDigits(two);
            int[] three_i = MakeDigits(three);
            DirectoryInfo amax_di, bmax_di;
            if (!order)
            {
                var x = aMax;
                aMax = bMax;
                bMax = aMax;
            }
            for (int amax_len = 0; amax_len < aMax.Count; amax_len++)
            {
                amax_di = Directory.CreateDirectory(di + "\\" + aMax[amax_len]);
                for (int f_len = 0; f_len < one.Length; f_len++)
                {
                    bmax_di = Directory.CreateDirectory(di + "\\" + amax_di.Name + "\\" + One.Split('\\')[0] + "_" + one_i[f_len]);
                    for (int m_len = 0; m_len < three.Length; m_len++)
                    {
                        FileStream fin = new FileStream(di + "\\" + amax_di.Name + "\\" + bmax_di.Name + "\\" + Three.Split('\\')[0] + "_" + three_i[m_len] + ".csv", FileMode.Create, FileAccess.Write);

                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            sw.Write(Two.Split('\\')[0] + "\\" + (order ? "b_max" : "a_max") + ";");

                            for (int bmax_len = 0; bmax_len < bMax.Count; bmax_len++) 
                            {
                                sw.Write(bMax[bmax_len] + ";");
                            }
                            sw.WriteLine();
                            for (int d_len = 0; d_len < two.Length; d_len++)
                            {
                                sw.Write(two_i[d_len] + ";");
                                for (int bmax_len = 0; bmax_len < bMax.Count; bmax_len++) 
                                {
                                    sw.Write(Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                   path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                   path + "\\" + Three + "\\" + three_i[m_len] + ".txt", aMax[amax_len], bMax[bmax_len]) + ";");
                                }
                                sw.WriteLine();
                            }
                        }
                        fin.Close();
                    }
                }
            }
        }

        protected virtual void Gen_AB(string[] one, string[] two, string[] three, string di, string One, string Two, string Three, bool order = true)
        {
            string path = Directory.GetCurrentDirectory();
            int[] one_i = MakeDigits(one);
            int[] two_i = MakeDigits(two);
            int[] three_i = MakeDigits(three);
            DirectoryInfo amax_di, bmax_di, one_dir, two_dir, three_dir;
            if (!order)
            {
                var x = aMax;
                aMax = bMax;
                bMax = aMax; 
            }
            for (int f_len = 0; f_len < one.Length; f_len++)
            {
                one_dir = Directory.CreateDirectory(di + "\\" + One.Split('\\')[0] + "_" + one_i[f_len]);
                for (int amax_len = 0; amax_len < aMax.Count; amax_len++)
                {
                    amax_di = Directory.CreateDirectory(di + "\\" + one_dir.Name + "\\" + aMax[amax_len]);
                    for (int bmax_len = 0; bmax_len < bMax.Count; bmax_len++)
                    {
                        FileStream fin = new FileStream(di + "\\" + one_dir.Name + "\\" + amax_di.Name + "\\" + bMax[bmax_len] + ".csv", FileMode.Create, FileAccess.Write);

                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            sw.Write(Two.Split('\\')[0] + "\\" + Three.Split('\\')[0] + ";");

                            for (int m_len = 0; m_len < three.Length; m_len++)
                            {
                                sw.Write(three_i[m_len] + ";");
                            }
                            sw.WriteLine();
                            for (int d_len = 0; d_len < two.Length; d_len++)
                            {
                                sw.Write(two_i[d_len] + ";");
                                for (int m_len = 0; m_len < three.Length; m_len++)
                                {
                                    sw.Write(Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                   path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                   path + "\\" + Three + "\\" + three_i[m_len] + ".txt", aMax[amax_len], bMax[bmax_len]) + ";");
                                }
                                sw.WriteLine();
                            }
                        }
                        fin.Close();
                    }
                }
            }
        }

        protected virtual void Gen__AB_(string[] one, string[] two, string[] three, string di, string One, string Two, string Three, bool order = true)
        {
            string path = Directory.GetCurrentDirectory();
            int[] one_i = MakeDigits(one);
            int[] two_i = MakeDigits(two);
            int[] three_i = MakeDigits(three);
            DirectoryInfo amax_di, bmax_di, one_dir, two_dir, three_dir;
            if (!order)
            {
                var x = aMax;
                aMax = bMax;
                bMax = aMax;
            }
            for (int f_len = 0; f_len < one.Length; f_len++)
            {
                one_dir = Directory.CreateDirectory(di + "\\" + One.Split('\\')[0] + "_" + one_i[f_len]);
                for (int d_len = 0; d_len < two.Length; d_len++)
                {
                    two_dir = Directory.CreateDirectory(di + "\\" + one_dir.Name + "\\" + Two.Split('\\')[0] + "_" + two_i[d_len]);
                    for (int amax_len = 0; amax_len < aMax.Count; amax_len++)
                    {
                        FileStream fin = new FileStream(di + "\\" + one_dir.Name + "\\" + two_dir.Name + "\\" + aMax[amax_len] + ".csv", FileMode.Create, FileAccess.Write);

                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            sw.Write((order ? "a_max" : "b_max") + "\\" + Three.Split('\\')[0] + ";");

                            for (int m_len = 0; m_len < three.Length; m_len++)
                            {
                                sw.Write(three_i[m_len] + ";");
                            }
                            sw.WriteLine();
                            for (int bmax_len = 0; bmax_len < bMax.Count; bmax_len++)
                            {
                                sw.Write(bMax[bmax_len] + ";");
                                for (int m_len = 0; m_len < three.Length; m_len++)
                                {
                                    sw.Write(Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                   path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                   path + "\\" + Three + "\\" + three_i[m_len] + ".txt", aMax[amax_len], bMax[bmax_len]) + ";");
                                }
                                sw.WriteLine();
                            }
                        }
                        fin.Close();
                    }
                }
            }
        }

        protected virtual void Gen___AB(string[] one, string[] two, string[] three, string di, string One, string Two, string Three, bool order = true)
        {
            string path = Directory.GetCurrentDirectory();
            int[] one_i = MakeDigits(one);
            int[] two_i = MakeDigits(two);
            int[] three_i = MakeDigits(three);
            DirectoryInfo amax_di, bmax_di, one_dir, two_dir, three_dir;
            if (!order)
            {
                var x = aMax;
                aMax = bMax;
                bMax = aMax;
            }
            for (int f_len = 0; f_len < one.Length; f_len++)
            {
                one_dir = Directory.CreateDirectory(di + "\\" + One.Split('\\')[0] + "_" + one_i[f_len]);
                for (int d_len = 0; d_len < two.Length; d_len++)
                {
                    two_dir = Directory.CreateDirectory(di + "\\" + one_dir.Name +  "\\" + Two.Split('\\')[0] + "_" + two_i[d_len]);
                    for  (int m_len = 0; m_len < three.Length; m_len++)
                    {
                        FileStream fin = new FileStream(di + "\\" + one_dir.Name + "\\" + two_dir.Name + "\\" + Three.Split('\\')[0] + three_i[m_len] + ".csv", FileMode.Create, FileAccess.Write);

                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            sw.Write((order ? "a_max" : "b_max") + "\\" + Three.Split('\\')[0] + ";");

                            for (int amax_len = 0; amax_len < aMax.Count; amax_len++)
                            {
                                sw.Write(aMax[amax_len] + ";");
                            }
                            sw.WriteLine();
                            for (int bmax_len = 0; bmax_len < bMax.Count; bmax_len++)
                            {
                                sw.Write(bMax[bmax_len] + ";");
                                for (int amax_len = 0; amax_len < aMax.Count; amax_len++)
                                {
                                    sw.Write(Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                   path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                   path + "\\" + Three + "\\" + three_i[m_len] + ".txt", aMax[amax_len], bMax[bmax_len]) + ";");
                                }
                                sw.WriteLine();
                            }
                        }
                        fin.Close();
                    }
                }
            }
        }

        protected virtual void Gen_A_B_(string[] one, string[] two, string[] three, string di, string One, string Two, string Three, bool order = true)
        {
            string path = Directory.GetCurrentDirectory();
            int[] one_i = MakeDigits(one);
            int[] two_i = MakeDigits(two);
            int[] three_i = MakeDigits(three);
            DirectoryInfo amax_di, bmax_di, one_dir, two_dir, three_dir;
            if (!order)
            {
                var x = aMax;
                aMax = bMax;
                bMax = aMax;
            }
            for (int f_len = 0; f_len < one.Length; f_len++)
            {
                one_dir = Directory.CreateDirectory(di + "\\" + One.Split('\\')[0] + "_" + one_i[f_len]);
                for (int amax_len = 0; amax_len < aMax.Count; amax_len++)
                {
                    amax_di = Directory.CreateDirectory(di + "\\" + one_dir.Name + "\\" + aMax[amax_len]);
                    for (int d_len = 0; d_len < two.Length; d_len++)
                    {
                        FileStream fin = new FileStream(di + "\\" + one_dir.Name + "\\" + amax_di.Name + "\\" + Two.Split('\\')[0] + "_" + two_i[d_len] + ".csv", FileMode.Create, FileAccess.Write);

                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            sw.Write((order ? "b_max" : "a_max") + "\\" + Three.Split('\\')[0] + ";");

                            for (int m_len = 0; m_len < three.Length; m_len++)
                            {
                                sw.Write(three_i[m_len] + ";");
                            }
                            sw.WriteLine();
                            for (int bmax_len = 0; bmax_len < bMax.Count; bmax_len++) 
                            {
                                sw.Write(bMax[bmax_len] + ";");
                                for (int m_len = 0; m_len < three.Length; m_len++)
                                {
                                    sw.Write(Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                   path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                   path + "\\" + Three + "\\" + three_i[m_len] + ".txt", aMax[amax_len], bMax[bmax_len]) + ";");
                                }
                                sw.WriteLine();
                            }
                        }
                        fin.Close();
                    }
                }
            }
        }

        public virtual void Create_Result()
        {
            string path = Directory.GetCurrentDirectory();
            string[] founds = Directory.GetFiles(path + "\\Foundations\\" + Found);
            string[] degrees = Directory.GetFiles(path + "\\Degrees\\" + Degree);
            string[] mods = Directory.GetFiles(path + "\\Mods\\" + Mod);

            DirectoryInfo di = CD(Choice, Found, Degree, Mod);
            var di_short = "Results\\" + di.Name;
            switch (Choice[0])
            {
                case "Foundations":
                    switch (Choice[1])
                    {
                        case "Degrees":
                            switch (Choice[2])
                            {
                                case "Mods":
                                    Gen___AB(founds, degrees, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "aMax":
                                    Gen__AB_(founds, degrees, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "bMax":
                                    Gen__AB_(founds, degrees, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                        case "Mods":
                            switch (Choice[2])
                            {
                                case "Degrees":
                                    Gen___AB(founds, mods, degrees , di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "aMax":
                                    Gen__AB_(founds, mods, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "bMax":
                                    Gen__AB_(founds, mods, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                        case "aMax":
                            switch (Choice[2])
                            {
                                case "Degrees":
                                    Gen_A_B_(founds, degrees, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);//Gen_A_B_
                                    break;
                                case "Mods":
                                    Gen_A_B_(founds, mods, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "bMax":
                                    Gen_AB(founds, mods, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                            }
                            break;
                        case "bMax":
                            switch (Choice[2])
                            {
                                case "Degrees":
                                    Gen_A_B_(founds, degrees, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);//Gen_A_B_
                                    break;
                                case "Mods":
                                    Gen_A_B_(founds, mods, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                                case "aMax":
                                    Gen_AB(founds, mods, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                    }
                    break;
                case "Degrees":
                    switch (Choice[1])
                    {
                        case "Foundations":
                            switch (Choice[2])
                            {
                                case "Mods":
                                    Gen___AB(degrees, founds, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "aMax":
                                    Gen__AB_(degrees, founds, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "bMax":
                                    Gen__AB_(degrees, founds, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                        case "Mods":
                            switch (Choice[2])
                            {
                                case "Foundations":
                                    Gen___AB(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "aMax":
                                    Gen__AB_(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "bMax":
                                    Gen__AB_(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                        case "aMax":
                            switch (Choice[2])
                            {
                                case "Foundations":
                                    Gen_A_B_(degrees, founds, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);//Gen_A_B_
                                    break;
                                case "Mods":
                                    Gen_A_B_(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "bMax":
                                    Gen_AB(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                            }
                            break;
                        case "bMax":
                            switch (Choice[2])
                            {
                                case "Foundations":
                                    Gen_A_B_(degrees, founds, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);//Gen_A_B_
                                    break;
                                case "Mods":
                                    Gen_A_B_(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                                case "aMax":
                                    Gen_AB(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                    }
                    break;
                case "Mods":
                    switch (Choice[1])
                    {
                        case "Foundations":
                            switch (Choice[2])
                            {
                                case "Degrees":
                                    Gen___AB(mods, founds, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "aMax":
                                    Gen__AB_(mods, founds, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "bMax":
                                    Gen__AB_(mods, founds, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                        case "Degrees":
                            switch (Choice[2])
                            {
                                case "Foundations":
                                    Gen___AB(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "aMax":
                                    Gen__AB_(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "bMax":
                                    Gen__AB_(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                        case "aMax":
                            switch (Choice[2])
                            {
                                case "Foundations":
                                    Gen_A_B_(mods, founds, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);//Gen_A_B_
                                    break;
                                case "Degrees":
                                    Gen_A_B_(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "bMax":
                                    Gen_AB(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                            }
                            break;
                        case "bMax":
                            switch (Choice[2])
                            {
                                case "Foundations":
                                    Gen_A_B_(mods, founds, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);//Gen_A_B_
                                    break;
                                case "Degrees":
                                    Gen_A_B_(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                                case "aMax":
                                    Gen_AB(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                    }
                    break;
                case "aMax":
                    switch (Choice[1])
                    {
                        case "Foundations":
                            switch (Choice[2])
                            {
                                case "Degrees":
                                    GenA__B(founds, degrees, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "Mods":
                                    GenA__B(founds, mods, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "bMax":
                                    GenAB(founds, mods, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                            }
                            break;
                        case "Degrees":
                            switch (Choice[2])
                            {
                                case "Foundations":
                                    GenA__B(degrees, founds, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "Mods":
                                    GenA__B(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "bMax":
                                    GenA_B_(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                            }
                            break;
                        case "Mods":
                            switch (Choice[2])
                            {
                                case "Foundations":
                                    GenA__B(mods, founds, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "Degrees":
                                    GenA__B(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "bMax":
                                    GenA_B_(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                            }
                            break;
                        case "bMax":
                            switch (Choice[2])
                            {
                                case "Foundations":
                                    GenAB(founds, mods, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "Degrees":
                                    GenAB(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                                case "Mods":
                                    GenAB(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                                    break;
                            }
                            break;
                    }
                    break;
                case "bMax":
                    switch (Choice[1])
                    {
                        case "Foundations":
                            switch (Choice[2])
                            {
                                case "Degrees":
                                    GenA__B(founds, degrees, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                                case "Mods":
                                    GenA__B(founds, mods, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                                case "aMax":
                                    GenAB(founds, mods, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                        case "Degrees":
                            switch (Choice[2])
                            {
                                case "Foundations":
                                    GenA__B(degrees, founds, mods, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                                case "Mods":
                                    GenA__B(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                                case "aMax":
                                    GenA_B_(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                        case "Mods":
                            switch (Choice[2])
                            {
                                case "Foundations":
                                    GenA__B(mods, founds, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                                case "Degrees":
                                    GenA__B(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                                case "aMax":
                                    GenA_B_(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                        case "aMax":
                            switch (Choice[2])
                            {
                                case "Foundations":
                                    GenAB(founds, mods, degrees, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                                case "Degrees":
                                    GenAB(degrees, mods, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                                case "Mods":
                                    GenAB(mods, degrees, founds, di_short, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, false);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }

        protected double Watch(string Found_file, string Degree_file, string Mod_file, BigInteger amax, BigInteger bmax)
        {
            Stopwatch stw = new Stopwatch();
            List<double> sums = new List<double>();
            double f_sum = 0;
            int f_count = 0;

            using (StreamReader srf = new StreamReader(Found_file))
            {
                string found;
                while ((found = srf.ReadLine()) != null)
                {
                    double d_sum = 0;
                    int d_count = 0;
                    using (StreamReader srd = new StreamReader(Degree_file))
                    {
                        string degree;
                        while ((degree = srd.ReadLine()) != null)
                        {
                            double m_sum = 0;
                            int m_count = 0;
                            using (StreamReader srm = new StreamReader(Mod_file))
                            {
                                string mod;
                                while ((mod = srm.ReadLine()) != null)
                                {
                                    stw.Start();
                                    LoopFunc(BigInteger.Parse(found), BigInteger.Parse(degree), BigInteger.Parse(mod), InvMethod, MultMethod, amax, bmax);
                                    stw.Stop();

                                    m_sum += stw.Elapsed.TotalMilliseconds;
                                    m_count++;
                                    stw.Reset();
                                }

                            }
                            d_sum += m_sum / m_count;
                            d_count++;
                        }
                    }
                    f_sum += d_sum / d_count;
                    f_count++;
                }
            }
            return f_sum / f_count;
        }
    }



    #region Binary
    class BinaryRL : BinaryPow
    {
        public BinaryRL(string found, string degree, string mod, string by, Inverse inv, Multiply mul) : base(found, degree, mod, by, inv, mul){ }
        
        public override string Name() { return "BinaryRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul) { PowFunctions.BinaryRL(found, pow, mod, inv, mul); }
    }
    class BinaryLR : BinaryPow
    {
        public BinaryLR(string found, string degree, string mod, string by, Inverse inv, Multiply mul) : base(found, degree, mod, by, inv, mul) { }

        public override string Name() { return "BinaryLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul) { PowFunctions.BinaryLR(found, pow, mod, inv, mul); }
    }
    class NAFBinaryRL : BinaryPow
    {
        public NAFBinaryRL(string found, string degree, string mod, string by, Inverse inv, Multiply mul) : base(found, degree, mod, by, inv, mul){ }

        public override string Name() { return "NAFBinaryRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul) { PowFunctions.NAFBinaryRL(found, pow, mod, inv, mul); }
    }
    class NAFBinaryLR : BinaryPow
    {
        public NAFBinaryLR(string found, string degree, string mod, string by, Inverse inv, Multiply mul) : base(found, degree, mod, by, inv, mul){ }

        public override string Name() { return "NAFBinaryLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul) { PowFunctions.NAFBinaryLR(found, pow, mod, inv, mul); }
    }
    class AddSubRL : BinaryPow
    {
        public AddSubRL(string found, string degree, string mod, string by, Inverse inv, Multiply mul) : base(found, degree, mod, by, inv, mul){ }

        public override string Name() { return "AddSubRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul) { PowFunctions.AddSubRL(found, pow, mod, inv, mul); }
    }
    class AddSubLR : BinaryPow
    {
        public AddSubLR(string found, string degree, string mod, string by, Inverse inv, Multiply mul) : base(found, degree, mod, by, inv, mul){ }

        public override string Name() { return "AddSubLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul) { PowFunctions.AddSubLR(found, pow, mod, inv, mul); }
    }
    class Joye_double_and_add : BinaryPow
    {
        public Joye_double_and_add(string found, string degree, string mod, string by, Inverse inv, Multiply mul) : base(found, degree, mod, by, inv, mul){ }

        public override string Name() { return "Joye_double_and_add"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul) { PowFunctions.Joye_double_and_add(found, pow, mod, inv, mul); }
    }
    class MontgomeryLadder : BinaryPow
    {
        public MontgomeryLadder(string found, string degree, string mod, string by, Inverse inv, Multiply mul) : base(found, degree, mod, by, inv, mul){ }

        public override string Name() { return "MontgomeryLadder"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul) { PowFunctions.MontgomeryLadder(found, pow, mod, inv, mul); }
    }
    class DBNS2RL : BinaryPow
    {
        public DBNS2RL(string found, string degree, string mod, string by, Inverse inv, Multiply mul) : base(found, degree, mod, by, inv, mul){ }

        public override string Name() { return "DBNS2RL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul) { PowFunctions.DBNS2RL(found, pow, mod, inv, mul); }
    }
    class DBNS2LR : BinaryPow
    {
        public DBNS2LR(string found, string degree, string mod, string by, Inverse inv, Multiply mul) : base(found, degree, mod, by, inv, mul){ }

        public override string Name() { return "DBNS2LR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul) { PowFunctions.DBNS2LR(found, pow, mod, inv, mul); }
    }
    class DBNS1RL : BinaryPowUp
    {
        bool convert_method;

        public DBNS1RL(string found, string degree, string mod, string by, Inverse inv, Multiply mul, string a_max, string b_max) : base(found, degree, mod, by, inv, mul, a_max, b_max)
        {
            convert_method = true;
        }
        public DBNS1RL(string found, string degree, string mod, string by, Inverse inv, Multiply mul, bool choice, string a_max, string b_max) : base(found, degree, mod, by, inv, mul, a_max, b_max)
        {
            convert_method = choice;
        }

        public override string Name() { return "DBNS1RL"; }

        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul, params BigInteger[] list)
        {
            PowFunctions.DBNS1RL(found, pow, mod, inv, mul, convert_method, (long)list[0], (long)list[1]);
        }
    }
    class DBNS1LR : BinaryPowUp
    {
        bool convert_method;

        public DBNS1LR(string found, string degree, string mod, string by, Inverse inv, Multiply mul, string a_max, string b_max) : base(found, degree, mod, by, inv, mul, a_max, b_max)
        {
            convert_method = true;
        }
        public DBNS1LR(string found, string degree, string mod, string by, Inverse inv, Multiply mul, bool choice, string a_max, string b_max) : base(found, degree, mod, by, inv, mul, a_max, b_max)
        {
            convert_method = choice;
        }

        public override string Name() { return "DBNS1LR"; }

        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, Inverse inv, Multiply mul, params BigInteger[] list)
        {
            PowFunctions.DBNS1LR(found, pow, mod, inv, mul, convert_method, (long)list[0], (long)list[1]);
        }
    }
    #endregion
    #region Window
    abstract class WindowPow
    {
        public Inverse InvMethod { get; set; }
        public Multiply MultMethod { get; set; }

        public string Found { get; set; }
        public string Degree { get; set; }
        public string Mod { get; set; }
        public string Choice { get; set; }
        public string Window { get; set; }
        public bool Table { get; set; }

        public WindowPow(string found, string degree, string mod, string by, string window, bool table, Inverse inv, Multiply mul)
        {
            Found = found;
            Degree = degree;
            Mod = mod;
            Choice = by;
            Window = window;
            Table = table;
            InvMethod = inv;
            MultMethod = mul;
        }

        public abstract string Name();

        protected abstract void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime, Inverse inv, Multiply mul);

        private int[] MakeDigits(string[] mas)
        {
            int[] mas_i = new int[mas.Length]; ;

            for (int i = 0; i < mas.Length; i++)
            {
                var s = mas[i].Split('\\');
                mas_i[i] = Int32.Parse(s[s.Length - 1].Split('.')[0]);
            }
            Array.Sort(mas_i);
            return mas_i;
        }

        private void Gen(string[] one, string[] two, string[] three, string di, string One, string Two, string Three, string w, bool Table)
        {
            string path = Directory.GetCurrentDirectory();
            int[] one_i = MakeDigits(one);
            int[] two_i = MakeDigits(two);
            int[] three_i = MakeDigits(three);
            var w_i = GenFunctions.ReadString(w);
            DirectoryInfo o_di;
            for (int f_len = 0; f_len < one.Length; f_len++)
            {
                o_di = Directory.CreateDirectory(di + "\\" + one_i[f_len]);
                double table_time;
                if (Table)
                {
                    for (int d_len = 0; d_len < two.Length; d_len++)
                    {
                        FileStream fin = new FileStream(o_di.FullName + "\\" +two_i[d_len] + ".csv", FileMode.Create, FileAccess.Write);

                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            sw.Write("Win" + "\\" + Three.Split('\\')[0] + ";");

                            for (int m_len = 0; m_len < three.Length; m_len++)
                            {
                                sw.Write(three_i[m_len] + ";");
                            }
                            sw.WriteLine();
                            for (int w_len = 0; w_len < w_i.Count; w_len++)
                            {
                                sw.Write(w_i[w_len] + ";");
                                for (int m_len = 0; m_len < three.Length; m_len++)
                                {
                                    sw.Write(Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                   path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                   path + "\\" + Three + "\\" + three_i[m_len] + ".txt", w_i[w_len], out table_time) + ";");
                                }
                                sw.WriteLine();
                            }
                        }
                        fin.Close();
                    }
                }
                else
                {
                    for (int d_len = 0; d_len < two.Length; d_len++)
                    {
                        FileStream fin = new FileStream(o_di.FullName + "\\" + two_i[d_len] + ".csv", FileMode.Create, FileAccess.Write);
                        FileStream table = new FileStream(o_di.FullName + "\\" + two_i[d_len] + "_table_time" + ".csv", FileMode.Create, FileAccess.Write);
                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            using (StreamWriter sw2 = new StreamWriter(table))
                            {
                                sw.Write("Win" + "\\" + Three.Split('\\')[0] + ";");
                                sw2.Write("Win" + "\\" + Three.Split('\\')[0] + ";");
                                for (int m_len = 0; m_len < three.Length; m_len++)
                                {
                                    sw.Write(three_i[m_len] + ";");
                                    sw2.Write(three_i[m_len] + ";");
                                }
                                sw.WriteLine();
                                sw2.WriteLine();
                                for (int w_len = 0; w_len < w_i.Count; w_len++)
                                {
                                    sw.Write(w_i[w_len] + ";");
                                    sw2.Write(w_i[w_len] + ";");
                                    for (int m_len = 0; m_len < three.Length; m_len++)
                                    {
                                        sw.Write((Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                       path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                       path + "\\" + Three + "\\" + three_i[m_len] + ".txt", w_i[w_len], out table_time) - table_time) + ";");
                                        sw2.Write(table_time + ";");
                                    }
                                    sw.WriteLine();
                                    sw2.WriteLine();
                                }
                            }
                        }
                        fin.Close();
                        table.Close();
                    }
                }
            }
        }

        private void GenWin(string[] one, string[] two, string[] three, string di, string One, string Two, string Three, string w, bool Table)
        {
            string path = Directory.GetCurrentDirectory();
            int[] one_i = MakeDigits(one);
            int[] two_i = MakeDigits(two);
            int[] three_i = MakeDigits(three);
            var w_i = GenFunctions.ReadString(w);
            DirectoryInfo w_di;
            for (int w_len = 0; w_len < w_i.Count; w_len++)
            {
                w_di = Directory.CreateDirectory(di + "\\" + w_i[w_len]);
                double table_time;
                if (Table)
                {
                    for (int f_len = 0; f_len < one.Length; f_len++)
                    {
                        FileStream fin = new FileStream(w_di.FullName + "\\" + One.Split('\\')[0] + "_" + one_i[f_len] + ".csv", FileMode.Create, FileAccess.Write);

                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            sw.Write(Two.Split('\\')[0] + "\\" + Three.Split('\\')[0] + ";");

                            for (int m_len = 0; m_len < three.Length; m_len++)
                            {
                                sw.Write(three_i[m_len] + ";");
                            }
                            sw.WriteLine();
                            for (int d_len = 0; d_len < two.Length; d_len++)
                            {
                                sw.Write(two_i[d_len] + ";");
                                for (int m_len = 0; m_len < three.Length; m_len++)
                                {
                                    sw.Write(Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                   path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                   path + "\\" + Three + "\\" + three_i[m_len] + ".txt", w_i[w_len], out table_time) + ";");
                                }
                                sw.WriteLine();
                            }
                        }
                        fin.Close();
                    }
                }
                else
                {
                    for (int f_len = 0; f_len < one.Length; f_len++)
                    {
                        FileStream fin = new FileStream(w_di.FullName + "\\" + One.Split('\\')[0] + "_" + one_i[f_len] + ".csv", FileMode.Create, FileAccess.Write);
                        FileStream table = new FileStream(w_di.FullName + "\\" + One.Split('\\')[0] + "_" + one_i[f_len] + "_table_time" + ".csv", FileMode.Create, FileAccess.Write);
                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            using (StreamWriter sw2 = new StreamWriter(table))
                            {
                                sw.Write(Two.Split('\\')[0] + "\\" + Three.Split('\\')[0] + ";");
                                sw2.Write(Two.Split('\\')[0] + "\\" + Three.Split('\\')[0] + ";");
                                for (int m_len = 0; m_len < three.Length; m_len++)
                                {
                                    sw.Write(three_i[m_len] + ";");
                                    sw2.Write(three_i[m_len] + ";");
                                }
                                sw.WriteLine();
                                sw2.WriteLine();
                                for (int d_len = 0; d_len < two.Length; d_len++)
                                {
                                    sw.Write(two_i[d_len] + ";");
                                    sw2.Write(two_i[d_len] + ";");
                                    for (int m_len = 0; m_len < three.Length; m_len++)
                                    {
                                        sw.Write((Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                       path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                       path + "\\" + Three + "\\" + three_i[m_len] + ".txt", w_i[w_len], out table_time) - table_time) + ";");
                                        sw2.Write(table_time + ";");
                                    }
                                    sw.WriteLine();
                                    sw2.WriteLine();
                                }
                            }
                        }
                        fin.Close();
                        table.Close();
                    }
                }
            }
        }

        private void GenWin2(string[] one, string[] two, string[] three, string di, string One, string Two, string Three, string w, bool Table)
        {
            string path = Directory.GetCurrentDirectory();
            int[] one_i = MakeDigits(one);
            int[] two_i = MakeDigits(two);
            int[] three_i = MakeDigits(three);
            var w_i = GenFunctions.ReadString(w);
            DirectoryInfo o_di;
            for (int f_len = 0; f_len < one.Length; f_len++)
            {
                o_di = Directory.CreateDirectory(di + "\\" + one_i[f_len]);
                double table_time;
                if (Table)
                {
                    for (int w_len = 0; w_len < w_i.Count; w_len++)
                    {
                        FileStream fin = new FileStream(o_di.FullName + "\\" + One.Split('\\')[0] + "_" + w_i[w_len] + ".csv", FileMode.Create, FileAccess.Write);

                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            sw.Write(Two.Split('\\')[0] + "\\" + Three.Split('\\')[0] + ";");

                            for (int m_len = 0; m_len < three.Length; m_len++)
                            {
                                sw.Write(three_i[m_len] + ";");
                            }
                            sw.WriteLine();
                            for (int d_len = 0; d_len < two.Length; d_len++)
                            {
                                sw.Write(two_i[d_len] + ";");
                                for (int m_len = 0; m_len < three.Length; m_len++)
                                {
                                    sw.Write(Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                   path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                   path + "\\" + Three + "\\" + three_i[m_len] + ".txt", w_i[w_len], out table_time) + ";");
                                }
                                sw.WriteLine();
                            }
                        }
                        fin.Close();
                    }
                }
                else
                {
                    for (int w_len = 0; w_len < w_i.Count; w_len++)
                    {
                        FileStream fin = new FileStream(o_di.FullName + "\\" + One.Split('\\')[0] + "_" + w_i[w_len] + ".csv", FileMode.Create, FileAccess.Write);
                        FileStream table = new FileStream(o_di.FullName + "\\" + One.Split('\\')[0] + "_" + w_i[w_len] + "_table_time" + ".csv", FileMode.Create, FileAccess.Write);
                        using (StreamWriter sw = new StreamWriter(fin))
                        {
                            using (StreamWriter sw2 = new StreamWriter(table))
                            {
                                sw.Write(Two.Split('\\')[0] + "\\" + Three.Split('\\')[0] + ";");
                                sw2.Write(Two.Split('\\')[0] + "\\" + Three.Split('\\')[0] + ";");
                                for (int m_len = 0; m_len < three.Length; m_len++)
                                {
                                    sw.Write(three_i[m_len] + ";");
                                    sw2.Write(three_i[m_len] + ";");
                                }
                                sw.WriteLine();
                                sw2.WriteLine();
                                for (int d_len = 0; d_len < two.Length; d_len++)
                                {
                                    sw.Write(two_i[d_len] + ";");
                                    sw2.Write(two_i[d_len] + ";");
                                    for (int m_len = 0; m_len < three.Length; m_len++)
                                    {
                                        sw.Write((Watch(path + "\\" + One + "\\" + one_i[f_len] + ".txt",
                                                       path + "\\" + Two + "\\" + two_i[d_len] + ".txt",
                                                       path + "\\" + Three + "\\" + three_i[m_len] + ".txt", w_i[w_len], out table_time) - table_time) + ";");
                                        sw2.Write(table_time + ";");
                                    }
                                    sw.WriteLine();
                                    sw2.WriteLine();
                                }
                            }
                        }
                        fin.Close();
                        table.Close();
                    }
                }
            }
        }

        private DirectoryInfo CD(string by, string Found, string Degree, string Mod, string window)
        {
            DirectoryInfo di;
            string path = Directory.GetCurrentDirectory();
            var lengts = GenFunctions.ReadString(window);
            di = Directory.CreateDirectory(String.Format("{0}\\{1}\\{2}_{3}_{4}_{5}_Window_{6}-{7}(" +by.Split(' ')[0][0] + "_" + by.Split(' ')[1][0] + ")#{8}",
                                         path, "Results", Name(), Mod.Split('#')[0], Found.Split('#')[0], Degree.Split('#')[0], lengts[0], lengts[lengts.Count - 1],
                                         DateTime.Now.ToLocalTime().ToString().Replace(':', '-')));
            return di;
        }

        public void Create_Result()
        {
            string path = Directory.GetCurrentDirectory();

            string[] founds = Directory.GetFiles(path + "\\Foundations\\" + Found);
            string[] degrees = Directory.GetFiles(path + "\\Degrees\\" + Degree);
            string[] mods = Directory.GetFiles(path + "\\Mods\\" + Mod);
            DirectoryInfo di = CD(Choice, Found, Degree, Mod, Window);
            switch (Choice.Split(' ')[0])
            {
                case "Foundations":
                    switch (Choice.Split(' ')[1])
                    {
                        case "Window":
                            GenWin2(founds, degrees, mods, di.FullName, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, Window, Table);
                            break;
                        case "Degrees":
                            Gen(founds, degrees, mods, di.FullName, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, Window, Table);
                            break;
                        case "Mods":
                            Gen(founds, mods, degrees, di.FullName, "Foundations\\" + Found, "Mods\\" + Mod, "Degrees\\" + Degree, Window, Table);
                            break;
                        default: goto smth_go_wrong;
                    }
                    break;

                case "Degrees":
                    switch (Choice.Split(' ')[1])
                    {
                        case "Window":
                            GenWin2(degrees, founds, mods, di.FullName, "Degrees\\" + Degree, "Foundations\\" + Found, "Mods\\" + Mod, Window, Table);
                            break;
                        case "Foundations":
                            Gen(degrees, founds, mods, di.FullName, "Degrees\\" + Degree, "Foundations\\" + Found, "Mods\\" + Mod, Window, Table);
                            break;
                        case "Mods":
                            Gen(degrees, mods, founds, di.FullName, "Degrees\\" + Degree, "Mods\\" + Mod, "Foundations\\" + Found, Window, Table);
                            break;
                        default: goto smth_go_wrong;
                    }
                    break;

                case "Mods":
                    switch (Choice.Split(' ')[1])
                    {
                        case "Window":
                            GenWin2(mods, degrees, founds, di.FullName, "Mods\\" + Mod, "Degrees\\" + Degree, "Foundations\\" + Found, Window, Table);
                            break;
                        case "Foundations":
                            Gen(mods, founds, degrees, di.FullName, "Mods\\" + Mod, "Foundations\\" + Found, "Degrees\\" + Degree, Window, Table);
                            break;
                        case "Degrees":
                            Gen(mods, degrees, founds, di.FullName, "Mods\\" + Mod, "Degrees\\" + Degree, "Foundations\\" + Found, Window, Table);
                            break;
                        default: goto smth_go_wrong;
                    }

                    break;


                case "Window":
                    switch (Choice.Split(' ')[1])
                    {
                        case "Degrees":
                            GenWin(degrees, founds, mods, di.FullName, "Degrees\\" + Degree, "Foundations\\" + Found, "Mods\\" + Mod, Window, Table);
                            break;
                        case "Foundations":
                            GenWin(founds, degrees, mods, di.FullName, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, Window, Table);
                            break;
                        case "Mods":
                            GenWin(mods, degrees, founds, di.FullName, "Mods\\" + Mod, "Degrees\\" + Degree, "Foundations\\" + Found, Window, Table);
                            break;
                        default: goto smth_go_wrong;
                    }
                    break;

                default:
                    smth_go_wrong:
                    MessageBox.Show("Wrong parametrs", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        public void Create_Result(string found, string degree, string mod, string by, string window, bool table)
        {
            string path = Directory.GetCurrentDirectory();
            
            string[] founds = Directory.GetFiles(path + "\\Foundations\\" + found);
            string[] degrees = Directory.GetFiles(path + "\\Degrees\\" + degree);
            string[] mods = Directory.GetFiles(path + "\\Mods\\" + mod);
            var lengts = GenFunctions.ReadString(Window);
            DirectoryInfo di = CD(Choice, found, degree, mod, Window);
            switch (Choice.Split(' ')[0])
            {
                case "Foundations":
                    switch (Choice.Split(' ')[1])
	                {
                        case "Window":
                            GenWin2(founds, degrees, mods,  di.FullName, "Foundations\\" + found, "Degrees\\" + degree, "Mods\\" + mod, Window, table);
                            break;
                        case "Degrees":
                            Gen(founds, degrees, mods, di.FullName, "Foundations\\" + found, "Degrees\\" + degree, "Mods\\" + mod, Window, table);
                            break;
                        case "Mods":
                            Gen(founds, mods, degrees, di.FullName, "Foundations\\" + found, "Mods\\" + mod, "Degrees\\" + degree, Window, table);
                            break;
                        default: goto smth_go_wrong;
	                }
                    break;

                case "Degrees":
                    switch (Choice.Split(' ')[1])
                    {
                        case "Window":
                            GenWin2(degrees, founds, mods, di.FullName, "Degrees\\" + degree, "Foundations\\" + found, "Mods\\" + mod, Window, table);
                            break;
                        case "Foundations":
                            Gen(degrees, founds, mods, di.FullName, "Degrees\\" + degree, "Foundations\\" + found, "Mods\\" + mod, Window, table);
                            break;
                        case "Mods":
                            Gen(degrees, mods, founds,  di.FullName, "Degrees\\" + degree, "Mods\\" + mod, "Foundations\\" + found, Window, table);
                            break;
                        default: goto smth_go_wrong;
                    }
                    break;

                case "Mods":
                    switch (Choice.Split(' ')[1])
                    {
                        case "Window":
                            GenWin2(mods, degrees, founds, di.FullName, "Mods\\" + mod, "Degrees\\" + degree, "Foundations\\" + found, Window, table);
                            break;
                        case "Foundations":
                            Gen(mods, founds, degrees, di.FullName, "Mods\\" + mod, "Foundations\\" + found, "Degrees\\" + degree, Window, table);
                            break;
                        case "Degrees":
                            Gen(mods, degrees, founds, di.FullName, "Mods\\" + mod, "Degrees\\" + degree, "Foundations\\" + found, Window, table);
                            break;
                        default: goto smth_go_wrong;
                    }
                    
                    break;


                case "Window":
                    switch (Choice.Split(' ')[1])
                    {
                        case "Degrees":
                            GenWin(degrees, founds, mods, di.FullName, "Degrees\\" + degree, "Foundations\\" + found, "Mods\\" + mod, Window, table);
                            break;
                        case "Foundations":
                            GenWin(founds, degrees, mods, di.FullName, "Foundations\\" + found, "Degrees\\" + degree, "Mods\\" + mod, Window, table);
                            break;
                        case "Mods":
                            GenWin(mods, degrees, founds, di.FullName, "Mods\\" + mod, "Degrees\\" + degree, "Foundations\\" + found, Window, table);
                            break;
                        default: goto smth_go_wrong;
                    }
                    break;

                default:
                smth_go_wrong:
                MessageBox.Show("Wrong parametrs", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        protected double Watch(string Found_file, string Degree_file, string Mod_file, int w, out double table_time)
        {
            Stopwatch stw = new Stopwatch();
            List<double> sums = new List<double>();
            double f_sum = 0;
            double t_d,t_m,t_t;
            table_time = 0;
            int f_count = 0;
            using (StreamReader srf = new StreamReader(Found_file))
            {
                string found;
                while ((found = srf.ReadLine()) != null)
                {
                    double d_sum = 0;
                    int d_count = 0;
                    t_d = 0;
                    using (StreamReader srd = new StreamReader(Degree_file))
                    {
                        string degree;
                        while ((degree = srd.ReadLine()) != null)
                        {
                            double m_sum = 0;
                            int m_count = 0;
                            t_m = 0;
                            using (StreamReader srm = new StreamReader(Mod_file))
                            {
                                string mod;
                                while ((mod = srm.ReadLine()) != null)
                                {
                                    stw.Start();
                                    LoopFunc(BigInteger.Parse(found), BigInteger.Parse(degree), BigInteger.Parse(mod), w, out t_t, InvMethod, MultMethod);
                                    stw.Stop();

                                    m_sum += stw.Elapsed.TotalMilliseconds;
                                    m_count++;
                                    t_m += t_t;
                                    stw.Reset();
                                }

                            }
                            d_sum += m_sum / m_count;
                            t_d += t_m / m_count;
                            d_count++;
                        }
                    }
                    f_sum += d_sum / d_count;
                    table_time += t_d / d_count;
                    f_count++;
                }
            }
            table_time = table_time / f_count;
            return f_sum / f_count;
        }
    }
    class WindowRL : WindowPow
    {
        public WindowRL(string found, string degree, string mod, string by, string window, bool table, Inverse inv, Multiply mul) :
                   base(found, degree, mod, by, window, table, inv, mul) { }

        public override string Name() { return "WindowRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime, Inverse inv, Multiply mul) { BigInteger e = PowFunctions.WindowRL(found, pow, mod, w, out TableTime, inv, mul); }
    }
    class WindowLR : WindowPow
    {
        public WindowLR(string found, string degree, string mod, string by, string window, bool table, Inverse inv, Multiply mul) :
                   base(found, degree, mod, by, window, table, inv, mul) { }

        public override string Name() { return "WindowLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime, Inverse inv, Multiply mul) { BigInteger e = PowFunctions.WindowLR(found, pow, mod, w, out TableTime, inv, mul); }
    }
    class SlideRL : WindowPow
    {
        public SlideRL(string found, string degree, string mod, string by, string window, bool table, Inverse inv, Multiply mul) :
                   base(found, degree, mod, by, window, table, inv, mul) { }

        public override string Name() { return "SlideRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime, Inverse inv, Multiply mul) { BigInteger e = PowFunctions.SlideRL(found, pow, mod, w, out TableTime, inv, mul); }
    }
    class SlideLR : WindowPow
    {
        public SlideLR(string found, string degree, string mod, string by, string window, bool table, Inverse inv, Multiply mul) :
                   base(found, degree, mod, by, window, table, inv, mul) { }

        public override string Name() { return "SlideLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime, Inverse inv, Multiply mul) { BigInteger e = PowFunctions.SlideLR(found, pow, mod, w, out TableTime, inv, mul); }
    }
    class NAFSlideRL : WindowPow
    {
        public NAFSlideRL(string found, string degree, string mod, string by, string window, bool table, Inverse inv, Multiply mul) :
                   base(found, degree, mod, by, window, table, inv, mul) { }

        public override string Name() { return "NAFSlideRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime, Inverse inv, Multiply mul) { BigInteger e = PowFunctions.NAFSlideRL(found, pow, mod, w, out TableTime, inv, mul); }
    }
    class NAFSlideLR : WindowPow
    {
        public NAFSlideLR(string found, string degree, string mod, string by, string window, bool table, Inverse inv, Multiply mul) :
                   base(found, degree, mod, by, window, table, inv, mul) { }

        public override string Name() { return "NAFSlideLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime, Inverse inv, Multiply mul) { BigInteger e = PowFunctions.NAFSlideLR(found, pow, mod, w, out TableTime, inv, mul); }
    }
    class NAFWindowRL : WindowPow
    {
        public NAFWindowRL(string found, string degree, string mod, string by, string window, bool table, Inverse inv, Multiply mul) :
                   base(found, degree, mod, by, window, table, inv, mul) { }

        public override string Name() { return "NAFWindowRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime, Inverse inv, Multiply mul) { BigInteger e = PowFunctions.NAFWindowRL(found, pow, mod, w, out TableTime, inv, mul); }
    }
    class NAFWindowLR : WindowPow
    {
        public NAFWindowLR(string found, string degree, string mod, string by, string window, bool table, Inverse inv, Multiply mul) :
                   base(found, degree, mod, by, window, table, inv, mul) { }

        public override string Name() { return "NAFWindowLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime, Inverse inv, Multiply mul) { BigInteger e = PowFunctions.NAFWindowLR(found, pow, mod, w, out TableTime, inv, mul); }
    }
    class wNAFSlideRL : WindowPow
    {
        public wNAFSlideRL(string found, string degree, string mod, string by, string window, bool table, Inverse inv, Multiply mul) :
                   base(found, degree, mod, by, window, table, inv, mul) { }

        public override string Name() { return "wNAFSlideRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime, Inverse inv, Multiply mul) { BigInteger e = PowFunctions.wNAFSlideRL(found, pow, mod, w, out TableTime, inv, mul); }
    }
    class wNAFSlideLR : WindowPow
    {
        public wNAFSlideLR(string found, string degree, string mod, string by, string window, bool table, Inverse inv, Multiply mul) :
                   base(found, degree, mod, by, window, table, inv, mul) { }

        public override string Name() { return "wNAFSlideLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime, Inverse inv, Multiply mul) { BigInteger e = PowFunctions.wNAFSlideLR(found, pow, mod, w, out TableTime, inv, mul); }
    }
    #endregion
}
