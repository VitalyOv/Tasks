namespace Domain.Entities;

public class Task
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime? UpdatedTime { get; set; }

    public Status Status { get; set; } = Status.created;
}

public enum Status
{
    created,
    running,
    finished,
}
