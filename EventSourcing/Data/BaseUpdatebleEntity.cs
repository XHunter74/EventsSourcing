using System.ComponentModel.DataAnnotations;

namespace EventSourcing.Data;

public abstract class BaseUpdatebleEntity:BaseEntity
{
    [Required] public virtual DateTime Updated { get; set; }
}