using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.EntityModels.Models;

public partial class ApprovalSetting
{
    [SwaggerSchema("簽核設定流水號")]
    public int Id { get; set; }

    [SwaggerSchema("表格型態")]
    public int TableType { get; set; }

    [SwaggerSchema("簽核名稱")]
    public string Name { get; set; } = null!;

    [SwaggerSchema("是否啟用")]
    public bool IsActive { get; set; }

    [JsonIgnore]
    public virtual ICollection<ApprovalStep> ApprovalSteps { get; set; } = new List<ApprovalStep>();
}
