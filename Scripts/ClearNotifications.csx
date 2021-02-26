using SandBox.View.Map;

/// <summary>
/// Removes all bubble notifications.
/// </summary>
void ClearNotifications() {
    SandBox.View.Map.MapScreen.Instance.MapNotificationView.ResetNotifications();
}
