// Fix: Avoid setting function index on load unless creating new
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using System.Text.Json;

namespace GAMINGCONSOLEMODE
{
    public sealed partial class shortcuts : Page
    {
        private readonly string[] gamepadButtons = new[]
        {
            "DPadUp", "DPadDown", "DPadLeft", "DPadRight",
            "Start", "Back", "LeftThumb", "RightThumb",
            "LeftShoulder", "RightShoulder", "A", "B", "X", "Y"
        };

        private readonly List<string> functions = new()
        {
            "taskmanager",
            "switch tab",
            "audio switch",
            "performance overlay"
        };

        public shortcuts()
        {
            this.InitializeComponent();
            LoadExistingShortcuts();
        }

        private void AddCustomShortcut(object sender, RoutedEventArgs e) => AddCustomShortcut();

        private void AddCustomShortcut(string key1 = null, string key2 = null, string function = null, bool enabled = false)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(ColorHelper.FromArgb(255, 30, 30, 30)),
                BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(255, 51, 51, 51)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(10)
            };

            var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, VerticalAlignment = VerticalAlignment.Center };

            var cbKey1 = CreateStyledComboBox("KEY1", key1);
            var cbKey2 = CreateStyledComboBox("KEY2", key2);
            var cbFunc = CreateStyledComboBox("Function", function);
            cbFunc.Items.Clear();
            foreach (var func in functions)
                cbFunc.Items.Add(new ComboBoxItem { Content = func });
            if (!string.IsNullOrEmpty(function))
                cbFunc.SelectedItem = cbFunc.Items.OfType<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == function);
            cbFunc.IsEnabled = !enabled;

            var plus = new TextBlock { Text = "+", VerticalAlignment = VerticalAlignment.Center, FontSize = 20, Width = 20, Foreground = new SolidColorBrush(Colors.White) };
            var equals = new TextBlock { Text = "=", VerticalAlignment = VerticalAlignment.Center, FontSize = 20, Width = 20, Foreground = new SolidColorBrush(Colors.White) };
            var toggle = new ToggleSwitch
            {
                Width = 60,
                VerticalAlignment = VerticalAlignment.Center,
                IsOn = enabled,
                IsEnabled = true // Toggle ist immer aktivierbar
            };
            toggle.Toggled += (s, e) => {
                cbFunc.IsEnabled = !toggle.IsOn;
                ToggleSwitch_Toggled(s, e);
            };

            var removeBtn = new Button
            {
                Content = "Delete",
                Background = new SolidColorBrush(ColorHelper.FromArgb(255, 51, 51, 51)),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(10, 5, 10, 5),
                BorderThickness = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center
            };
            removeBtn.Click += (s, args) => {
                ShortcutPanel.Children.Remove(border);
                DeleteShortcutFile(cbFunc);
            };

            cbKey1.SelectionChanged += (s, e) => {
                if (toggle.IsOn)
                {
                    var k1 = (cbKey1.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    var k2 = (cbKey2.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    var f = (cbFunc.SelectedItem as ComboBoxItem)?.Content?.ToString();
                    if (string.IsNullOrEmpty(k1) || k1 == "KEY1" || string.IsNullOrEmpty(k2) || k2 == "KEY2" || string.IsNullOrEmpty(f) || f == "Function")
                    {
                        toggle.IsOn = false;
                        return;
                    }
                }
                SaveShortcutConfig(cbKey1, cbKey2, cbFunc, toggle.IsOn);
            };
            cbKey2.SelectionChanged += (s, e) => { if (toggle.IsOn) SaveShortcutConfig(cbKey1, cbKey2, cbFunc, toggle.IsOn); };
            cbFunc.SelectionChanged += (s, e) => {
                if (toggle.IsOn)
                {
                    cbFunc.IsEnabled = false;
                    SaveShortcutConfig(cbKey1, cbKey2, cbFunc, toggle.IsOn);
                }
            };

            panel.Children.Add(cbKey1);
            panel.Children.Add(plus);
            panel.Children.Add(cbKey2);
            panel.Children.Add(equals);
            panel.Children.Add(cbFunc);
            panel.Children.Add(toggle);
            panel.Children.Add(removeBtn);

            border.Child = panel;
            ShortcutPanel.Children.Add(border);
        }

        private ComboBox CreateStyledComboBox(string placeholder, string selected = null)
        {
            var combo = new ComboBox
            {
                Width = 200,
                PlaceholderText = placeholder,
                Background = new SolidColorBrush(ColorHelper.FromArgb(255, 40, 40, 40)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(255, 70, 70, 70))
            };
            foreach (var btn in gamepadButtons)
                combo.Items.Add(new ComboBoxItem { Content = btn });
            if (!string.IsNullOrEmpty(selected))
                combo.SelectedItem = combo.Items.OfType<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == selected);
            return combo;
        }

        private void LoadExistingShortcuts()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GCMSettings", "shortcuts");
            if (!Directory.Exists(dir)) return;

            foreach (var file in Directory.GetFiles(dir, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var data = JsonSerializer.Deserialize<ShortcutData>(json);
                    if (data != null)
                        AddCustomShortcut(data.Key1, data.Key2, data.Function, data.Enabled);
                }
                catch { continue; }
            }
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            var panel = toggle?.Parent as StackPanel;
            if (panel == null) return;

            var cbKey1 = panel.Children.OfType<ComboBox>().ElementAt(0);
            var cbKey2 = panel.Children.OfType<ComboBox>().ElementAt(1);
            var cbFunc = panel.Children.OfType<ComboBox>().ElementAt(2);

            string key1 = (cbKey1.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "KEY1";
            string key2 = (cbKey2.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "KEY2";
            string func = (cbFunc.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Function";

            // Placeholder check
            if (key1 == "KEY1" || key2 == "KEY2" || func == "Function")
            {
                toggle.IsOn = false;
                return;
            }

            // Duplikate
            if (toggle.IsOn)
            {
                bool duplicate = ShortcutPanel.Children.OfType<Border>()
                    .Select(b => b.Child as StackPanel)
                    .Where(p => p != null && p != panel)
                    .Any(p =>
                    {
                        var toggleOther = p.Children.OfType<ToggleSwitch>().FirstOrDefault();
                        var funcOther = p.Children.OfType<ComboBox>().ElementAt(2);
                        var item = funcOther.SelectedItem as ComboBoxItem;
                        return toggleOther != null && toggleOther.IsOn && item != null && item.Content?.ToString() == func;
                    });

                if (duplicate)
                {
                    toggle.IsOn = false;
                    return;
                }
            }

            SaveShortcutConfig(cbKey1, cbKey2, cbFunc, toggle.IsOn);
        }


        private void SaveShortcutConfig(ComboBox cb1, ComboBox cb2, ComboBox cbFunc, bool isEnabled)
        {
            string key1 = (cb1.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "None";
            string key2 = (cb2.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "None";
            string func = (cbFunc.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Function";

            if (key1 == "KEY1" || key2 == "KEY2" || func == "Function")
                return; // Shortcut not save when not all inside

            var data = new ShortcutData
            {
                Key1 = key1,
                Key2 = key2,
                Function = func,
                Enabled = isEnabled
            };

            string json = JsonSerializer.Serialize(data);
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GCMSettings", "shortcuts");
            Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, func + ".json");
            File.WriteAllText(path, json);
        }


        private void DeleteShortcutFile(ComboBox cbFunc)
        {
            string func = (cbFunc.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Function";
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GCMSettings", "shortcuts");
            string path = Path.Combine(dir, func + ".json");

            bool otherStillActive = ShortcutPanel.Children.OfType<Border>()
                .Select(b => b.Child as StackPanel)
                .Where(p => p != null)
                .Any(p =>
                {
                    var combo = p.Children.OfType<ComboBox>().ElementAt(2);
                    var toggle = p.Children.OfType<ToggleSwitch>().FirstOrDefault();
                    var item = combo.SelectedItem as ComboBoxItem;
                    return combo != cbFunc && item != null && item.Content?.ToString() == func && toggle?.IsOn == true;
                });

            if (!otherStillActive && File.Exists(path))
                File.Delete(path);
        }

        private class ShortcutData
        {
            public string Key1 { get; set; }
            public string Key2 { get; set; }
            public string Function { get; set; }
            public bool Enabled { get; set; }
        }
    }
}
