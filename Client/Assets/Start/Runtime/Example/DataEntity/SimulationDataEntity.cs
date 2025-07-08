namespace Start
{
    public class SimulationDataEntity_1 : DataEntity
    {
        public int Id = 10086;
        
        public int Age = 18;

        public string Name = "Jack";
        
        
        public override void Initialize()
        {
            RegisterNetUpdate(DataSetManager.Instance.GetDataSet<SimulationDataSet_1>(),Sync);
        }

        private void Sync(SimulationDataSet_1 simulationDataSet)
        {
            Id = simulationDataSet.Id;
            Age = simulationDataSet.Age;
            Name = simulationDataSet.Name;
        }
    }

    public class SimulationDataEntityCollection_1 : DataEntityCollection<SimulationDataEntity_1>
    {
        
    }
    
    public class SimulationDataEntity_2 : DataEntity<int>
    {
        public int Id = 10086111;
        
        public int Age = 18111;

        public string Name = "Jacksssss";
        
        public override void Initialize()
        {
            RegisterNetUpdate(DataSetManager.Instance.GetDataSet<SimulationDataSet_2>(),Sync);
        }

        private void Sync(SimulationDataSet_2 simulationDataSet)
        {
            Id = simulationDataSet.Id;
            Age = simulationDataSet.Age;
            Name = simulationDataSet.Name;
        }
    }
    
    public class SimulationDataEntityCollection_2 : DataEntityCollection<int,SimulationDataEntity_2>
    {
        protected override bool HasKey(int key)
        {
            return true;
        }
    }
}