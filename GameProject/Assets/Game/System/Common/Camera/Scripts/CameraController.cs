//==================================================================
/// <summary>
/// カメラ制御クラス
/// </summary>
//==================================================================
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SGGames.Game.Sys
{
    public partial class CameraController : MonoBehaviour
    {
        [SerializeField]
        private CameraData[] m_CameraTable;

        // 
        [Header("※Normal CameraのみのIndex")]
        [SerializeField] int _nowNormalCameraIndex = 0;

        /// <summary>
        /// 対象のカメラ取得
        /// </summary>
        /// <param name="camid">カメラID</param>
        /// <returns></returns>
        public CameraData GetCamera(CameraData.eCameraID camid)
        {
            return m_CameraTable[(int)camid];
        }
        public CameraData NormalCamera  { get { return m_CameraTable[(int)CameraData.eCameraID.Normal]; } }
        public CameraData DemoCamera    { get { return m_CameraTable[(int)CameraData.eCameraID.Demo]; } }
        public CameraData FreeCamera    { get { return m_CameraTable[(int)CameraData.eCameraID.Free]; } }

        // 全てのカメラ対象の現在のカメラ
        public ReactiveProperty<CameraData> CurrentCamData { get; set; } = new();
        // NormaCamera専用の現在のカメラ
        public CameraData               CurrentNormalCamData    => m_CameraTable[_nowNormalCameraIndex];
        public CinemachineVirtualCamera CurrentVCam             => m_CameraTable[_nowNormalCameraIndex].VCam;

        void Awake()
        {
            // 初期設定
            for (int i = 0; i < m_CameraTable.Length; i++)
            {
                m_CameraTable[i].Initialize(this);
            }
        }

        // 子GameObjectのVirtualCameraに、targetを設定
        public void SetCameraTarget(Transform target)
        {
            foreach (var vcamData in m_CameraTable)
            {
                vcamData.SetTarget(target);
            }
        }

        [System.Serializable]
        public class CameraData : Unity.Cinemachine.AxisState.IInputAxisProvider
        {
            [SerializeField]
            eCameraID m_CameraID;
            public eCameraID CameraID => m_CameraID;

            [SerializeField]
            protected CinemachineVirtualCamera m_VCam;
            public CinemachineVirtualCamera VCam => m_VCam;

            Unity.Cinemachine.CinemachineFramingTransposer m_Transposer;
            Unity.Cinemachine.CinemachinePOV m_CinemachinePOV;

            public CameraController m_OwnerCtrl { get; private set; }

            /// <summary>
            /// カメラID
            /// m_CameraTableに登録されているカメラ順
            /// </summary>
            public enum eCameraID
            {
                Normal,
                Demo,
                Free,

                Debug
            }

            public void Initialize(CameraController owner)
            {
                m_OwnerCtrl = owner;

                m_Transposer = m_VCam.GetCinemachineComponent<Unity.Cinemachine.CinemachineFramingTransposer>();
                m_CinemachinePOV = m_VCam.GetCinemachineComponent<Unity.Cinemachine.CinemachinePOV>();
            }

            public void SetInputAxis()
            {
                m_CinemachinePOV.m_HorizontalAxis.SetInputAxisProvider(0, this);
                m_CinemachinePOV.m_VerticalAxis.SetInputAxisProvider(1, this);
            }

            public void SetCurrent(bool enable)
            {
                m_VCam.Priority = enable ? 10 : -100;
            }

            public float GetAngleY() => m_CinemachinePOV.m_HorizontalAxis.Value;
            public void SetAngleY(float angle) => m_CinemachinePOV.m_HorizontalAxis.Value = angle;

            public void SetTarget(Transform target)
            {
                m_VCam.Follow = target;
                m_VCam.LookAt = target;
            }

            float Unity.Cinemachine.AxisState.IInputAxisProvider.GetAxisValue(int axis)
            {
                return 0;
            }
        }
    }
}