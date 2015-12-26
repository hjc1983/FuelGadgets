using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FGapp.Models
{
    public class Article
    {
        List<string> Tag_Items = new List<string>();
        List<string> Comment_Items = new List<string>();

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, DataType(DataType.MultilineText)]
        public string Abstract { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Url, Required]
        public string URL { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PublishedDate { get; set; } //user guid...etc

        public bool IsPublished { get; set; }

        public List<string> Tags
        {
            get { return Tag_Items; }
            set { Tag_Items = value; }
        }

        public List<string> Comments
        {
            get { return Comment_Items; }
            set { Comment_Items = value; }
        }

        [JsonProperty(PropertyName = "FEATURETYPE")]
        public string FeatureType { get { return "Article"; } }
    }
}