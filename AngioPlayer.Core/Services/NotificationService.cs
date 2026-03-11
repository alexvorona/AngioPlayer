using Microsoft.Toolkit.Uwp.Notifications;

namespace AngioPlayer.Services;

public interface INotificationService
{
    void ShowError(string message);
}

public class NotificationService : INotificationService
{
    public void ShowError(string message)
    {
        new ToastContentBuilder()
            .AddText("Error")
            .AddText(message)
            .Show();
    }
}