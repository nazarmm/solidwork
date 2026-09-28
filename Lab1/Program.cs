using System;
using System.Runtime.InteropServices;
using SldWorks;
using SwConst;

namespace SolidWorksLab
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Побудова фінальної деталі: 1 база + 5 вирізів...");

            ISldWorks swApp;
            try
            {
                swApp = (ISldWorks)Marshal.GetActiveObject("SldWorks.Application");
            }
            catch
            {
                Console.WriteLine("Помилка: Запустіть SolidWorks!");
                Console.ReadLine();
                return;
            }

            // 1. СТВОРЕННЯ ДОКУМЕНТА
            string defaultTemplate = swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
            ModelDoc2 swDoc = (ModelDoc2)swApp.NewDocument(defaultTemplate, 0, 0, 0);

            if (swDoc == null) return;

            void SelectFrontPlane()
            {
                swDoc.ClearSelection2(true);
                if (swDoc.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0)) return;
                if (swDoc.Extension.SelectByID2("Спереду", "PLANE", 0, 0, 0, false, 0, null, 0)) return;
                swDoc.Extension.SelectByID2("Плоскость спереди", "PLANE", 0, 0, 0, false, 0, null, 0);
            }

            void SelectRightPlane()
            {
                swDoc.ClearSelection2(true);
                if (swDoc.Extension.SelectByID2("Right Plane", "PLANE", 0, 0, 0, false, 0, null, 0)) return;
                if (swDoc.Extension.SelectByID2("Справа", "PLANE", 0, 0, 0, false, 0, null, 0)) return;
                swDoc.Extension.SelectByID2("Плоскость справа", "PLANE", 0, 0, 0, false, 0, null, 0);
            }

            // КРОК 1: Будуємо Т-профіль спереду
            SelectFrontPlane();
            swDoc.SketchManager.InsertSketch(true);
            swDoc.SketchManager.CreateLine(-0.0175, 0, 0, -0.0175, -0.010, 0);
            swDoc.SketchManager.CreateLine(-0.0175, -0.010, 0, 0.0325, -0.010, 0);
            swDoc.SketchManager.CreateLine(0.0325, -0.010, 0, 0.0325, 0, 0);
            swDoc.SketchManager.CreateLine(0.0325, 0, 0, 0.015, 0, 0);
            swDoc.SketchManager.CreateLine(0.015, 0, 0, 0.015, 0.025, 0);
            swDoc.SketchManager.CreateLine(0.015, 0.025, 0, 0, 0.025, 0);
            swDoc.SketchManager.CreateLine(0, 0.025, 0, 0, 0, 0);
            swDoc.SketchManager.CreateLine(0, 0, 0, -0.0175, 0, 0);
            swDoc.FeatureManager.FeatureExtrusion2(true, false, false, 0, 0, 0.080, 0.01, false, false, false, false, 0, 0, false, false, false, false, true, true, true, 0, 0, false);
            swDoc.ClearSelection2(true);

            // КРОК 2: ВИРІЗ 1 (Верхня траншея)
            SelectRightPlane();
            swDoc.SketchManager.InsertSketch(true);
            swDoc.SketchManager.CreateCornerRectangle(-0.010, 0.025, 0, -0.070, 0.015, 0);
            swDoc.FeatureManager.FeatureCut4(false, false, false, 9, 1, 0.1, 0.1, false, false, false, false, 0, 0, false, false, false, false, false, true, true, true, true, false, 0, 0, false, false);
            swDoc.ClearSelection2(true);

            // КРОК 3: ВИРІЗ 2 (Ліва нижня ніжка)
            SelectRightPlane();
            swDoc.SketchManager.InsertSketch(true);
            swDoc.SketchManager.CreateCornerRectangle(-0.070, -0.010, 0, -0.080, -0.005, 0);
            swDoc.FeatureManager.FeatureCut4(false, false, false, 9, 1, 0.1, 0.1, false, false, false, false, 0, 0, false, false, false, false, false, true, true, true, true, false, 0, 0, false, false);
            swDoc.ClearSelection2(true);

            // КРОК 4: ВИРІЗ 3 (Права нижня ніжка)
            SelectRightPlane();
            swDoc.SketchManager.InsertSketch(true);
            swDoc.SketchManager.CreateCornerRectangle(0, -0.005, 0, -0.010, -0.010, 0);
            swDoc.FeatureManager.FeatureCut4(false, false, false, 9, 1, 0.1, 0.1, false, false, false, false, 0, 0, false, false, false, false, false, true, true, true, true, false, 0, 0, false, false);
            swDoc.ClearSelection2(true);

            // КРОК 5: ВИРІЗ 4 (Верхній центральний паз)
            SelectRightPlane();
            swDoc.SketchManager.InsertSketch(true);
            swDoc.SketchManager.CreateCornerRectangle(-0.035, 0.010, 0, -0.045, 0.015, 0);
            swDoc.FeatureManager.FeatureCut4(false, false, false, 9, 1, 0.1, 0.1, false, false, false, false, 0, 0, false, false, false, false, false, true, true, true, true, false, 0, 0, false, false);
            swDoc.ClearSelection2(true);

            // КРОК 6: ВИРІЗ 5 (Нижній центральний паз)
            SelectRightPlane();
            swDoc.SketchManager.InsertSketch(true);
            swDoc.SketchManager.CreateCornerRectangle(-0.035, -0.005, 0, -0.045, -0.010, 0);
            swDoc.FeatureManager.FeatureCut4(false, false, false, 9, 1, 0.1, 0.1, false, false, false, false, 0, 0, false, false, false, false, false, true, true, true, true, false, 0, 0, false, false);
            swDoc.ClearSelection2(true);

            // ФІНАЛ
            swDoc.ShowNamedView2("*Isometric", 7);
            swDoc.ViewZoomtofit2();

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}