using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitRuisseau
{
    public class FileSerelizer
    {
        private string folderPath;
        public FileSerelizer(string _folderPath)
        {
            folderPath = _folderPath;
        }
        public FileSerelizer()
        {

        }
        public List<TagLib.File> SerelizeFilesToTagLibFiles(List<FileInfo> files)
        {
            List<TagLib.File> taglibFiles = new List<TagLib.File>();
            files.ForEach(file =>
            {
                var tagLibFile = TagLib.File.Create(file.FullName);
                taglibFiles.Add(tagLibFile);
            });
            return taglibFiles;
        }
        public string SerelizeCatalog(List<TagLib.File> files, GenericEnvelope envelop)
        {
            var fileInfoList = new List<object>();

            files.ForEach(file =>
            {
                var fileInfo = new
                {
                    Title = System.IO.Path.GetFileNameWithoutExtension(file.Name),
                    Artist = file.Tag.FirstPerformer,
                    Type = System.IO.Path.GetExtension(file.Name),
                    Size = file.Length,
                    Duration = file.Properties.Duration,
                };
                fileInfoList.Add(fileInfo);
            });
            var contentOfFile = new
            {
                Content = fileInfoList.ToArray(),
            };
            string stringContentFile = System.Text.Json.JsonSerializer.Serialize(contentOfFile, new JsonSerializerOptions { WriteIndented = true });
            var contentToSend = new
            {
                MessageType = envelop.MessageType,
                SenderId = envelop.SenderId,
                EnveloppeJson = stringContentFile
            };
            return System.Text.Json.JsonSerializer.Serialize(contentToSend, new JsonSerializerOptions { WriteIndented = true });
        }
        public string SerelizeMessageType1(GenericEnvelope envelop)
        {
            var messageInfo = new
            {
                MessageType = envelop.MessageType,
                SenderId = envelop.SenderId,
            };
            return JsonConvert.SerializeObject(messageInfo, Formatting.Indented);
        }
    }
}
