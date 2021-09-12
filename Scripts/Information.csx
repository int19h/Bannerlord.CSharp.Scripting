using SandBox.View.Map;

/// <summary>
/// Removes all quick info notifications.
/// </summary>
void ClearQuickInfo() {
    var vm = (GameNotificationVM)typeof(GameNotificationManager).InvokeMember("_dataSource", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
    vm.ClearNotifications();
}

/// <summary>
/// Removes all map notices (bubbles on the right).
/// </summary>
void ClearMapNotices() {
    MapScreen.Instance.MapNotificationView.ResetNotifications();
}

