//*****************************************************************************************************************
//*****************************************************************************************************************
//*****************************************************************************************************************
/*!

 *    @file     PlatformIos.cs
 *    @brief    iOS用ユーティリティ実装
 *
 */
//*****************************************************************************************************************
//*****************************************************************************************************************
//*****************************************************************************************************************
#if UNITY_IPHONE
using UnityEngine;
using System.Runtime.InteropServices;


namespace SGSys {
    public class PlatformIos : PlatformImpl
    {
        [DllImport ("__Internal")]	private static extern void XeSysUtility_Initialize();
        [DllImport ("__Internal")]	private static extern void XeSysUtility_ShowAlertDialog( string title, string body, string ok );
        [DllImport ("__Internal")]	private static extern void XeSysUtility_ShowIndicator();
        [DllImport ("__Internal")]	private static extern void XeSysUtility_HideIndicator();
        [DllImport ("__Internal")]	private static extern float XeSysUtility_EnableBatteryMonitoring( bool enabled );
        [DllImport ("__Internal")]	private static extern float XeSysUtility_GetBatteryLevel();
        [DllImport ("__Internal")]	private static extern int XeSysUtility_GetBatteryStatus();
        [DllImport ("__Internal")]	private static extern void XeSysUtility_ShareMessage( string subject, string title, string body );
        [DllImport ("__Internal")]	private static extern void XeSysUtility_SetClipboardText( string text );
        [DllImport ("__Internal")]	private static extern void XeSysUtility_WebViewRemoveAllCookie();
        [DllImport ("__Internal")]  private static extern long XeSysUtility_CalcStorageAvailableSize();

        public PlatformIos() : base("ios")
        {
        }
        
        public override void	Initialize()
        {
            XeSysUtility_Initialize();

            GameObject obj = new GameObject("PlatformIos");
            SystemManager.Instance.AddToSystemGroup( obj );
        }
    
        public override string	GetOsVersion()
        {
            return UnityEngine.iOS.Device.systemVersion;
        }
        public override string	GetSignature()
        {
            return "ios-signature";
        }
        public override string	GetSignature2()
        {
            return "0";
        }
        

        public override void	ShowAlertDialog( string title, string body, string ok )
        {
            XeSysUtility_ShowAlertDialog( title, body, ok );
        }

        public override void ShowIndicator()
        {
            XeSysUtility_ShowIndicator();
        }
        
        public override void HideIndicator()
        {
            XeSysUtility_HideIndicator();
        }



        public override void EnableLog(bool enabled)
        {
        }

        public override void InitializePreference (string root) 
        {
        }

        public override string LoadPreference(string key)
        {
            return IOSKeyChain.GetData( key );
        }

        public override void SavePreference(string key, string value)
        {
            IOSKeyChain.SetData( key, value );
        }

        public override bool HasPreference(string key)
        {
            return IOSKeyChain.ContainsKey(key);
        }

        public override void DeletePreference(string key)
        {
            IOSKeyChain.DeleteData( key );
        }

        public override void EnableBatteryMonitoring(bool enabled)
        {
            XeSysUtility_EnableBatteryMonitoring( enabled );
        }

#if false   //UnityEngine.SystemInfo.batteryLevelが正しく無い時はこちらを利用する
        public override float GetBatteryLevel() {
            return XeSysUtility_GetBatteryLevel();
        }
#endif

#if false   //UnityEngine.SystemInfo.batteryStatusが正しく無い時はこちらを利用する
        public override BatteryStatus GetBatteryStatus() {
            return (BatteryStatus)XeSysUtility_GetBatteryStatus();
        }
#endif

        /// <summary>
        /// メッセージ共有
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        public override void ShareMessage(string subject, string title, string body)
        {
            XeSysUtility_ShareMessage(subject, title, body);
        }

        /// <summary>
        /// システムのクリップボードへテキストをコピー
        /// </summary>
        /// <param name="text"></param>
        public override void SetClipboardText(string text)
        {
            XeSysUtility_SetClipboardText(text);
        }


        public override void WebViewRemoveAllCookie()
        {
            XeSysUtility_WebViewRemoveAllCookie();
        }

        /// <summary>
        /// ストレージの空き容量を返す
        /// </summary>
        /// <returns></returns>
        public override long CalcStorageAvailableSize ()
        {
            return XeSysUtility_CalcStorageAvailableSize();
        }
    }


} //namespace SGSys

#endif //UNITY_IPHONE
