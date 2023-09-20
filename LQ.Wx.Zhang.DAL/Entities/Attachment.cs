using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQ.Wx.Zhang.DAL
{
    public class Attachment : IdEntity
    {
        public Attachment(string name,string path,string ext,string fileName) { 
            Name = name;
            Path = path;
            Ext = ext;
            FileName = fileName;
            Items = new List<Item>();
        }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Ext { get; set; }
        public string FileName { get; set; }
        public int ModuleType { get; set; }
        public List<Item> Items { get; set; }
        [NotMapped]
        public string FullName => Name + Path + Ext;
    }
}
