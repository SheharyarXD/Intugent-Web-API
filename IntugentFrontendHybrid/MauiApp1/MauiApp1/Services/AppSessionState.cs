namespace MauiApp1.Services
{
    public class AppSessionState
    {
        public int UserId { get; private set; }

        public event Action? Changed;

        public void SetUserId(int userId)
        {
            UserId = userId;
            Changed?.Invoke();
        }

        public void Clear()
        {
            UserId = 0;
            Changed?.Invoke();
        }
    }
}
