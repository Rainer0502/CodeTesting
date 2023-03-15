using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Recorder : MonoBehaviour
{
    public List<GameObject> TargetObjcts = new List<GameObject>();
    [SerializeField]
    public SerializableList<RecorderObject> recorderObjects = new SerializableList<RecorderObject>();

    public GameObject StartRecordButton;
    public GameObject StopRecordButton;

    public TextMeshProUGUI actualLoaded;
    public Image Indicator;

    public TMP_InputField inputField;
    public float playBackInterval;

    public bool isRecordingn = false;
    public bool isPlayingBack = false;

    public void Start()
    {
        SetStartRecordingInfo();
    }
    private void Update()
    {
        if (isRecordingn)
        {
            Record();
            Indicator.color = Color.green;
            StartRecordButton.SetActive(false);
            StopRecordButton.SetActive(true);
        }
        else
        {
            StartRecordButton.SetActive(true);
            StopRecordButton.SetActive(false);
            Indicator.color = Color.red;
        }
    }

    private void SetStartRecordingInfo()
    {
        for (int i = 0; i < TargetObjcts.Count; i++)
        {
            recorderObjects.list.Add(new RecorderObject(TargetObjcts[i], 
                                                   TargetObjcts[i].transform.position, 
                                                   TargetObjcts[i].GetComponent<Image>().color, 
                                                   TargetObjcts[i].GetComponent<ButtonScript>()));
        }
    }
    private void Record()
    {
        for (int i = 0; i < recorderObjects.list.Count; i++)
        {
            recorderObjects.list[i].RecordState();
        }
    }
    public void SetOriginalPos()
    {
        for (int i = 0; i < recorderObjects.list.Count; i++)
        {
            recorderObjects.list[i].GoOriginalState();
        }
    }
    public void SetRecordState( bool state)
    {
        if (inputField.text.Length <= 0) return;

        isRecordingn = state;
    }
    public void ClearLists()
    {
        {
            for (int i = 0; i < recorderObjects.list.Count; i++)
            {
                recorderObjects.list[i].PositionsSaved.Clear();
                recorderObjects.list[i].ColorsSaved.Clear();
            }
        }
    }
    public void StartPlayback()
    {
        if (inputField.text.Length <= 0) return;

        isPlayingBack = true;
        for (int i = 0; i < recorderObjects.list.Count; i++)
        {
            StartCoroutine(recorderObjects.list[i].Play(playBackInterval));
            Debug.Log("ddd");
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
}