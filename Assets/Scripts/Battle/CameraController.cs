using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AtentsPro
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraController : MonoBehaviour
    {
        public void SetDollyTrack(CinemachineSmoothPath path)
        {
            if (GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>() != null)
            {
                GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>().m_Path = path;
                GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 0f;
            }
        }

        public void MoveTo(int dest, float duration)
        {
            StartCoroutine(MoveToCo(dest,duration));
        }

        IEnumerator MoveToCo(int trackIndex, float duration)
        {
            float start = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float next = Mathf.Lerp(start, (float)trackIndex, t);
                GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = next;

                yield return null;
            }
        }
    }
}
