using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Dispatcher.Models
{
    public class DispatchRequestType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool ForSelf { get; set; }

        [JsonIgnore]
        public byte[] ValidRequestersBytes { get; set; }

        [NotMapped]
        public string[] ValidRequesters
        {
            get
            {
                if (ValidRequestersBytes == null)
                {
                    return null;
                }

                var ms = new MemoryStream(ValidRequestersBytes);
                BinaryFormatter bf = new BinaryFormatter();
                return (string[])bf.Deserialize(ms);
            }
            set
            {
                if (value == null)
                {
                    ValidRequestersBytes = null;
                    return;
                }

                var ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, value);
                ValidRequestersBytes = ms.ToArray();
            }
        }
    }
}