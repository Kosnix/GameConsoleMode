﻿#pragma checksum "F:\GCM\GAMINGCONSOLEMODE\launcher.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E2E1A85F54242C1E9F95D7E6A1BD2CF3D0BB0F26ED1CE85395A04C7143D9EBA1"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GAMINGCONSOLEMODE
{
    partial class launcher : 
        global::Microsoft.UI.Xaml.Controls.Page, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2502")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // launcher.xaml line 203
                {
                    this.textbox_custom_path = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBox>(target);
                    ((global::Microsoft.UI.Xaml.Controls.TextBox)this.textbox_custom_path).TextChanged += this.textbox_custom_path_TextChanged;
                }
                break;
            case 3: // launcher.xaml line 197
                {
                    this.pichcustompath = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.pichcustompath).Click += this.pichcustompath_Click;
                }
                break;
            case 4: // launcher.xaml line 199
                {
                    this.textbox_select_custom_path = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 5: // launcher.xaml line 186
                {
                    this.use_custom = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ToggleSwitch>(target);
                    ((global::Microsoft.UI.Xaml.Controls.ToggleSwitch)this.use_custom).Toggled += this.use_custom_Toggled;
                }
                break;
            case 6: // launcher.xaml line 143
                {
                    this.textbox_playnite_path = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBox>(target);
                    ((global::Microsoft.UI.Xaml.Controls.TextBox)this.textbox_playnite_path).TextChanged += this.textbox_playnite_path_TextChanged;
                }
                break;
            case 7: // launcher.xaml line 137
                {
                    this.pichplaynitepath = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.pichplaynitepath).Click += this.pichplaynitepath_Click;
                }
                break;
            case 8: // launcher.xaml line 139
                {
                    this.textbox_select_playnite_path = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 9: // launcher.xaml line 126
                {
                    this.use_playnite = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ToggleSwitch>(target);
                    ((global::Microsoft.UI.Xaml.Controls.ToggleSwitch)this.use_playnite).Toggled += this.use_playnite_Toggled;
                }
                break;
            case 10: // launcher.xaml line 83
                {
                    this.textbox_steam_path = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBox>(target);
                    ((global::Microsoft.UI.Xaml.Controls.TextBox)this.textbox_steam_path).TextChanged += this.textbox_steam_path_TextChanged;
                }
                break;
            case 11: // launcher.xaml line 77
                {
                    this.pichsteampath = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.pichsteampath).Click += this.pichsteampath_Click;
                }
                break;
            case 12: // launcher.xaml line 79
                {
                    this.textbox_select_steam_path = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 13: // launcher.xaml line 66
                {
                    this.use_steam_bp = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ToggleSwitch>(target);
                    ((global::Microsoft.UI.Xaml.Controls.ToggleSwitch)this.use_steam_bp).Toggled += this.use_steam_bp_Toggled;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }


        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2502")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

