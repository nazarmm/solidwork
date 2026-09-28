using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SldWorks;
using SwConst;

namespace Lab3
{
    public partial class Form1 : Form
    {
        SldWorks.SldWorks sldApp;
        ModelDoc2 swDoc;

        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Plain Carbon Steel"); // Вуглецева сталь
            comboBox1.Items.Add("Cast Alloy Steel");   // Легована сталь
            comboBox1.Items.Add("Ductile Iron");       // Ковкий чавун (як у прикладі методички)
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }

        // КНОПКА 1: Зміна розмірів та призначення матеріалу
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                sldApp = (SldWorks.SldWorks)Marshal.GetActiveObject("SldWorks.Application");
                swDoc = (ModelDoc2)sldApp.ActiveDoc;

                if (swDoc == null)
                {
                    MessageBox.Show("Відкрийте деталь Lab3.SLDPRT у SolidWorks!");
                    return;
                }

                // 1. Зміна внутрішнього діаметра (D_Inner)
                if (double.TryParse(textBox1.Text, out double dInner) && dInner > 0)
                {
                    Dimension dimInner = (Dimension)swDoc.Parameter("D_Inner@Sketch1");
                    if (dimInner != null) dimInner.SystemValue = dInner / 1000.0;
                }

                // 2. Зміна зовнішнього діаметра (D_Outer)
                if (double.TryParse(textBox2.Text, out double dOuter) && dOuter > 0)
                {
                    Dimension dimOuter = (Dimension)swDoc.Parameter("D_Outer@Sketch1");
                    if (dimOuter != null) dimOuter.SystemValue = dOuter / 1000.0;
                }

                // 3. Зміна матеріалу деталі
                string material = comboBox1.SelectedItem.ToString();
                PartDoc swPart = (PartDoc)swDoc;

                // Шлях до англомовної бібліотеки матеріалів SolidWorks 
                string matDbPath = @"C:\Program Files\SolidWorks Corp\SolidWorks\lang\english\sldmaterials\solidworks materials.sldmat";

                // Застосування матеріалу згідно з синтаксисом методички
                swPart.SetMaterialPropertyName2("Default", matDbPath, material);

                // Оновлення моделі
                swDoc.EditRebuild3();
                swDoc.ShowNamedView2("*Isometric", 7);
                swDoc.ViewZoomtofit2();

                MessageBox.Show($"Оновлено! Матеріал встановлено: {material}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }

        // КНОПКА 2: Отримання масових характеристик та центру мас
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                sldApp = (SldWorks.SldWorks)Marshal.GetActiveObject("SldWorks.Application");
                swDoc = (ModelDoc2)sldApp.ActiveDoc;

                if (swDoc == null) return;

                // 1. Встановлення мітки центру мас на 3D-моделі[cite: 14]
                Feature CenterOfMass = (Feature)swDoc.FeatureManager.InsertCenterOfMass();

                // 2. Виділення базового елемента (Boss-Extrude1) для розрахунку маси
                bool status = swDoc.Extension.SelectByID2("Boss-Extrude1", "BODYFEATURE", 0, 0, 0, false, 0, null, 0);

                // 3. Виклик стандартного вікна масових характеристик SolidWorks
                swDoc.ToolsMassProps();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                sldApp = (SldWorks.SldWorks)Marshal.GetActiveObject("SldWorks.Application");
                swDoc = (ModelDoc2)sldApp.ActiveDoc;

                if (swDoc == null)
                {
                    MessageBox.Show("Відкрийте деталь у SolidWorks!");
                    return;
                }

                // 1. Примусове повернення розмірів Варіанту 6 (ділимо на 1000 для метрів)
                Dimension dimInner = (Dimension)swDoc.Parameter("D_Inner@Sketch1");
                Dimension dimOuter = (Dimension)swDoc.Parameter("D_Outer@Sketch1");

                if (dimInner != null) dimInner.SystemValue = 60.0 / 1000.0;
                if (dimOuter != null) dimOuter.SystemValue = 80.0 / 1000.0;

                // 2. Повернення стандартного матеріалу (беремо перший зі списку)
                PartDoc swPart = (PartDoc)swDoc;
                string matDbPath = @"C:\Program Files\SolidWorks Corp\SolidWorks\lang\english\sldmaterials\solidworks materials.sldmat";
                string defaultMaterial = "Plain Carbon Steel";
                swPart.SetMaterialPropertyName2("Default", matDbPath, defaultMaterial);

                // 3. Перебудова 3D-моделі
                swDoc.EditRebuild3();
                swDoc.ShowNamedView2("*Isometric", 7);
                swDoc.ViewZoomtofit2();

                // 4. Оновлення інтерфейсу форми, щоб цифри збігалися з моделлю
                textBox1.Text = "60";
                textBox2.Text = "80";
                comboBox1.SelectedIndex = 0; // Повертає випадаючий список на першу позицію

                MessageBox.Show("Відлагодження: Деталь успішно скинуто до розмірів 60 та 80!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка під час скидання: " + ex.Message);
            }
        }
    }
}