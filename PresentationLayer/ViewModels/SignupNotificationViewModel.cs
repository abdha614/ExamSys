namespace PresentationLayer.ViewModels
{
    public class SignupNotificationViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime RequestedAt { get; set; }
        public bool IsHandled { get; set; }
    }
}
