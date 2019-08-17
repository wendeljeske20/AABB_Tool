using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Display : MonoBehaviour
{
    public Tool tool;
    public Slider pointSizeSlider, boxRedColorSlider, boxGreenColorSlider, boxBlueColorSlider, boxAlphaColorSlider;
    public Toggle wireframeToggle, quadToggle;
    public Text pointAmountText;



    void Start()
    {
        LoadConfigData();
        UpdateUI();
    }

    void Update()
    {

        pointAmountText.text = "Point Amount: " + tool.pointCount.ToString();
        UpdateTool();

    }

    void UpdateTool()
    {
        tool.pointSize = pointSizeSlider.value;
        tool.wireframeMode = wireframeToggle.isOn;
        tool.quadMode = quadToggle.isOn;
        tool.boxColor = new Color(boxRedColorSlider.value, boxGreenColorSlider.value, boxBlueColorSlider.value, boxAlphaColorSlider.value);
    }

    public void UpdateUI()
    {
        pointSizeSlider.value = tool.pointSize;
        wireframeToggle.isOn = tool.wireframeMode;
        quadToggle.isOn = tool.quadMode;

        boxRedColorSlider.value = tool.boxColor.r;
        boxGreenColorSlider.value = tool.boxColor.g;
        boxBlueColorSlider.value = tool.boxColor.b;
        boxAlphaColorSlider.value = tool.boxColor.a;
    }

    public void SaveConfigData()
    {
        StreamWriter configFileWriter = new StreamWriter("Assets/Resources/configData.txt");

        configFileWriter.WriteLine(tool.wireframeMode ? "1" : "0");
        configFileWriter.WriteLine(tool.quadMode ? "1" : "0");
        configFileWriter.WriteLine(tool.pointSize.ToString());
        configFileWriter.WriteLine(tool.boxColor.r + " " + tool.boxColor.g + " " + tool.boxColor.b + " " + tool.boxColor.a);
        configFileWriter.Close();
    }

    public void LoadConfigData()
    {
        StreamReader configFileReader = new StreamReader("Assets/Resources/configData.txt");

        string text = configFileReader.ReadToEnd();

        if (text != "")
        {
            text = text.Trim();
            string[] splittedText = text.Split(' ', '\n');

            if (splittedText[0].Trim() == "1")
                tool.wireframeMode = true;
            else
                tool.wireframeMode = false;

            if (splittedText[1].Trim() == "1")
                tool.quadMode = true;
            else
                tool.quadMode = false;


            float.TryParse(splittedText[2], out tool.pointSize);
            float.TryParse(splittedText[3], out tool.boxColor.r);
            float.TryParse(splittedText[4], out tool.boxColor.g);
            float.TryParse(splittedText[5], out tool.boxColor.b);
            float.TryParse(splittedText[6], out tool.boxColor.a);

            UpdateUI();
        }
        configFileReader.Close();
    }


}
