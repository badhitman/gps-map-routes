using System.Collections.Generic;

namespace WpfApp.models
{
    public class PipelineModel : AbstractModel
    {
        public List<SensorModel> Sensors { get; set; }

        public string Title { get => "Трубопровод #" + Id; }
    }
}
