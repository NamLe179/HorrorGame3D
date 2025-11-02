using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class HidingSpot : MonoBehaviour
{
	[Header("Detection Settings")]
	[SerializeField] private float noiseThreshold = 0.02f; // ngưỡng âm lượng
	[SerializeField] private float checkInterval = 0.2f;  // tần suất kiểm tra (giây)
	[SerializeField] private float gain = 150f;            // khuếch đại tín hiệu

	[Header("UI Elements")]	
	[SerializeField] private GameObject noiseUIParent; // chứa toàn bộ UI noise bar
	[SerializeField] private Image noiseFill; // phần fill của thanh noise bar
	[SerializeField] private float uiSmoothSpeed = 8f; // tốc độ làm mượt

	private AudioClip micClip;
	private string micDevice;
	private bool isPlayerInside = false;
	private float nextCheckTime = 0f;
	private float displayVolume = 0f; // giá trị hiển thị mượt

	// Biến public để UI có thể lấy giá trị
	public float CurrentVolume { get; private set; }

	private void Start()
	{
		// Ẩn UI khi chưa vào vùng trốn
		if (noiseUIParent != null)
			noiseUIParent.SetActive(false);

		// lấy device mic mặc định
		if (Microphone.devices.Length > 0)
			micDevice = Microphone.devices[0];
		else
			Debug.LogWarning("Không tìm thấy microphone!");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("Player đã vào vùng trốn");
			StartMic();
			isPlayerInside = true;

			// Hiện UI
			if (noiseUIParent != null)
				noiseUIParent.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("Player rời khỏi vùng trốn");
			StopMic();
			isPlayerInside = false;

			// Ẩn UI
			if (noiseUIParent != null)
				noiseUIParent.SetActive(false);
		}
	}

	private void Update()
	{
		if (isPlayerInside && micClip != null)
		{
			if (Time.time >= nextCheckTime)
			{
				nextCheckTime = Time.time + checkInterval;
				float volume = GetMicVolume();
				CurrentVolume = volume;

				if (volume > noiseThreshold)
				{
					Debug.Log("🔊 Phát hiện tiếng động!");
					// Gọi AI kẻ đuổi tại đây, ví dụ:
					// EnemyAI.Instance.DetectPlayer(transform.position);
				}
			}

			// Làm mượt UI fill bar
			displayVolume = Mathf.Lerp(displayVolume, CurrentVolume, Time.deltaTime * uiSmoothSpeed);
			if (noiseFill != null)
				noiseFill.fillAmount = Mathf.Clamp01(displayVolume * 100f); // nhân lên để rõ hơn
		}
	}

	private void StartMic()
	{
		if (string.IsNullOrEmpty(micDevice)) return;

		Microphone.End(micDevice);
		micClip = Microphone.Start(micDevice, true, 1, 22050);
		StartCoroutine(WaitMicReady());
	}

	private IEnumerator WaitMicReady()
	{
		yield return new WaitForSeconds(0.1f);
		Debug.Log($"🎙️ Mic '{micDevice}' started. Pos={Microphone.GetPosition(micDevice)}");
	}

	private void StopMic()
	{
		if (micDevice == null) return;
		Microphone.End(micDevice);
		micClip = null;
		CurrentVolume = 0f;
		displayVolume = 0f;

		// Reset thanh UI
		if (noiseFill != null)
			noiseFill.fillAmount = 0f;
	}

	private float GetMicVolume()
	{
		if (micClip == null || string.IsNullOrEmpty(micDevice))
			return 0f;

		int sampleSize = 256;
		float[] samples = new float[sampleSize];
		int micPos = Microphone.GetPosition(micDevice) - sampleSize + 1;

		if (micPos < 0) return 0f;

		try
		{
			micClip.GetData(samples, micPos);
		}
		catch
		{
			return 0f;
		}

		// Tính RMS và khuếch đại nhẹ
		float sum = 0f;
		for (int i = 0; i < sampleSize; i++)
			sum += samples[i] * samples[i];

		float rms = Mathf.Sqrt(sum / sampleSize);
		return Mathf.Clamp01(rms * gain); // giữ trong [0,1]
	}
}
