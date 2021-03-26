using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper;
using Grasshopper.Kernel;

using Grasshopper.Kernel.Special;


//https://www.grasshopper3d.com/forum/topics/is-it-possible-to-call-the-functions-of-a-grasshopper-component


namespace MyFirstComponent
{

    /* 
     * -------------------- DataStructures -----------------
    */

    public struct ComponentHolder {
        
        public ComponentHolder(string name, Guid guid, List<ComponentParam> inputs, List<ComponentParam> outputs) {
            Name = name;
            Guid = guid;
            Inputs = inputs;
            Outputs = outputs;
        }
        
        public string Name { get; }
        public Guid Guid { get; }
        public List<ComponentParam> Inputs { get; }
        public List<ComponentParam> Outputs { get; }

        public override string ToString() {
            string str = $"{Name}: \n \t guid:{Guid}\n \t \t Inputs: ";
            foreach (ComponentParam par in Inputs) {
                str += "\n \t \t \t" +par.ToString();
            }
            str += "\n \t \t Outputs: ";
            foreach (ComponentParam par in Outputs) {
                str += "\n \t \t \t" + par.ToString();
            }
            return str;
        }

    }

    //This struct holds the params affecting a component (it's inputs and outputs)
    public struct ComponentParam
    {
        public ComponentParam(string connectionPointNickname, string connectedComponentName, Guid connectedComponentGuid) {
            ConnectionPointNickname = connectionPointNickname;
            ConnectedComponentName = connectedComponentName;
            ConnectedComponentGuid = connectedComponentGuid;
        }

        public string ConnectionPointNickname { get; }

        public string ConnectedComponentName { get; }
        public Guid ConnectedComponentGuid { get; }
        //find way of also adding data which is passed 


        public override string ToString() => $"{ConnectionPointNickname}: {ConnectedComponentName} -> DATA";

    }
    

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
            
            List<ComponentHolder> componentsDescription = new List<ComponentHolder>();
            
            //int len = ghdoc.ActiveObjects().Count();
            
            //This gets the components but misses the "params" (eg button, ...)
            foreach (var element in ghdoc.Objects.OfType<GH_Component>()) {
                List<ComponentParam> inputParams = new List<ComponentParam>();
                List<ComponentParam> outputParams = new List<ComponentParam>();
                

                //Reading all values inputed from the component ----------------------------------------
                foreach (IGH_Param inputGhParam in element.Params.Input) {
                   
                    foreach (var sourceComponent in inputGhParam.Sources) {
                        //sourceComponent.VolatileData.ToString();  //TODO figure out how to get data being passed
                        inputParams.Add(new ComponentParam(inputGhParam.NickName, sourceComponent.Name, sourceComponent.InstanceGuid));
                    }
                }
                
                //Reading all values which the component outputs ----------------------------------------
                foreach (IGH_Param outputGhParam in element.Params.Output) {

                    foreach (var sourceComponent in outputGhParam.Recipients) {
                        //sourceComponent.VolatileData.ToString();  //TODO figure out how to get data being passed
                        outputParams.Add(new ComponentParam(outputGhParam.NickName, sourceComponent.Name, sourceComponent.InstanceGuid));
                    }
                }

                ComponentHolder currentComponent = new ComponentHolder(element.Name, element.InstanceGuid, inputParams, outputParams);
                componentsDescription.Add(currentComponent);
            }
            

            //-------
            DA.SetDataList(0, componentsDescription);
        }

        

    }


}
