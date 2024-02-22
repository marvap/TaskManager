using System;
using System.Collections.Generic;

namespace TaskManager.Data;

public partial class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsSolver { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
