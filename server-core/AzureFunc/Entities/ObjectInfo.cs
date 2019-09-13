using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.WindowsAzure.Storage.Table;
using System.ComponentModel.DataAnnotations;
//using Microsoft.Azure.Cosmos.Table;

namespace AzureFunc.Entities
{
    public class ObjectInfo
    {
        public string id { get; set; }
        [Required(ErrorMessage = "Title is required." )]
        public string title { get; set; }
        public string description { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? startDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? endDate { get; set; }
        public bool? isComplete { get; set; }

        public DateTime? createdDate { get; set; }
        public string createdBy { get; set; }
    }
    public class ObjectItem : TableEntity
    {
        public ObjectItem()
        {
            this.PartitionKey = FunctionsSettings.PartitionKey;
        }

        public string id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsComplete { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }

    public static class ObjectExtensions
    {
        public static ObjectItem MapToTableEntity(this ObjectInfo objectInfo)
        {
            return new ObjectItem
            {
                RowKey = objectInfo.id ?? Guid.NewGuid().ToString(),
                Title = objectInfo.title,
                Description = objectInfo.description,
                StartDate = objectInfo.startDate,
                EndDate = objectInfo.endDate,
                IsComplete = objectInfo.isComplete,
                CreatedDate = objectInfo.createdDate,
                CreatedBy = objectInfo.createdBy
              //  PartitionKey = "ObjectItem"
            };
        }

        public static ObjectInfo MapFromTableEntity(this ObjectItem item)
        {
            return new ObjectInfo
            {
                id = item.RowKey,
                title = item.Title,
                description = item.Description,
                startDate = item.StartDate,
                endDate = item.EndDate,
                isComplete = item.IsComplete,
                createdBy = item.CreatedBy,
                createdDate = item.CreatedDate
            };
        }
    }
}
