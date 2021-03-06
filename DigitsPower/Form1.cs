﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Numerics;
using System.IO;
using System.Diagnostics;
using System.Threading;
using static DigitsPower.HelpMethods;
//using System.Security.Cryptography;
using static DigitsPower.MontgomeryMethods;

namespace DigitsPower
{
    //1.Binary RL
    //2.Binary LR
    //3.Window RL
    //4.Window LR
    //5.Slide Window RL
    //6.Slide Window LR
    //7.1.NAF Binary RL
    //7.2.NAF Binary RL_2
    //8.NAF Binary LR
    //9.NAF Slide RL
    //10.NAF Slide LR
    //11.NAF Window RL
    //12.NAF Window LR
    //13.wNAF Slide RL
    //14.wNAF Slide LR
    //15.Add Sub RL
    //16.Add Sub LR
    //17.Joye double & add
    //18.Montgomery ladder
    //19.DBNS 1 RL
    //20.DBNS 1 LR
    //21.DBNS 2 RL
    //22.DBNS 2 LR
    
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            CreateDirectories();
            Axis1Box.SelectedIndex = 0;
            Axis2Box.SelectedIndex = 1;
            tabControl1.SelectedIndex = 2;
        }

        #region InitializeMethods
        private void CreateDirectories()
        {
            string path = Directory.GetCurrentDirectory();

            try
            {
                Directory.CreateDirectory(path + "\\Foundations");
                Directory.CreateDirectory(path + "\\Degrees");
                Directory.CreateDirectory(path + "\\Mods");
                Directory.CreateDirectory(path + "\\Results");
                UpdateDirectoryList();
                OperCheckList.SetItemChecked(0, true);
                OperCheckList.SetItemChecked(1, true);
                if (FoundDir.Items.Count != 0)
                {
                    FoundDir.SelectedIndex = 0;
                }
                if (DegreeDir.Items.Count != 0)
                {
                    DegreeDir.SelectedIndex = 0;
                }
                if (ModsDir.Items.Count != 0)
                {
                    ModsDir.SelectedIndex = 0;
                }
                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateDirectoryList()
        {
            try
            {
                string path = Directory.GetCurrentDirectory();
                string[] dirs = Directory.GetDirectories(path + "\\Degrees");
                DegreesList.Items.Clear();
                DegreeDir.Items.Clear();
                foreach (string s in dirs)
                {
                    string[] dirs2 = s.Split('\\');
                    DegreesList.Items.Add(dirs2[dirs2.Length - 1]);
                    DegreeDir.Items.Add(dirs2[dirs2.Length - 1]);
                }
                dirs = Directory.GetDirectories(path + "\\Foundations");
                FoundationsList.Items.Clear();
                FoundDir.Items.Clear();
                foreach (string s in dirs)
                {
                    string[] dirs2 = s.Split('\\');
                    FoundationsList.Items.Add(dirs2[dirs2.Length - 1]);
                    FoundDir.Items.Add(dirs2[dirs2.Length - 1]);
                }

                dirs = Directory.GetDirectories(path + "\\Mods");
                ModsList.Items.Clear();
                ModsDir.Items.Clear();
                foreach (string s in dirs)
                {
                    string[] dirs2 = s.Split('\\');
                    ModsList.Items.Add(dirs2[dirs2.Length - 1]);
                    ModsDir.Items.Add(dirs2[dirs2.Length - 1]);
                }
                dirs = Directory.GetDirectories(path + "\\Results");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region GenerationMethods
        private void GenFile(GenFunctions.random_num func, string dir, string len, string count, string type , string radix)
        {
            string path = Directory.GetCurrentDirectory();
            var lenght = GenFunctions.ReadString(len);
            try
            {
                var di = Directory.CreateDirectory($"{path}\\{dir}\\{type}{lenght[0]}_{lenght[lenght.Count - 1]}_{count}({radix})#{DateTime.Now.ToLocalTime().ToString().Replace(':', '-')}");
                FileStream fin;
                for (int j = 0; j < lenght.Count; j++)
                {
                    path = di.FullName + "\\" + lenght[j] + ".txt";

                    fin = new FileStream(path, FileMode.Create, FileAccess.Write);
                    // Open the stream and read it back.
                    using (StreamWriter sr = new StreamWriter(fin))
                    {
                        for (int i = 0; i < Int32.Parse(count); i++)
                        {
                            sr.WriteLine(func(lenght[j], radix));
                        }
                    }
                    fin.Close();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            UpdateDirectoryList();
        }

        private void GenFound_Click(object sender, EventArgs e)
        {
            GenFile(GenFunctions.random_max, "Foundations", FoundLenght.Text,FoundCount.Text, "Found",comboBox2.Text);
        }

        private void GenDegree_Click(object sender, EventArgs e)
        {
            GenFile(GenFunctions.random_max, "Degrees", DegreeLenght.Text, DegreeCount.Text, "Degree", comboBox3.Text);
        }

        private void GenMod_Click(object sender, EventArgs e)
        {
            switch (ModType.Text)
            {
                case "Degree of 2":
                    GenFile(GenFunctions.random_two, "Mods", ModLenght.Text, ModCount.Text, "Mod_Degree of two_", comboBox4.Text);
                    break;
                case "Odd number":
                    GenFile(GenFunctions.random_odd, "Mods",ModLenght.Text,ModCount.Text, "Mod_Odd number_", comboBox4.Text);
                    break;
                case "Prime number":
                    GenFile(GenFunctions.random_simple, "Mods", ModLenght.Text, ModCount.Text, "Mod_Prime number_", comboBox4.Text);
                    break;
                default:
                    GenFile(GenFunctions.random_max, "Mods", ModLenght.Text, ModCount.Text, "Mod_Random number_", comboBox4.Text);
                    break;
            }
            
        }
        #endregion

        #region CalculateMethods
        private void ResultsButton_Click(object sender, EventArgs e)
        {
            //ResultsButton.Enabled = false;
            AdditionalParameters.diapA = textAmax.Text;
            AdditionalParameters.diapB = textBmax.Text;
            
            if (Axis1Box.SelectedItem == Axis2Box.SelectedItem)
            {
                MessageBox.Show("You must select two different axis!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string choice = binAxis.Text;
            string choicew = $"{Axis1Box.SelectedItem} {Axis2Box.SelectedItem}";
            string choiceb = $"{binAxis_1.SelectedItem} {binAxis_2.SelectedItem} {binAxis_3.SelectedItem}";
            if (OperCheckList.CheckedIndices.Count > 0)
            {
                var p = new FuncParam { found = FoundDir.SelectedItem.ToString(), degree = DegreeDir.SelectedItem.ToString(),
                                        mod = ModsDir.SelectedItem.ToString(), choice = choice, choiceb = choiceb, choicew = choicew, winMode = WinMode.Text,
                                        winChecked = TableWith.Checked };
               (new Thread(GetResults)).Start(p);
            }
        }

        private void GetResults(object parameters)
        {
            FuncParam p = (parameters as FuncParam);
            AdditionalParameters.montFlag = aMontFlag.Checked;
            if(AdditionalParameters.montFlag)
            {
                AdditionalParameters.mul = MontgomeryMultDomain;
                AdditionalParameters.outRes = outMontgomeryDomain;
                AdditionalParameters.inRes = toMontgomeryDomain;
            }
            for (int i = 0; i < OperCheckList.CheckedIndices.Count; i++)
            {
                //DateTime.Now.ToLocalTime().ToString().Replace(':', '-')
                #region Binary
                if (OperCheckList.CheckedIndices[i] == 0)  { (new    BinaryRL(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 1)  { (new    BinaryLR(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 6)  { (new NAFBinaryRL(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 7)  { (new NAFBinaryLR(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 14) { (new    AddSubRL(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 15) { (new    AddSubLR(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 16) { (new Joye_double_and_add(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 17) { (new    MontgomeryLadder(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 18) { (new DBNS1RL(p.found, p.degree, p.mod, p.choiceb, true, AdditionalParameters.diapA, AdditionalParameters.diapB)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 19) { (new DBNS1RL(p.found, p.degree, p.mod, p.choiceb, false, AdditionalParameters.diapA, AdditionalParameters.diapB)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 20) { (new DBNS1LR(p.found, p.degree, p.mod, p.choiceb, true, AdditionalParameters.diapA, AdditionalParameters.diapB)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 21) { (new DBNS1LR(p.found, p.degree, p.mod, p.choiceb, false, AdditionalParameters.diapA, AdditionalParameters.diapB)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 22) { (new DBNS2RL(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 23) { (new DBNS2RL(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 24) { (new Bonus1(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 25) { (new Bonus2(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 26) { (new SimpleMul(p.found, p.degree, p.mod, p.choice)).Create_Result(); continue; }
                #endregion

                #region Window
                if (OperCheckList.CheckedIndices[i] == 2) { (new WindowRL(p.found, p.degree, p.mod, p.choicew, p.winMode, p.winChecked)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 3) { (new WindowLR(p.found, p.degree, p.mod, p.choicew, p.winMode, p.winChecked)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 4) { (new  SlideRL(p.found, p.degree, p.mod, p.choicew, p.winMode, p.winChecked)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 5) { (new  SlideRL(p.found, p.degree, p.mod, p.choicew, p.winMode, p.winChecked)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 8) { (new NAFSlideRL(p.found, p.degree, p.mod, p.choicew, p.winMode, p.winChecked)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 9) { (new NAFSlideLR(p.found, p.degree, p.mod, p.choicew, p.winMode, p.winChecked)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 10) { (new NAFWindowRL(p.found, p.degree, p.mod, p.choicew, p.winMode, p.winChecked)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 11) { (new NAFWindowLR(p.found, p.degree, p.mod, p.choicew, p.winMode, p.winChecked)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 12) { (new wNAFSlideRL(p.found, p.degree, p.mod, p.choicew, p.winMode, p.winChecked)).Create_Result(); continue; }
                if (OperCheckList.CheckedIndices[i] == 13) { (new wNAFSlideLR(p.found, p.degree, p.mod, p.choicew, p.winMode, p.winChecked)).Create_Result(); continue; }
                #endregion

            }
            
            string path = Directory.GetCurrentDirectory();
            string[] dirs = Directory.GetDirectories(path + "\\Results");
            var result = MessageBox.Show("All done", "Result",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Information);
            
        }

        private void Calculatebutton_Click(object sender, EventArgs e)
        {
            AdditionalParameters.A = Int64.Parse(textA.Text);
            AdditionalParameters.B = Int64.Parse(textB.Text);

            BigInteger mod = BigInteger.Parse(modText.Text);
            BigInteger pow = BigInteger.Parse(PowerText.Text);
            int window = Int32.Parse(WindowText.Text);
            BigInteger num = BigInteger.Parse(NumberText.Text);
            List<string> s = new List<string>();
            double table = 0;
            OperationsResult.Items.Clear();

            if (OperationsList.CheckedIndices.Count > 0)
                Show(num, pow, mod, window, table);
        }
        #endregion
        
        #region CheckButtons
        private void CheckAllbutton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < OperationsList.Items.Count; i++)
            {
                 OperationsList.SetItemChecked(i, true); 
            }
        }

        private void CheckNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < OperCheckList.Items.Count; i++)
            {
                OperCheckList.SetItemChecked(i, false);
            }
        }

        private void CheckNonebutton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < OperationsList.Items.Count; i++)
            {
                OperationsList.SetItemChecked(i, false);

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < OperCheckList.Items.Count; i++)
            {
                OperCheckList.SetItemChecked(i, true);
            }
        }

        #endregion

        #region OpenDirectories
        private void ModsDir_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(@"~\bin\Debug\Mods");
        }

        private void DegreeDir_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(@"~\bin\Debug\Degrees");
        }

        private void FoundDir_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(@"~\bin\Debug\Foudations");
        }

        #endregion

        private void TableWith_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

    public class FuncParam
    {
        public string found;
        public string degree;
        public string mod;
        public string choice;
        public string choicew;
        public string choiceb;
        public string winMode;
        public bool winChecked;
    }

    public static class AdditionalParameters
    {
        public static long A = 15;
        public static long B = 17;

        public static string diapA;
        public static string diapB;
        public static bool montFlag;
        public static OutRes outRes = (res, mod, inverse) => res; 
        public static Multiply mul = (x, y, z, inv) => (x * y) % z;
        public static InRes inRes = (ref BigInteger a, ref BigInteger b, BigInteger m) => 0;
        //public static Inverse inv = PowFunctions.Euclid_2_1;
    }
}
