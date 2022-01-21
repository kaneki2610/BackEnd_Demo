using System;

namespace BackendSession2.Core.Models
{
    public class AreaModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string ParentId { get; set; }
        public string Type { get; set; }
        public string ProvinceId { get; set; }
        //public DateTime Created { get; set; }
        //public DateTime Update { get; set; }
        public AreaModel MixBaseObject(AreaModel areaModel)
        {
            var model = areaModel;
            model.Id = Id;
            if (!String.IsNullOrEmpty(Code)) model.Code = Code;
            if (!String.IsNullOrEmpty(Name)) model.Name = Name;
            if (!String.IsNullOrEmpty(Latitude)) model.Latitude = Latitude;
            if (!String.IsNullOrEmpty(Longitude)) model.Longitude = Longitude;
            if (!String.IsNullOrEmpty(ParentId)) model.ParentId = ParentId;
            if (!String.IsNullOrEmpty(Type)) model.Type = Type;
            if (!String.IsNullOrEmpty(ProvinceId)) model.ProvinceId = ProvinceId;
            //model.Created = Created;
            //model.Update = Update;
            return model;
        }
    }
}

