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
        public virtual string Name() { return ""; }
        protected virtual void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod) { }
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
        private void Gen(string[] one, string[] two, string[] three, string path, string di,string One,string Two,string Three)
        {
            int[] one_i = MakeDigits(one);
            int[] two_i = MakeDigits(two);
            int[] three_i = MakeDigits(three);

            for (int f_len = 0; f_len < one.Length; f_len++)
            {
                FileStream fin = new FileStream(di + "\\" + One.Split('\\')[0] + "_"+ one_i[f_len] + ".csv", FileMode.Create, FileAccess.Write);

                using (StreamWriter sw = new StreamWriter(fin))
                {
                    sw.Write(Two.Split('\\')[0] + "\\" + Three.Split('\\')[0] + ";");
                    
                    for (int m_len = 0; m_len < three.Length; m_len++)
                    {
                        sw.Write(three_i[m_len] + "(ms)" + ";");
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
                fin.Close();
            }
        }
        public void Create_Result(string Found, string Degree, string Mod, string by)
        {
            string path = Directory.GetCurrentDirectory();
            string[] founds = Directory.GetFiles(path + "\\Foundations\\" + Found);
            string[] degrees = Directory.GetFiles(path + "\\Degrees\\" + Degree);
            string[] mods = Directory.GetFiles(path + "\\Mods\\" + Mod);
            

            DirectoryInfo di;
            switch (by)
            {
                case "Foundations":
                   di = Directory.CreateDirectory($"{path}\\Results\\{Name()}_{Mod.Split('#')[0]}_{Found.Split('#')[0]}+_{Degree.Split('#')[0]}#{DateTime.Now.ToLocalTime().ToString().Replace(':', '-')}");
                    Gen(founds, degrees, mods, path, di.FullName, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod);
                    break;
                case "Degrees":
                    di = Directory.CreateDirectory(String.Format("{0}\\{1}\\{2}_{3}_{4}_{5}+#{6}",
                path, "Results", Name(), Mod.Split('#')[0], Found.Split('#')[0], Degree.Split('#')[0], DateTime.Now.ToLocalTime().ToString().Replace(':', '-')));
                    Gen(degrees, founds, mods, path, di.FullName, "Degrees\\" + Degree, "Foundations\\" + Found, "Mods\\" + Mod);
                    break;
                case "Mods":
                    di = Directory.CreateDirectory(String.Format("{0}\\{1}\\{2}_{3}+_{4}_{5}#{6}",
                path, "Results", Name(), Mod.Split('#')[0], Found.Split('#')[0], Degree.Split('#')[0], DateTime.Now.ToLocalTime().ToString().Replace(':', '-')));
                    Gen(mods, founds, degrees, path, di.FullName, "Mods\\" + Mod, "Foundations\\" + Found, "Degrees\\" + Degree);
                    break;
                default:
                    try
                    {
                        di = Directory.CreateDirectory(String.Format("{0}\\{1}\\{2}_{3}_{4}+_{5}#{6}",
                path, "Results", Name(), Mod.Split('#')[0], Found.Split('#')[0], Degree.Split('#')[0], DateTime.Now.ToLocalTime().ToString().Replace(':', '-')));
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
                                    LoopFunc(BigInteger.Parse(found), BigInteger.Parse(degree), BigInteger.Parse(mod));
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
    #region Binary
    class BinaryRL : BinaryPow
    {
        public override string Name() { return "BinaryRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod) { PowFunctions.BinaryRL(found, pow, mod); }
        ////public override void Result(ListBox l) { l.Items.Add("BinaryRL done"); base.Result(l); }
    }
    class BinaryLR : BinaryPow
    {
        public override string Name() { return "BinaryLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod) { PowFunctions.BinaryLR(found, pow, mod); }
        //public override void Result(ListBox l) { l.Items.Add("BinaryLR done"); base.Result(l); }
    }
    class NAFBinaryRL : BinaryPow
    {
        public override string Name() { return "NAFBinaryRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod) { PowFunctions.NAFBinaryRL(found, pow, mod); }
        //public override void Result(ListBox l) { l.Items.Add("NAFBinaryRL done"); base.Result(l); }
    }
    class NAFBinaryLR : BinaryPow
    {
        public override string Name() { return "NAFBinaryLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod) { PowFunctions.NAFBinaryLR(found, pow, mod); }
        //public override void Result(ListBox l) { l.Items.Add("NAFBinaryLR done"); base.Result(l); }
    }
    class AddSubRL : BinaryPow
    {
        public override string Name() { return "AddSubRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod) { PowFunctions.AddSubRL(found, pow, mod); }
        //public override void Result(ListBox l) { l.Items.Add("AddSubRL done"); base.Result(l); }
    }
    class AddSubLR : BinaryPow
    {
        public override string Name() { return "AddSubLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod) { PowFunctions.AddSubLR(found, pow, mod); }
        //public override void Result(ListBox l) { l.Items.Add("AddSubLR done"); base.Result(l); }
    }
    class DBNS2RL : BinaryPow
    {
        public override string Name() { return "DBNS2RL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod) { PowFunctions.DBNS2RL(found, pow, mod,PowFunctions.Euclid_2_1); }
        //public override void Result(ListBox l) { l.Items.Add("DBNS2RL done"); base.Result(l); }
    }
    class DBNS2LR : BinaryPow
    {
        public override string Name() { return "DBNS2LR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod) { PowFunctions.DBNS2LR(found, pow, mod, PowFunctions.Euclid_2_1); }
        //public override void Result(ListBox l) { l.Items.Add("DBNS2LR done"); base.Result(l); }
    }
    class DBNS1RL : BinaryPow
    {
        bool convert_method;

        public DBNS1RL() : base()
        {
            convert_method = true;
        }

        public DBNS1RL(bool choice)
        {
            convert_method = choice;
        }
        public override string Name() { return "DBNS1RL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod) { PowFunctions.DBNS1RL(found, pow, mod, convert_method); }
        //public override void Result(ListBox l) { l.Items.Add("DBNS2RL done"); base.Result(l); }
    }
    class DBNS1LR : BinaryPow
    {
        bool convert_method;

        public DBNS1LR() : base()
        {
            convert_method = true;
        }

        public DBNS1LR(bool choice)
        {
            convert_method = choice;
        }
        public override string Name() { return "DBNS1LR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod) { PowFunctions.DBNS1LR(found, pow, mod, convert_method); }
        //public override void Result(ListBox l) { l.Items.Add("DBNS2LR done"); base.Result(l); }
    }
    #endregion
    #region Window
    abstract class WindowPow
    {
        public abstract string Name();
        protected abstract void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime);
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
        public void Create_Result(string Found, string Degree, string Mod, string by, string window,bool Table)
        {
            string path = Directory.GetCurrentDirectory();
            
            string[] founds = Directory.GetFiles(path + "\\Foundations\\" + Found);
            string[] degrees = Directory.GetFiles(path + "\\Degrees\\" + Degree);
            string[] mods = Directory.GetFiles(path + "\\Mods\\" + Mod);
            var lengts = GenFunctions.ReadString(window);
            DirectoryInfo di = CD(by, Found, Degree, Mod, window);;
            switch (by.Split(' ')[0])
            {
                case "Foundations":
                    switch (by.Split(' ')[1])
	                {
                        case "Window":
                            GenWin2(founds, degrees, mods,  di.FullName, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, window, Table);
                            break;
                        case "Degrees":
                            Gen(founds, degrees, mods, di.FullName, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, window, Table);
                            break;
                        case "Mods":
                            Gen(founds, mods, degrees, di.FullName, "Foundations\\" + Found, "Mods\\" + Mod, "Degrees\\" + Degree, window, Table);
                            break;
                        default: goto smth_go_wrong;
	                }
                    break;

                case "Degrees":
                    switch (by.Split(' ')[1])
                    {
                        case "Window":
                            GenWin2(degrees, founds, mods, di.FullName, "Degrees\\" + Degree, "Foundations\\" + Found, "Mods\\" + Mod, window, Table);
                            break;
                        case "Foundations":
                            Gen(degrees, founds, mods, di.FullName, "Degrees\\" + Degree, "Foundations\\" + Found, "Mods\\" + Mod, window, Table);
                            break;
                        case "Mods":
                            Gen(degrees, mods, founds,  di.FullName, "Degrees\\" + Degree, "Mods\\" + Mod, "Foundations\\" + Found, window, Table);
                            break;
                        default: goto smth_go_wrong;
                    }
                    break;

                case "Mods":
                    switch (by.Split(' ')[1])
                    {
                        case "Window":
                            GenWin2(mods, degrees, founds, di.FullName, "Mods\\" + Mod, "Degrees\\" + Degree, "Foundations\\" + Found, window, Table);
                            break;
                        case "Foundations":
                            Gen(mods, founds, degrees, di.FullName, "Mods\\" + Mod, "Foundations\\" + Found, "Degrees\\" + Degree, window, Table);
                            break;
                        case "Degrees":
                            Gen(mods, degrees, founds, di.FullName, "Mods\\" + Mod, "Degrees\\" + Degree, "Foundations\\" + Found, window, Table);
                            break;
                        default: goto smth_go_wrong;
                    }
                    
                    break;


                case "Window":
                    switch (by.Split(' ')[1])
                    {
                        case "Degrees":
                            GenWin(degrees, founds, mods, di.FullName, "Degrees\\" + Degree, "Foundations\\" + Found, "Mods\\" + Mod, window, Table);
                            break;
                        case "Foundations":
                            GenWin(founds, degrees, mods, di.FullName, "Foundations\\" + Found, "Degrees\\" + Degree, "Mods\\" + Mod, window, Table);
                            break;
                        case "Mods":
                            GenWin(mods, degrees, founds, di.FullName, "Mods\\" + Mod, "Degrees\\" + Degree, "Foundations\\" + Found, window, Table);
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
       
        private void GenWin(string[] one, string[] two, string[] three,  string di, string One, string Two, string Three, string w, bool Table)
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
                                    LoopFunc(BigInteger.Parse(found), BigInteger.Parse(degree), BigInteger.Parse(mod), w, out t_t);
                                    stw.Stop();

                                    m_sum += (double)(stw.ElapsedTicks / 10);
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
        public override string Name() { return "WindowRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime) { BigInteger e = PowFunctions.WindowRL(found, pow, mod, w, out TableTime); }
        //public override void Result(ListBox l) { l.Items.Add("WindowRL done"); base.Result(l); }
    }
    class WindowLR : WindowPow
    {
        public override string Name() { return "WindowLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime) { BigInteger e = PowFunctions.WindowLR(found, pow, mod, w, out TableTime); }
        //public override void Result(ListBox l) { l.Items.Add("WindowLR done"); base.Result(l); }
    }
    class SlideRL : WindowPow
    {
        public override string Name() { return "SlideRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime) { BigInteger e = PowFunctions.SlideRL(found, pow, mod, w, out TableTime); }
        //public override void Result(ListBox l) { l.Items.Add("SlideRL done"); base.Result(l); }
    }
    class SlideLR : WindowPow
    {
        public override string Name() { return "SlideLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime) { BigInteger e = PowFunctions.SlideLR(found, pow, mod, w, out TableTime); }
        //public override void Result(ListBox l) { l.Items.Add("SlideLR done"); base.Result(l); }
    }
    class NAFSlideRL : WindowPow
    {
        public override string Name() { return "NAFSlideRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime) { BigInteger e = PowFunctions.NAFSlideRL(found, pow, mod, w, out TableTime); }
        //public override void Result(ListBox l) { l.Items.Add("NAFSlideRL done"); base.Result(l); }
    }
    class NAFSlideLR : WindowPow
    {
        public override string Name() { return "NAFSlideLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime) { BigInteger e = PowFunctions.NAFSlideLR(found, pow, mod, w, out TableTime); }
        //public override void Result(ListBox l) { l.Items.Add("NAFSlideLR done"); base.Result(l); }
    }
    class NAFWindowRL : WindowPow
    {
        public override string Name() { return "NAFWindowRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime) { BigInteger e = PowFunctions.NAFWindowRL(found, pow, mod, w, out TableTime); }
        //public override void Result(ListBox l) { l.Items.Add("NAFWindowRL done"); base.Result(l); }
    }
    class NAFWindowLR : WindowPow
    {
        public override string Name() { return "NAFWindowLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime) { BigInteger e = PowFunctions.NAFWindowLR(found, pow, mod, w, out TableTime); }
        //public override void Result(ListBox l) { l.Items.Add("NAFWindowLR done"); base.Result(l); }
    }
    class wNAFSlideRL : WindowPow
    {
        public override string Name() { return "wNAFSlideRL"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime) { BigInteger e = PowFunctions.wNAFSlideRL(found, pow, mod, w, out TableTime); }
        //public override void Result(ListBox l) { l.Items.Add("wNAFSlideRL done"); base.Result(l); }
    }
    class wNAFSlideLR : WindowPow
    {
        public override string Name() { return "wNAFSlideLR"; }
        protected override void LoopFunc(BigInteger found, BigInteger pow, BigInteger mod, int w, out double TableTime) { BigInteger e = PowFunctions.wNAFSlideLR(found, pow, mod, w, out TableTime); }
        //public override void Result(ListBox l) { l.Items.Add("wNAFSlideLR done"); base.Result(l); }
    }
    #endregion
}
