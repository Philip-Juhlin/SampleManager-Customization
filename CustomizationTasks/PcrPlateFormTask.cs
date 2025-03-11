using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thermo.SampleManager.Common.Data;
using Thermo.SampleManager.Library;
using Thermo.SampleManager.Library.EntityDefinition;
using Thermo.SampleManager.ObjectModel;
using Thermo.SampleManager.Library.FormDefinition;
using Thermo.SampleManager.Tasks;

namespace Customization.Tasks
{
    [SampleManagerTask("PcrPlateFormTask","DASHBOARD", "plateEvaluationDashboard")]
    public class PcrPlateFormTask : DefaultFormTask
    {
        protected override void MainFormLoaded()
        {
            base.MainFormLoaded();
            
            var q = EntityManager.CreateQuery<Plate>();
            q.AddEquals(PlatePropertyNames.PcrPlateStage, "1");
            q.AddAnd();
            q.AddLessThan(PlatePropertyNames.PcrStarted, DateTime.Now);
            
            
            IEntityCollection plates = EntityManager.Select(q);
         

            foreach (var plate in plates)
            {
                
                Plate plate1 = plate as Plate;
                plate1.PerformAction("PLATEFORMCHECK");
                
            }

            EntityManager.Commit();

        }


    }

}
