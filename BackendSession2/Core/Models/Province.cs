using System;

namespace BackendSession2.Core.Models
{
    public class ProvinceModel
    {
        public string Id { get; set; }
        public string ProvinceName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Code { get; set; }
        public DateTime Created { get; set; }
        public ProvinceModel MixBaseObject(ProvinceModel provinceModel)
        {
            var model = provinceModel;
            if (!String.IsNullOrEmpty(Id)) model.Id = Id;
            if (!String.IsNullOrEmpty(ProvinceName)) model.ProvinceName = ProvinceName;
            if (!String.IsNullOrEmpty(Latitude)) model.Latitude = Latitude;
            if (!String.IsNullOrEmpty(Longitude)) model.Longitude = Longitude;
            if (!String.IsNullOrEmpty(Code)) model.Code = Code;
            return model;
        }
    }
}
