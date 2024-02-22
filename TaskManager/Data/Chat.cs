using System;
using System.Collections.Generic;

namespace TaskManager.Data;

public partial class Chat
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int CreatorUserId { get; set; }

    public string Text { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public virtual User CreatorUser { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;
}
