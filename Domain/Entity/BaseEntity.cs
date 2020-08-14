using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class BaseEntity
    {
        public string Id { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsDeleted { get; set; }

        public String CreateUserId { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public string LastUpdateId { get; set; }
    }

    public static class BaseEntityExtend
    {
        public static void InitEntity(this BaseEntity entity)
        {
            entity.Id = Guid.NewGuid().ToString("N");
            entity.CreateTime = DateTime.Now;
        }

        public static void CreateEntity(this BaseEntity baseEntity,string id)
        {
            baseEntity.InitEntity();
            baseEntity.LastUpdateTime = DateTime.Now;
            baseEntity.CreateUserId = id;
        }
    }
}
