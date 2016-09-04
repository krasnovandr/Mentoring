using System.IO;
using System.Xml.Serialization;
using ServiceBusClient;

namespace MainService
{
    public static class ServiceSettings
    {
        public const string SettingsFilePath = "../../Settings.xml";

        public static WorkerServiceSettings ReadSettings()
        {
            var reader = new XmlSerializer(typeof(WorkerServiceSettings));
            var file = new StreamReader(SettingsFilePath);
            var overview = (WorkerServiceSettings)reader.Deserialize(file);

            file.Close();

            return overview;
        }

        public static void WriteSettings(WorkerServiceSettings settings)
        {
            var writer = new XmlSerializer(typeof(WorkerServiceSettings));

            FileStream file = File.Open(SettingsFilePath, FileMode.OpenOrCreate);

            writer.Serialize(file, settings);
            file.Close();
        }
    }
}
