using System;
using System.Collections.Generic;

namespace TaskManager.Data;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CompanyId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Task> TaskCreatorUsers { get; set; } = new List<Task>();

    public virtual ICollection<Task> TaskSolverUsers { get; set; } = new List<Task>();
}
