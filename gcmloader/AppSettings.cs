using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace GAMINGCONSOLEMODE
{
    public class AppSettings
    {
        private static readonly string SettingsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings");
        private static readonly string SettingsFilePath = Path.Combine(SettingsFolder, "settings.json");

        // Ensures folder and default config file exist
        public static void FirstStart()
        {
            if (!Directory.Exists(SettingsFolder))
            {
                Directory.CreateDirectory(SettingsFolder);
                Console.WriteLine($"Settings folder created at: {SettingsFolder}");
            }

            if (!File.Exists(SettingsFilePath))
            {
                initialconfig();
            }
            else
            {
                Console.WriteLine("Settings file already exists.");
            }
        }

        // Save a specific key-value pair
        public static void Save(string key, object value)
        {
            try
            {
                JObject settings;

                // Load existing JSON or create a new one
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    settings = string.IsNullOrWhiteSpace(json) ? new JObject() : JObject.Parse(json);
                }
                else
                {
                    settings = new JObject();
                }

                // Update or add the key-value pair
                settings[key] = JToken.FromObject(value);

                // Save the updated JSON back to the file
                File.WriteAllText(SettingsFilePath, settings.ToString(Formatting.Indented));
                Console.WriteLine($"Saved: {key} = {value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Save method: {ex.Message}");
            }
        }

        // Load a specific key from the JSON file
        public static T Load<T>(string key)
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    var settings = string.IsNullOrWhiteSpace(json) ? new JObject() : JObject.Parse(json);

                    if (settings.ContainsKey(key))
                    {
                        return settings[key].ToObject<T>();
                    }
                }
                throw new Exception($"Key '{key}' not found in the settings.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Load method: {ex.Message}");
                throw;
            }
        }

        // Initialize default settings on first start
        public static void initialconfig()
        {
            try
            {
                var defaultSettings = new JObject
                {
                    ["launcher"] = "steam",
                    ["steamlauncherpath"] = @"C:\Program Files (x86)\Steam\steam.exe"
                };

                File.WriteAllText(SettingsFilePath, defaultSettings.ToString(Formatting.Indented));
                Console.WriteLine($"Default settings file created at: {SettingsFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in initialconfig method: {ex.Message}");
            }
        }
    }
}



