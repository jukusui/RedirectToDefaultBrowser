﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Launcher.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ExResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExResources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Launcher.Properties.ExResources", typeof(ExResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   すべてについて、現在のスレッドの CurrentUICulture プロパティをオーバーライドします
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Shold be Only One Argument on Startup に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string ArgTooMany {
            get {
                return ResourceManager.GetString("ArgTooMany", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Could Not Open Browser に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string ExBrowser {
            get {
                return ResourceManager.GetString("ExBrowser", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cannot Recognize This &quot;microsoft-edge&quot; Scheme に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string ExMsEdgeScheme {
            get {
                return ResourceManager.GetString("ExMsEdgeScheme", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Could Not Recognize as URL に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string ExNotUrl {
            get {
                return ResourceManager.GetString("ExNotUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Could Not Find URL に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string ExNoUrl {
            get {
                return ResourceManager.GetString("ExNoUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   An Error Occurred When Applying &quot;{0}&quot; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string ExRedirect {
            get {
                return ResourceManager.GetString("ExRedirect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recognized URL was Not &quot;http&quot; or &quot;https&quot; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string ExScheme {
            get {
                return ResourceManager.GetString("ExScheme", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Unknown Error に類似しているローカライズされた文字列を検索します。
        /// </summary>
        public static string ExUnknown {
            get {
                return ResourceManager.GetString("ExUnknown", resourceCulture);
            }
        }
    }
}
