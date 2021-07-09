using Microsoft.Xrm.Sdk;
using System;

namespace MvcTeam.Utilities.Models
{
    public class EntityObject
    {
        public Entity Entity { get; protected set; }

        public string GetValue(string key)
        {
            if (Entity.Attributes.ContainsKey(key))
            {
                var type = (Entity[key]).GetType();

                if (type == typeof(string)) { return Entity[key].ToString(); }
                if (type == typeof(int)) { return Entity[key].ToString(); }
                if (type == typeof(OptionSetValue)) { return ((OptionSetValue)Entity[key]).Value.ToString(); }
                if (type == typeof(bool)) { return Entity[key].ToString(); }
                if (type == typeof(double)) { return Entity[key].ToString(); }
                if (type == typeof(EntityReference)) { return ((EntityReference)Entity[key]).Id.ToString(); }
                if (type == typeof(Money)) { { return ((Money)Entity[key]).Value.ToString(); } }
            }
            return "";
        }
        public Guid Id { get { return Entity.Id; } set { Entity.Id = value; } }
        public void SetValue(string key, object value)
        {
            Entity[key] = value;
        }

        public EntityObject()
        {

        }
        public EntityObject(Entity entity)
        {
            this.Entity = entity;
        }
    }
}
