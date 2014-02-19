using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdkMapControlWP8CS.ViewModels
{
    public class SoundData
    {
    
            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }
            [JsonProperty(PropertyName = "filepath")]
            public string FilePath { get; set; }
            [JsonProperty(PropertyName = "description")]
            public string Description { get; set; }
            [JsonProperty(PropertyName = "containername")]
            public string ContainerName { get; set; }
            [JsonProperty(PropertyName = "resourcename")]
            public string ResourceName { get; set; }
            [JsonProperty(PropertyName = "sasQueryString")]
            public string SasQueryString { get; set; }
            [JsonProperty(PropertyName = "sound")]
            public string Sound { get; set; }
            [JsonProperty(PropertyName = "latitude")]
            public double Latitude { get; set; }
            [JsonProperty(PropertyName = "longitude")]
            public double Longitude { get; set; }
       
    }
}