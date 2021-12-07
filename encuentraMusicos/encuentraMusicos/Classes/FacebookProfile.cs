using System;
using System.Collections.Generic;
using System.Text;

namespace encuentraMusicos.Classes
{
    class FacebookProfile
    {
        public string Name { get; set; }
        public Picture Picture { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
    }
    public class Picture
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public bool IsSilhouette { get; set; }
        public string Url { get; set; }
    }
}
