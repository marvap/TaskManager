using System;
using System.Collections.Generic;

namespace TaskManager.Data;

public partial class Task
{
    public int Id { get; set; }

    public DateTime DateCreated { get; set; }

    public string Title { get; set; } = null!;

    public string Text { get; set; } = null!;

    public string? SolutionText { get; set; } = null;

    public int CreatorUserId { get; set; }

    public int? SolverUserId { get; set; }

    public DateTime DueDate { get; set; }

    public int Priority { get; set; }

    public int Status { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<CheckList> CheckLists { get; set; } = new List<CheckList>();

    public virtual User CreatorUser { get; set; } = null!;

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual User? SolverUser { get; set; }
}
