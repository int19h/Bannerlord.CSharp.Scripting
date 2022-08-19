using SandBox.View.Map;

/// <summary>
/// Removes all quick info notifications.
/// </summary>
void ClearQuickInfo() {
    var ggn = GauntletGameNotification.Current;
    var vm = (GameNotificationVM)ggn.GetType().InvokeMember("_dataSource", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, ggn, null);
    vm.ClearNotifications();
}

/// <summary>
/// Removes all map notices (bubbles on the right).
/// </summary>
void ClearMapNotices() {
    MapScreen.Instance.MapNotificationView.ResetNotifications();
}

