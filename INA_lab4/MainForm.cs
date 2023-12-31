namespace INA_lab4
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            btnTests.Enabled = false;
        }
        private int _t { get; set; }
        private double _a { get; set; }
        private double _b { get; set; }
        private double _d { get; set; }

        private void btnTests_Click(object sender, EventArgs e)
        {
            var form = new TestForm(tbBestXreal.Text, tbBestXbin.Text, tbBestFxReal.Text, _a, _b, _d);
            btnTests.Enabled = false;
            form.Show();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            btnTests.Enabled = true;
            _t = Convert.ToInt32(tbT.Text);
            _a = Convert.ToDouble(tbA.Text);
            _b = Convert.ToDouble(tbB.Text);
            _d = Convert.ToDouble(cbD.Text);
            var newList = Calculate();
            dataGridView1.Rows.Clear();
            SetTable(newList);
            SetBest(newList);
        }

        private void SetBest(List<string> newList)
        {
            tbBestXreal.Text = dataGridView1.Rows[0].Cells[0].Value.ToString();
            tbBestXbin.Text = dataGridView1.Rows[0].Cells[1].Value.ToString();
            tbBestFxReal.Text = dataGridView1.Rows[0].Cells[2].Value.ToString();
        }
        
        private List<string> Calculate()
        {
            List<string> newObj = new List<string>();
            formsPlot1.Plot.Clear();
            double[] mainLine = new double[_t];
            double[] index = new double[_t];
            for (int k = 0; k < index.Length; k++)
            {
                index[k] += k;
            }
            for (int i = 0; i < _t; i++)
            {
                MainObject obj = new MainObject(_a, _b, _d);
                obj._xBin = RandBin.GetValue(_a, _b, _d);
                var listOfobj = obj.GetDescendants();
                double[] allXbin = new double[listOfobj.Count];
                
                for (int j = 0; j < listOfobj.Count; j++)
                {
                    string str = listOfobj[j];
                    
                    if (GetFx(GetxReal(obj._xBin)) < GetFx(GetxReal(str)))
                    {
                        obj._xBin = str;
                    }
                    allXbin[j] = GetFx(GetxReal(obj._xBin));
                }

                mainLine[i] = GetFx(GetxReal(obj._xBin));
                if (i > 0 && mainLine[i] < mainLine[i - 1])
                {
                    mainLine[i] = mainLine[i - 1];
                }

                formsPlot1.Plot.AddLine(i, allXbin.Min(), i, allXbin.Max(), color: Color.Blue);
                newObj.Add(obj._xBin);

            }
            formsPlot1.Plot.AddScatter(index, mainLine, color: Color.Red, lineWidth: 3);
            formsPlot1.Plot.AxisAuto();
            formsPlot1.Render();
            return newObj;
        }

        private void SetTable(List<string> list)
        {
            string filePath = "output_table.txt";

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                Console.SetOut(sw);

                Console.WriteLine("xReal\t\t xBin\t\t FxReal");
                foreach (string obj in list)
                {
                    Console.WriteLine($"{GetxReal(obj)}\t\t {obj}\t\t {GetFx(GetxReal(obj))}");
                }

                Console.SetOut(Console.Out);
            }

            foreach (string obj in list)
            {
                dataGridView1.Rows.Add(GetxReal(obj), obj, GetFx(GetxReal(obj)));
            }

            dataGridView1.Sort(dataGridView1.Columns["FxReal"], System.ComponentModel.ListSortDirection.Descending);
        }

        internal double GetxReal(string xBin)
        {
            int l = (int)Math.Floor(Math.Log((_b - _a) / _d, 2) + 1.0);
            long xInt_xBin = Convert.ToInt64(xBin, 2);
            return ((_b - _a) * xInt_xBin) / (Math.Pow(2.0, l) - 1.0) + _a;
        }

        internal double GetFx(double xReal)
        {
            return (xReal % 1.0) * (Math.Cos(20.0 + Math.PI + xReal)) - Math.Sin(xReal);
        }
    }
}