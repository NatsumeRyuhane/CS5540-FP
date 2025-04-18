using TMPro;

namespace Objects
{
    public record Objective() 
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public TextMeshProUGUI UIElement { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFailed { get; set; }
    }
}
