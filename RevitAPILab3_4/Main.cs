using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPILab3_4
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var selectedRef = uidoc.Selection.PickObject(ObjectType.Element, "Выберите элемент");
            var selectedElement = doc.GetElement(selectedRef);
            if (selectedElement is FamilyInstance)
            {
                using (Transaction ts = new Transaction(doc, "Set parameters"))
                {
                    ts.Start();
                    var familyInstance = selectedElement as FamilyInstance;

                    var pipeOutsideDiameter = familyInstance.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble();
                    pipeOutsideDiameter = UnitUtils.ConvertFromInternalUnits(pipeOutsideDiameter, DisplayUnitType.DUT_MILLIMETERS);

                    var pipeInsideDiameter = familyInstance.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM).AsDouble();
                    pipeInsideDiameter = UnitUtils.ConvertFromInternalUnits(pipeInsideDiameter, DisplayUnitType.DUT_MILLIMETERS);

                    Parameter nameParameter = familyInstance.LookupParameter("Наменование");
                    nameParameter.Set($"Труба {Math.Round(pipeOutsideDiameter, 1)} мм / {Math.Round(pipeInsideDiameter, 1)} мм");
                    ts.Commit();
                }                   
            }
          
            return Result.Succeeded;
        }
    }
}
