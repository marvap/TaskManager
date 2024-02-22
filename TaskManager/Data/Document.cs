using System;
using System.Collections.Generic;

namespace TaskManager.Data;

public partial class Document
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public string Title { get; set; } = null!;

    public byte[] Content { get; set; } = null!;

    public string DocumentType { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;
}
