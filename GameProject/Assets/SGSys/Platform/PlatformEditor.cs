//*****************************************************************************************************************
//*****************************************************************************************************************
//*****************************************************************************************************************
/*!

 *    @file     PlatformEditor.cs
 *    @brief    Unityエディターモード時のユーティリティ
 *
 */
//*****************************************************************************************************************
//*****************************************************************************************************************
//*****************************************************************************************************************
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SGSys
{
	public class PlatformEditor : PlatformImpl
	{
		private string mSignature2;

		public PlatformEditor() : base("editor")
		{
		}
		
		public override void	Initialize()
		{
			mSignature2 = "0";
		}
		
		public override string	GetOsVersion()
		{
			return "99.99.99";
		}

		public override string	GetDeviceName()
		{
			return "UnityEditor";
		}

		public override string GetSignature()
		{
			return "unknown-signature-editor";
		}

		public override string GetSignature2()
		{
			return mSignature2;
		}
#if GAME_DEBUG
		public override void Debug_SetSignature2(string sig2)
		{
			mSignature2 = sig2;
		}
#endif

		public override void	ShowAlertDialog( string title, string body, string ok )
		{
			DebugLog.Warning( SystemConst.DebugGroup.System, "ShowAlertDialog : unimplement");
		}

		public override void ShowIndicator()
		{
			DebugLog.Warning( SystemConst.DebugGroup.System, "ShowIndicator : unimplement" );
		}

		public override void HideIndicator()
		{
			DebugLog.Warning( SystemConst.DebugGroup.System, "HideIndicator : unimplement" );
		}
		
		public override SystemConst.Language GetLanguage()
		{
			return SystemConst.Language.Japanese;
		}

		public override SystemConst.Country GetCountry()
		{
			return SystemConst.Country.Japan;
		}

		public override string GetDeviceUniqueId()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}

		public override void EnableLog(bool enabled)
		{
		}


		public override void EnableBatteryMonitoring(bool enabled)
		{
		}

		public override float GetBatteryLevel()
		{
			return 1.0f;	//Editor上は決め打ちの値
		}

		public override BatteryStatus GetBatteryStatus()
		{
			return BatteryStatus.Full;	//Editor上は決め打ちの値
		}

		/// <summary>
		/// プリファレンス保存用パスの取得
		/// </summary>
		/// <param name="key">プリファレンスのキー</param>
		/// <returns>保存先パス</returns>
		private string GetPreferencePath( string key )
		{
			string path = Application.temporaryCachePath + "/" + this.prefRoot + key;
			return path;
		}

		public override string LoadPreference(string key)
		{
			string path = GetPreferencePath( key );
			System.IO.FileInfo fi = new System.IO.FileInfo(path);
			if ( !fi.Exists )
			{
				return null;
			}

			if ( 0 >= fi.Length )
			{
				return null;
			}

			System.IO.StreamReader sr = new System.IO.StreamReader(path);
			string text = sr.ReadToEnd();
			sr.Close();
			return text;
		}

		public override void SavePreference(string key, string value)
		{
			string path = GetPreferencePath( key );
			System.IO.StreamWriter sw = new System.IO.StreamWriter( path, false );
			sw.Write( value );
			sw.Close();
		}

		public override void DeletePreference(string key)
		{
			string path = GetPreferencePath( key );
			System.IO.File.Delete(path);
		}

		public override bool HasPreference(string key)
		{
			string path = GetPreferencePath( key );
			if ( System.IO.File.Exists( path ) )
			{
				return true;
			}
			return false;
		}

		public override void InitializePreference(string root)
		{
			this.prefRoot = root;
		}

		/// <summary>
		/// メッセージ共有
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="title"></param>
		/// <param name="body"></param>
		public override void ShareMessage(string subject, string title, string body)
		{
			var output = string.Format("{0}/{1}_{2}.txt", Application.persistentDataPath, subject, Utility.GetLocalDateTime(Utility.GetCurrentUnixTime()).ToString("yyyy-M-d_HHmmss"));
			if (!string.IsNullOrEmpty(output))
			{
				System.IO.StreamWriter sw = new System.IO.StreamWriter(output, false);
				sw.Write(body);
				sw.Close();
#if UNITY_EDITOR
				EditorUtility.OpenWithDefaultApp(output);
#endif
			}
		}

		/// <summary>
		/// システムのクリップボードへテキストをコピー
		/// </summary>
		/// <param name="text"></param>
		public override void SetClipboardText(string text)
		{
			GUIUtility.systemCopyBuffer = text;
		}

		public override void WebViewRemoveAllCookie()
		{
		}


        /// <summary>
        /// ストレージの空き容量を計算する
        /// </summary>
        /// <returns></returns>
        public override long CalcStorageAvailableSize ()
		{
            var dir = System.IO.Directory.GetCurrentDirectory();
            var drive = System.IO.Path.GetPathRoot(dir);
            var di = new System.IO.DriveInfo(dir);
            return di.AvailableFreeSpace;
        }
    }
} //namespace SGLib

#endif //UNITY_EDITOR
