////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace GpsMapRoutes.models
{
    /// <summary>
    /// Труба/Маршрут
    /// </summary>
    public class PipelineModel : AbstractModel
    {
        /// <summary>
        /// Точки, связаные с текущим маршрутом
        /// </summary>
        public List<SensorModel> Sensors { get; set; }

        /// <summary>
        /// Заголовок маршрута
        /// </summary>
        public string Title { get => "Маршрут #" + Id; }
    }
}
