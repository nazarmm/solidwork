using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SldWorks;
using SwConst;

namespace Lab2 // Зверніть увагу, щоб назва (namespace) збігалася з вашою
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SldWorks.SldWorks sldApp;
            try
            {
                sldApp = (SldWorks.SldWorks)Marshal.GetActiveObject("SldWorks.Application");
            }
            catch
            {
                MessageBox.Show("Запустіть SolidWorks та відкрийте деталь!");
                return;
            }

            ModelDoc2 swDoc = (ModelDoc2)sldApp.ActiveDoc;
            if (swDoc == null)
            {
                MessageBox.Show("Відкрийте файл деталі у SolidWorks!");
                return;
            }

            // Спеціальний прапорець, щоб знати, чи ми взагалі щось змінили
            bool modelChanged = false;

            // 1. ПЕРЕВІРЯЄМО ТІЛЬКИ ПЕРШЕ ПОЛЕ (Довжина)
            // Якщо поле не порожнє і там правильне число більше нуля:
            if (double.TryParse(textBox1.Text, out double length) && length > 0)
            {
                Dimension dimLength = (Dimension)swDoc.Parameter("L_Base@Sketch1");
                if (dimLength != null)
                {
                    dimLength.SystemValue = length / 1000.0;
                    modelChanged = true; // Фіксуємо, що зміна відбулася
                }
            }

            // 2. ПЕРЕВІРЯЄМО ТІЛЬКИ ДРУГЕ ПОЛЕ (Висота)
            if (double.TryParse(textBox2.Text, out double height) && height > 0)
            {
                Dimension dimHeight = (Dimension)swDoc.Parameter("H_Cylinders@Boss-Extrude2");
                if (dimHeight != null)
                {
                    dimHeight.SystemValue = height / 1000.0;
                    modelChanged = true;
                }
            }

            // 3. ПЕРЕВІРЯЄМО ТІЛЬКИ ТРЕТЄ ПОЛЕ (Товщина)
            if (double.TryParse(textBox3.Text, out double thickness) && thickness > 0)
            {
                Dimension dimThickness = (Dimension)swDoc.Parameter("T_Base@Boss-Extrude1");
                if (dimThickness != null)
                {
                    dimThickness.SystemValue = thickness / 1000.0;
                    modelChanged = true;
                }
            }

            // Якщо ми змінили хоча б ОДИН параметр, перебудовуємо деталь
            if (modelChanged)
            {
                swDoc.EditRebuild3(); // Команда перебудови з методички
                swDoc.ShowNamedView2("*Isometric", 7);
                swDoc.ViewZoomtofit2();
                MessageBox.Show("Деталь успішно оновлено!");
            }
            else
            {
                // Якщо всі поля порожні або там введено текст/нулі
                MessageBox.Show("Введіть правильне число хоча б в одне поле!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SldWorks.SldWorks sldApp;
            try
            {
                sldApp = (SldWorks.SldWorks)Marshal.GetActiveObject("SldWorks.Application");
            }
            catch
            {
                MessageBox.Show("Помилка: SolidWorks не запущено!");
                return;
            }

            ModelDoc2 swDoc = (ModelDoc2)sldApp.ActiveDoc;
            if (swDoc == null)
            {
                MessageBox.Show("Відкрийте файл деталі у SolidWorks!");
                return;
            }

            try
            {
                // 1. Знаходимо розміри в SolidWorks
                Dimension dimLength = (Dimension)swDoc.Parameter("L_Base@Sketch1");
                Dimension dimHeight = (Dimension)swDoc.Parameter("H_Cylinders@Boss-Extrude2");
                Dimension dimThickness = (Dimension)swDoc.Parameter("T_Base@Boss-Extrude1");

                // 2. Жорстко задаємо оригінальні розміри Варіанту 6 (ділимо на 1000 для метрів)
                if (dimLength != null) dimLength.SystemValue = 70.0 / 1000.0;
                if (dimHeight != null) dimHeight.SystemValue = 25.0 / 1000.0;
                if (dimThickness != null) dimThickness.SystemValue = 11.0 / 1000.0;

                // 3. Перебудовуємо 3D-модель
                swDoc.EditRebuild3();
                swDoc.ShowNamedView2("*Isometric", 7);
                swDoc.ViewZoomtofit2();

                // 4. Оновлюємо текстові поля на формі, щоб вони відображали скинуті значення
                textBox1.Text = "70";
                textBox2.Text = "25";
                textBox3.Text = "11";

                MessageBox.Show("Відлагодження: Деталь успішно скинуто до початкових розмірів (70, 25, 11)!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка під час скидання: " + ex.Message);
            }
        }
    }
}