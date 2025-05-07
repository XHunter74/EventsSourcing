using System.ComponentModel.DataAnnotations;

namespace EventSourcing.Data;

public abstract class BaseEntity
{
    [Required] public virtual DateTime Created { get; set; }
}