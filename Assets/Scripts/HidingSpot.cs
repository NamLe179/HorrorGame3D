using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class HidingSpot : MonoBehaviour
{
	[Header("Detection Settings")]
	[SerializeField] private float noiseThreshold = 0.1f; // ngưỡng âm lượng
	[SerializeField] private float checkInterval = 0.2f;  // tần suất kiểm tra (giây)

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
				Debug.Log("Âm lượng: " + volume);

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
				noiseFill.fillAmount = Mathf.Clamp01(displayVolume * 15f); // nhân lên để rõ hơn
		}
	}

	private void StartMic()
	{
		if (micDevice == null) return;
		micClip = Microphone.Start(micDevice, true, 1, 44100);
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
		if (micClip == null) return 0f;

		int sampleSize = 128;
		float[] samples = new float[sampleSize];
		int micPos = Microphone.GetPosition(micDevice) - sampleSize + 1;
		if (micPos < 0) return 0f;

		micClip.GetData(samples, micPos);

		// Tính RMS (Root Mean Square) → cường độ âm
		float sum = 0f;
		foreach (float s in samples)
			sum += s * s;

		return Mathf.Sqrt(sum / sampleSize);
	}
}
