using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

[System.Serializable]
public class RecorderObject
{
    #region Vars
    public GameObject target;
    public ButtonScript ButtonSC;
    public Vector3 StartPos;
    public List<Vector3> PositionsSaved = new List<Vector3>();
    public Color OriginalColor;
    public List<Color> ColorsSaved = new List<Color>();
    #endregion
    public RecorderObject(GameObject target, Vector3 startPos, Color originalColor, ButtonScript button)
    {
        this.target = target;
        StartPos = startPos;
        OriginalColor = originalColor;
        ButtonSC = button;
    }
    #region functions

    public void RefreshPosition()
    {
        this.StartPos = target.transform.position;
        this.OriginalColor = target.GetComponent<Image>().color;
    }
    public void RecordState()
    {
        if (target != null)
            PositionsSaved.Add(target.transform.position);
        ColorsSaved.Add(target?.GetComponent<Image>().color ?? Color.white);
    }
    public void GoOriginalState()
    {
        target.transform.position = StartPos;
        target.GetComponent<Image>().color = OriginalColor;

    }
    public IEnumerator Play(float playBackInterval)
    {
        GoOriginalState();

        yield return new WaitForSeconds(playBackInterval);
        for (int i = 0; i < PositionsSaved.Count; i++)
        {
            target.transform.position = PositionsSaved[i];
            target.GetComponent<Image>().color = ColorsSaved[i];
            yield return new WaitForSeconds(playBackInterval);
        }
        GoOriginalState();
    }
    #endregion
}