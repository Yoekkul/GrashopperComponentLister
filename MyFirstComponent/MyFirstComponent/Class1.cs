using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper;
using Grasshopper.Kernel;

using Grasshopper.Kernel.Special;


//https://www.grasshopper3d.com/forum/topics/is-it-possible-to-call-the-functions-of-a-grasshopper-component


namespace MyFirstComponent
{
    

    public class Class1 : GH_Component
    {

        //GH_Document ghenv = new Grasshopper.Kernel.GH_Document();
        GH_Document ghdoc;

        public Class1(): base("MyFirst", "MFC", "My first component", "Extra", "Simple") {
            
        }

        public override Guid ComponentGuid {
            get { return new Guid("419c3a3a-cc48-4717-8cef-5f5647a5ecfc"); } //This is an invented value
        }

        

        protected override void RegisterInputParams(GH_InputParamManager pManager) {
            pManager.AddTextParameter("String", "S", "Satisfy the machine", GH_ParamAccess.item);
            
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Output", "O", "Outputted data", GH_ParamAccess.list);
        }


        //SolveInstance only executes when it gets new input
        protected override void SolveInstance(IGH_DataAccess DA) {
            ghdoc = Instances.ActiveCanvas.Document;
            //ghenv.Component.Params.Input[];


            List<string> componentNames = new List<string>();
            List<string> connections = new List<string>();
            
            
            /*
            foreach(var attr in ghdoc.Attributes) {

                componentNames.Add(attr.ToString());

                //GH_Component data = attr.GetTopLevel.DocObject;

            
                
            }
            */

            
            //ghdoc.Objects.GetEnumerator();
            int len = ghdoc.ActiveObjects().Count();
            
            
            //This gets the components but misses the "params" (eg button, ...)
            foreach (var element in ghdoc.Objects.OfType<GH_Component>()) {       //GH_Component in            IGH_ActiveObject != GH_Component
                componentNames.Add(element.Name);
                
                
                
                //We only need to check inputs since we have a DAG (!exception are "source" components)
                foreach (IGH_Param ghParam in element.Params.Input) {  //IGH_Param in
                                                                    //ghparam.Attributes.GetTopLevel.DocObject  //<- check what this returns

                    

                    String connection = ghParam.NickName+ ": [";
                    foreach (var sourceComponent in ghParam.Sources) {
                        //sourceComponent.VolatileData.ToString;
                        //sourceComponent.InstanceGuid
                        connection += sourceComponent.Name + ", "+ sourceComponent.InstanceGuid;
                    }
                    connection += "]";
                    connections.Add(connection);
                }
                
                
            }
            

            //-------
            //DA.SetDataList(0, componentNames);
            DA.SetDataList(0, connections);
        }

        

    }


}
