using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CanvasRecorder : MonoBehaviour
{
    public string objectTag;
    public TMP_InputField inputField;
    public Button startRecordingButton;
    public Button stopRecordingButton;
    public Button playRecordingButton;
    public GameObject recordingIndicator;
    public GameObject canvas;
    public float recordingInterval = 0.001f;

    private bool isRecording = false;
    private List<Vector3> recordedPositions = new List<Vector3>();
    private List<Color> recordedColors = new List<Color>();
    private int currentPositionIndex = 0;
    private bool isPlayingBack = false;
    private float playbackStartTime = 0f;
    private Image indicatorImage;
    private Color playbackColor = new Color(0.5f, 0f, 1f);
    private int recordingNumber = 0;

    private void Awake()
    {
        startRecordingButton.onClick.AddListener(StartRecording);
        stopRecordingButton.onClick.AddListener(StopRecording);
        playRecordingButton.onClick.AddListener(PlayRecording);
        indicatorImage = recordingIndicator.GetComponent<Image>();
        indicatorImage.color = Color.green;
    }

    private void Update()
    {
        if (isRecording)
        {
            GameObject obj = GameObject.FindGameObjectWithTag(objectTag);
            recordedPositions.Add(obj.transform.position);
            recordedColors.Add(obj.GetComponent<Image>().color);
        }

        if (isPlayingBack)
        {
            float timeElapsed = Time.time - playbackStartTime;
            int newPositionIndex = Mathf.FloorToInt(timeElapsed / recordingInterval);
            if (newPositionIndex < recordedPositions.Count)
            {
                GameObject obj = GameObject.FindGameObjectWithTag(objectTag);
                obj.transform.position = recordedPositions[newPositionIndex];
                obj.GetComponent<Image>().color = recordedColors[newPositionIndex];
            }
            else
            {
                isPlayingBack = false;
                currentPositionIndex = 0;
                indicatorImage.color = Color.green;
            }
        }
    }

    private void StartRecording()
    {
        if (inputField.text.Length <= 0) 
        {
            Debug.Log("Rellene el texto");
            return;
        }
        startRecordingButton.gameObject.SetActive(false);
        stopRecordingButton.gameObject.SetActive(true);
        isRecording = true;
        recordedPositions.Clear();
        recordedColors.Clear();
        indicatorImage.color = Color.red;
        InvokeRepeating(nameof(RecordPosition), 0f, recordingInterval);
    }

    private void StopRecording()
    {
        startRecordingButton.gameObject.SetActive(true);
        stopRecordingButton.gameObject.SetActive(false);

        isRecording = false;
        indicatorImage.color = Color.green;
        CancelInvoke(nameof(RecordPosition));
        SaveRecording();
    }

    private void PlayRecording()
    {
        isPlayingBack = true;
        currentPositionIndex = 0;
        playbackStartTime = Time.time;
        indicatorImage.color = playbackColor;
    }

    private void RecordPosition()
    {
        GameObject obj = GameObject.FindGameObjectWithTag(objectTag);
        recordedPositions.Add(obj.transform.position);
        recordedColors.Add(obj.GetComponent<Image>().color);
    }

    private void SaveRecording()
    {
        string[] lines = new string[recordedPositions.Count];
        for (int i = 0; i < recordedPositions.Count; i++)
        {
            lines[i] = string.Format("{0},{1},{2},{3},{4},{5}", recordedPositions[i].x, recordedPositions[i].y, recordedPositions[i].z,
                recordedColors[i].r, recordedColors[i].g, recordedColors[i].b);
        }

        string fileName = string.Format("{0}{1}.csv", inputField.text, recordingNumber);
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        File.WriteAllLines(filePath, lines);

        recordingNumber++;
    }
    public void DeleteRecording(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log(string.Format("Recording {0} deleted.", fileName));
        }
        else
        {
            Debug.LogWarning(string.Format("Recording {0} not found.", fileName));
        }
    }
    public void DeleteAllRecordings()
    {
        string[] fileNames = Directory.GetFiles(Application.persistentDataPath, "*.csv");

        foreach (string fileName in fileNames)
        {
            File.Delete(fileName);
        }

        Debug.Log("All recordings deleted.");
    }
    public void LoadRecording(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            recordedPositions.Clear();
            recordedColors.Clear();

            foreach (string line in lines)
            {
                string[] values = line.Split(',');

                if (values.Length >= 6)
                {
                    float x = float.Parse(values[0]);
                    float y = float.Parse(values[1]);
                    float z = float.Parse(values[2]);
                    Vector3 position = new Vector3(x, y, z);

                    float r = float.Parse(values[3]);
                    float g = float.Parse(values[4]);
                    float b = float.Parse(values[5]);
                    Color color = new Color(r, g, b);

                    recordedPositions.Add(position);
                    recordedColors.Add(color);
                }
            }

            Debug.Log(string.Format("Recording {0} loaded.", fileName));
        }
        else
        {
            Debug.LogWarning(string.Format("Recording {0} not found.", fileName));
        }
    }
}