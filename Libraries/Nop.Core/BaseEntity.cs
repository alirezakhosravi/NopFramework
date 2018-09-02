using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nop.Core
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract partial class BaseEntity
    {
        public BaseEntity()
        {

        }

        public BaseEntity(Type entityType)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType), "Type");
        }

        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Entity Type.
        /// </summary>
        [NotMapped]
        public Type EntityType { get; private set; }
    }
}
