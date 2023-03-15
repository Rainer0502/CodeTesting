using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

[System.Serializable]
public class RecorderObject
{
    public GameObject target;
    public ButtonScript ButtonSC;
    public Vector3 StartPos;
    public List<Vector3> PositionsSaved = new List<Vector3>();
    public Color OriginalColor;
    public List<Color> ColorsSaved = new List<Color>();

    public RecorderObject(GameObject target, Vector3 startPos, Color originalColor, ButtonScript button)
    {
        this.target = target;
        this.StartPos = startPos;
        this.OriginalColor = originalColor;
        this.ButtonSC = button;
    }

    public void RecordState()
    {
        if (target)
            PositionsSaved.Add(target.transform.position);
        ColorsSaved.Add(target.GetComponent<Image>().color);
    }
    public void GoOriginalState()
    {
        target.transform.position = this.StartPos;
        target.GetComponent<Image>().color = OriginalColor;

    }
    public IEnumerator Play(float playBackInterval)
    {
        GoOriginalState();

        yield return new WaitForSeconds(playBackInterval);
        foreach (Vector3 pos in PositionsSaved)
        {
            target.transform.position = pos;
            target.GetComponent<Image>().color = ColorsSaved[PositionsSaved.IndexOf(pos)];
            yield return new WaitForSeconds(playBackInterval);
        }
        GoOriginalState();
    }
}