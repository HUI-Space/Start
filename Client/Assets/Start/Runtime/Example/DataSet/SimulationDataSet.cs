namespace Start
{
    public class SimulationDataSet_1 : DataSetBase
    {
        public int Id = 10086;
        
        public int Age = 18;

        public string Name = "Jack";
        
        
        public override void Register()
        {
            RegisterNetMessage<Simulation_ToC>(1,1,OnSimulationCallback);
        }

        public override void UnRegister()
        {
            UnRegisterNetMessage<Simulation_ToC>(1,1,OnSimulationCallback);
        }

        private void OnSimulationCallback(uint code, Simulation_ToC msg)
        {
            Id = msg.Id;
            Age = msg.Age;
            Name = msg.Name;
        }
    }
    
    
    public class SimulationDataSet_2 : DataSetBase
    {
        public int Id = 100001;
        
        public int Age = 28;

        public string Name = "Mick";
        
        
        public override void Register()
        {
            RegisterNetMessage<Simulation_ToC>(1,1,OnSimulationCallback);
        }

        public override void UnRegister()
        {
            UnRegisterNetMessage<Simulation_ToC>(1,1,OnSimulationCallback);
        }

        private void OnSimulationCallback(uint code, Simulation_ToC msg)
        {
            Id = msg.Id;
            Age = msg.Age;
            Name = msg.Name;
        }
        
    }

    public class Simulation_ToC
    {
        public int Id;
        public int Age;
        public string Name;
    }
}