using System;
using System.Collections.Generic;

namespace TaskManager.Data;

public partial class CheckList
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public string Text { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public int Status { get; set; }

    public virtual Task Task { get; set; } = null!;
}
