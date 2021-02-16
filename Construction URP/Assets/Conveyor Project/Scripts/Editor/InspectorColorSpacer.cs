using UnityEngine;
using System.Collections;
using UnityEditor;

// This class defines the ColorSpacer attribute, so that
// it can be used in your regular MonoBehaviour scripts:

public class ColorSpacer : PropertyAttribute
{
    public float spaceHeight;
    public float lineHeight;
    public float lineWidth;
    public Color lineColor = Color.red;

    public ColorSpacer(float spaceHeight, float lineHeight, float lineWidth, float r, float g, float b)
    {
        this.spaceHeight = spaceHeight;
        this.lineHeight = lineHeight;
        this.lineWidth = lineWidth;

        // unfortunately we can't pass a color through as a Color object
        // so we pass as 3 floats and make the object here
        this.lineColor = new Color(r, g, b);
    }
}


// This defines how the ColorSpacer should be drawn
// in the inspector, when inspecting a GameObject with
// a MonoBehaviour which uses the ColorSpacer attribute

[CustomPropertyDrawer(typeof(ColorSpacer))]
public class ColorSpacerDrawer : DecoratorDrawer
{
    ColorSpacer colorSpacer
    {
        get { return ((ColorSpacer)attribute); }
    }

    public override float GetHeight()
    {
        return base.GetHeight() + colorSpacer.spaceHeight;
    }

    public override void OnGUI(Rect position)
    {
        // calculate the rect values for where to draw the line in the inspector
        float lineX = (position.x + (position.width / 2)) - colorSpacer.lineWidth / 2;
        float lineY = position.y + (colorSpacer.spaceHeight / 2);
        float lineWidth = colorSpacer.lineWidth;
        float lineHeight = colorSpacer.lineHeight;

        // Draw the line in the calculated place in the inspector
        // (using the built in white pixel texture, tinted with GUI.color)
        Color oldGuiColor = GUI.color;
        GUI.color = colorSpacer.lineColor;
        EditorGUI.DrawPreviewTexture(new Rect(lineX, lineY, lineWidth, lineHeight), Texture2D.whiteTexture);
        GUI.color = oldGuiColor;
    }
}