using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Recorder : MonoBehaviour
{
    #region Vars
    [SerializeField]
    private List<GameObject> targetObjects = new List<GameObject>();
    [SerializeField]
    private SerializableList<RecorderObject> recorderObjects = new SerializableList<RecorderObject>();
    [SerializeField]
    private GameObject startRecordButton;
    [SerializeField]
    private GameObject stopRecordButton;

    [SerializeField]
    private TextMeshProUGUI actualLoaded;
    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private Image indicator;

    [SerializeField]
    private float playBackInterval;

    private bool isRecording = false;
    #endregion

    #region Unity Functions
    private void Start()
    {
        SetStartRecordingInfo();
    }
    private void Update()
    {
        if (isRecording)
        {
            Record();
            OnStop();
        }
        else
        {
            OnsSart();
        }
    }
    #endregion

    #region Functions
    private void OnStop()
    {
        indicator.color = Color.green;
        startRecordButton.SetActive(false);
        stopRecordButton.SetActive(true);
    }
    private  void OnsSart()
    {
        startRecordButton.SetActive(true);
        stopRecordButton.SetActive(false);
        indicator.color = Color.red;
    }
    private void SetStartRecordingInfo()
    {
        foreach (GameObject targetObject in targetObjects)
        {
            recorderObjects.list.Add(new RecorderObject(targetObject,
                targetObject.transform.position,
                targetObject.GetComponent<Image>().color,
                targetObject.GetComponent<ButtonScript>()));
        }
    }
    private void Record()
    {
        foreach (RecorderObject recorderObject in recorderObjects.list)
        {
            recorderObject.RecordState();
        }
    }
    public void SetOriginalPos()
    {
        foreach (RecorderObject recorderObject in recorderObjects.list)
        {
            recorderObject.GoOriginalState();
        }
    }
    public void SetRecordState(bool state)
    {
        if (inputField.text.Length <= 0) return;

        isRecording = state;
    }
    public void ClearLists()
    {
        recorderObjects.list.ForEach(recorderObject =>
        {
            recorderObject.PositionsSaved.Clear();
            recorderObject.ColorsSaved.Clear();
        });
    }
    public void StartPlayback()
    {
        if (inputField.text.Length <= 0) return;

        foreach (RecorderObject recorderObject in recorderObjects.list)
        {
            StartCoroutine(recorderObject.Play(playBackInterval));
        }
    }
    public void SaveRecord()
    {
        if (inputField.text.Length <= 0) return;

        string json = JsonUtility.ToJson(recorderObjects);

        PlayerPrefs.SetString(inputField.text, json);
        Debug.Log(json);
        actualLoaded.text = inputField.text;
    }
    public void LoadRecord()
    {
        if (inputField.text.Length <= 0) return;

        string json = PlayerPrefs.GetString(inputField.text);
        if (json.Length > 1)
            recorderObjects = JsonUtility.FromJson<SerializableList<RecorderObject>>(json);
        actualLoaded.text = inputField.text;

    }
    public void DeleteSaves()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion
}