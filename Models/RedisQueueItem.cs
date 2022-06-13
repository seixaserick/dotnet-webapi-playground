using System.ComponentModel.DataAnnotations;

namespace Models;

public class RedisQueueItem
{

    public string id { get; set; } = Guid.NewGuid().ToString();
 

    public DateTime queued_at { get; set; } = DateTime.Now;

    [Required]
    public string item_type { get; set; }

    [Required]
    public object item_content { get; set; }

}
